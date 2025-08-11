using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using waluty.Models;

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
        
        //subroute for fetching exchange rates
        [HttpGet("rates")]
        public async Task<IActionResult> GetRates()
        {
            //fetching exchange rates from NBP API and storing response to a variable
            var nbp_response = await _httpClient.GetAsync("https://api.nbp.pl/api/exchangerates/tables/A?format=json");

            //checking if response is successful, sending error msg if not
            if (!nbp_response.IsSuccessStatusCode)
                return StatusCode((int)nbp_response.StatusCode, "Error fetching data");

            //converting response to objects
            var nbp_response_objects = await nbp_response.Content.ReadFromJsonAsync < List < ExchangeRateTable>>();
            return Ok(nbp_response_objects);
        }
    }
}