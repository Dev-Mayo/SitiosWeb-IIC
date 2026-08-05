using System.Data;

namespace ADMExpedientePersonal.Api.Empleados.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}
