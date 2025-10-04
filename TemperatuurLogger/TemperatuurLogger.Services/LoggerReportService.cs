using System.Linq;
using System.Collections.Generic;
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
        public IEnumerable<LoggerReportSource> Getloggers(string year)
        {
            int y = int.Parse(year);
            var result = new List<LoggerReportSource>();

            DeviceService.With(() =>
                result.AddRange(DeviceService.Instance.GetAllLoggers()
                    .Select(l => new LoggerReportSource(l.Name, l.SerialNumber))));
            var s = new DateTime(y, 1, 1, 0, 0, 0);
            var e = new DateTime(y, 12, 31, 23, 59, 59);

            MeasurementService.With(year, () =>
            {
                foreach (var lrs in result)
                {
                    var dl = MeasurementService.Instance.GetDownloads(lrs.SerialNumber, s, e);

                    lrs.NoData = dl.Length == 0;
                    if (dl.Length > 0)
                    {
                        lrs.From = dl.OrderBy(x => x.StartTime).FirstOrDefault().StartTime;
                        lrs.To = dl.OrderByDescending(x => x.EndTime).FirstOrDefault().EndTime;
                    }
                }
            });

            return result;
        }

        public TemperatureLogger GetTemperatureLogger(string serialnumber)
        {
            return Query.All<TemperatureLogger>().Single(l => l.SerialNumber == serialnumber);
        }

        public LoggerReportData[] GetReportDataPerDay(string serialNumber, DateTime from, DateTime to)
        {
            //begin day to end of day.
            from = from.Date;
            to = to.Date.AddDays(1.0d);
            var year = from.Year.ToString();
            var results = new List<LoggerReportData>();

            MeasurementService.With(year, () =>
            {

                var downloads = Query.All<MeasurementDownload>()
                    .Where(dl => dl.SerialNumber == serialNumber &&
                                 (dl.StartTime <= to && dl.EndTime > from))
                    .OrderBy(dl => dl.StartTime)
                    .ToList();

                if (downloads.Count == 0)
                    throw new Exception("Geen temperaturen voor deze datums. Werd de logger al uitgelezen ?");


                var startOfDay = from.Date;
                var endOfDay = startOfDay.AddDays(1.0d);
                var sum = 0M;
                var cnt = 0;

                foreach (var download in downloads)
                {
                    var data = Query.All<Measurement>()
                                .Where(m => m.Download==download && m.Timestamp >= from && m.Timestamp < to)
                                .OrderBy(m => m.Timestamp)
                                .ToList();
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
                    
                }
                if(cnt>0) { 
                    var avg = sum / cnt;
                    results.Add(new LoggerReportData(startOfDay, avg));
                }
            });
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
