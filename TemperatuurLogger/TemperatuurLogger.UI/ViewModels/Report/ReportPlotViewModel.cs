using System;
using System.Linq;
using System.Collections.Generic;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.UI.ViewModels
{
	public class ReportPlotViewModel : DatabaseBoundViewModelBase
	{
		public DateTime FromDate { get; set; }

		public DateTime EndDate { get; set; }

		public LoggerReportModel SelectedLogger { get; set; }

		private ICanPlot plotter;

		public ICanPlot Plotter
		{
			get => plotter;
			set { plotter = value;
				GetData();
				plotter.Plot($"{SelectedLogger.Name}: {FromDate:dd-MM-yyyy} - {EndDate:dd-MM-yyyy}",CurrentDataPointSet);
			}
		}

		private void GetData()
		{
			ExecuteTransactionally(() =>
			{
				var logger = Query.All<Logger>().Single(x => x.SerialNumber == SelectedLogger.SerialNumber);
				var downloads = logger.Downloads.Where(d =>
				   d.StartTime <= FromDate && d.EndTime >= FromDate ||
				   d.StartTime <= EndDate && d.EndTime >= EndDate ||
				   d.StartTime >= FromDate && d.EndTime <= EndDate
				).OrderBy(d => d.DownloadTime);

				//todo : handle overlaps, group into months.
				var datapoints = downloads
					.SelectMany(d => d.Measurements)
					.Select(m => new DataPoint { Timestamp = m.Timestamp, Temperature = m.Temperature })
					.ToList();
				DataPointSets = new[] { datapoints };
			});
		}

		public List<DataPoint>[] DataPointSets { get; set; }

		public int CurrentDataPointSetIndex { get; set; }

		public List<DataPoint> CurrentDataPointSet => DataPointSets[CurrentDataPointSetIndex];


		public ReportPlotViewModel()
		{
	
			
		}		
	}
}
