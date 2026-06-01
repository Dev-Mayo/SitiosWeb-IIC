using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class AdminUsuarioDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["SEG"].ConnectionString;

        public List<Usuario> ObtenerUsuarios()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<Usuario>(
                    "sp_listar_usuarios",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<Usuario>(
                    "sp_obtener_usuario_por_id",
                    new { p_id_usuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                ).FirstOrDefault();
            }
        }

        public int InsertarUsuario(string username, string fullname, string email,
            string password, string roles)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_crear_usuario",
                    new
                    {
                        p_username = username,
                        p_fullname = fullname,
                        p_email = email,
                        p_password = password,
                        p_roles = roles
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public int ActualizarUsuario(int idUsuario, string username, string fullname,
            string email, string estado, string roles, string password)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_actualizar_usuario",
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
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public int EliminarUsuario(int idUsuario)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_eliminar_usuario",
                    new { p_id_usuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public int CambiarEstado(int idUsuario, string estado)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_cambiar_estado_usuario",
                    new { p_id_usuario = idUsuario, p_estado = estado },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public bool ValidarDuplicados(string username, string correo, int? idUsuarioExcluir = null)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(*) FROM usuarios 
                    WHERE (LOWER(nombreusuario) = LOWER(@username) 
                        OR LOWER(correo) = LOWER(@correo))
                    AND (@idExcluir IS NULL OR id_usuario != @idExcluir)";
                return db.ExecuteScalar<int>(sql, new
                {
                    username,
                    correo,
                    idExcluir = idUsuarioExcluir
                }) > 0;
            }
        }

        public List<Rol> ObtenerRoles()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<Rol>("SELECT id_rol, nombre_rol FROM roles ORDER BY nombre_rol")
                         .ToList();
            }
        }
    }
}