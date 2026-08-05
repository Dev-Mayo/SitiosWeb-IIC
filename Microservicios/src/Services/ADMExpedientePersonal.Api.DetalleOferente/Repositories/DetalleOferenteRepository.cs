using ADMExpedientePersonal.Api.DetalleOferente.Models;
using ADMExpedientePersonal.Api.DetalleOferente.Models;
using ADMExpedientePersonal.Api.DetalleOferente.Data;
using Dapper;

namespace ADMExpedientePersonal.Api.DetalleOferente.Repositories
{
    public class DetalleOferenteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DetalleOferenteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<DetalleOferenteResponse> ObtenerDetalleOferenteAsync(
            DetalleOferenteRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT
                        identificacion AS Identificacion,
                        tipo_identificacion AS TipoIdentificacion,
                        nombre_completo AS NombreCompleto,
                        fecha_nacimiento AS FechaNacimiento,
                        contratado AS Contratado
                    FROM oferentes
                    WHERE identificacion = @Identificacion;";

                var oferente =
                    await connection.QueryFirstOrDefaultAsync<DetalleOferenteResponse>(
                        sql,
                        new
                        {
                            Identificacion = request.Identificacion
                        }
                    );

                if (oferente == null)
                    return null;

                oferente.Correos =
                    await ObtenerCorreosAsync(request.Identificacion);

                oferente.Telefonos =
                    await ObtenerTelefonosAsync(request.Identificacion);

                oferente.PreparacionAcademica =
                    await ObtenerPreparacionAcademicaAsync(
                        request.Identificacion
                    );

                oferente.ExperienciaLaboral =
                    await ObtenerExperienciaLaboralAsync(
                        request.Identificacion
                    );

                oferente.Concursos =
                    await ObtenerConcursosAsync(request.Identificacion);

                return oferente;
            }
        }

        private async Task<List<string>> ObtenerCorreosAsync(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT email
                    FROM oferente_emails
                    WHERE identificacion = @Identificacion
                    ORDER BY id;";

                return (await connection.QueryAsync<string>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                )).ToList();
            }
        }

        private async Task<List<string>> ObtenerTelefonosAsync(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT telefono
                    FROM oferente_telefonos
                    WHERE identificacion = @Identificacion
                    ORDER BY id;";

                return (await connection.QueryAsync<string>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                )).ToList();
            }
        }

        private async Task<List<PreparacionAcademicaResponse>>
            ObtenerPreparacionAcademicaAsync(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT
                        codigo_institucion AS CodigoInstitucion,
                        titulo AS Titulo,
                        fecha_inicio AS FechaInicio,
                        fecha_fin AS FechaFin
                    FROM preparacion_acad
                    WHERE oferente_id = @Identificacion
                    ORDER BY fecha_inicio DESC;";

                return (await connection
                    .QueryAsync<PreparacionAcademicaResponse>(
                        sql,
                        new
                        {
                            Identificacion = identificacion
                        }
                    )).ToList();
            }
        }

        private async Task<List<ExperienciaLaboralResponse>>
            ObtenerExperienciaLaboralAsync(string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT
                        empresa AS Empresa,
                        puesto AS Puesto,
                        fecha_inicio AS FechaInicio,
                        fecha_fin AS FechaFin
                    FROM exp_laboral
                    WHERE oferente_id = @Identificacion
                    ORDER BY fecha_inicio DESC;";

                return (await connection
                    .QueryAsync<ExperienciaLaboralResponse>(
                        sql,
                        new
                        {
                            Identificacion = identificacion
                        }
                    )).ToList();
            }
        }

        private async Task<List<ConcursoResponse>> ObtenerConcursosAsync(
            string identificacion)
        {
            using (var connection = _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT
                        c.codigo_concurso AS CodigoConcurso,
                        c.nombre AS Nombre,
                        c.fecha_inicio AS FechaInicio,
                        c.fecha_fin AS FechaFin,
                        c.estado AS Estado
                    FROM oferente_concursos oc
                    INNER JOIN concursos c
                        ON c.codigo_concurso = oc.codigo_concurso
                    WHERE oc.identificacion = @Identificacion
                    ORDER BY c.fecha_inicio DESC;";

                return (await connection.QueryAsync<ConcursoResponse>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                )).ToList();
            }
        }
    }
}
