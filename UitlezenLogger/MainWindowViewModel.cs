using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UitlezenLogger.WizardPages;
using Xceed.Wpf.Toolkit;

namespace UitlezenLogger
{
	public class MainWindowViewModel
	{
		private WizardPage[] pages;

		public WizardPage[] WizardPages
		{
			get { return pages; }
		}

		private void InitPages()
		{
			var startPage = new StartPage().ToWizardPage();

          
			var connectingPage = new ConnectingPage().ToWizardPage();
			startPage.NextPage = connectingPage;
			startPage.CanCancel = true;
			startPage.CanSelectNextPage = true;

			var readingPage = new ReadLoggerPage().ToWizardPage();
			connectingPage.CanCancel = true;
			connectingPage.NextPage = readingPage;            

			var storeDataPage=new StoreDataPage().ToWizardPage();
			readingPage.NextPage = storeDataPage;
			
			var clearLoggerPage=new ClearLoggerPage().ToWizardPage();
			readingPage.NextPage = clearLoggerPage;

			var endPage=new EndPage().ToWizardPage(); ;
			clearLoggerPage.NextPage = endPage;
			endPage.PreviousPage = clearLoggerPage;
			endPage.CanFinish = true;


			pages=new WizardPage[]
			{
				startPage,
				connectingPage,
				readingPage,
				storeDataPage,
				clearLoggerPage,
				endPage
				
			};

		}

		public MainWindowViewModel()
		{
			InitPages();
		}
	}
}
