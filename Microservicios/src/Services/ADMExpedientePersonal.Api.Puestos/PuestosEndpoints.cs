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

            // GET /api/puestos
            group.MapGet("/",
                async ([FromServices] PuestoService service) =>
                {
                    var result = await service.ListarPuestosDisponiblesAsync();

                    if (!result.Success)
                        return Results.Problem(result.Mensaje, statusCode: 500);

                    return Results.Ok(result.Puestos);
                })
                .WithName("ListarPuestosDisponibles");
        }
    }
}
