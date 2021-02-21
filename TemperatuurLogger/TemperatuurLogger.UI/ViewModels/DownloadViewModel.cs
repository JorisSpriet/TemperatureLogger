using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TemperatuurLogger.Protocol;

namespace TemperatuurLogger.UI.ViewModels
{
	class DownloadViewModel : ViewModelBase
	{
		private DownloadViewModelState state;
		private Device device;

		public ReactiveCommand<ICanNext,Unit> Next { get; private set; }

		public ReactiveCommand<ICanClose, Unit> Cancel { get; private set; }

		public DownloadViewModelState State
		{
			get => state;
			set => this.RaiseAndSetIfChanged(ref state, value);
					
		}

		#region step 1 stuff

		ObservableAsPropertyHelper<string> detectionStatus;
		public string DetectionStatus
		{
			get => detectionStatus.Value;			
		}

		string GetDetectionStatus()
		{
			switch(State) {
				case DownloadViewModelState.Idle: return "Logger niet gevonden.";
				case DownloadViewModelState.Detecting: return "Logger aan het zoeken";
				case DownloadViewModelState.Detected: return $"Logger gevonden : {device?.SerialNumber ?? "?"}";				
			}
			return "-";
		}

		public ReactiveCommand<Unit,Unit> Detect { get; private set; }


		void DoDetect()
		{
			Dispatcher.UIThread.InvokeAsync(LaunchDetection);
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

		#endregion

		#region step 2 stuff

		void DoRetrieveInfo()
		{	
			Dispatcher.UIThread.InvokeAsync(RetrieveInfo);
		}

		async Task<DownloadViewModelState> RetrieveInfo()
		{
			if (State != DownloadViewModelState.RetrievingInfo)
				throw new InvalidOperationException();

			State = await Task.Run<DownloadViewModelState>(() => {

				var info = device.GetDetailsFromDevice();
		
				return DownloadViewModelState.InfoRetrieved;
			});
			return State;
		}

		#endregion

		#region step 3 stuff
		void DoDownload()
		{
			//TODO 0 JS Implement DoDownload()
			throw new NotImplementedException("DoDownload not implemented yet");
		}
		#endregion

		#region step 4 stuff
		void DoPersist()
		{
			//TODO 0 JS Implement DoPersist()
			throw new NotImplementedException("DoPersist not implemented yet");
		}
		#endregion

		#region step 5 stuff
		void DoReset()
		{
			device.ClearDataOnDevice();
			//TODO 0 JS Implement Device.SetClock();
			//device.SetClock();
			State = DownloadViewModelState.Done;
		}
		#endregion

		Unit DoCancel(ICanClose view)
		{
			view?.Close();
			return Unit.Default;
		}

		void InitConditions()
		{
			this.WhenAnyValue(vm => vm.State,
				s => s == DownloadViewModelState.Idle || s == DownloadViewModelState.Detecting || s == DownloadViewModelState.Detected)
				.Select(b => GetDetectionStatus())
				.ToProperty(this, vm => vm.DetectionStatus, out detectionStatus);
		}

		void InitCommands()
		{
			//Canceling
			var canCancel = this.WhenAnyValue(vm => vm.State, (x) => x == DownloadViewModelState.Idle ||
				x == DownloadViewModelState.Detected ||
				x == DownloadViewModelState.InfoRetrieved);
			Cancel = ReactiveCommand.Create<ICanClose, Unit>(DoCancel, canCancel);


			var canDetect = this.WhenAnyValue(vm => vm.State, x => x == DownloadViewModelState.Idle);
			Detect = ReactiveCommand.Create(DoDetect, canDetect);

			var canNext = this.WhenAnyValue(vm => vm.State, x =>  CanDoNext(x));
			Next = ReactiveCommand.Create<ICanNext,Unit>(DoNext, canNext);			
		}

		bool CanDoNext(DownloadViewModelState state)
		{
			return state == DownloadViewModelState.Detected ||
				state == DownloadViewModelState.InfoRetrieved ||
				state == DownloadViewModelState.Downloaded ||
				state == DownloadViewModelState.Persisted;// ||
				//state = DownloadViewModelState.Done
		}

		void HandleStateTransition()
		{
			switch(State) {
				case DownloadViewModelState.RetrievingInfo:
					DoRetrieveInfo();
					break;
				case DownloadViewModelState.Downloading:
					DoDownload();
					break;
				case DownloadViewModelState.Persisting:
					DoPersist();
					break;
				case DownloadViewModelState.Resetting:
					DoReset();
					break;
				default:
					throw new InvalidOperationException($"Nothing to launch when state is {State}");
			}
		}


		Unit DoNext(ICanNext canNext)
		{
			canNext.Next();

			State = State + 1;
			HandleStateTransition();


			return Unit.Default;
		}

	
		public DownloadViewModel()
		{
			InitConditions();			

			InitCommands();

			DoDetect();
		}
	}
}
