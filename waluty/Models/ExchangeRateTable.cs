namespace waluty.Models
{
    //model of nbp's exchange rate table
    public class ExchangeRateTable
    {
        public int Id { get; set; }
        //table number
        public string No { get; set; } = "";

        //table date
        public DateTime EffectiveDate { get; set; }
    }
}