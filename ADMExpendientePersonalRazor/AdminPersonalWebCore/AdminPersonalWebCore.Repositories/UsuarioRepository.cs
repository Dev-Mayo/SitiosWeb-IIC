using AdminPersonalWebCore.Entities;
using Dapper;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class UsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Usuario Login(string username, string password)
        {
            using var db = _connectionFactory.CreateConnection("SEG");
            return db.Query<Usuario>("sp_validar_login",
                new { p_username = username, p_password = password },
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Usuario ObtenerPorNombre(string username)
        {
            using var db = _connectionFactory.CreateConnection("SEG");
            return db.Query<Usuario>(
                "SELECT id_usuario, nombreusuario, nombre_completo, correo, estado FROM usuarios WHERE nombreusuario = @username",
                new { username }).FirstOrDefault();
        }

        public void BloquearUsuario(int idUsuario)
        {
            using var db = _connectionFactory.CreateConnection("SEG");
            db.Execute("UPDATE usuarios SET estado = 'Bloqueado' WHERE id_usuario = @idUsuario",
                new { idUsuario });
        }
    }
}