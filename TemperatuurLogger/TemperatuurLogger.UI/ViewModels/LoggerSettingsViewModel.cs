using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemperatuurLogger.UI.Views;

namespace TemperatuurLogger.UI.ViewModels
{
	public class LoggerSettingsViewModel : ViewModelBase
	{
		public class LoggerData
		{	
			public string Serial {  get; set; }
			public string Name { get; set; }
			public string PurchaseDate {  get;set; }
			public string LastDownloadDate { get; set; }
			public string LastBatteryRefreshDate { get; set; }

		}

		public LoggerData[] Loggers => new[]
			{
			new LoggerData{Serial="123", Name="logger 1", PurchaseDate="01/01/2020", LastDownloadDate="31/12/2024", LastBatteryRefreshDate="01/01/2025 "},
            new LoggerData{Serial="123", Name="logger 1", PurchaseDate="01/01/2020", LastDownloadDate="31/12/2024", LastBatteryRefreshDate="01/01/2025 "},
            new LoggerData{Serial="123", Name="logger 1", PurchaseDate="01/01/2020", LastDownloadDate="31/12/2024", LastBatteryRefreshDate="01/01/2025 "},
            new LoggerData{Serial="123", Name="logger 1", PurchaseDate="01/01/2020", LastDownloadDate="31/12/2024", LastBatteryRefreshDate="01/01/2025 "},
			};


		#region bindable stuff
		public string Name { get; set; } = "APOTHEEK SAELENS";

		public int SamplingInterval { get; set; } = 900;

		public int LoggingInterval { get; set; } = 900;

		public LoggerStartMode StartMode { get; } = LoggerStartMode.DelayedStart;

		public int Ch1High { get; } = 10;
		
		public int Ch1Low { get; } = 0;
		
		public int Ch2High { get; } = 10;
		
		public int Ch2Low { get; } = 0;

		public ReactiveCommand<Unit,Unit> Send { get; }
		
		public ReactiveCommand<ICanClose,Unit> Close { get; }

		#endregion

		private void SendCommand()
		{
			//TODO 0 JS IMPLEMENT SETTING PROPERTIES
			//  WE'll HARDCODE
			//			15 min logging interval
			//			15 min sampling interval
			//			15 min start delay; start mode always "delay"
			//			alarms are less important....	
		}

		private Unit CloseCommand(ICanClose view)
		{
			view?.Close();
			return Unit.Default;
		}

		public LoggerSettingsViewModel()
		{
			Send = ReactiveCommand.Create(SendCommand);
			Close = ReactiveCommand.Create<ICanClose,Unit>(CloseCommand);
		}
	}
}
