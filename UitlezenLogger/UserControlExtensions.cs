using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace UitlezenLogger
{
	public static class UserControlExensions
	{
		public static WizardPage ToWizardPage(this FrameworkElement control)
		{
			var page = new WizardPage
			{
				Content = control
			};
			return page;
		}
	}
}
