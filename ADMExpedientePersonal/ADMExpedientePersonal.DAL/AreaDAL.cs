using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class AreaDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;

        public List<Area> ObtenerTodos()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT
                a.codigo_area,
                a.nombre,
                a.jefatura,
                e.nombre_completo AS nombre_jefatura
            FROM admin_areas a
            LEFT JOIN empleados e
                ON a.jefatura = e.empleado_id
            ORDER BY a.codigo_area DESC";

                return db.Query<Area>(sql).ToList();
            }
        }

        public Area ObtenerPorId(int id)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        codigo_area,
                        nombre,
                        jefatura
                    FROM admin_areas
                    WHERE codigo_area = @id";

                return db.Query<Area>(sql, new { id }).FirstOrDefault();
            }
        }

        public void Insertar(Area area)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO admin_areas (codigo_area, nombre, jefatura)
                    VALUES (@codigo_area, @nombre, @jefatura)";

                db.Execute(sql, area);
            }
        }

        public void Actualizar(Area area)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE admin_areas
                    SET nombre = @nombre,
                        jefatura = @jefatura
                    WHERE codigo_area = @codigo_area";

                db.Execute(sql, area);
            }
        }

        public void Eliminar(int id)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM admin_areas
                    WHERE codigo_area = @id";

                db.Execute(sql, new { id });
            }
        }



        public List<Empleado> ObtenerJefaturas()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT
                empleado_id,
                nombre_completo
            FROM empleados
            ORDER BY nombre_completo";

                return db.Query<Empleado>(sql).ToList();
            }
        }

    }// AreaDAL
}