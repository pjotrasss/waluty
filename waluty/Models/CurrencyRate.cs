namespace waluty.Models
{
    //model of nbp's currency rate record in exchange rate table
    public class CurrencyRate
    {
        public int Id { get; set; }
        //currency name
        public string Currency { get; set; } = "";
        //currency code
        public string Code { get; set; } = "";
        //exchange rate
        public decimal Mid { get; set; }

        ///<summary>
        ///This foreign key is nullable for <see cref="waluty.Services.ApiCurrenciesService.prepareFetchCurrenciesViewModel(System.DateTime)"/>
        ///</summary>
        public int ExchangeRateTableId { get; set; }
    }
}