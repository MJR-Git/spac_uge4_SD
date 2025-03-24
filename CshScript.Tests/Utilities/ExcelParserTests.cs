namespace CshScript.Tests.Utilities;
using Xunit;
using CshScript.Utilities;
using CshScript.Tests.TestUtils;
using CshScript.Tests.TestUtilitys;

public class ExcelParserTests
{

    [Fact]
    public void ParseExcel()
    {
        // Given
        var path = Path.Combine(SlnPath.TryGetSolutionDirectoryInfo().FullName, "CshScript.Tests\\Resourses\\pdf.xlsx");
        var numberOfRows = 2;

        // When
        var actual = ExcelParser.ParseExcel(path, numberOfRows);
        Thread.Sleep(10);

        // Then
        Assert.Equal(numberOfRows, actual.Count);


    }
    [Fact]
    public void ExcelNotFound()
    {
        // Given
        string path = "notfound.xlsx";
        var numberOfRows = 2;
        // When

        // Then
        Assert.Throws<FileNotFoundException>(() =>
        {
            var actual = ExcelParser.ParseExcel(path, numberOfRows);
        });
    }
}
