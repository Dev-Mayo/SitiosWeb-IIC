using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class AccionPersonalRepository
    {
        private readonly string _connectionString;

        public AccionPersonalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<AccionPersonal> ObtenerTodos()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    ap.accion_id AS AccionId,
                    ap.codigo_accion AS CodigoAccion,
                    ap.fecha AS Fecha,
                    ap.descripcion AS Descripcion,
                    ap.empleado_id AS EmpleadoId,
                    ap.jefatura_id AS JefaturaId,
                    e.nombre_completo AS NombreEmpleado,
                    j.nombre_completo AS NombreJefatura
                FROM acciones_personal ap
                INNER JOIN empleados e
                    ON ap.empleado_id = e.empleado_id
                INNER JOIN empleados j
                    ON ap.jefatura_id = j.empleado_id
                ORDER BY ap.accion_id DESC;";

            return db.Query<AccionPersonal>(sql).ToList();
        }

        public AccionPersonal ObtenerPorId(int id)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    accion_id AS AccionId,
                    codigo_accion AS CodigoAccion,
                    fecha AS Fecha,
                    descripcion AS Descripcion,
                    empleado_id AS EmpleadoId,
                    jefatura_id AS JefaturaId
                FROM acciones_personal
                WHERE accion_id = @id;";

            return db.QueryFirstOrDefault<AccionPersonal>(sql, new { id });
        }

        public void Insertar(AccionPersonal accion)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
        INSERT INTO acciones_personal
        (
            codigo_accion,
            fecha,
            descripcion,
            empleado_id,
            jefatura_id
        )
        VALUES
        (
            @CodigoAccion,
            @Fecha,
            @Descripcion,
            @EmpleadoId,
            @JefaturaId
        );";

            db.Execute(sql, accion);
        }

        public void Actualizar(AccionPersonal accion)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
        UPDATE acciones_personal
        SET
            codigo_accion = @CodigoAccion,
            fecha = @Fecha,
            descripcion = @Descripcion,
            empleado_id = @EmpleadoId,
            jefatura_id = @JefaturaId
        WHERE accion_id = @AccionId;";

            db.Execute(sql, accion);
        }

        public void Eliminar(int id)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
        DELETE FROM acciones_personal
        WHERE accion_id = @id;";

            db.Execute(sql, new { id });
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
    }
}