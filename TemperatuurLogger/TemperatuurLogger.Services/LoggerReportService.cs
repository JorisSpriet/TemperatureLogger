using Microsoft.VisualBasic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TemperatuurLogger.Model;
using Xtensive.Orm;
using Xtensive.Orm.Rse;

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
                //.Where(dl => (dl.EndTime >= from && dl.EndTime < to) ||
                //             (dl.StartTime >= from && dl.StartTime < to))
                .Where(dl => dl.Logger == logger &&
                             ((from >= dl.StartTime && from < dl.EndTime) ||
                             (to >= dl.StartTime && to < dl.EndTime)))
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
                foreach (var d in data)
                {
                    if (d.Timestamp > endOfDay)
                    {
                        var avg = cnt > 0 ? sum / cnt : 0M;
                        results.Add(new LoggerReportData(startOfDay, avg));
                        sum = 0M;
                        cnt = 0;
                        startOfDay = endOfDay;
                        endOfDay = startOfDay.AddDays(1d);
                    }
                    cnt++;
                    sum += d.Temperature;
                }
                {
                    var avg = cnt > 0 ? sum / cnt : 0M;
                    results.Add(new LoggerReportData(startOfDay, avg));
                }
            }
            return results.ToArray();
        }
        const string DateFormat = "dd-MM-yyyy";
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
            return (directory, filename);
        }

        private string EnsureFileDoesNotExist(string directory, string filename)
        {
            var fullFileName = Path.Combine(directory, filename);

            if (!File.Exists(fullFileName))
                return filename;

            var fn = Path.GetFileNameWithoutExtension(filename);
            var ext = Path.GetExtension(filename);
            int seq = 1;
            do
            {
                filename = $"{fn} ({seq++}.{ext}";
                fullFileName = Path.Combine(directory, filename);
            } while ((File.Exists(fullFileName)));
            return filename;
        }

        public void GeneratePdfReport(string directory, string filename, LoggerReportModel report)
        {
            filename = EnsureFileDoesNotExist(directory, filename);

            var df = GetReportFileName(directory, filename, report);
            directory = df.directory;
            filename = df.filename;

            var from = report.From.ToString(DateFormat);
            var to = report.To.ToString(DateFormat);
            var title = $"Apotheek Sigrid Saelens BV - Temperaturen {report.Name}";
            var subtitle = $"{from}{(from != to ? " t.e.m." + to : "")}";

            int offset = 0;
            var elements = report.Data.
                Select(d => $"{d.Date.ToString("dd-MM-yyyy")}    {d.Temperature.ToString("F1")} °C")
                .ToList();

            QuestPDF.Settings.License = LicenseType.Community;

            const int take = 80;
            const int takehalf = 40;

            Document
                .Create(container =>
                    {
                        while (elements.Any())
                        {
                            var pageElements = elements.Take(take).ToList();
                            elements = elements.Skip(take).ToList();

                            container.Page(page =>
                            {
                                page.Size(PageSizes.A4);
                                page.Margin(2, Unit.Centimetre);
                                page.PageColor(Colors.White);
                                page.DefaultTextStyle(x => x.FontSize(20));

                                page.Header()
                                    .Text(td =>
                                    {
                                        td.Line(title).SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);
                                    });

                                page.Content().Element(container =>
                                {
                                    container.Row(row =>
                                    {

                                        row.RelativeItem().Column(stack =>
                                        {
                                            pageElements
                                                .Take(takehalf)
                                                .ToList()
                                                .ForEach(x =>
                                                    stack.Item()
                                                        .Text(x)
                                                        .FontFamily("Calibri")
                                                        .FontSize(10));
                                        });


                                        row.RelativeItem()
                                        .BorderLeft(1f)
                                        .Column(stack =>
                                        {
                                            pageElements
                                                .Skip(takehalf)
                                                .ToList()
                                                .ForEach(x =>
                                                    stack.Item()
                                                        .PaddingLeft(10f)
                                                        .Text(x)
                                                        .FontFamily("Calibri")
                                                        .FontSize(10));
                                        });
                                    });
                                });

                                page.Footer()
                                .BorderTop(1f, Unit.Point)
                                .Element(container =>
                                {
                                    container.Row(row =>
                                    {
                                        row.RelativeItem().Column(column =>
                                        {
                                            column.Item()
                                            .AlignLeft()
                                            .Text(subtitle)
                                            .FontSize(12);
                                        });

                                        row.RelativeItem().Column(column =>
                                        {
                                            column.Item()
                                            .AlignRight()
                                            .Text(x => x.CurrentPageNumber().FontSize(12));
                                        });
                                    });
                                });

                            });
                        }
                    })
                    .GeneratePdf(Path.Combine(directory, filename));
        }


    }
}
