using System;
using System.Linq;
using System.Collections.Generic;
using TemperatuurLogger.Model;
using Xtensive.Orm;
using TemperatuurLogger.Services;
using Avalonia.Controls;

namespace TemperatuurLogger.UI.ViewModels
{
	public partial class ReportViewModel
	{
		public string Directory { get; set; }

        public string FileName {  get; set; }

        private void ChooseDestination()
        {

            var report = new LoggerReportModel(SelectedLogger.Name, SelectedLogger.SerialNumber, FromDate, EndDate, null);
            var df = new LoggerReportService().GetReportFileName(Directory, FileName, report);
            Directory =df.directory;
            FileName = df.filename;
            //fill in default directory (settings)
            //fill in auto-generated filename

        }
        private void DoRender(UserControl p)
        {
            var svc = new LoggerReportService();
            LoggerReportData[] data = null;

            ExecuteTransactionally(() => data= svc.GetReportDataPerDay(SelectedLogger.SerialNumber, FromDate, EndDate));
            var report = new LoggerReportModel(SelectedLogger.Name,
                SelectedLogger.SerialNumber,
                FromDate,
                EndDate,
                data);
            
                svc.GeneratePdfReport(Directory,FileName, report);

        }
    }
}
