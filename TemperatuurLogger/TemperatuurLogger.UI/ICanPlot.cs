using System;
using System.Collections.Generic;

namespace TemperatuurLogger.UI
{
	public interface ICanPlot
	{
		void Plot(string title, List<DataPoint> dataPoints);
	}

	public class DataPoint
	{
		public DateTime Timestamp;

		public decimal Temperature;
	}
}
