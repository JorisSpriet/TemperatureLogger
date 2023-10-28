using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TemperatuurLogger.Model;
using Xtensive.Orm;
using Xtensive.Core;

namespace TemperatuurLogger.UI.ViewModels
{
	public class ReportCriteriaViewModel : DatabaseBoundViewModelBase
	{
		private LoggerReportModel selectedLogger;
		private DateTime fromDate;
		private DateTime endDate;

		public event EventHandler LoggerSelected;

		public ObservableCollection<LoggerReportModel> Loggers { get; } = new();

		public DateTime FromDate
		{
			get => fromDate;
			set => this.RaiseAndSetIfChanged(ref fromDate, value);
		}

		public DateTime EndDate
		{
			get => endDate;
			set => this.RaiseAndSetIfChanged(ref endDate, value);
		}

		public LoggerReportModel SelectedLogger
		{
			get => selectedLogger;
			set
			{
				this.RaiseAndSetIfChanged(ref selectedLogger, value);
				FromDate = value.AvailableFrom;
				EndDate = value.AvailableTo;
				LoggerSelected?.Invoke(this, EventArgs.Empty);
			}
		}

		void FetchAvailabelLoggers()
		{

			ExecuteTransactionally(() =>
			{
				var loggers = Query.All<Logger>()
					.Where(l => l.Downloads.Any())
					.ToArray();
				var data = loggers
					.Select(l =>
					{
						var measurements = l.Downloads.SelectMany(d => d.Measurements);
						var oldest = measurements.OrderBy(m => m.Timestamp).First().Timestamp;
						var newest = measurements.OrderByDescending(m => m.Timestamp).First().Timestamp;
						return new LoggerReportModel(l.Name, l.SerialNumber, oldest, newest);
					});
				data.ForEach(i => Loggers.Add(i));
				SelectedLogger = data.Last();
			});
		}
		public ReportCriteriaViewModel()
		{

			FetchAvailabelLoggers();
		}
	}
}
