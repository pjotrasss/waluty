using Dapper;
//dbconnection factory
using waluty.Data;
//data models
using waluty.Models;

namespace waluty.Repositories
{
    public class ExchangeRateTableRepository
    {
        //field for storing factory reference
        private readonly DbConnectionFactory _factory;
        public ExchangeRateTableRepository(DbConnectionFactory factory) => _factory = factory;



        //method for selecting all dates in table
        public async Task<IEnumerable<ExchangeRateTable>> SelectAllTables()
        {
            //connection to db
            using var conn = _factory.Create();

            const string SelectAllDatesQuery = "Select Id, No, EffectiveDate From ExchangeRateTables Order By EffectiveDate Desc";
            return await conn.QueryAsync<ExchangeRateTable>(SelectAllDatesQuery);
        }



        //method for checking if table for given date exists in db
        public async Task<Boolean> CheckTableExistsByDate(DateTime exchangeRateTableDate)
        {
            //connection to db
            using var conn = _factory.Create();

            const string selectTableByDateQuery = @"
                Select Count(1)
                From ExchangeRateTables
                Where EffectiveDate=@exchangeRateTableDate";

            var result = await conn.ExecuteScalarAsync<int>(selectTableByDateQuery, new { exchangeRateTableDate });

            return result > 0;
        }



        //method for inserting rates table to db
        public async Task<int> InsertRatesTable(ExchangeRateTable exchangeRateTable)
        {
            //connection to db
            using var conn = _factory.Create();

            const string insertTableQuery = @"
                Insert Into ExchangeRateTables (No, EffectiveDate)
                Values (@No, @EffectiveDate);
                Select Cast(Scope_Identity() as int)";

            var id = await conn.ExecuteScalarAsync<int>(insertTableQuery, exchangeRateTable);
            return id;
        }
    }
}