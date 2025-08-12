using Microsoft.EntityFrameworkCore;

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
        [Precision(18, 6)]
        public decimal Mid { get; set; }

        //Foreign Key
        public int ExchangeRateTableId { get; set; }
        public ExchangeRateTable ExchangeRateTable { get; set; } = null!;
    }
}