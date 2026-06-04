using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class AdminUsuarioRepository
    {
        private readonly string _conn;
        public AdminUsuarioRepository(string conn) => _conn = conn;

        public List<Usuario> ObtenerUsuarios()
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Usuario>("sp_listar_usuarios",
                commandType: CommandType.StoredProcedure).ToList();
        }

        public Usuario ObtenerPorId(int idUsuario)
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Usuario>("sp_obtener_usuario_por_id",
                new { p_id_usuario = idUsuario },
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public void Insertar(string username, string fullname, string email,
            string password, string roles)
        {
            using var db = new MySqlConnection(_conn);
            db.Execute("sp_crear_usuario",
                new
                {
                    p_username = username,
                    p_fullname = fullname,
                    p_email = email,
                    p_password = password,
                    p_roles = roles
                },
                commandType: CommandType.StoredProcedure);
        }

        public void Actualizar(int idUsuario, string username, string fullname,
            string email, string estado, string roles, string password)
        {
            using var db = new MySqlConnection(_conn);
            db.Execute("sp_actualizar_usuario",
                new
                {
                    p_id_usuario = idUsuario,
                    p_username = username,
                    p_fullname = fullname,
                    p_email = email,
                    p_estado = estado,
                    p_roles = roles,
                    p_password = string.IsNullOrEmpty(password) ? null : password
                },
                commandType: CommandType.StoredProcedure);
        }

        public void Eliminar(int idUsuario)
        {
            using var db = new MySqlConnection(_conn);
            db.Execute("sp_eliminar_usuario",
                new { p_id_usuario = idUsuario },
                commandType: CommandType.StoredProcedure);
        }

        public void CambiarEstado(int idUsuario, string estado)
        {
            using var db = new MySqlConnection(_conn);
            db.Execute("sp_cambiar_estado_usuario",
                new { p_id_usuario = idUsuario, p_estado = estado },
                commandType: CommandType.StoredProcedure);
        }

        public bool ValidarDuplicados(string username, string correo, int? idExcluir = null)
        {
            using var db = new MySqlConnection(_conn);
            return db.ExecuteScalar<int>(@"
                SELECT COUNT(*) FROM usuarios
                WHERE (LOWER(nombreusuario) = LOWER(@username)
                    OR LOWER(correo) = LOWER(@correo))
                AND (@idExcluir IS NULL OR id_usuario != @idExcluir)",
                new { username, correo, idExcluir }) > 0;
        }

        public List<Rol> ObtenerRoles()
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Rol>("SELECT id_rol, nombre_rol FROM roles ORDER BY nombre_rol")
                     .ToList();
        }
    }
}