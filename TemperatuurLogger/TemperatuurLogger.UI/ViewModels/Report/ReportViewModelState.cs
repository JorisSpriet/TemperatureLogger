namespace TemperatuurLogger.UI.ViewModels
{
	public enum ReportViewModelState : int
	{
		/// <summary>
		/// Data entry: user must select a logger and from/to date
		/// </summary>
		DataEntry = 0,

        /// <summary>
        /// A logger and a from/to date is selected.
        /// </summary>
        DataEntered=1,

		/// <summary>
		/// Logger and time period was selected. User can select target file dir and name.
		/// </summary>
		ReportGeneration = 2,
        ReportGenerating = 3,
				/// <summary>
		/// Done.
		/// </summary>
		Done = 4,
	}
}
