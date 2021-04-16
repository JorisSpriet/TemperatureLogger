namespace TemperatuurLogger.UI.ViewModels
{
	public enum DownloadViewModelState : int
	{
		/// <summary>
		/// No logger detected yet; waiting for it
		/// </summary>
		Idle = 0,
		/// <summary>
		/// Scanning for serial ports, and trying to find the logger
		/// </summary>
		Detecting = 1,
		/// <summary>
		/// Found logger; triggers transition to step 2
		/// </summary>
		Detected = 2,
		/// <summary>
		/// Getting info from the logger
		/// </summary>
		RetrievingInfo = 3,
		/// <summary>
		/// Showing the info
		/// </summary>
		InfoRetrieved = 4,
		/// <summary>
		/// Downloading from the logger
		/// </summary>
		Downloading = 5,
		/// <summary>
		/// Download done, triggers transition to step 4
		/// </summary>
		Downloaded = 6,
		/// <summary>
		/// Storing data into database
		/// </summary>
		Persisting = 7,
		/// <summary>
		/// Data stored into database, triggers transition to step 5
		/// </summary>
		Persisted = 8,
		Resetting = 9,
		Done = 10
	}
}
