using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace ADMExpedientePersonal.DAL
{
    public class AccionPersonalDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;
        public List<AccionPersonal> ObtenerTodos()
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ap.accion_id,
                        ap.codigo_accion,
                        ap.fecha,
                        ap.descripcion,
                        ap.empleado_id,
                        ap.jefatura_id,
                        e.nombre_completo AS nombre_empleado,
                        j.nombre_completo AS nombre_jefatura
                    FROM acciones_personal ap
                    INNER JOIN EMP.empleados e ON ap.empleado_id = e.empleado_id
                    INNER JOIN EMP.empleados j ON ap.jefatura_id = j.empleado_id
                    ORDER BY ap.fecha DESC;";

                return conexion.Query<AccionPersonal>(sql).ToList();
            }
        }

        public AccionPersonal ObtenerPorId(int accionId)
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        accion_id,
                        codigo_accion,
                        fecha,
                        descripcion,
                        empleado_id,
                        jefatura_id
                    FROM acciones_personal
                    WHERE accion_id = @accionId;";

                return conexion.QueryFirstOrDefault<AccionPersonal>(sql, new { accionId });
            }
        }

        public void Insertar(AccionPersonal accion)
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO EMP.acciones_personal
                    (
                        codigo_accion,
                        fecha,
                        descripcion,
                        empleado_id,
                        jefatura_id
                    )
                    VALUES
                    (
                        @codigo_accion,
                        @fecha,
                        @descripcion,
                        @empleado_id,
                        @jefatura_id
                    );";

                conexion.Execute(sql, accion);
            }
        }

        public void Actualizar(AccionPersonal accion)
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE EMP.acciones_personal
                    SET
                        fecha = @fecha,
                        descripcion = @descripcion,
                        empleado_id = @empleado_id,
                        jefatura_id = @jefatura_id
                    WHERE accion_id = @accion_id;";

                conexion.Execute(sql, accion);
            }
        }

        public void Eliminar(int accionId)
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM acciones_personal
                    WHERE accion_id = @accionId;";

                conexion.Execute(sql, new { accionId });
            }
        }

        public List<Empleado> ObtenerEmpleados()
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        empleado_id,
                        nombre_completo
                    FROM empleados
                    ORDER BY nombre_completo;";

                return conexion.Query<Empleado>(sql).ToList();
            }
        }
    }
}
