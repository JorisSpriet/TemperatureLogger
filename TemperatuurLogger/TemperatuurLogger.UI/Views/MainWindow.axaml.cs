using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TemperatuurLogger.UI.Views
{
	public class MainWindow : Window, IWindow
	{
		public MainWindow()
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
