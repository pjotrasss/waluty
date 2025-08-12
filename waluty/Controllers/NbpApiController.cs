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
        private readonly HttpClient _httpClient;
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
            //! because ReturnedTable can't be null - check in ShowCurrenciesFromApi before calling this method
            var ReturnedTable = ReturnedTables.FirstOrDefault()!;

            var ExistingTable = await _db.ExchangeRateTables
                .FirstOrDefaultAsync(table => table.EffectiveDate == ReturnedTable.EffectiveDate);
            if (ExistingTable != null)
            {
                Console.WriteLine($"Table for date {ReturnedTable.EffectiveDate} already exists, stopping execution");
                return;
            }

            //adding table to db
            _db.ExchangeRateTables.Add(ReturnedTable);
            await _db.SaveChangesAsync();
            Console.WriteLine($"Added exchange rate table for date {ReturnedTable.EffectiveDate} to db");

            var CurrencyObjects = ReturnedTable.Rates.Select(
                currency => new CurrencyRate
                {
                    Currency = currency.Currency,
                    Code = currency.Code,
                    Mid = currency.Mid,
                    ExchangeRateTableId = ReturnedTable.Id
                }
            ).ToList();

            //adding objects to db queue
            _db.CurrencyRates.AddRange(CurrencyObjects);

            //adding queued objects to db
            try
            {
                await _db.SaveChangesAsync();
                Console.WriteLine($"Added {CurrencyObjects.Count} currencies to db");
            } catch (Exception exception)
            {
                Console.WriteLine($"Error saving to db {exception}");
            }
        }

        //method for showing currencies from API and saving them to db
        [HttpGet("api-currencies")]
        public async Task<IActionResult> ShowCurrenciesFromApi()
        {
            var currencies_data = await GetCurrenciesInfo();

            //checking if data is null
            if (currencies_data == null || !currencies_data.Any())
                return StatusCode(500, "error fetching currencies data");

            //calling method for saving data from API to db
            await SaveCurrenciesToDb(currencies_data);

            return Ok(currencies_data);
        }

        //method for showing currencies rates from db
        [HttpGet("db-currencies")]
        public async Task<IActionResult> ShowCurrenciesFromDb()
        {
            var CurrenciesRates = await _db.CurrencyRates
                .Include(currency => currency.ExchangeRateTable).ToListAsync();
            return Ok(CurrenciesRates);
        }
    }
}