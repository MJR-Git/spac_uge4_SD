using System.Diagnostics;
using CshScript;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;

Stopwatch stopwatch = Stopwatch.StartNew();

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Json", optional: false, reloadOnChange: true);
IConfiguration configuration = builder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddHttpClient("pdfClient");
services.AddTransient<PdfDownloader>();
var serviceProvider = services.BuildServiceProvider();
var pdfDownloader = serviceProvider.GetRequiredService<PdfDownloader>();

string excelPath = configuration["Paths:excelPath"] ?? throw new InvalidOperationException("Excel path is not configured.");
string existingPdfsPath = configuration["existingPdfsPath"] ?? string.Empty;
string outputPath = configuration["Paths:outputPath"] ?? throw new InvalidOperationException("Output path is not configured.");

Console.WriteLine("\nParsing excel-file...");
List<PdfUrl> urlList = ExcelParser.ParseExcel(excelPath);
Console.WriteLine("Parsing Complete\n");

Console.WriteLine("Downloading files...");
await pdfDownloader.DownloadPdfsAsync(urlList, outputPath);
Console.WriteLine("Downloading complete\n");

Console.WriteLine("Creating list...");
ListCreater.CreateList(urlList);
Console.WriteLine("List created\n");

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);





