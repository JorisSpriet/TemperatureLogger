using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TemperatuurLogger.UI.Views
{
	public class DownloadView : Window, ICanClose
	{
		public DownloadView()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
