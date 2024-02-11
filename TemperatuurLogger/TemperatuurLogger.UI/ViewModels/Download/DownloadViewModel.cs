using Avalonia.Controls;
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

		private DeviceDetailsInfoModel deviceDetailsInfo;
		private DeviceSample[] samples;

		public ReactiveCommand<ICanNext, Unit> Next { get; private set; }

		public ReactiveCommand<ICanClose, Unit> Cancel { get; private set; }

		public DownloadViewModelState State
		{
			get => state;
			set
			{
				this.RaiseAndSetIfChanged(ref state, value);
				this.RaisePropertyChanged(nameof(NextActionText));
				this.RaisePropertyChanged(nameof(CancelActionText));
			}

		}

		public string CancelActionText => State == DownloadViewModelState.Done ? "Sluiten" : "Annuleren";
		public string NextActionText
		{
			get
			{
				switch (State) {
					case DownloadViewModelState.Detected: return "Uitlezen info";
					case DownloadViewModelState.InfoRetrieved: return "Uitlezen logger";
					case DownloadViewModelState.Downloaded: return "Opslaan";
					case DownloadViewModelState.Persisted: return "Logger legen";
					default: return "";
				}
			}
		}

		public DeviceDetailsInfoModel DeviceDetailsInfo
		{
			get => deviceDetailsInfo;
			set => this.RaiseAndSetIfChanged(ref deviceDetailsInfo, value);
		}

		#region step 1 stuff

		ObservableAsPropertyHelper<string> detectionStatus;
		public string DetectionStatus
		{
			get => detectionStatus.Value;
		}

		string GetDetectionStatus()
		{
			switch (State) {
				case DownloadViewModelState.Idle: return "Logger niet gevonden.";
				case DownloadViewModelState.Detecting: return "Logger aan het zoeken";
				case DownloadViewModelState.Detected: return $"Logger gevonden : {device?.SerialNumber ?? "?"}";
			}
			return "-";
		}


		public ReactiveCommand<Unit, Unit> Detect { get; private set; }


		void DoDetect()
		{
			Dispatcher.UIThread.InvokeAsync(LaunchDetection);
		}

		async Task<DownloadViewModelState> LaunchDetection()
		{
			if (State != DownloadViewModelState.Idle)
				throw new InvalidOperationException();

			State = DownloadViewModelState.Detecting;

			State = await Task.Run<DownloadViewModelState>(() =>
			{

				//System.Threading.Thread.Sleep(15000);

				var df = new DeviceFinder();
				device = df.FindLoggerOnPort(DeviceFinder.DefaultPreferredPort);

				return device == null ? DownloadViewModelState.Idle : DownloadViewModelState.Detected;
			});
			return State;
		}

		#endregion

		#region step 2 stuff

		void DoRetrieveInfo(UserControl p)
		{
			Dispatcher.UIThread.InvokeAsync(() => RetrieveInfo(p));
		}

		async Task<DownloadViewModelState> RetrieveInfo(UserControl p)
		{
			if (State != DownloadViewModelState.RetrievingInfo)
				throw new InvalidOperationException();

			State = await Task.Run(() =>
			{

				deviceDetailsInfo = new DeviceDetailsInfoModel(device.GetDetailsFromDevice());

				Dispatcher.UIThread.InvokeAsync(() =>
				{
					p.DataContext = deviceDetailsInfo;
				});

				return DownloadViewModelState.InfoRetrieved;
			});
			return State;
		}

		#endregion

		#region step 3 stuff
		async Task<DownloadViewModelState> DoDownload(UserControl p)
		{
			if (State != DownloadViewModelState.Downloading)
				throw new InvalidOperationException();

			var downloadProgressModel = new DownloadProgressViewModel();
			p.DataContext = downloadProgressModel;

			State = await Task.Run(() =>
			{
				samples = downloadProgressModel.DownloadSamples(device).Result;
				return DownloadViewModelState.Downloaded;
			});

			return State;
		}
		#endregion

		#region step 4 stuff
		async Task<DownloadViewModelState> DoPersist(UserControl p)
		{
			if (State != DownloadViewModelState.Persisting)
				throw new InvalidOperationException();

			var persistViewModel = new PersistViewModel();
			p.DataContext = persistViewModel;
			State = await Task.Run(() =>
			{
				persistViewModel.Persist(deviceDetailsInfo.SerialNumber, samples);
				return DownloadViewModelState.Persisted;
			});
			return State;
		}
		#endregion

		#region step 5 stuff
		DownloadViewModelState DoReset(UserControl p)
		{
			//device.ClearDataOnDevice();
			device.SetClock();
			State = DownloadViewModelState.Done;
			return State;
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

			this.WhenAnyValue(vm => vm.State,
				s => s == DownloadViewModelState.InfoRetrieved)
				.Select(b => deviceDetailsInfo)
				.ToProperty(this, vm => vm.DeviceDetailsInfo, deviceDetailsInfo);

		}

		void InitCommands()
		{
			//Canceling
			var canCancel = this.WhenAnyValue(vm => vm.State, (x) => x == DownloadViewModelState.Idle ||
				x == DownloadViewModelState.Detected ||
				x == DownloadViewModelState.InfoRetrieved ||
				x == DownloadViewModelState.Downloaded ||
				x == DownloadViewModelState.Done);
			Cancel = ReactiveCommand.Create<ICanClose, Unit>(DoCancel, canCancel);


			var canDetect = this.WhenAnyValue(vm => vm.State, x => x == DownloadViewModelState.Idle);
			Detect = ReactiveCommand.Create(DoDetect, canDetect);

			var canNext = this.WhenAnyValue(vm => vm.State, x => CanDoNext(x));
			Next = ReactiveCommand.Create<ICanNext, Unit>(DoNext, canNext);
		}

		bool CanDoNext(DownloadViewModelState state)
		{
			return state == DownloadViewModelState.Detected ||
				state == DownloadViewModelState.InfoRetrieved ||
				state == DownloadViewModelState.Downloaded ||
				state == DownloadViewModelState.Persisted;
		}

		void HandleStateTransition(UserControl p)
		{
			switch (State) {
				case DownloadViewModelState.RetrievingInfo:
					DoRetrieveInfo(p);
					break;
				case DownloadViewModelState.Downloading:
					DoDownload(p);
					break;
				case DownloadViewModelState.Persisting:
					DoPersist(p);
					break;
				case DownloadViewModelState.Resetting:
					DoReset(p);
					break;
				case DownloadViewModelState.Done:
					this.RaisePropertyChanged("CancelActionText");
					break;
				default:
					throw new InvalidOperationException($"Nothing to launch when state is {State}");
			}
		}


		Unit DoNext(ICanNext canNext)
		{
			var p = canNext.Next();

			State = State + 1;
			HandleStateTransition(p);

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
