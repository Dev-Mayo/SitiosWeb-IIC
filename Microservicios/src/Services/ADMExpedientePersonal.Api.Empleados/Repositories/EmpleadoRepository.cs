using ADMExpedientePersonal.Api.Empleados.Models;
using ADMExpedientePersonal.Api.Empleados.Data;
using Dapper;

namespace ADMExpedientePersonal.Api.Empleados.Repositories
{
    public class EmpleadoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;


        public EmpleadoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> RegistrarEmpleadoAsync(EmpleadoRequest request)
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

                        int empleadoId = await connection.ExecuteScalarAsync<int>(
                            sqlEmpleado,
                            request,
                            transaction
                        );

                        if (request.Correos != null)
                        {
                            foreach (var correo in request.Correos)
                            {
                                await connection.ExecuteAsync(
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
                                await connection.ExecuteAsync(
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

        public async Task<bool> MarcarOferenteComoContratadoAsync(
            string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                connection.Open();

                string sql = @"
                    UPDATE oferentes
                    SET contratado = 1
                    WHERE identificacion = @Identificacion;";

                int filasAfectadas = await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                );

                return filasAfectadas > 0;
            }
        }

        public async Task<bool> ExisteEmpleadoAsync(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                string sql = @"
                    SELECT COUNT(1)
                    FROM empleados
                    WHERE identificacion = @Identificacion;";

                return await connection.ExecuteScalarAsync<int>(
                    sql,
                    new { Identificacion = identificacion }
                ) > 0;
            }
        }

        public async Task<int> ProbarConexionEmpAsync()
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                const string sql = @"
                    SELECT COUNT(*)
                    FROM empleados;";

                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }
        public async Task<bool> ExistePuestoAsync(int puestoId) // verifica si el puesto ya existe en la base de datos
        {
            using (var connection = _connectionFactory.CreateConnection("EMP"))
            {
                const string sql = @"
            SELECT COUNT(1)
            FROM puestos
            WHERE puesto_id = @PuestoId;";

                return await connection.ExecuteScalarAsync<int>(
                    sql,
                    new { PuestoId = puestoId }
                ) > 0;
            }
        }
        public async Task<bool> ExisteOferenteAsync(string identificacion) // verifica si el oferente ya existe en la base de datos
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                const string sql = @"
            SELECT COUNT(1)
            FROM oferentes
            WHERE identificacion = @Identificacion;";

                return await connection.ExecuteScalarAsync<int>(
                    sql,
                    new { Identificacion = identificacion }
                ) > 0;
            }
        }

        public async Task<bool> EstaContratadoAsync(string identificacion) // verifica si el oferente ya ha sido marcado como contratado
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                const string sql = @"
            SELECT contratado
            FROM oferentes
            WHERE identificacion = @Identificacion;";

                var contratado = await connection.ExecuteScalarAsync<int?>(
                    sql,
                    new { Identificacion = identificacion }
                );

                return contratado == 1;
            }
        }

    }
}
