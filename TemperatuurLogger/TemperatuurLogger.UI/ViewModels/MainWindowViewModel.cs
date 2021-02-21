using ReactiveUI;
using System.Reactive;
using TemperatuurLogger.UI.Views;

namespace TemperatuurLogger.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ReactiveCommand<IWindow, Unit> StartDownloadWizard { get; }

		private Unit StartDownload(IWindow mwv)
		{
			var dlv = new DownloadView();
			dlv.DataContext = new DownloadViewModel();
			dlv.ShowDialog(mwv);
			return Unit.Default;
		}

		public MainWindowViewModel()
		{
			StartDownloadWizard = ReactiveCommand.Create<IWindow,Unit>(StartDownload);
		}
	}
}
