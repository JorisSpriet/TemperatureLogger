using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TemperatuurLogger
{
	public class ImportFromFilePresenter : IImportFromFilePresenter, INotifyPropertyChanged
	{
		public IImportFromFileView View { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public IObservable<ATALoggerLib.FileLogItem> FilePreviewData
		{
			get { throw new NotImplementedException(); }
		}

		private ICommand loadFileCommand;
		private ICommand storeFileCommand;


		public ICommand LoadLogsFromFile
		{
			get { return loadFileCommand; }
		}

		public ICommand StoreLogsFromFile
		{
			get { return loadFileCommand; }
		}

		public ImportFromFilePresenter(IImportFromFileView view)
		{
			View = view;
		}

	}
}
