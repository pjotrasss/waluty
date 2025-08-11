namespace waluty.Models {
    //model of nbp's exchange rate table
    public class ExchangeRateTable
    {
        //table number
        public string No { get; set; }

        //exchange rate table date
        public DateTime EffectiveDate { get; set; }

        //all exchange rates in table
        public List<CurrencyRate> Rates { get; set; }
    }
}