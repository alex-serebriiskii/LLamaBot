using System.Data;

namespace SchizoLlamaBot.Database
{
    public interface IDbContext
   {
      public IDbConnection CreateConnection();
      public IDbConnection CreateConnection(string connectionStringName);
   }
}