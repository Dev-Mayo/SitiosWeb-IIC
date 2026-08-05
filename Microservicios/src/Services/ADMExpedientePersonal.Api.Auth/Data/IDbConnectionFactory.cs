using System.Data;

namespace ADMExpedientePersonal.Api.Auth.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}
