using Dapper;
//db connection factory
using waluty.Data;
//data models
using waluty.Models;

namespace waluty.Repositories
{
    public class CurrencyRateRepository
    {
        //field for storing factory reference
        private readonly DbConnectionFactory _factory;
        //creating connection using factory
        public CurrencyRateRepository(DbConnectionFactory factory) => _factory = factory;

        //method for selecting all record from currency rates table
        public async Task<IEnumerable<CurrencyRate>> SelectCurrenciesByDate(int Id)
        {
            //creating connection using create method from factory
            using var conn = _factory.Create();

            const string selectByDateQuery = @"
                Select CurrencyRates.Id, Currency, Code, Mid, ExchangeRateTableId From CurrencyRates
                Join ExchangeRateTables
                On ExchangeRateTables.Id=CurrencyRates.ExchangeRateTableId
                Where ExchangeRateTables.Id=@Id";
            return await conn.QueryAsync<CurrencyRate>(selectByDateQuery, new { Id });
        }



        public async Task<int> InsertMany(IEnumerable<CurrencyRate> currencyRates)
        {
            var ratesList = currencyRates.ToList();
            if (ratesList.Count == 0)
                return 0;

            using var conn = _factory.Create();

            const string insertManyQuery = @"
                Insert Into CurrencyRates (Currency, Code, Mid, ExchangeRateTableId)
                Values (@Currency, @Code, @Mid, @ExchangeRateTableId)";
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var affected = await conn.ExecuteAsync(insertManyQuery, ratesList, transaction);
                transaction.Commit();
                return affected;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}