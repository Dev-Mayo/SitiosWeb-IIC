using ADMExpedientePersonal.WebServices.Repositories.Data;
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
        public bool MarcarOferenteComoContratado(
    string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
            {
                connection.Open();

                string sql = @"
            UPDATE oferentes
            SET contratado = 1
            WHERE identificacion = @Identificacion;
        ";

                int filasAfectadas = connection.Execute(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                );

                return filasAfectadas > 0;
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

        public int ProbarConexionEMP() // este es solo para la prueba a ver si sirve la conexión
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                const string sql = @"
            SELECT COUNT(*)
            FROM empleados;";

                return connection.ExecuteScalar<int>(sql);
            }
        }
    }
}