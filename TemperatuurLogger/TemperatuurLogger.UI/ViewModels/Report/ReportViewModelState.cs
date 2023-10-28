namespace TemperatuurLogger.UI.ViewModels
{
	public enum ReportViewModelState : int
	{
		/// <summary>
		/// Data Entry
		/// </summary>
		DataEntry = 0,
		/// <summary>
		/// Scanning for serial ports, and trying to find the logger
		/// </summary>
		DataValidated = 1,
		/// <summary>
		/// Found logger; triggers transition to step 2
		/// </summary>
		Rendering = 2,
		/// <summary>
		/// Getting info from the logger
		/// </summary>
		Rendered = 3,
		/// <summary>
		/// Showing the info
		/// </summary>
		Printing = 4,
		/// <summary>
		/// Downloading from the logger
		/// </summary>
		Printed = 5,
		/// <summary>
		/// Report done
		/// </summary>
		Done = 6,
	}
}
