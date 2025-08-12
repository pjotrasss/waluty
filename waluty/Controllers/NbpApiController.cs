using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using waluty.Models;
using waluty.Data;
using Microsoft.EntityFrameworkCore;

namespace waluty.Controllers
{
    //controller for fetching exchange rates from NBP API
    [ApiController]
    //base route for controller
    [Route("/nbp")]
    public class NbpApiController : ControllerBase
    {
        //adding httpclient
        private HttpClient _httpClient;
        private readonly CurrenciesDbContext _db;

        public NbpApiController(HttpClient httpClient, CurrenciesDbContext db)
        {
            _httpClient = httpClient;
            _db = db;
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

        //method for saving currencies info to db
        private async Task SaveCurrenciesToDb(List<ExchangeRateTable> ReturnedTables)
        {
            var ReturnedTable = ReturnedTables.FirstOrDefault();

            //iterating through currencies in API response
            //! because ReturnedTable can't be null - check in ShowCurrencies() before calling this method
            foreach (var currency in ReturnedTable!.Rates)
            {
                var CurrencyObject = new CurrencyRate
                {
                    Currency = currency.Currency,
                    Code = currency.Code,
                    Mid = currency.Mid
                };

                //queuing object to be added to db
                _db.CurrencyRates.Add(CurrencyObject);
                Console.WriteLine($"Added {CurrencyObject.Currency} to database queue");
            }

            //adding queued objects to db

            int AddedCurrenciesCount = _db.ChangeTracker.Entries<CurrencyRate>().Count(e => e.State == EntityState.Added);

            try
            {
                await _db.SaveChangesAsync();
                Console.WriteLine($"Added {AddedCurrenciesCount} currencies to db");
            } catch (Exception exception)
            {
                Console.WriteLine($"Error saving to db {exception}");
            }
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> ShowCurrencies()
        {
            var currencies_data = await GetCurrenciesInfo();

            //checking if data is null
            if (currencies_data == null || !currencies_data.Any())
                return StatusCode(500, "error fetching currencies data");

            //calling method for saving data from API to db
            await SaveCurrenciesToDb(currencies_data);

            return Ok(currencies_data);
        }
    }
}