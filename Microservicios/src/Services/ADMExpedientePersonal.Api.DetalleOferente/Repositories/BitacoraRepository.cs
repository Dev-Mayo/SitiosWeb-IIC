using ADMExpedientePersonal.Api.DetalleOferente.Data;
using Dapper;

namespace ADMExpedientePersonal.Api.DetalleOferente.Repositories
{
    public class BitacoraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task RegistrarAsync(
            string usuario,
            string accion,
            string descripcionJson)
        {
            using (var connection = _connectionFactory.CreateConnection("BIT"))
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

                await connection.ExecuteAsync(sql, new
                {
                    Usuario = usuario,
                    Accion = accion,
                    Descripcion = descripcionJson
                });
            }
        }
    }
}
