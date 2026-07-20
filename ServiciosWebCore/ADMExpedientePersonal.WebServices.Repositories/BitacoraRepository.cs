using ADMExpedientePersonal.WebServices.Repositories.Data;
using Dapper;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class BitacoraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Registrar(
            string usuario,
            string accion,
            string descripcionJson)
        {
            using (var connection =
                _connectionFactory.CreateConnection("BIT"))
            {
                const string sql = @"
                    INSERT INTO bitacoras
                    (
                        usuario,
                        accion,
                        descripcion
                    )
                    VALUES
                    (
                        @Usuario,
                        @Accion,
                        @Descripcion
                    );";

                connection.Execute(sql, new
                {
                    Usuario = usuario,
                    Accion = accion,
                    Descripcion = descripcionJson
                });
            }
        }
    }
}