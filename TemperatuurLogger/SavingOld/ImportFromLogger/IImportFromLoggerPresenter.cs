using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ATALoggerLib;

namespace TemperatuurLogger
{
	public interface IImportFromLoggerPresenter
	{
		ImportLoggerState State { get;}

		IImportFromLoggerView View { get; }

		
	}
}
