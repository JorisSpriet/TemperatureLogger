using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using TemperatuurLogger.Protocol;

namespace TemperatuurLogger.UI.ViewModels
{
	class DownloadViewModel : ViewModelBase
	{
		private DownloadViewModelState state;
		private Device device;

		public ReactiveCommand<Unit,Unit> Next { get; }

		public ReactiveCommand<ICanClose, Unit> Cancel { get; private set; }

		public DownloadViewModelState State
		{
			get => state;
			set => this.RaiseAndSetIfChanged(ref state, value);
					
		}

		async Task<DownloadViewModelState> LaunchDetection()
		{
			if (State != DownloadViewModelState.Idle)
				throw new InvalidOperationException();

			State = DownloadViewModelState.Detecting;

			State = await Task.Run<DownloadViewModelState>(() => {

				System.Threading.Thread.Sleep(15000);

				var df = new DeviceFinder();
				device = df.FindLoggerOnPort(DeviceFinder.DefaultPreferredPort);
				
				return device==null ? DownloadViewModelState.Idle : DownloadViewModelState.Detected;
			});
			return State;
		}

		//ObservableAsPropertyHelper<bool> canCancel;	
		//public bool CanCancel => canCancel.Value;
				

		Unit DoCancel(ICanClose view)
		{
			view?.Close();
			return Unit.Default;
		}

		//void InitConditions()
		//{
		//	//When can we cancel ?
		//	this.WhenAny(vm => vm.State, (s) =>
		//	{
		//		var x = s.Value;
		//		return x == DownloadViewModelState.Idle ||
		//		x == DownloadViewModelState.Detected ||
		//		x == DownloadViewModelState.InfoRetrieved;

		//	}).ToProperty(this, x=> x.CanCancel, out canCancel, false);
		//}

		void InitCommands()
		{
			//Canceling
			var canCancel = this.WhenAnyValue(vm => vm.State, (x) => x == DownloadViewModelState.Idle ||
				x == DownloadViewModelState.Detected ||
				x == DownloadViewModelState.InfoRetrieved);
			Cancel = ReactiveCommand.Create<ICanClose, Unit>(DoCancel, canCancel);



		}

		
		public DownloadViewModel()
		{
			//InitConditions();
			

			InitCommands();

			Dispatcher.UIThread.InvokeAsync(LaunchDetection);
		}
	}
}
