using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UitlezenLogger
{
	public class ResourceProvider : INotifyPropertyChanged
	{
		private static Properties.Resources resources = new Properties.Resources();

		public Properties.Resources Resources
		{
			get { return resources; }
			set { RaisePropertyChanged("Resources"); }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
