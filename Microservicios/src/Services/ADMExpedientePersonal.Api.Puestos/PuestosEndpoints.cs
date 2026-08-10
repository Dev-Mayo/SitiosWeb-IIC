using ADMExpedientePersonal.Api.Puestos.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADMExpedientePersonal.Api.Puestos
{
    public static class PuestosEndpoints
    {
        public static void MapPuestosEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/puestos")
                .WithTags("Puestos")
                .RequireCors("ReactDev")
                .RequireAuthorization();

            // GET /api/puestos?pagina=1&tamanoPagina=10
            group.MapGet("/",
                async (
                    [FromQuery] int pagina,
                    [FromQuery] int tamanoPagina,
                    [FromServices] PuestoService service
                ) =>
                {
                    if (pagina <= 0)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "La página debe ser mayor a cero."
                        });
                    }

                    if (tamanoPagina <= 0)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El tamaño de página debe ser mayor a cero."
                        });
                    }

                    var result = await service
                        .ListarPuestosDisponiblesAsync(
                            pagina,
                            tamanoPagina
                        );

                    if (!result.Success)
                    {
                        return Results.Problem(
                            result.Mensaje,
                            statusCode: 500
                        );
                    }

                    return Results.Ok(result);
                })
                .WithName("ListarPuestosDisponibles");
        }
    }
}