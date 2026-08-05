using ADMExpedientePersonal.Api.Oferentes.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADMExpedientePersonal.Api.Oferentes
{
    public static class OferentesEndpoints
    {
        public static void MapOferentesEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/oferentes")
                .WithTags("Oferentes")
                .RequireCors("ReactDev")
                .RequireAuthorization();

            // GET /api/oferentes?codigoPuesto=123&pagina=1&tamanoPagina=10
            group.MapGet("/",
                async (
                    [FromServices] OferenteService service,
                    HttpContext context,
                    [FromQuery] int ? codigoPuesto,
                    [FromQuery] int pagina = 1,
                    [FromQuery] int tamanoPagina = 10) =>
                {
                    if (codigoPuesto is null|| codigoPuesto <= 0)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "Debe indicar un código de puesto válido."
                        });
                    }

                    if (pagina <= 0)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "La página debe ser mayor a cero."
                        });
                    }

                    if (tamanoPagina <= 0 || tamanoPagina > 100)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El tamaño de página debe estar entre 1 y 100."
                        });
                    }

                    var usuario =
                        context.User.Identity?.Name
                        ?? "USUARIO_NO_IDENTIFICADO";

                    var result =
                        await service.ObtenerOferentesPorPuestoAsync(
                            codigoPuesto.Value,
                            pagina,
                            tamanoPagina,
                            usuario
                        );

                    if (!result.Success)
                    {
                        return result.CodigoEstado switch
                        {
                            400 => Results.BadRequest(new
                            {
                                mensaje = result.Mensaje
                            }),

                            404 => Results.NotFound(new
                            {
                                mensaje = result.Mensaje
                            }),

                            _ => Results.Problem(
                                detail: result.Mensaje,
                                statusCode: 500
                            )
                        };
                    }

                    return Results.Ok(new
                    {
                        datos = result.Oferentes,
                        pagina = result.Pagina,
                        tamanoPagina = result.TamanoPagina,
                        totalRegistros = result.TotalRegistros,
                        totalPaginas = result.TotalPaginas
                    });
                })
                .WithName("ObtenerOferentesPorPuesto");
        }
    }
}