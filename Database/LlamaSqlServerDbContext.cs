using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SchizoLlamaBot.Database
{
    public class LlamaSqlServerDbContext : IDbContext
    {
        private readonly DbConfig _config;
        public LlamaSqlServerDbContext(IOptions<DbConfig> config)
        {
           _config = config.Value; 
        }
        
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_config.ConnectionString);
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}