namespace waluty.Models
{
    public class CurrenciesView
    {
        public required List<DateTime> AvailableDates { get; set; }
        public DateTime? SelectedDate { get; set; }
        public required List<CurrencyRate> Rates { get; set; }
    }
}
