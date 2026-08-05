using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace ADMExpedientePersonal.Api.Auth.Data
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection(string connectionName)
        {
            var connectionString = _configuration.GetConnectionString(connectionName);
            return new MySqlConnection(connectionString);
        }
    }
}
