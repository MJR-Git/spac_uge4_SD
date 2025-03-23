using Polly;
using Polly.Retry;

namespace CshScript.Utilities
{
    public class PdfDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PdfDownloader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        // Initiates the download of PDFs from a list of URLs, ensuring resilience and concurrency control.
        public async Task DownloadPdfsAsync(List<Models.PdfUrl> urlList, string downloadPath)
        {
            // Configure a resilience pipeline with retry, concurrency limiter, and timeout strategies
            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                    Delay = TimeSpan.FromSeconds(1.5),
                    MaxRetryAttempts = 2,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                })
                .AddConcurrencyLimiter(30, 500)
                .AddTimeout(TimeSpan.FromSeconds(80))
                .Build();

            // Create an HTTP client for downloading PDFs
            HttpClient client = _httpClientFactory.CreateClient("pdfClient");

            // Initiate parallel download tasks for each URL that hasn't been downloaded yet
            List<Task> downloadTasks = urlList
                .Where(url => !File.Exists(Path.Combine(downloadPath, $"{url.Brnummer}.pdf")))
                .Select(url => DownloadPdf(url, client, downloadPath, pipeline))
                .ToList();
            await Task.WhenAll(downloadTasks);
        }


        //Attempts to download a PDF from the primary or alternative URL, marking the URL as downloaded upon success.
        static async Task DownloadPdf(Models.PdfUrl url, HttpClient client, string downloadPath, ResiliencePipeline pipeline)
        {
            string pdfPath = Path.Combine(downloadPath, $"{url.Brnummer}.pdf");
            if (url.Url != null)
            {
                if (await TryDownloadPdf(url.Url, pdfPath, client, pipeline))
                {
                    url.Downloaded = true;
                    return;
                }
            }
            if (url.AlternativeUrl != null)
            {
                if (await TryDownloadPdf(url.AlternativeUrl, pdfPath, client, pipeline))
                {
                    url.Downloaded = true;
                    return;
                }
            }
        }

        // Tries to download a PDF from the given URL, ensuring the content is a valid PDF.
        //returnsTrue if the PDF was successfully downloaded, otherwise false.
        static async Task<bool> TryDownloadPdf(string url, string pdfPath, HttpClient client, ResiliencePipeline pipeline)
        {
            try
            {
                // Check if the URL points to a valid PDF file
                if (!await IsPDFHeader(url, client, pipeline))
                {
                    return false;
                }

                // Download the PDF and save it to the specified path
                await pipeline.ExecuteAsync(async ct =>
                {
                    using Stream pdfStream = await client.GetStreamAsync(url, ct);
                    using FileStream fileStream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await pdfStream.CopyToAsync(fileStream, ct);
                });
                Console.WriteLine($"Successfully downloaded PDF from URL: {url}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading PDF {url} from URL: {url}. Exception: {ex.Message}");
                return false;
            }
        }


        // Checks if the content at the given URL is a valid PDF by inspecting the header.
        // returns True if the content is a valid PDF, otherwise false.
        public static async Task<bool> IsPDFHeader(string url, HttpClient client, ResiliencePipeline pipeline)
        {
            try
            {
                using HttpResponseMessage response = await pipeline.ExecuteAsync(async ct => await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct));
                response.EnsureSuccessStatusCode();

                byte[] buffer = new byte[5];
                await using Stream contentStream = await response.Content.ReadAsStreamAsync();
                int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                // Check if the first few bytes match the PDF file signature
                return bytesRead >= buffer.Length &&
                       buffer[0] == 0x25 && // %
                       buffer[1] == 0x50 && // P
                       buffer[2] == 0x44 && // D
                       buffer[3] == 0x46 && // F
                       buffer[4] == 0x2D; // -
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}