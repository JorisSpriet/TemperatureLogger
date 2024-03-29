using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TemperatuurLogger.UI.Views
{
	public partial class ReportView : Window, ICanClose, ICanNext
	{
		public ReportView()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		public UserControl Next()
		{
			var carousel = this.FindControl<Carousel>("carousel");
			carousel.Next();
			return carousel.SelectedItem as UserControl;
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
