using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CommandLine;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace GtfsXlsxCli
{
    public class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('x', "to-xlsx", Required = false, HelpText = "Convert from GTFS static to an Excel workbook")]
            public bool? ToXlsx { get; set; }

            [Option('f', "from", Required = true, HelpText = "File to read from")]
            public string From { get; set; }

            [Option('t', "to", Required = true, HelpText = "File to write to")]
            public string To { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    using (var serviceProvider = new ServiceCollection()
                        .AddLogging(config => config
                            .AddConsole()
                            .SetMinimumLevel(options.Verbose ? LogLevel.Trace : LogLevel.Warning))
                        .BuildServiceProvider())
                    {
                        if (DetermineToXlsx(options))
                        {
                            GtfsToXlsx(options, serviceProvider);
                        }
                        else
                        {
                            XlsxToGtfs(options, serviceProvider);
                        }
                    }
                });
        }

        private static bool DetermineToXlsx(Options o)
        {
            if (o.ToXlsx != null) return o.ToXlsx.Value;

            if (o.To.EndsWith(".xslx")) return true;
            if (o.From.EndsWith(".zip")) return true;

            return false;
        }

        private static void XlsxToGtfs(Options o, ServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation($"Reading {o.From}");
            logger.LogInformation($"Writing {o.To}");
            using (var xlsx = new ExcelPackage(new FileInfo(o.From)))
            using (var to = new FileStream(o.To, FileMode.Create))
            using (var gtfs = new ZipArchive(to, ZipArchiveMode.Create))
            {
                foreach (var sheet in xlsx.Workbook.Worksheets.OrderBy(s => s.Name))
                {
                    logger.LogInformation($"Reading data from sheet {sheet.Name}");
                    logger.LogInformation($"Writing {sheet.Name}");
                    var entry = gtfs.CreateEntry(sheet.Name);
                    using (var file = entry.Open())
                    using (var writer = new StreamWriter(file))
                    using (var csv = new CsvWriter(writer))
                    {
                        foreach (var row in Enumerable.Range(1, sheet.Dimension.End.Row))
                        {
                            foreach (var column in Enumerable.Range(1, sheet.Dimension.End.Column))
                            {
                                csv.WriteField(sheet.Cells[row, column].Text);
                            }
                            csv.NextRecord();
                        }
                    }
                }
            }
        }

        private static void GtfsToXlsx(Options o, ServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation($"Unzipping {o.From}");
            using (var from = new FileStream(o.From, FileMode.Open))
            using (var gtfs = new ZipArchive(from, ZipArchiveMode.Read))
            using (var xlsx = new ExcelPackage())
            using (var to = new FileStream(o.To, FileMode.Create))
            {
                foreach (var entry in gtfs.Entries.OrderBy(e => e.Name))
                {
                    logger.LogInformation($"Reading {entry.Name}");
                    using (var file = entry.Open())
                    using (var reader = new StreamReader(file))
                    using (var csv = new CsvReader(reader))
                    using (var dr = new CsvDataReader(csv))
                    {
                        logger.LogInformation($"Loading data into sheet {entry.Name}");
                        var table = new DataTable();
                        table.Load(dr);
                        var sheet = xlsx.Workbook.Worksheets.Add(entry.Name);
                        sheet.Cells.LoadFromDataTable(table, true);
                    }
                }

                logger.LogInformation($"Writing {o.To}");
                xlsx.SaveAs(to);
            }
        }
    }
}
