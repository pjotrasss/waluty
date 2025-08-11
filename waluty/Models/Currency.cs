namespace waluty.Models {
    //model of nbp's currency rate record in exchange rate table
    public class CurrencyRate {
        //currency name
        public string Currency { get; set; }
        //currency code
        public string Code { get; set; }
        //exchange rate
        public decimal Mid { get; set; }
    }
}