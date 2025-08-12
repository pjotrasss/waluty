using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

        [JsonIgnore] //preventing json cycling
        public ExchangeRateTable ExchangeRateTable { get; set; } = null!;
    }
}