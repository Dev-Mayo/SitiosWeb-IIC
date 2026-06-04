using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AdminPersonalWebCore.Repository
{
    public class UsuarioRepository
    {
        private readonly string _conn;
        public UsuarioRepository(string conn) => _conn = conn;

        public Usuario Login(string username, string password)
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Usuario>("sp_validar_login",
                new { p_username = username, p_password = password },
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Usuario ObtenerPorNombre(string username)
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Usuario>(
                "SELECT id_usuario, nombreusuario, nombre_completo, correo, estado FROM usuarios WHERE nombreusuario = @username",
                new { username }).FirstOrDefault();
        }

        public void BloquearUsuario(int idUsuario)
        {
            using var db = new MySqlConnection(_conn);
            db.Execute("UPDATE usuarios SET estado = 'Bloqueado' WHERE id_usuario = @idUsuario",
                new { idUsuario });
        }
    }
}