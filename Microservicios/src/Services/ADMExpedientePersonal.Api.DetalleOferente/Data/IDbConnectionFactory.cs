using System.Data;

namespace ADMExpedientePersonal.Api.DetalleOferente.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}
