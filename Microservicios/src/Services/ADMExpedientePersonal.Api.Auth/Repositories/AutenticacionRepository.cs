using ADMExpedientePersonal.Api.Auth.Models;
using ADMExpedientePersonal.Api.Auth.Data;
using Dapper;
using System.Data;

namespace ADMExpedientePersonal.Api.Auth.Repositories
{
    public class AutenticacionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AutenticacionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario> ValidarCredencialesAsync(AutenticacionRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                return await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "sp_validar_login",
                    new
                    {
                        p_username = request.Usuario,
                        p_password = request.Password
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<Usuario> ObtenerPorNombreAsync(string username)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                return await connection.QueryFirstOrDefaultAsync<Usuario>(
                    @"SELECT id_usuario, nombreusuario, nombre_completo, correo, estado
                      FROM usuarios
                      WHERE nombreusuario = @username",
                    new { username }
                );
            }
        }

        public async Task BloquearUsuarioAsync(int idUsuario)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                await connection.ExecuteAsync(
                    "UPDATE usuarios SET estado = 'Bloqueado' WHERE id_usuario = @idUsuario",
                    new { idUsuario }
                );
            }
        }
    }
}
