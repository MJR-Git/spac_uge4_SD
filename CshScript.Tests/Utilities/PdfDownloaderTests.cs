namespace CshScript.Tests.Utilities;

using System.Diagnostics.Tracing;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CshScript.Models;
using CshScript.Tests.TestUtilitys;
using CshScript.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Moq;
using Moq.Protected;
using RichardSzalay.MockHttp;
using Xunit.Sdk;

/// <summary>
/// This test class assuemes that the excel parser works
/// </summary>
public class PdfDownloaderTests : IDisposable
{
    private static readonly string excelPath = Path.Combine(SlnPath.TryGetSolutionDirectoryInfo().FullName, "CshScript.Tests/Resourses/pdf.xlsx");
    private static readonly string pathIn = Path.Combine(SlnPath.TryGetSolutionDirectoryInfo().FullName, "CshScript.Tests/Resourses/pdfs_in");
    private static readonly string pathOut = Path.Combine(SlnPath.TryGetSolutionDirectoryInfo().FullName, "CshScript.Tests/Resourses/pdfs_out");
    PdfDownloader? pdfDownloader { get; set; }
    List<PdfUrl>? pdfs { get; set; }
    CancellationTokenSource cansel { get; set; }
    MockHttpMessageHandler mockHttp { get; set; }
    public PdfDownloaderTests()
    {
        cansel = new CancellationTokenSource();
        mockHttp = CreateMock();

        var services = new ServiceCollection();
        services.AddHttpClient("pdfClient").ConfigurePrimaryHttpMessageHandler(() => mockHttp);

        services.AddTransient<PdfDownloader>();
        pdfDownloader = services.BuildServiceProvider().GetRequiredService<PdfDownloader>();
        pdfs = ExcelParser.ParseExcel(excelPath, 5);

    }

    private MockHttpMessageHandler CreateMock()
    {
        var mockHttp = new MockHttpMessageHandler();

        // Setup a respond for the user api (including a wildcard in the URL)
        // PDFs
        mockHttp.When("https://www.aramex.com/docs/default-source/default-document-library/annual-report-2016--en.pdf")
                .Respond("application/pdf", new MemoryStream(File.ReadAllBytes($"{pathIn}/BR50481.pdf")));
        mockHttp.When("https://www.bhp.com/-/media/documents/investors/annual-reports/2017/bhpsustainabilityreport2017.pdf")
                .Respond("application/pdf", new MemoryStream(File.ReadAllBytes($"{pathIn}/BR50968.pdf")));
        // HTML
        mockHttp.When("http://arpeissig.at/wp-content/uploads/2016/02/D7_NHB_ARP_Final_2.pdf")
                .Respond("application/html", new MemoryStream(File.ReadAllBytes($"{pathIn}/BR50014.html")));

        return mockHttp;
    }


    public void Dispose()
    {
        pdfDownloader = null;
        pdfs = null;
        mockHttp.Dispose();
        Array.ForEach(Directory.GetFiles(pathOut), File.Delete);

    }

    /// <summary>
    /// This tests if the program does not try to make pdfs out of html
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DownloadOnlyPdfs()
    {
        // Given
        string pdfPath1 = $"{pathOut}/BR50481.pdf";
        string pdfPath2 = $"{pathOut}/BR50968.pdf";
        string pdfPath3 = $"{pathOut}/BR50014.pdf";
        pdfs = pdfs!.Where(p => !p.Brnummer.Equals("BR52291")).ToList();
        // When
        await pdfDownloader!.DownloadPdfsAsync(pdfs, pathOut);

        // Then
        Assert.True(File.Exists(pdfPath1));
        Assert.True(File.Exists(pdfPath2));
        Assert.False(File.Exists(pdfPath3));
    }
    [Fact]
    public async Task NoEmptyAlternetiveUrls()
    {
        // Given
        string pdfPath1 = $"{pathOut}/BR50481.pdf";
        string pdfPath2 = $"{pathOut}/BR50968.pdf";

        var noEmptyAlternativeUrls = pdfs!.Where(p =>
                p.AlternativeUrl != "")
            .ToList();

        // When
        await pdfDownloader!.DownloadPdfsAsync(noEmptyAlternativeUrls!, pathOut);

        // Then
        Assert.True(File.Exists(pdfPath1));
        Assert.True(File.Exists(pdfPath2));

    }


    [Fact]
    public async Task FailInTryDownlad()
    {
        // Given
        pdfs = pdfs!.Where(p => p.Brnummer.Equals("BR52291")).ToList();
        var request = mockHttp.When(pdfs[0].Url!);

        Stream pdfStream = new MemoryStream(File.ReadAllBytes($"{pathIn}/BR50968.pdf"));
        request.Respond(async () =>
        {
            await Task.Delay(10);
            var response = new HttpResponseMessage();

            response.Content = new StreamContent(pdfStream);
            response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return response;
        });

        pdfDownloader!.DownloadPdfsAsync(pdfs, pathOut);
        while (pdfStream.CanSeek && pdfStream.Position == 0)
        {
            await Task.Delay(1);
        }
        pdfStream.Close();

        Assert.Empty(Directory.GetFiles(pathOut));

    }
    
}
