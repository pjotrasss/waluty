using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using waluty.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace waluty.Controllers
{
    //controller for fetching exchange rates from NBP API
    [ApiController]
    //base route for controller
    [Route("/nbp")]
    public class NbpApiController : ControllerBase
    {
        private HttpClient _httpClient;

        public NbpApiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //method for fetching currencies data from NBP API
        private async Task<List<ExchangeRateTable>?> GetCurrenciesInfo()
        {
            //fetching storing response to a variable
            var nbp_response = await _httpClient.GetAsync("https://api.nbp.pl/api/exchangerates/tables/A?format=json");

            //checking if response is successful, sending error msg if not
            if (!nbp_response.IsSuccessStatusCode)
                return null;

            //converting API response to list of objects
            return await nbp_response.Content.ReadFromJsonAsync<List<ExchangeRateTable>>();
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> ShowCurrencies()
        {
            var currencies_data = await GetCurrenciesInfo();

            //checking if data is null
            if (currencies_data == null)
                return StatusCode(500, "error fetching currencies data");
            return Ok(currencies_data);
        }
    }
}