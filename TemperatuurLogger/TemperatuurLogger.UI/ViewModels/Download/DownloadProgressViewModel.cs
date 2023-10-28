using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TemperatuurLogger.Model;
using TemperatuurLogger.Protocol;

namespace TemperatuurLogger.UI.ViewModels
{
	public class DownloadProgressViewModel : ViewModelBase
	{
		private int percentage;
		private string downloadProgress="";
		

		public int Percentage {
			get => percentage;
			set => this.RaiseAndSetIfChanged(ref percentage, value);
		}
		
		public string DownloadProgress
		{
			get => downloadProgress;
			set => this.RaiseAndSetIfChanged(ref downloadProgress, value);
		}



		public Task<DeviceSample[]> DownloadSamples(IDevice device)
		{
			return device.GetSamplesFromDevice(UpdateUI);

		}

		private void UpdateUI(int percentageCompleted, int bytesReceived, int totalNumberOfBytes)
		{
			Dispatcher.UIThread.InvokeAsync(() =>
			{
				Percentage = percentageCompleted;
				DownloadProgress = $"{bytesReceived} van {totalNumberOfBytes}";
			});
		}

		public DownloadProgressViewModel()
		{
			
		}

		
	}
}
