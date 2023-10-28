using Avalonia.Controls;

namespace TemperatuurLogger.UI
{
	public static class WindowExtensions
	{
		public static void ShowDialog(this Window window, IWindow owner)
		{
			var owningWindow = (Window)owner;
			window.ShowDialog(owningWindow);			
		}


	}
}
