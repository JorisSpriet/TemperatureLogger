namespace TemperatuurLogger.Services
{
    public class LoggerReportData
    {
        public DateTime Date { get; private set; }

        public decimal Temperature {  get; private set; }

        public LoggerReportData(DateTime date, decimal temperature)
        {
            Date=date;
            Temperature=temperature;
        }

    }
}
