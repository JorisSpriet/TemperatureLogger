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
	class PersistViewModel : DatabaseBoundViewModelBase
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
			ExecuteTransactionally(() =>
			{
				var logger = Query.All<Logger>().Single(x => x.SerialNumber == serialNumber);
				var startTime = samples.Min(s => s.TimeStamp).Date;
				var endTime = samples.Max(s => s.TimeStamp).Date;
				var mdl = new MeasurementDownload(logger, DateTime.Now.Date, startTime, endTime);
				var c = 1;
				foreach(var sample in samples) {
					new Measurement(mdl, sample.TimeStamp, sample.Temperature);
					ShowProgress(samples.Length, c++);
				}
			});
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
