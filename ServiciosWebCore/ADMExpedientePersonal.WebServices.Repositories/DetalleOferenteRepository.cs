using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class DetalleOferenteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DetalleOferenteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public DetalleOferenteResponse ObtenerDetalleOferente(
            DetalleOferenteRequest request)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
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
                    connection.QueryFirstOrDefault<DetalleOferenteResponse>(
                        sql,
                        new
                        {
                            Identificacion = request.Identificacion
                        }
                    );

                if (oferente == null)
                    return null;

                oferente.Correos =
                    ObtenerCorreos(request.Identificacion);

                oferente.Telefonos =
                    ObtenerTelefonos(request.Identificacion);

                oferente.PreparacionAcademica =
                    ObtenerPreparacionAcademica(
                        request.Identificacion
                    );

                oferente.ExperienciaLaboral =
                    ObtenerExperienciaLaboral(
                        request.Identificacion
                    );

                oferente.Concursos =
                    ObtenerConcursos(request.Identificacion);

                return oferente;
            }
        }

        private List<string> ObtenerCorreos(string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT email
                    FROM oferente_emails
                    WHERE identificacion = @Identificacion
                    ORDER BY id;";

                return connection.Query<string>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                ).ToList();
            }
        }

        private List<string> ObtenerTelefonos(string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
            {
                string sql = @"
                    SELECT telefono
                    FROM oferente_telefonos
                    WHERE identificacion = @Identificacion
                    ORDER BY id;";

                return connection.Query<string>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                ).ToList();
            }
        }

        private List<PreparacionAcademicaResponse>
            ObtenerPreparacionAcademica(string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
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

                return connection
                    .Query<PreparacionAcademicaResponse>(
                        sql,
                        new
                        {
                            Identificacion = identificacion
                        }
                    )
                    .ToList();
            }
        }

        private List<ExperienciaLaboralResponse>
            ObtenerExperienciaLaboral(string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
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

                return connection
                    .Query<ExperienciaLaboralResponse>(
                        sql,
                        new
                        {
                            Identificacion = identificacion
                        }
                    )
                    .ToList();
            }
        }

        private List<ConcursoResponse> ObtenerConcursos(
            string identificacion)
        {
            using (var connection =
                _connectionFactory.CreateConnection("OFE"))
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

                return connection.Query<ConcursoResponse>(
                    sql,
                    new
                    {
                        Identificacion = identificacion
                    }
                ).ToList();
            }
        }
    }
}