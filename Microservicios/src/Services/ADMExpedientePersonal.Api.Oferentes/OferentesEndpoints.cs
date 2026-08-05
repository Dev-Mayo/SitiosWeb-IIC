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

            // GET /api/oferentes?codigoPuesto=123
            group.MapGet("/",
                async ([FromServices] OferenteService service,
                       HttpContext context,
                       [FromQuery] int codigoPuesto) =>
                {
                    if (codigoPuesto <= 0)
                        return Results.BadRequest(new { mensaje = "Debe indicar un código de puesto válido." });

                    var usuario = context.User.Identity?.Name ?? "USUARIO_NO_IDENTIFICADO";
                    var result = await service.ObtenerOferentesPorPuestoAsync(codigoPuesto, usuario);

                    if (!result.Success)
                        return Results.Problem(result.Mensaje, statusCode: 500);

                    return Results.Ok(result.Oferentes);
                })
                .WithName("ObtenerOferentesPorPuesto");
        }
    }
}
