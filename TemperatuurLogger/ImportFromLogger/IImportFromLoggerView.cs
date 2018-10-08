using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemperatuurLogger
{
	public interface IImportFromLoggerView
	{
		IImportFromLoggerPresenter Presenter { get; set; }
	}
}
