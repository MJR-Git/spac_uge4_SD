namespace CshScript.Tests.Utilities;

using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using CshScript.Models;
using CshScript.Tests.TestUtilitys;
using CshScript.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public PdfDownloaderTests()
    {
        var mockHttp = CreateMock();
        var services = new ServiceCollection();
        services.AddHttpClient("pdfClient").ConfigurePrimaryHttpMessageHandler(() => mockHttp);
        services.AddTransient<PdfDownloader>();
        pdfDownloader = services.BuildServiceProvider().GetRequiredService<PdfDownloader>();
        pdfs = ExcelParser.ParseExcel(excelPath, 2);
        // todo: clean dir
    }

    private static MockHttpMessageHandler CreateMock()
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
                .Respond("application/html", new MemoryStream(File.ReadAllBytes($"{pathIn}/BR50014.html"))); // Respond with JSON

        return mockHttp;
    }

    public void Dispose()
    {
        pdfDownloader = null;
        pdfs = null;
    }

    [Fact]
    public async Task DownloadOnlyPdfs()
    {
        // Given
        string pdfPath1 = $"{pathOut}/BR50418.pdf";
        string pdfPath2 = $"{pathOut}/BR50968.pdf";
        string pdfPath3 = $"{pathOut}/BR50014.pdf";

        // When
        await pdfDownloader!.DownloadPdfsAsync(pdfs!, pathOut);

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

        var noEmptyAlternativeUrls = pdfs!.Where(p => p.AlternativeUrl != "").ToList();

        // When
        await pdfDownloader!.DownloadPdfsAsync(noEmptyAlternativeUrls!, pathOut);

        // Then
        Assert.True(File.Exists(pdfPath1));
        Assert.True(File.Exists(pdfPath2));

    }
    [Fact]
    public void FailInTryDownlad()
    {
        // Given
    
        // When
    
        // Then
    }
}
