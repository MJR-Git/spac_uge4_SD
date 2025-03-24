namespace CshScript.Tests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            new PdfUrl() { Brnummer = "BR2", Url = "url1", AlternativeUrl = "alturl1", Downloaded = true },
            new PdfUrl() { Brnummer = "BR3", Url = "url1", AlternativeUrl = "alturl1", Downloaded = true }
        };

        // When
        ListCreater.CreateList(expectedList);

        // Then
        var actualList = ExcelParser.ParseExcel(DownloadedPdf, 1);
        Assert.Equal(3, actualList.Count);
        foreach (var actualPdf in actualList)
        {
            var expectedPdf = expectedList.Where(p => p.Brnummer == actualPdf.Brnummer).First();
            Assert.NotNull(expectedPdf);
            Assert.Equal(actualPdf, expectedPdf);
        }

    }

    [Fact]
    public void AltUrl()
    {
        // Given

        // When

        // Then
    }
    [Fact]
    public void Failed()
    {
        // Given

        // When

        // Then
    }

}