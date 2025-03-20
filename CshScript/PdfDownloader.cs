using Polly;


namespace CshScript
{
    public class PdfDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PdfDownloader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task DownloadPdfsAsync(List<PdfUrl> urlList, string downloadPath, ResiliencePipeline pipeline)
        {
            using HttpClient client = _httpClientFactory.CreateClient("pdfClient");

            List<Task> downloadTasks = urlList
                .Where(url => !File.Exists(Path.Combine(downloadPath, $"{url.Brnummer}.pdf")))
                .Select(url => DownloadPdf(url, client, downloadPath, pipeline))
                .ToList();
            await Task.WhenAll(downloadTasks);
        }

        static async Task DownloadPdf(PdfUrl url, HttpClient client, string downloadPath, ResiliencePipeline pipeline)
        {
            
            string pdfPath = Path.Combine(downloadPath, $"{url.Brnummer}.pdf");
            if (await TryDownloadPdf(url.Url, pdfPath, client, pipeline))
            {
                url.Downloaded = true;
                return;
            }
            if (url.AlternativeUrl != null)
            {
                if (await TryDownloadPdf(url.AlternativeUrl, pdfPath, client, pipeline))
                {
                    url.Downloaded= true;
                    return;
                };
            }

        }

        static async Task<bool> TryDownloadPdf(string url, string pdfPath, HttpClient client, ResiliencePipeline pipeline)
        {
            try
            {
                if (!await IsPDFHeader(url, client, pipeline))
                {
                    return false;
                }

                byte[] pdfBytes = await pipeline.ExecuteAsync(async ct => await client.GetByteArrayAsync(url, ct));
                await File.WriteAllBytesAsync(pdfPath, pdfBytes);
                Console.WriteLine($"Successfully downloaded PDF from URL: {url}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading PDF from URL: {url}. Exception: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> IsPDFHeader(string url, HttpClient client, ResiliencePipeline pipeline)
        {
            try
            {
                using HttpResponseMessage response = await pipeline.ExecuteAsync(async ct => await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct));
                response.EnsureSuccessStatusCode();

                byte[] buffer = new byte[5];
                await using Stream contentStream = await response.Content.ReadAsStreamAsync();
                int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                return bytesRead >= buffer.Length &&
                       buffer[0] == 0x25 &&
                       buffer[1] == 0x50 &&
                       buffer[2] == 0x44 &&
                       buffer[3] == 0x46 &&
                       buffer[4] == 0x2D;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}