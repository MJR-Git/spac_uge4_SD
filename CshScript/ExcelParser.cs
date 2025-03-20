using ClosedXML.Excel;


namespace CshScript
{
    public class ExcelParser()
    {
        public static List<PdfUrl> ParseExcel(string excelPath)
        {
            List<PdfUrl> urlList = new List<PdfUrl>();
            using (var workbook = new XLWorkbook(excelPath))
            {
                var excelWorksheet = workbook.Worksheet(1);
                var rows = excelWorksheet.RowsUsed().Skip(1);
                var headers = excelWorksheet.Row(1).Cells().ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber);

                foreach (var row in rows.Take(500))
                {
                    urlList.Add(new PdfUrl
                    {
                        Brnummer = row.Cell(headers["BRnum"]).Value.ToString(),
                        Url = row.Cell(headers["Pdf_URL"]).Value.ToString(),
                        AlternativeUrl = row.Cell(headers["Report Html Address"]).Value.ToString()
                    });
                }
                return urlList;
            }
        }
    }
}