using System.Data;

namespace ADMExpedientePersonal.Api.Puestos.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}
