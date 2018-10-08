using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatuurLogger
{
	public interface IImportFromFileView
	{
		IImportFromFilePresenter GetPresenter();
	}
}
