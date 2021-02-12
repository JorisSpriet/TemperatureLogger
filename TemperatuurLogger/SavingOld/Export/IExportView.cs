using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatuurLogger
{
	public interface IExportView
	{
		IExportPresenter Presenter { get; }
	}
}
