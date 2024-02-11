namespace TemperatuurLogger.Services
{
    public class LoggerReportSourceModel
    {
        public string Name { get; }

        public string SerialNumber { get; }

        public DateTime AvailableFrom { get; }

        public DateTime AvailableTo { get; }

        public LoggerReportSourceModel(string name, string serialNumber, DateTime from, DateTime to)
        {
            Name = name;
            SerialNumber = serialNumber;
            AvailableFrom = from;
            AvailableTo = to;
        }

    }
}
