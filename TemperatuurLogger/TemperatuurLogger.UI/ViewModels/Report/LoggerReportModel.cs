using System;

namespace TemperatuurLogger.UI.ViewModels
{
	public class LoggerReportModel
	{
		public string Name { get; }

		public string SerialNumber { get; }

		public DateTime AvailableFrom { get; }

		public DateTime AvailableTo { get; }

		public LoggerReportModel(string name, string serialNumber, DateTime from, DateTime to)
		{
			Name = name;
			SerialNumber = serialNumber;
			AvailableFrom = from;
			AvailableTo = to;
		}
	}
}
