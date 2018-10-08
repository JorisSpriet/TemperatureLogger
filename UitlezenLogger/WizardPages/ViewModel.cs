using System.Windows.Controls;
using Prism.Mvvm;

namespace UitlezenLogger
{
	public class ViewModel<T> : BindableBase, IViewModel<T> where T : IView
    {

        public virtual T View { get; }

        public ViewModel(T view)
        {
            View = view;
        }
		
	}
}
