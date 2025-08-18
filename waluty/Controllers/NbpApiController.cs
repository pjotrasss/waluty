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
    public class NbpApiController : Controller
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
        private async Task<List<ExchangeRateTable>?> GetCurrenciesInfo(DateTime RatesDate)
        {
            //formating date
            var FormattedDate = RatesDate.ToString("yyyy-MM-dd");
            //fetching storing response to a variable
            var nbp_response = await _httpClient.GetAsync($"https://api.nbp.pl/api/exchangerates/tables/A/{FormattedDate}/?format=json");

            //checking if response is successful, returning null if not
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
        public async Task<IActionResult> ShowCurrenciesFromApi(DateTime? date)
        {
            //if date is null ask user to choose it
            if (date == null)
            {
                var ViewData = new FetchCurrenciesView
                {
                    Message = "Choose date to fetch exchange rates"
                };
                return View(ViewData);
            }

            var currencies_data = await GetCurrenciesInfo(date.Value);

            //checking if data is null
            if (currencies_data == null || !currencies_data.Any())
            {
                var ViewData = new FetchCurrenciesView
                {
                    Message = "Error Fetching data from API",
                    SelectedDate = date.Value
                };
                return View(ViewData);
            };

            //calling method for saving data from API to db
            await SaveCurrenciesToDb(currencies_data);
            var ApiData = new FetchCurrenciesView
            {
                Message = "Exchange rates fetched succesfully",
                SelectedDate = date.Value
            };
            return View(ApiData);
        }

        //method for showing currencies rates from db
        [HttpGet("db-currencies")]
        public async Task<IActionResult> ShowCurrenciesFromDb(DateTime? selectedDate)
        {
            //find all unique table dates
            var ADates = await _db.ExchangeRateTables
                .Select(table => table.EffectiveDate.Date)
                .Distinct()
                .OrderByDescending(date => date.Date)
                .ToListAsync();

            //checking if date is specified by user
            //assigning current date if not
            if (selectedDate == null)
            {
                selectedDate = DateTime.Today;
            }

            //select currencies from db for specified date
            //groupby is for selecting only one record for every currency
            var CurrenciesRates = await _db.CurrencyRates
                .Include(currency => currency.ExchangeRateTable)
                .Where(currency => currency.ExchangeRateTable.EffectiveDate.Date == selectedDate)
                .GroupBy(currency => currency.Code)
                .Select(group => group.First())
                .ToListAsync();

            var ViewData = new CurrenciesView
            {
                AvailableDates = ADates,
                SelectedDate = selectedDate,
                Rates = CurrenciesRates
            };

            return View(ViewData);
        }
    }
}