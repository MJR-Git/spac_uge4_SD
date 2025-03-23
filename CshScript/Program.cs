using System.Diagnostics;
using CshScript.Models;
using CshScript.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

// Start a stopwatch to measure the execution time of the program
Stopwatch stopwatch = Stopwatch.StartNew();

// Build the configuration from the appsettings.Json file
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Json", optional: false, reloadOnChange: true);
IConfiguration configuration = builder.Build();

// Set up dependency injection
var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddHttpClient("pdfClient");
services.AddTransient<PdfDownloader>();
var serviceProvider = services.BuildServiceProvider();
var pdfDownloader = serviceProvider.GetRequiredService<PdfDownloader>();

// Retrieve configuration values
string excelPath = configuration["Paths:excelPath"] ?? throw new InvalidOperationException("Excel path is not configured.");
string outputPath = configuration["Paths:outputPath"] ?? throw new InvalidOperationException("Output path is not configured.");
int numberOfRows = configuration.GetValue<int>("Parsing:NumberOfRows");

Console.WriteLine("\nParsing excel-file...");
// Parse the Excel file to get a list of PDF URLs
List<PdfUrl> urlList = ExcelParser.ParseExcel(excelPath, numberOfRows);
Console.WriteLine("Parsing Complete\n");

Console.WriteLine("Downloading files...");
// Download the PDFs from the parsed URLs
await pdfDownloader.DownloadPdfsAsync(urlList, outputPath);
Console.WriteLine("Downloading complete\n");

Console.WriteLine("Creating list...");
// Create a list from the downloaded PDFs
ListCreater.CreateList(urlList);
Console.WriteLine("List created\n");

// Stop the stopwatch and print the elapsed time
stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);





