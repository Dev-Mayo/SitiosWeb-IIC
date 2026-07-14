using ADMExpedientePersonal.WebServices.Entities.Models;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using Dapper;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class AutenticacionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AutenticacionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Usuario ValidarCredenciales(AutenticacionRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                return connection.Query<Usuario>(
                    "sp_validar_login",
                    new
                    {
                        p_username = request.Usuario,
                        p_password = request.Password
                    },
                    commandType: CommandType.StoredProcedure
                ).FirstOrDefault();
            }
        }

        public Usuario ObtenerPorNombre(string username)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                return connection.Query<Usuario>(
                    @"SELECT id_usuario, nombreusuario, nombre_completo, correo, estado
                      FROM usuarios
                      WHERE nombreusuario = @username",
                    new { username }
                ).FirstOrDefault();
            }
        }

        public void BloquearUsuario(int idUsuario)
        {
            using (var connection = _connectionFactory.CreateConnection("SEG"))
            {
                connection.Execute(
                    "UPDATE usuarios SET estado = 'Bloqueado' WHERE id_usuario = @idUsuario",
                    new { idUsuario }
                );
            }
        }
    }
}