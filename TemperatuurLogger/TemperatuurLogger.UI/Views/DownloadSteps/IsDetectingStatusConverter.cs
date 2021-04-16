using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TemperatuurLogger.UI.Views
{
	public class IsDetectingStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.Equals(TemperatuurLogger.UI.ViewModels.DownloadViewModelState.Detecting);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
