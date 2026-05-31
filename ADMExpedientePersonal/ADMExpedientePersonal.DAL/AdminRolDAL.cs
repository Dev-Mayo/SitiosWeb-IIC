using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.DAL
{
    public class AdminRolDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SEG"].ConnectionString;

        public bool RolEnUso_bit(int idRol)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var result = conn.QuerySingle<int>("sp_rol_en_uso_bit", new { p_id_rol = idRol }, commandType: CommandType.StoredProcedure);
                return result == 1;
            }
        }

        public int eliminarRol(int id_rol)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM roles WHERE id_rol = @id_rol";
                int filas = db.Execute(sql, new { id_rol = id_rol });
                return filas;
            }
        }
        public int InsertarRol(string nombre_rol)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO roles (nombre_rol) VALUES (@nombre_rol)";
                int filas = db.Execute(sql, new { nombre_rol });
                return filas;
            }
        }
        public int ActualizarRol(int id_rol, string nombre_rol)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE roles SET nombre_rol = @nombre_rol WHERE id_rol = @id_rol";
                int filas = db.Execute(sql, new { id_rol, nombre_rol });
                return filas;
            }
        }

        public List<Rol> ObtenerRol(int? id_rol = null, string nombre_rol = null) // Obtener rol por ID, por nombre o todos los roles si no se pasan parámetros
        {
            using (var db = new MySqlConnection(connectionString))
            {
                if (id_rol != null)
                {
                    string sql = "SELECT id_rol, nombre_rol FROM roles WHERE id_rol = @id_rol";
                    var rol = db.Query<Rol>(sql, new { id_rol }).ToList();
                    return rol;
                }
                else if (nombre_rol != null)
                {
                    string sql = "SELECT id_rol, nombre_rol FROM roles WHERE nombre_rol = @nombre_rol";
                    var rol = db.Query<Rol>(sql, new { nombre_rol }).ToList();
                    return rol;
                }
                else
                {
                    // Si no se pasa parametro, retorna todos los roles
                    string sql = "SELECT id_rol, nombre_rol FROM roles";
                    var roles = db.Query<Rol>(sql).ToList();
                    return roles;
                }
            }
        }

        public bool ValidarDuplicados(string nombre_rol)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM roles WHERE LOWER(nombre_rol) = LOWER(@nombre_rol)";
                int count = db.ExecuteScalar<int>(sql, new { nombre_rol });
                return count > 0;
            }
        }
    }
}
