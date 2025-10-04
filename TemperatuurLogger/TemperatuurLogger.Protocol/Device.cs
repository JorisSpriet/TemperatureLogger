using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace TemperatuurLogger.Protocol
{

    public class Device : IDisposable, IDevice
    {
        private static ILog logger = LogManager.GetLogger("DEVICE");

        private readonly DeviceComPortDetails portDetails;

        private int serialFrameSize;

        private Stopwatch sw = new Stopwatch();

        private readonly bool x;

        public string SerialNumber { get; private set; }

        private void WithSerialPort(Action<SerialPort> action)
        {
            logger.Debug($"Getting serial port {portDetails.PortName}..");
            var serialPort = portDetails.GetSerialPort();

            action(serialPort);
        }

        private void GetInfoDetails1(SerialPort serialPort, DeviceDetails result)
        {
            logger.Debug($"Sending command 'get info (1)'");

            var question = Messages.GetDataInfo1Message(SerialNumber, x);
            var details1 = Get<AnswerGetInfoDetails1Message>(serialPort, question);

            if (!details1.IsValid())
                throw new Exception("Received invalid details from device.");

            result.Description = Encoding.ASCII.GetString(details1.Description, 0, 16);
            result.SerialNumber = Encoding.ASCII.GetString(details1.SerialNumber, 0, 10);
            result.Model = Encoding.ASCII.GetString(details1.Model, 0, 6);
            result.NumberOfSamples = details1.NumberOfSamples;
            result.OffsetCh1 = Convert.ToDecimal(details1.OffsetCh1) / 10M;
            result.OffsetCh2 = Convert.ToDecimal(details1.OffsetCh2) / 10M;
            result.DelayTime = details1.DelayTime;
        }

        private void GetInfoDetails2(SerialPort serialPort, DeviceDetails result)
        {
            logger.Debug($"Sending command 'get info (2)'");
            var question = Messages.GetDataInfo2Message(SerialNumber, x);
            var details2 = Get<AnswerGetInfoDetails2Message>(serialPort, question);

            result.SampleInterval = details2.SampleInterval;
        }

        public DeviceDetails GetDetailsFromDevice()
        {
            var result = new DeviceDetails();
            WithSerialPort((serialPort) =>
            {
                GetInfoDetails1(serialPort, result);
                GetInfoDetails2(serialPort, result);

                if (SerialNumber != result.SerialNumber)
                    throw new Exception("Received invalid details from device : different serial number");
            });
            return result;
        }

        T Get<T>(SerialPort serialPort, byte[] question = null)
        {
            SendQuestion(serialPort, question);

            var buffer = new byte[Marshal.SizeOf<T>()];
            var offset = 0;
            var noDataCounter = 0;

            sw.Restart();
            while (offset < buffer.Length || noDataCounter > 1000)
            {
                //TODO 1 JS PROTECT AGAINST ENDLESS LOOP
                var bytecount = buffer.Length - offset;
                int r = serialPort.BaseStream.Read(buffer, offset, bytecount);
                offset += r;
                logger.Debug($"\treceived {r} bytes ({offset} of {buffer.Length})");
                if (r == 0)
                {
                    Thread.Sleep(5);
                    noDataCounter++;
                }
                else noDataCounter = 0;
            }
            if (noDataCounter > 0)
                throw new Exception("Logger antwoordt niet.  Contacteer technische dienst.");
            logger.Debug($"Received a total of {offset} bytes");

            var result = Utils.Map<T>(buffer);
            return result;
        }

        private void GetData(SerialPort serialPort, DataMessageSample[] data, SamplesReadingCallback callback)
        {

            var messageCount = data.Length / 15;
            var totalBytes = messageCount * Marshal.SizeOf<DataMessage>();
            var rest = data.Length % 15;
            if (rest > 0)
            {
                messageCount++;
                totalBytes = totalBytes + Marshal.SizeOf<DataMessageHeader>() +
                    rest * Marshal.SizeOf<DataMessageSample>() +
                    Marshal.SizeOf<DataMessageTail>();
            }
            var buffer = new byte[Marshal.SizeOf<DataMessage>() * messageCount];
            var offset = 0;

            var estimatedTransferTime = Convert.ToInt64(totalBytes * serialFrameSize * 1000) / portDetails.BaudRate;
            logger.Info($"Data : {totalBytes} estimated transfer time {estimatedTransferTime} ms");

            var question = Messages.GetDataMessage(SerialNumber);
            SendQuestion(serialPort, question);

            sw.Restart();
            while (offset < totalBytes)
            {
                //TODO 1 JS PROTECT AGAINST ENDLESS LOOP                
                var bytecount = buffer.Length - offset;
                if (serialPort.BytesToRead < 1)
                {
                    Thread.Sleep(100);
                    continue;
                }
                int r = serialPort.BaseStream.Read(buffer, offset, bytecount);
                offset += r;
                decimal progress = Convert.ToDecimal(offset * 100 / buffer.Length);
                if (r > 0)
                    logger.Debug($"\treceived {r} bytes ({offset} of {buffer.Length}) ({progress:##0.##}%)");
                if (r == 0)
                    Thread.Sleep(1);
                DoCallback(offset, totalBytes, callback);
            }
            sw.Stop();
            logger.Info($"Received a total of {offset} bytes in {sw.Elapsed}");
            if (rest > 0)
            {
                var dmts = Marshal.SizeOf<DataMessageTail>();
                var targetIndex = buffer.Length - Marshal.SizeOf<DataMessageTail>();
                var srcIndex = targetIndex - (15 - rest) * Marshal.SizeOf<DataMessageSample>();
                Array.Copy(buffer, srcIndex, buffer, targetIndex, dmts);
            }



            var dataMessages = Utils.MapArray<DataMessage>(buffer);
            var c = 0;
            for (int i = 0; i < dataMessages.Length; i++)
            {
                var samples = dataMessages[i].GetSamples();
                for (int j = 0; j < 15 && c < data.Length; j++, c++)
                {
                    data[c] = samples[j];
                }
            }
        }

        public void SetClock()
        {
            WithSerialPort((serialPort) =>
            {
                var message = Messages.GetSetClockMessage(SerialNumber, x);
                Get<AnswerSetClockMessage>(serialPort, message);
            });
        }

        private void DoCallback(int offset, int totalBytes, SamplesReadingCallback callback)
        {
            var p = Convert.ToInt32(offset * 100.0 / totalBytes);
            callback?.Invoke(p, offset, totalBytes);
        }

        private void SendQuestion(SerialPort port, byte[] question = null)
        {

            if (port.BytesToRead > 0)
            {
                var buffer = new byte[port.BytesToRead];
                port.Read(buffer, 0, buffer.Length);
                logger.Warn("There were bytes to read.... ?");
            }
            if (question?.Length > 0)
            {
                port.Write(question, 0, question.Length);
                Thread.Sleep(500);
            }
        }
        public async Task<DeviceSample[]> GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback)
        {
            var result = new List<DeviceSample>();
            await Task.Run(() =>
            {

                WithSerialPort((serialPort) =>
                {
                    var details = new DeviceDetails();
                    GetInfoDetails1(serialPort, details);

                    var data = new DataMessageSample[details.NumberOfSamples];
                    GetData(serialPort, data, samplesReadingCallback);

                    int sampleNumber = 1;
                    foreach (var sample in data)
                    {
                        result.Add(new DeviceSample
                        {
                            ID = sampleNumber++,
                            Temperature = Convert.ToDecimal(sample.Temperature) / 10M,
                            TimeStamp = sample.GetTimeStamp()
                        });
                    }
                });
            });

            return result.ToArray();
        }




        public void ClearDataOnDevice()
        {
            WithSerialPort((serialPort) =>
            {
                // Get<AnswerClearDataMessage>(Messages.GetClearDataMessage(SerialNumber, x));
                SendQuestion(serialPort, Messages.GetClearDataMessage(SerialNumber, x));
                //we probably should check the answer...
            });
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Logger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

        public Device(string serialNumber, DeviceComPortDetails portDetails)
        {
            SerialNumber = serialNumber;
            this.portDetails = portDetails;
            serialFrameSize = portDetails.DataBits + (int)portDetails.StopBits + ((int)portDetails.Parity > 0 ? 1 : 0);

            x = serialNumber.StartsWith("HE");
        }
    }

}