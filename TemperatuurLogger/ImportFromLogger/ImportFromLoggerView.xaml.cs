using System.Windows;
using System.Windows.Controls;

namespace TemperatuurLogger
{
	/// <summary>
	/// Interaction logic for ImportFromLoggerView.xaml
	/// </summary>
	public partial class ImportFromLoggerView : UserControl, IImportFromLoggerView
	{

		public IImportFromLoggerPresenter Presenter
		{
			get { return this.DataContext as IImportFromLoggerPresenter; }
			set { DataContext = value; }
		}
	

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Button1 Clicked !");
		}

		public ImportFromLoggerView()
		{
			InitializeComponent();
		}


	}
}
