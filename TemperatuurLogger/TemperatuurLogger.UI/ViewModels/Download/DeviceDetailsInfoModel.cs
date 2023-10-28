using ReactiveUI;
using System.Linq;
using System.Reactive;
using TemperatuurLogger.Model;
using TemperatuurLogger.Protocol;
using Xtensive.Orm;

namespace TemperatuurLogger.UI.ViewModels
{
	public class DeviceDetailsInfoModel : DatabaseBoundViewModelBase
	{
		string loggerName = "onbekend";
		string lastDownload = "onbekend";
		DeviceDetails details;
		
		public ReactiveCommand<Unit,Unit> CreateLogger { get; private set; }

		public bool LoggerIsKnown { get; set; }

		public string LoggerName {
			get => loggerName;
			set => this.RaiseAndSetIfChanged(ref loggerName, value);
		}

		public string SerialNumber => details.SerialNumber;
		public int NumberOfLogs => details.NumberOfSamples;

		public string LastDownload
		{
			get => lastDownload;
			set => this.RaiseAndSetIfChanged(ref lastDownload, value);
		}

		private void RetrieveInfoOnLogger()
		{
			ExecuteTransactionally(() =>
			{
				var l = Query.All<Logger>().SingleOrDefault(x => x.SerialNumber == details.SerialNumber);

				LoggerIsKnown = l != null;

				if (!LoggerIsKnown) return;

				LoggerName = l.Name;

				var last = Query.All<MeasurementDownload>()
					.Where(x => x.Logger == l)
					.OrderByDescending(x => x.DownloadTime)
					.FirstOrDefault();
				LastDownload = last?.DownloadTime.ToString("dd/MM/yyyy") ?? "onbekend";
			}
			);
		}

		private Unit DoCreateLogger(Unit arg)
		{
			ExecuteTransactionally(() =>
				{
					var logger = new Logger(SerialNumber, LoggerName);
					LoggerIsKnown = true;
				}
			);

			return Unit.Default;
		}


		void InitCommands()
		{
			//var can
			var canCreateLogger = this.WhenAnyValue(vm => vm.LoggerIsKnown, x => !x);
			CreateLogger = ReactiveCommand.Create<Unit, Unit>(DoCreateLogger, canCreateLogger);

		}


		public DeviceDetailsInfoModel(DeviceDetails details)
		{
			this.details = details;

			InitCommands();

			RetrieveInfoOnLogger();
		}

		
	}
}
