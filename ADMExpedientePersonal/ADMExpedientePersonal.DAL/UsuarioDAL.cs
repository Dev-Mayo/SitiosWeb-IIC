using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class UsuarioDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["SEG"].ConnectionString;

        public Usuario ObtenerUsuario(string username)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        id_usuario,
                        nombreusuario,
                        nombre_completo,
                        correo,
                        password,
                        estado
                    FROM usuarios
                    WHERE nombreusuario = @username";

                return db.Query<Usuario>(sql, new { username }).FirstOrDefault();
            }
        }

        public void BloquearUsuario(int idUsuario)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE usuarios SET estado = 'Bloqueado' WHERE id_usuario = @idUsuario";
                db.Execute(sql, new { idUsuario });
            }
        }

        public Usuario Login(string username, string password)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<Usuario>("sp_validar_login", new
                {
                    p_username = username,
                    p_password = password
                }, commandType: CommandType.StoredProcedure)
                .FirstOrDefault();
            }
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT id_usuario, nombreusuario, nombre_completo, correo, estado
            FROM usuarios
            WHERE id_usuario = @idUsuario";

                return db.Query<Usuario>(sql, new { idUsuario }).FirstOrDefault();
            }
        }
    }
}