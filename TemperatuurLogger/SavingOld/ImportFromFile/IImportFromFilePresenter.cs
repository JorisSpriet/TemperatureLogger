using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ATALoggerLib;

namespace TemperatuurLogger
{
	public interface IImportFromFilePresenter
	{
		IImportFromFileView View { get;  }

		IObservable<FileLogItem> FilePreviewData { get; }

		ICommand LoadLogsFromFile { get; }

		ICommand StoreLogsFromFile { get; }
	}
}
