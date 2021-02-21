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
		#region bindable stuff
		public string Name { get; set; } = "APOTHEEK SAELENS";

		public int SamplingInterval { get; set; } = 180;

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
			//			3  min sampling interval
			//			15 start delay; start mode always "delay"
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
