namespace TemperatuurLogger.Services
{
    public class LoggerReportModel
    {
        public string Name { get; }

        public string SerialNumber { get; }

        public DateTime From { get; }

        public DateTime To { get; }

        public LoggerReportData[] Data {  get; }

        public LoggerReportModel(string name, string serialNumber, DateTime from, DateTime to, LoggerReportData[] data)
        {
            Name = name;
            SerialNumber = serialNumber;
            From = from;
            To = to;
            Data= data;
        }

    }
}
