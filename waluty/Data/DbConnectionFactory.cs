using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace waluty.Data
{
    public class DbConnectionFactory
    {
        //field for storing connection string
        private readonly string _connectionString;
        //finding connection string and assigning to field
        public DbConnectionFactory(string connectionString) => _connectionString = connectionString;
        //method for creating connection
        public DbConnection Create() => new SqlConnection(_connectionString);
    }
}