using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class ContratacionRepository
    {
        private readonly string _connectionString;

        public ContratacionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Oferente> ObtenerOferentesDisponibles()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    identificacion AS Identificacion,
                    tipo_identificacion AS TipoIdentificacion,
                    nombre_completo AS NombreCompleto,
                    fecha_nacimiento AS FechaNacimiento,
                    contratado AS Contratado
                FROM OFE.oferentes
                WHERE contratado = 0
                ORDER BY nombre_completo;";

            return db.Query<Oferente>(sql).ToList();
        }

        public List<Puesto> ObtenerPuestos()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    puesto_id AS PuestoId,
                    nombre AS Nombre,
                    salario AS Salario
                FROM EMP.puestos
                ORDER BY nombre;";

            return db.Query<Puesto>(sql).ToList();
        }

        public List<Empleado> ObtenerJefaturas()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT
                    empleado_id AS EmpleadoId,
                    nombre_completo AS NombreCompleto
                FROM EMP.empleados
                ORDER BY nombre_completo;";

            return db.Query<Empleado>(sql).ToList();
        }

        public int ContratarEmpleado(ContratacionRequest request)
        {
            using var db = new MySqlConnection(_connectionString);
            db.Open();

            using var transaction = db.BeginTransaction();

            try
            {
                string sqlOferente = @"
                    SELECT
                        identificacion AS Identificacion,
                        tipo_identificacion AS TipoIdentificacion,
                        nombre_completo AS NombreCompleto,
                        fecha_nacimiento AS FechaNacimiento,
                        contratado AS Contratado
                    FROM OFE.oferentes
                    WHERE identificacion = @IdentificacionOferente;";

                var oferente = db.QueryFirstOrDefault<Oferente>(
                    sqlOferente,
                    new { request.IdentificacionOferente },
                    transaction
                );

                if (oferente == null)
                    throw new Exception("El oferente seleccionado no existe.");

                if (oferente.Contratado == 1)
                    throw new Exception("El oferente seleccionado ya fue contratado.");

                string sqlInsertEmpleado = @"
                    INSERT INTO EMP.empleados
                        (identificacion, tipo_identificacion, nombre_completo, fecha_nacimiento, puesto_id)
                    VALUES
                        (@Identificacion, @TipoIdentificacion, @NombreCompleto, @FechaNacimiento, @PuestoId);

                    SELECT LAST_INSERT_ID();";

                int empleadoId = db.ExecuteScalar<int>(
                    sqlInsertEmpleado,
                    new
                    {
                        oferente.Identificacion,
                        oferente.TipoIdentificacion,
                        oferente.NombreCompleto,
                        oferente.FechaNacimiento,
                        request.PuestoId
                    },
                    transaction
                );

                string sqlCopiarEmails = @"
                    INSERT INTO EMP.empleado_emails (empleado_id, email)
                    SELECT @EmpleadoId, email
                    FROM OFE.oferente_emails
                    WHERE identificacion = @IdentificacionOferente;";

                db.Execute(
                    sqlCopiarEmails,
                    new
                    {
                        EmpleadoId = empleadoId,
                        request.IdentificacionOferente
                    },
                    transaction
                );

                string sqlCopiarTelefonos = @"
                    INSERT INTO EMP.empleado_telefonos (empleado_id, telefono)
                    SELECT @EmpleadoId, telefono
                    FROM OFE.oferente_telefonos
                    WHERE identificacion = @IdentificacionOferente;";

                db.Execute(
                    sqlCopiarTelefonos,
                    new
                    {
                        EmpleadoId = empleadoId,
                        request.IdentificacionOferente
                    },
                    transaction
                );

                string sqlMarcarContratado = @"
                    UPDATE OFE.oferentes
                    SET contratado = 1
                    WHERE identificacion = @IdentificacionOferente;";

                db.Execute(
                    sqlMarcarContratado,
                    new { request.IdentificacionOferente },
                    transaction
                );

                string sqlAccionPersonal = @"
                    INSERT INTO EMP.acciones_personal
                        (codigo_accion, fecha, descripcion, empleado_id, jefatura_id)
                    VALUES
                        (@CodigoAccion, CURDATE(), @Descripcion, @EmpleadoId, @JefaturaId);";

                db.Execute(
                    sqlAccionPersonal,
                    new
                    {
                        CodigoAccion = empleadoId,
                        Descripcion = "Contratación de empleado",
                        EmpleadoId = empleadoId,
                        request.JefaturaId
                    },
                    transaction
                );

                transaction.Commit();
                return empleadoId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}