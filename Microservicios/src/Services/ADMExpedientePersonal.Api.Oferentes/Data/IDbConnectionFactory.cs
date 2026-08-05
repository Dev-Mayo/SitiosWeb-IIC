using System.Data;

namespace ADMExpedientePersonal.Api.Oferentes.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}
