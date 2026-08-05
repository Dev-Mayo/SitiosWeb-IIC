using ADMExpedientePersonal.Api.DetalleOferente.Services;

namespace ADMExpedientePersonal.Api.DetalleOferente
{
    public static class DetalleOferenteEndpoints
    {
        public static void MapDetalleOferenteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/oferentes")
                .WithTags("DetalleOferente")
                .RequireCors("ReactDev")
                .RequireAuthorization();

            // GET /api/oferentes/{identificacion}
            group.MapGet("/{identificacion}",
                async (DetalleOferenteService service, HttpContext context, string identificacion) =>
                {
                    if (string.IsNullOrWhiteSpace(identificacion))
                        return Results.BadRequest(new { mensaje = "La identificación del oferente es requerida." });

                    var usuario = context.User.Identity?.Name ?? "USUARIO_NO_IDENTIFICADO";
                    var result = await service.ObtenerDetalleOferenteAsync(identificacion, usuario);

                    if (!result.Exito)
                    {
                        if (result.Mensaje.Contains("No se encontró"))
                            return Results.NotFound(new { mensaje = result.Mensaje });

                        if (result.Mensaje.StartsWith("Error técnico"))
                            return Results.Problem(result.Mensaje, statusCode: 500);

                        return Results.BadRequest(new { mensaje = result.Mensaje });
                    }

                    return Results.Ok(result);
                })
                .WithName("ObtenerDetalleOferente");
        }
    }
}
