using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using TemperatuurLogger.Model;
using TemperatuurLogger.Protocol;
using Xtensive.Orm;

namespace TemperatuurLogger.UI.ViewModels
{
	class PersistViewModel : ViewModelBase
	{
		private int progressPercentage;		
		private string persistingProgress="";

		public int Percentage
		{
			get => progressPercentage;
			set => this.RaiseAndSetIfChanged(ref progressPercentage, value);
		}

		public string PersistingProgress
		{
			get => persistingProgress;
			set => this.RaiseAndSetIfChanged(ref persistingProgress, value);
		}

		public void Persist(string serialNumber, DeviceSample[] samples)
		{
			var years = samples.Select(x => x.TimeStamp.Year).Distinct()
				.Order()
				.ToList();

			foreach(var year in years) {
				var relevantSamples = 
					samples.Where(x => x.TimeStamp.Year== year)
					.OrderBy(x=>x.TimeStamp)
					.Select(x => (x.TimeStamp, x.Temperature))
					.ToArray();

				var y = year.ToString();
				MeasurementService.With(y, () =>
				{
					MeasurementService.Instance.RegisterDownloadedData(serialNumber, relevantSamples);
				});
			}				
		}

		private void ShowProgress(int nOfSamples, int nOfPersistedSamples)
		{
			Dispatcher.UIThread.InvokeAsync(() =>
			{
				Percentage = Convert.ToInt32(nOfPersistedSamples * 100 / nOfSamples);
				PersistingProgress = $"{nOfSamples} van  {nOfPersistedSamples} opgeslagen.";
			});
		}
		
	}
}
