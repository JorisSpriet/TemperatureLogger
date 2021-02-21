using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TemperatuurLogger.UI.Views
{
	public class DownloadView : Window, ICanClose, ICanNext
	{
		public DownloadView()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		public void Next()
		{
			this.FindControl<Carousel>("carousel").Next();

		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);			
		}
	}
}
