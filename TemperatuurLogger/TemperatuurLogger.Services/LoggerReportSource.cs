namespace TemperatuurLogger.Services
{
    public class LoggerReportSource
    {
        public string Name { get; }

        public string SerialNumber { get; }

        public DateTime? From {  get; set; }

        public DateTime? To { get; set; }

        public bool NoData {  get; set; }

        public LoggerReportSource(string name, string serialNumber)
        {
            Name = name;
            SerialNumber = serialNumber;
        }

    }
}
