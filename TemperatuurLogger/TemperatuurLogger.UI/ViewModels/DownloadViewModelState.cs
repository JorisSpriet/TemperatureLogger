namespace TemperatuurLogger.UI.ViewModels
{
	public enum DownloadViewModelState
	{
		/// <summary>
		/// No logger detected yet; waiting for it
		/// </summary>
		Idle,
		/// <summary>
		/// Scanning for serial ports, and trying to find the logger
		/// </summary>
		Detecting,
		/// <summary>
		/// Found logger; triggers transition to step 2
		/// </summary>
		Detected,
		/// <summary>
		/// Getting info from the logger
		/// </summary>
		RetrievingInfo,
		/// <summary>
		/// Showing the info
		/// </summary>
		InfoRetrieved,
		/// <summary>
		/// Downloading from the logger
		/// </summary>
		Downloading,
		/// <summary>
		/// Download done, triggers transition to step 4
		/// </summary>
		Downloaded,
		/// <summary>
		/// Storing data into database
		/// </summary>
		Persisting,
		/// <summary>
		/// Data stored into database, triggers transition to step 5
		/// </summary>
		Persisted,
		Resetting,
		Done
	}
}
