using Microsoft.VisualBasic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.Services
{
    public class LoggerReportService
    {
        public IEnumerable<LoggerReportSourceModel> GetAvailableData()
        {

            foreach (var logger in Query.All<Logger>().ToArray())
            {

                var q = Query.All<MeasurementDownload>()
                    .Where(md => md.Logger == logger);

                var from = q.OrderBy(x => x.ID).FirstOrDefault();
                if (from == null)
                    continue;
                var to = q.OrderByDescending(x => x.ID).First();
                var fromDate = from.StartTime.Date;
                var toDate = to.EndTime.Date;
                yield return new LoggerReportSourceModel(logger.Name, logger.SerialNumber, fromDate, toDate);
            }
        }

        public Logger GetTemperatureLogger(string serialnumber)
        {
            return Query.All<Logger>().Single(l => l.SerialNumber == serialnumber);
        }

        public LoggerReportData[] GetReportDataPerDay(string serialNumber, DateTime from, DateTime to)
        {
            var logger = GetTemperatureLogger(serialNumber);

            //begin day to end of day.
            from = from.Date;
            to = to.Date.AddDays(1.0d);

            var downloads = Query.All<MeasurementDownload>()
                .Where(dl => (dl.EndTime >= from && dl.EndTime < to) ||
                             (dl.StartTime >= from && dl.StartTime < to))
                .OrderBy(dl => dl.StartTime)
                .ToList();

            if (downloads.Count == 0)
                throw new Exception("Geen temperaturen voor deze datum. Werd de logger al uitgelezen ?");

            var results = new List<LoggerReportData>();
            var startOfDay = from.Date;
            var endOfDay = startOfDay.AddDays(1.0d);
            var sum = 0M;
            var cnt = 0;

            foreach (var download in downloads)
            {
                
                    var data = Query.All<Measurement>()
                      .Where(m => m.Download == download && m.Timestamp >= from && m.Timestamp < to);
                    foreach(var d in data)
                    {
                        if(d.Timestamp>endOfDay)
                        {
                            var avg = sum / cnt;
                            results.Add(new LoggerReportData(startOfDay, avg));
                            sum = 0M;
                            cnt = 0;
                            startOfDay=endOfDay;
                            endOfDay=startOfDay.AddDays(1d);
                        }
                        cnt++;
                        sum += d.Temperature;
                    }
            }
            return results.ToArray();
        }
        const string DateFormat = "dd\\/MM\\/yyyy";
        public (string directory, string filename) GetReportFileName(string directory, string filename, LoggerReportModel report)
        {
            
            if (string.IsNullOrWhiteSpace(directory))
                directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Temperaturen");
            if (string.IsNullOrWhiteSpace(filename))
            {
                var from = report.From.ToString(DateFormat);
                var to = report.To.ToString(DateFormat);
                filename = from != to ? $"{from} t.e.m. {to}.pdf" : $"{from}.pdf";
            }
            return (directory,filename);
        }

        public void GeneratePdfReport(string directory, string filename, LoggerReportModel report)
        {
            var df = GetReportFileName(directory,filename,report);
            directory=df.directory;
            filename = df.filename;

            var from = report.From.ToString(DateFormat);
            var to = report.To.ToString(DateFormat);
            var title = $"Temparaturen {report.Name} {from}{(from != to ? "-" + to : "")}";
            
            QuestPDF.Settings.License = LicenseType.Community;

            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        page.Header()
                            .Text($"Temperaturen {report.Name} {report.From}")
                            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Spacing(20);

                                x.Item().Text(string.Join("\n",report.Data.Select(d => $"{d.Date.ToString("dd\\/MM\\/yyyy")}  {d.Temperature}")));
                                //x.Item().Image(Placeholders.Image(200, 100));
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                })
                .GeneratePdf(Path.Combine(directory,filename));
        }


    }
}
