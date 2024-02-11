using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TemperatuurLogger.UI.Views
{
	public class IsDetectedStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !value?.Equals(TemperatuurLogger.UI.ViewModels.DownloadViewModelState.Detected);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
