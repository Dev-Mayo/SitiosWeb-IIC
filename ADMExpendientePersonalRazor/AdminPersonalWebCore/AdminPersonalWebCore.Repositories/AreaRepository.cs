using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class AreaRepository
    {
        private readonly string _connectionString;

        public AreaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Area> ObtenerTodos()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    a.codigo_area AS CodigoArea,
                    a.nombre AS Nombre,
                    a.jefatura AS Jefatura,
                    e.nombre_completo AS NombreJefatura
                FROM admin_areas a
                INNER JOIN empleados e ON a.jefatura = e.empleado_id
                ORDER BY a.codigo_area DESC;";

            return db.Query<Area>(sql).ToList();
        }

        public Area ObtenerPorId(int codigoArea)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    codigo_area AS CodigoArea,
                    nombre AS Nombre,
                    jefatura AS Jefatura
                FROM admin_areas
                WHERE codigo_area = @codigoArea;";

            return db.QueryFirstOrDefault<Area>(sql, new { codigoArea });
        }

        public void Insertar(Area area)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                INSERT INTO admin_areas
                    (codigo_area, nombre, jefatura)
                VALUES
                    (@CodigoArea, @Nombre, @Jefatura);";

            db.Execute(sql, area);
        }

        public void Actualizar(Area area)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                UPDATE admin_areas
                SET
                    nombre = @Nombre,
                    jefatura = @Jefatura
                WHERE codigo_area = @CodigoArea;";

            db.Execute(sql, area);
        }

        public void Eliminar(int codigoArea)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                DELETE FROM admin_areas
                WHERE codigo_area = @codigoArea;";

            db.Execute(sql, new { codigoArea });
        }

        public List<Empleado> ObtenerEmpleados()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    empleado_id AS EmpleadoId,
                    nombre_completo AS NombreCompleto
                FROM empleados
                ORDER BY nombre_completo ASC;";

            return db.Query<Empleado>(sql).ToList();
        }
        public bool ExisteCodigo(int codigoArea)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
        SELECT COUNT(1)
        FROM admin_areas
        WHERE codigo_area = @codigoArea";

            return db.ExecuteScalar<int>(sql, new { codigoArea }) > 0;
        }
    }
}