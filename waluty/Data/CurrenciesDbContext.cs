using Microsoft.EntityFrameworkCore;
using waluty.Models;

namespace waluty.Data
{
    public class CurrenciesDbContext(DbContextOptions<CurrenciesDbContext> options) : DbContext(options)
    {
        //adding virtual tables to EF Core
        public DbSet<ExchangeRateTable> ExchangeRateTables { get; set; }
        public DbSet<CurrencyRate> CurrencyRates {  get; set; }

        //defining relations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CurrencyRate>()
                .HasOne(currency => currency.ExchangeRateTable)
                .WithMany(table => table.Rates)
                .HasForeignKey(currency => currency.ExchangeRateTableId)
                //for deleting records from table storing currencies when
                //corresponding record from ExchangeRateTable is deleted
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}