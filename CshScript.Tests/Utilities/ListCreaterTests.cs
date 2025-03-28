namespace CshScript.Tests.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CshScript.Models;
using CshScript.Utilities;
using Xunit;


public class ListCreaterTests
{
    private string DownloadedPdf = "../output/DownloadedPdf's.xlsx";

    [Fact]
    public void Url()
    {
        // Given
        var expectedList = new List<PdfUrl> {
            new PdfUrl() { Brnummer = "BR1", Url = "url1", AlternativeUrl = "alturl1", Downloaded = true },
            new PdfUrl() { Brnummer = "BR2", Url = "url1", AlternativeUrl = null, Downloaded = true },
            new PdfUrl() { Brnummer = "BR3", Url = null, AlternativeUrl = null, Downloaded = false }
        };

        // When
        ListCreater.CreateList(expectedList);

        // Then
        Assert.True(File.Exists(DownloadedPdf));
        var actualList = ParseExcel(DownloadedPdf, expectedList.Count);

        foreach (PdfUrl actual in actualList)
        {
            var expected = expectedList.Where(e => e.Brnummer == actual.Brnummer).First();
            Assert.NotNull(expected);
            Assert.Equal(expected, actual);
        }

    }

    /// <summary>
    /// Used to read the values from the created reprot
    /// </summary>
    /// <param name="excelPath"></param>
    /// <param name="numberOfRows"></param>
    /// <returns></returns>
    private static List<PdfUrl> ParseExcel(string excelPath, int numberOfRows)
    {
        var urlList = new ConcurrentBag<PdfUrl>();
        using (var workbook = new XLWorkbook(excelPath))
        {
            var excelWorksheet = workbook.Worksheet(1);
            var rows = excelWorksheet.RowsUsed().Skip(1).Take(numberOfRows).ToList();
            var headers = excelWorksheet.Row(1).Cells().ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber);
            Parallel.ForEach(rows, row =>
            {
                urlList.Add(new PdfUrl
                {
                    Brnummer = row.Cell(headers["Brnummer"]).Value.ToString(),
                    Url = row.Cell(headers["Url"]).Value.ToString(),
                    AlternativeUrl = row.Cell(headers["AlternativeUrl"]).Value.ToString(),
                    Downloaded = row.Cell(headers["DownloadStatus"]).Value.ToString() == "Downloaded" ? true : false
                });
            });
            return urlList.ToList();
        }
    }
}