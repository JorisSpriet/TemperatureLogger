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

        private readonly SerialPort serialPort;

        private int serialFrameSize;

        private Stopwatch sw = new Stopwatch();

        private readonly bool x;

        public string SerialNumber { get; private set; }

        private void WithSerialPort(Action action)
		{
            logger.Debug($"Opening port {serialPort.PortName}..");
            serialPort.Open();
            logger.Debug($"Serial port {serialPort.PortName} opened.");

            try {
                action();
            }
            finally {
                serialPort.Close();
            }
		}

        private void GetInfoDetails1(DeviceDetails result)
        {
            logger.Debug($"Sending command 'get info (1)'");

            var question = Messages.GetDataInfo1Message(SerialNumber, x);
            var details1 = Get<AnswerGetInfoDetails1Message>(question);

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

        private void GetInfoDetails2(DeviceDetails result)
        {
            logger.Debug($"Sending command 'get info (2)'");
            var question = Messages.GetDataInfo2Message(SerialNumber, x);
            var details2 = Get<AnswerGetInfoDetails2Message>(question);

            result.SampleInterval = details2.SampleInterval;
        }

        public DeviceDetails GetDetailsFromDevice()
        {
            var result = new DeviceDetails();
            WithSerialPort(() =>
            {
                GetInfoDetails1(result);
                GetInfoDetails2(result);

                if (SerialNumber != result.SerialNumber)
                    throw new Exception("Received invalid details from device : different serial number");
            });
            return result;
        }

        T Get<T>(byte[] question = null)
        {
            SendQuestion(question);

            var buffer = new byte[Marshal.SizeOf<T>()];
            var offset = 0;

            sw.Restart();
            while (offset < buffer.Length) {
                //TODO 1 JS PROTECT AGAINST ENDLESS LOOP
                var bytecount = buffer.Length - offset;
                int r = serialPort.BaseStream.Read(buffer, offset, bytecount);
                offset += r;
                logger.Debug($"\treceived {r} bytes ({offset} of {buffer.Length})");
                if (r == 0)
                    Thread.Sleep(1);
            }
            logger.Debug($"Received a total of {offset} bytes");

            var result = Utils.Map<T>(buffer);
            return result;
        }

        private void GetData(DataMessageSample[] data)
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

            var estimatedTransferTime = Convert.ToInt64(totalBytes * serialFrameSize * 1000) / serialPort.BaudRate;
            logger.Info($"Data : {totalBytes} estimated transfer time {estimatedTransferTime} ms");

            var question = Messages.GetDataMessage(SerialNumber);
            SendQuestion(question);
            
            sw.Restart();
            while (offset < totalBytes)
            {
                //TODO 1 JS PROTECT AGAINST ENDLESS LOOP                
                var bytecount = buffer.Length - offset;
                int r = serialPort.BaseStream.Read(buffer, offset, bytecount);
                offset += r;
                decimal progress = Convert.ToDecimal(offset * 100 / buffer.Length);
                if(r>0)
                    logger.Debug($"\treceived {r} bytes ({offset} of {buffer.Length}) ({ progress:##0.##}%)");
                if (r == 0)
                    Thread.Sleep(1);
            }
            sw.Start();
            logger.Info($"Received a total of {offset} bytes in {sw.Elapsed}");
            if(rest>0) {
                var dmts = Marshal.SizeOf<DataMessageTail>();
                var targetIndex = buffer.Length - Marshal.SizeOf<DataMessageTail>();
                var srcIndex = targetIndex- (15 - rest) * Marshal.SizeOf<DataMessageSample>();
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
        private void SendQuestion(byte[] question = null)
        {
            if (question?.Length > 0)
            {
                serialPort.Write(question, 0, question.Length);
                Thread.Sleep(450);
            }
        }
        public async Task<DeviceSample[]> GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback)
        {
            var result = new List<DeviceSample>();
            await Task.Run(() =>
            {
                WithSerialPort(() =>
                {
                    var details = new DeviceDetails();
                    GetInfoDetails1(details);

                    var data = new DataMessageSample[details.NumberOfSamples];
                    GetData(data);

                    int sampleNumber = 1;
                    foreach (var sample in data) {
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
            WithSerialPort(() =>
            {
                Get<AnswerClearDataMessage>(Messages.GetClearDataMessage(SerialNumber, x));
                
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
                serialPort.Dispose();

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

        public Device(string serialNumber, SerialPort port)
        {
            SerialNumber = serialNumber;
            serialPort = port;
            serialFrameSize = port.DataBits + (int)port.StopBits + ((int)port.Parity > 0 ? 1 : 0);

            x = serialNumber.StartsWith("HE");
        }
    }

}