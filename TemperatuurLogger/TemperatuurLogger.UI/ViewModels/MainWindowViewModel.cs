using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;
using TemperatuurLogger.UI.Views;

namespace TemperatuurLogger.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ReactiveCommand<IWindow, Unit> StartDownloadWizardCommand { get; }

		public ReactiveCommand<IWindow, Unit> StartReportCommand { get; }

		public ReactiveCommand<IWindow,Unit> ExitCommand { get;}

		public ReactiveCommand<IWindow,Unit> SettingsCommand {  get; }

		private Unit StartDownloadWizard(IWindow mwv)
		{
			var dlv = new DownloadView();
			dlv.DataContext = new DownloadViewModel();
			dlv.ShowDialog(mwv);
			return Unit.Default;
		}
		private Unit Exit(IWindow mwv)
		{
			(mwv as Avalonia.Controls.Window).Close();
			return Unit.Default;
		}

		private Unit StartReport(IWindow mwv)
		{
			var rvm = new ReportViewModel();
			var dlv = new ReportView();
			dlv.DataContext = rvm;
			dlv.ShowDialog(mwv);
			return Unit.Default;
		}

        private Unit ShowSettings(IWindow mwv)
        {
            var svm = new LoggerSettingsViewModel();
            var sv = new LoggerSettingsView();
            sv.DataContext = svm;
            sv.ShowDialog(mwv);
            return Unit.Default;
        }

        public MainWindowViewModel()
		{
			StartDownloadWizardCommand = ReactiveCommand.Create<IWindow,Unit>(StartDownloadWizard);
			StartReportCommand = ReactiveCommand.Create<IWindow, Unit>(StartReport);
			SettingsCommand = ReactiveCommand.Create<IWindow, Unit>(ShowSettings);
			ExitCommand = ReactiveCommand.Create<IWindow,Unit>(Exit);

		}
	}
}
