using ADMExpedientePersonal.WebServices.Repositories.Data;
using ADMExpedientePersonal.WebServices.Repositories;

namespace ADMExpedientePersonal.WebServices.Common
{
    public static class RepositoryFactory
    {
        private static readonly IDbConnectionFactory _connectionFactory =
            new MySqlConnectionFactory();

        public static EmpleadoRepository CrearEmpleadoRepository()
        {
            return new EmpleadoRepository(_connectionFactory);
        }

        public static AutenticacionRepository CrearAutenticacionRepository()
        {
            return new AutenticacionRepository(_connectionFactory);
        }
    }
}