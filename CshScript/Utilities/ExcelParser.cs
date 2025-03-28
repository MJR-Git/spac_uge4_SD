using System.Collections.Concurrent;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace CshScript.Utilities
{
    public class ExcelParser
    {
        // Parses the specified Excel file to extract PDF URLs.

        public static List<Models.PdfUrl> ParseExcel(string excelPath, int numberOfRows)
        {
            // ConcurrentBag to store the parsed PDF URLs
            var urlList = new ConcurrentBag<Models.PdfUrl>();

            // Open the Excel workbook
            using (var workbook = new XLWorkbook(excelPath))
            {
                // Get the first worksheet in the workbook
                var excelWorksheet = workbook.Worksheet(1);

                // Get the rows to be processed, skipping the header row and taking the specified number of rows
                var rows = excelWorksheet.RowsUsed().Skip(500).Take(numberOfRows).ToList(); // TODO: 1 insted of 500, so the first 499 of nonheadder rows are not skiped

                // Create a dictionary to map header names to their column numbers
                var headers = excelWorksheet.Row(1).Cells().ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber);

                // Process each row in parallel to extract PDF URL data
                Parallel.ForEach(rows, row =>
                {
                    urlList.Add(new Models.PdfUrl
                    {
                        Brnummer = row.Cell(headers["BRnum"]).Value.ToString(),
                        Url = row.Cell(headers["Pdf_URL"]).Value.ToString(),
                        AlternativeUrl = row.Cell(headers["Report Html Address"]).Value.ToString()
                    });
                });

                // Return the list of parsed PDF URLs
                return urlList.ToList();
            }
        }
    }
}