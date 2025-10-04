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
		private LoggerReportSource selectedLogger;
		private DateTimeOffset fromDate;
        private DateTime loggerFromDate;
		private DateTimeOffset endDate;
        private DateTime loggerEndDate;

		public event EventHandler LoggerSelected;

		public ObservableCollection<LoggerReportSource> Loggers { get; } = new();

        public int ReportYear
        {
            get => reportYear;
            set {

                if (reportYear != value) {
                    GetLoggers(value);
                }
                this.RaiseAndSetIfChanged(ref reportYear, value);
            }
        }

        public string AvailableDataText => GetAvailableDataText();

        private string GetAvailableDataText()
        {
            if(SelectedLogger== null) {
                return "Kies een jaartal, dan een logger";
            }

            return 
                SelectedLogger.NoData ? "Geen metingen voor deze logger." :
                $"Metingen van {SelectedLogger.From?.Date.ToString("dd\\/MM\\/yyyy")} t.e.m {SelectedLogger.To?.Date.ToString("dd\\/MM\\/yyyy")}";
        }

        public DateTimeOffset FromDate
		{
			get => fromDate;
			set => fromDate = value;
		}
        
        public DateTimeOffset EndDate
		{
			get => endDate;
			set => endDate = value;
		}

		public LoggerReportSource SelectedLogger
		{
			get => selectedLogger;
			set
			{
				this.RaiseAndSetIfChanged(ref selectedLogger, value);
				LoggerSelected?.Invoke(this, EventArgs.Empty);
                FromDate = selectedLogger?.From?.Date ?? new DateTime(reportYear,1,1);
                EndDate = selectedLogger?.To?.Date ?? new DateTime(reportYear,12,31);
                this.RaisePropertyChanged(nameof(AvailableDataText));
			}
		}

        private void GetLoggers(int year)
        {
            var svc = new LoggerReportService();
            Loggers.Clear();
            Loggers.AddRange(svc.Getloggers(year.ToString()));
            
            if(Loggers.Count>0) { 
                SelectedLogger = Loggers.First();
            }
        }
		
	}
}
