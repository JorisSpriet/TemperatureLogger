using System;
using System.Linq;
using System.Collections.Generic;
using TemperatuurLogger.Model;
using Xtensive.Orm;
using TemperatuurLogger.Services;
using Avalonia.Controls;
using ReactiveUI;

namespace TemperatuurLogger.UI.ViewModels
{
    public partial class ReportViewModel
    {

        private string directory;
        private string filename;

        public string Directory
        {
            get => directory;
            set
            {
                this.RaiseAndSetIfChanged(ref directory, value);
            }
        }

        public string FileName
        {
            get => filename;
            set => this.RaiseAndSetIfChanged(ref filename, value);
        }

        private void ChooseDestination()
        {

            var report = new LoggerReportModel(SelectedLogger.Name, SelectedLogger.SerialNumber, FromDate.DateTime, EndDate.DateTime, null);
            var df = new LoggerReportService().GetReportFileName(Directory, FileName, report);
            Directory = df.directory;
            FileName = df.filename;
            //fill in default directory (settings)
            //fill in auto-generated filename

        }
        private void DoRender(UserControl p)
        {
            var svc = new LoggerReportService();
            LoggerReportData[] data = svc.GetReportDataPerDay(SelectedLogger.SerialNumber, FromDate.DateTime, EndDate.DateTime);
            var report = new LoggerReportModel(SelectedLogger.Name,
                SelectedLogger.SerialNumber,
                FromDate.DateTime,
                EndDate.DateTime,
                data);

            svc.GeneratePdfReport(Directory, FileName, report);

            State = ReportViewModelState.Done;

        }
    }
}
