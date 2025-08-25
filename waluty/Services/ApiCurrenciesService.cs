using waluty.Dtos.External.Nbp;
using waluty.Models;
using waluty.Repositories;

namespace waluty.Services
{
    public class ApiCurrenciesService
    {
        //httpclient for api requests
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateTableRepository _exchangeRateTableRepository;
        private readonly CurrencyRateRepository _currencyRateRepository;

        public ApiCurrenciesService(
            HttpClient httpClient,
            ExchangeRateTableRepository exchangeRateTableRepository,
            CurrencyRateRepository currencyRateRepository)
        {
            _httpClient = httpClient;
            _exchangeRateTableRepository = exchangeRateTableRepository;
            _currencyRateRepository = currencyRateRepository;
        }

        //method for fetching currencies data from NBP API
        public async Task<FetchCurrenciesView?> prepareFetchCurrenciesViewModel(DateTime RatesDate)
        {
            if (await _exchangeRateTableRepository.CheckTableExistsByDate(RatesDate))
                return null;
            
            //formating date
            var FormattedDate = RatesDate.ToString("yyyy-MM-dd");
            //fetching rates & storing response to a variable
            var nbpResponse = await _httpClient.GetAsync($"https://api.nbp.pl/api/exchangerates/tables/A/{FormattedDate}/?format=json");

            //checking if response is successful
            if (!nbpResponse.IsSuccessStatusCode)
                return null;

            var tablesDto = await nbpResponse.Content.ReadFromJsonAsync<NbpTableDto[]>();
            if (tablesDto == null || tablesDto.Length == 0)
                return null;

            var tableDto = tablesDto[0];
            var exchangeRateTable = new ExchangeRateTable
            {
                No = tableDto.No,
                EffectiveDate = tableDto.EffectiveDate
            };

            var tableId = await _exchangeRateTableRepository.InsertRatesTable(exchangeRateTable);

            var currencyRates = tableDto.Rates.Select(rate => new CurrencyRate
            {
                Currency = rate.Currency,
                Code = rate.Code,
                Mid = rate.Mid,
                ExchangeRateTableId = tableId
            }).ToList();

            var insertedCurrenciesCount = await _currencyRateRepository.InsertMany(currencyRates);

            var viewModel = new FetchCurrenciesView
            {
                SelectedDate = RatesDate,
                Message = $"Succesfully inserted {insertedCurrenciesCount} currencies' rates"
            };

            return viewModel;
        }
    }
}
