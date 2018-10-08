using System.ComponentModel;

namespace TemperatuurLogger
{
	public class ImportFromLoggerPresenter : IImportFromLoggerPresenter, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ImportLoggerState state;

		public ImportLoggerState State
		{
			get { return state; }
			set
			{
				state = value;
				RaisePropertyChanged("State");
			}
		}
		public IImportFromLoggerView View { get; private set; }

		private void RaisePropertyChanged(string v)
		{
			if (PropertyChanged == null)
				return;
			PropertyChanged(this, new PropertyChangedEventArgs(v));
		}

		public ImportFromLoggerPresenter(IImportFromLoggerView view)
		{
			View = view;
			view.Presenter = this;

		}
	}
}
