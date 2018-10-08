using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UitlezenLogger.WizardPages;
using Xceed.Wpf.Toolkit;

namespace UitlezenLogger
{

    public interface IViewModel
    { }
	public interface IViewModel<T> : IViewModel where T : IView
    {
		
        T View { get; }
		
	}
}
