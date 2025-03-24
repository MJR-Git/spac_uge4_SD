namespace CshScript.Tests.Utilities;

using System.Threading.Tasks;
using RichardSzalay.MockHttp;

public class PdfDownloaderTests
{

    [Fact]
    public async Task Test1Async()
    {
        var mockHttp = new MockHttpMessageHandler();
        
        // Setup a respond for the user api (including a wildcard in the URL)
        mockHttp.When("http://localhost/api/user/*")
                .Respond("application/json", "{'name' : 'Test McGee'}"); // Respond with JSON

        // Inject the handler or client into your application code
        var client = new HttpClient(mockHttp);

        var response = await client.GetAsync("http://localhost/api/user/1234");
        // or without async: var response = client.GetAsync("http://localhost/api/user/1234").Result;

        var json = await response.Content.ReadAsStringAsync();

        // No network connection required
        Console.Write(json); // {'name' : 'Test McGee'}
    }
}
