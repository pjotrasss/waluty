namespace waluty.Models
{
    public class CurrenciesView
    {
        public required List<ExchangeRateTable> AvailableTables { get; set; }
        public int? SelectedTableId { get; set; }
        public required List<CurrencyRate> Rates { get; set; }
    }
}
