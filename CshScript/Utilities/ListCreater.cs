using System.ComponentModel.DataAnnotations;
using ClosedXML.Excel;

namespace CshScript.Utilities
{
    public class ListCreater
    {
        public static void CreateList(List<Models.PdfUrl> urlList)
        {
            // Create a new workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add("Downloaded pdf's");

                worksheet.Cell(1, 1).Value = "Brnummer";
                worksheet.Cell(1, 2).Value = "Url";
                worksheet.Cell(1, 3).Value = "AlternativeUrl";
                worksheet.Cell(1, 4).Value = "DownloadStatus";

                // Add some data
                int i = 2;
                foreach (Models.PdfUrl url in urlList)
                {
                    worksheet.Cell(i, 1).Value = url.Brnummer;
                    worksheet.Cell(i, 2).Value = url.Url;
                    worksheet.Cell(i, 3).Value = url.AlternativeUrl;
                    worksheet.Cell(i, 4).Value = url.Downloaded ? "Downloaded" : "Not Downloaded";
                    i++;
                }
                // Save the workbook
                workbook.SaveAs("../output/DownloadedPdf's.xlsx");
            }
        }
    }
}