using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;
using System.Text.Json;

namespace Gest.Controllers
{
    public class CryptoController : Controller
    {
        private string apiKey = "";
        private readonly IConfiguration _configuration;

        public CryptoController(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = _configuration["CKey:ApiKey"];
        }
        public IActionResult Index()
        {
            string jsonResponse = makeAPICall();
            Console.WriteLine("");
            return View();
        }

        public string makeAPICall()
        {
            
            var URL = new UriBuilder("https://sandbox-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "5000";
            queryString["convert"] = "USD";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", apiKey);
            client.Headers.Add("Accepts", "application/json");
            return client.DownloadString(URL.ToString());

        }
    }
}
