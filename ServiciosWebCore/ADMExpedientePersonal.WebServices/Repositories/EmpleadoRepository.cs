using ADMExpedientePersonal.WebServices.Data;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using Dapper;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class EmpleadoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmpleadoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public int RegistrarEmpleado(EmpleadoRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sqlEmpleado = @"
                            INSERT INTO empleados
                            (identificacion, tipo_identificacion, nombre_completo, fecha_nacimiento, puesto_id)
                            VALUES
                            (@Identificacion, @TipoIdentificacion, @NombreCompleto, @FechaNacimiento, @PuestoId);

                            SELECT LAST_INSERT_ID();";

                        int empleadoId = connection.ExecuteScalar<int>(
                            sqlEmpleado,
                            request,
                            transaction
                        );

                        if (request.Correos != null)
                        {
                            foreach (var correo in request.Correos)
                            {
                                connection.Execute(
                                    @"INSERT INTO empleado_emails (empleado_id, email)
                                      VALUES (@EmpleadoId, @Email);",
                                    new { EmpleadoId = empleadoId, Email = correo },
                                    transaction
                                );
                            }
                        }

                        if (request.Telefonos != null)
                        {
                            foreach (var telefono in request.Telefonos)
                            {
                                connection.Execute(
                                    @"INSERT INTO empleado_telefonos (empleado_id, telefono)
                                      VALUES (@EmpleadoId, @Telefono);",
                                    new { EmpleadoId = empleadoId, Telefono = telefono },
                                    transaction
                                );
                            }
                        }

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

        public bool ExisteEmpleado(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                string sql = @"
                    SELECT COUNT(1)
                    FROM empleados
                    WHERE identificacion = @Identificacion;";

                return connection.ExecuteScalar<int>(
                    sql,
                    new { Identificacion = identificacion }
                ) > 0;
            }
        }
    }
}