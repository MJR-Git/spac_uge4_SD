using System.Diagnostics;
using CshScript;
using Microsoft.Extensions.DependencyInjection;
using Polly;

var services = new ServiceCollection();
services.AddHttpClient("pdfClient");
services.AddTransient<PdfDownloader>();
    
var serviceProvider = services.BuildServiceProvider();

var pdfDownloader = serviceProvider.GetRequiredService<PdfDownloader>();

string excelPath = "../data/GRI_2017_2020 (1).xlsx";
string? existingPdfsPath = "output/dwn";
string? outputPath = "../output/dwn";

Stopwatch stopwatch = Stopwatch.StartNew();

var pipeline = new ResiliencePipelineBuilder()
    //.AddRetry(new RetryStrategyOptions
    //{
        //ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        //Delay = TimeSpan.FromSeconds(2),
        //MaxRetryAttempts = 2,
        //BackoffType = DelayBackoffType.Exponential,
        //UseJitter = true

    //})
    .AddConcurrencyLimiter(50, 250)
    .AddTimeout(TimeSpan.FromSeconds(60))
    .Build();

List<PdfUrl> urlList = ExcelParser.ParseExcel(excelPath);

await pdfDownloader.DownloadPdfsAsync(urlList, outputPath, pipeline);

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);





