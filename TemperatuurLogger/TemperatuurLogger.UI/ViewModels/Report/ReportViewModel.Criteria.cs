using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TemperatuurLogger.Services;
using DynamicData;

namespace TemperatuurLogger.UI.ViewModels
{
    public partial class ReportViewModel
    {
		private LoggerReportSourceModel selectedLogger;
		private DateTime fromDate;
        private DateTime loggerFromDate;
		private DateTime endDate;
        private DateTime loggerEndDate;

		public event EventHandler LoggerSelected;

		public ObservableCollection<LoggerReportSourceModel> Loggers { get; } = new();

        public DateTime LoggerFromDate
        {
            get => loggerFromDate;
            set
            {
                this.RaiseAndSetIfChanged(ref loggerFromDate, value);
            }
        }

        public DateTime FromDate
		{
			get => fromDate;
			set 
            {
                if(value < LoggerFromDate.Date)
                    throw new ArgumentOutOfRangeException($"Enkel metinging vanaf {LoggerFromDate.ToString("dd\\/MM\\/yyyy")}");
                this.RaiseAndSetIfChanged(ref fromDate, value);
               this.RaisePropertyChanged(nameof(FromDateText));
            }
		}
        public string FromDateText => $"Van :{fromDate.ToString("dd-MM-yyyy")}";

        public DateTime LoggerEndDate
        {
            get => loggerEndDate;
            set
            {
                this.RaiseAndSetIfChanged(ref loggerEndDate, value);
            }
        }

        public DateTime EndDate
		{
			get => endDate;
			set
            {
                if(value>LoggerEndDate)
                    throw new ArgumentOutOfRangeException($"Enkel metingen tem {LoggerEndDate.ToString("dd\\/MM\\/yyyy")}");
                this.RaiseAndSetIfChanged(ref endDate, value);
                this.RaisePropertyChanged(nameof(EndDateText));
            }
		}

        public string EndDateText => $"Tot :{endDate.ToString("dd-MM-yyyy")}";

		public LoggerReportSourceModel SelectedLogger
		{
			get => selectedLogger;
			set
			{
				this.RaiseAndSetIfChanged(ref selectedLogger, value);
				LoggerFromDate = value.AvailableFrom;
                FromDate = value.AvailableFrom;
                LoggerEndDate = value.AvailableTo;
                EndDate = value.AvailableTo;
				LoggerSelected?.Invoke(this, EventArgs.Empty);
			}
		}

        private void GetLoggers()
        {
            var svc = new LoggerReportService();
            ExecuteTransactionally(() =>
                Loggers.AddRange(svc.GetAvailableData())
            );
            if(Loggers.Count>0) { 
                SelectedLogger = Loggers.First();

            }
        }
		
	}
}
