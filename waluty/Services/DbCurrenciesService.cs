using waluty.Models;
using waluty.Repositories;

namespace waluty.Services
{
    public class DbCurrenciesService
    {
        //adding repositories to service
        private readonly CurrencyRateRepository _currencyRateRepository;
        private readonly ExchangeRateTableRepository _exchangeRateTableRepository;

        public DbCurrenciesService(CurrencyRateRepository currencyRateRepository, ExchangeRateTableRepository exchangeRateTableRepository)
        {
            _currencyRateRepository = currencyRateRepository;
            _exchangeRateTableRepository = exchangeRateTableRepository;
        }
        
        public async Task<CurrenciesView> prepareDbCurrenciesViewModel(int exchangeRateTableId)
        {
            var exchangeRatesByDate = await _currencyRateRepository.SelectCurrenciesByDate(exchangeRateTableId);
            var availableTables = await _exchangeRateTableRepository.SelectAllTables();

            var ViewModel = new CurrenciesView
            {
                AvailableTables = availableTables.ToList(),
                SelectedTableId = exchangeRateTableId,
                Rates = exchangeRatesByDate.ToList()
            };
            return ViewModel;
        }
    }
}
