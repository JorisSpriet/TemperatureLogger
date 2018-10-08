using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatuurLogger
{
	public class ExportPresenter : IExportPresenter, INotifyPropertyChanged
	{
		public IExportView View { get ; private set ; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ExportPresenter(IExportView view)
		{
			View = view;
		}
	}
}
