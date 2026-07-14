using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace ADMExpedientePersonal.WebServices.Repositories.Data
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection(string connectionName)
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings[connectionName]
                .ConnectionString;

            return new MySqlConnection(connectionString);
        }
    }
}