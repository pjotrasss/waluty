using Microsoft.EntityFrameworkCore;
using waluty.Models;

namespace waluty.Data
{
    public class CurrenciesDbContext(DbContextOptions<CurrenciesDbContext> options) : DbContext(options)
    {
        //adding virtual tables to EF Core
        public DbSet<ExchangeRateTable> ExchangeRateTables { get; set; }
        public DbSet<CurrencyRate> CurrencyRates {  get; set; }
    }
}