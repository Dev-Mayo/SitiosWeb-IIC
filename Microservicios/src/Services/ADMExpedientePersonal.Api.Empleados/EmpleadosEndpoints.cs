using ADMExpedientePersonal.Api.Empleados.Services;
using ADMExpedientePersonal.Api.Empleados.Models;
using ADMExpedientePersonal.Api.Empleados.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ADMExpedientePersonal.Api.Empleados
{
    public static class EmpleadosEndpoints
    {
        public static void MapEmpleadosEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/empleados")
                .WithTags("Empleados")
                .RequireCors("ReactDev")
                .RequireAuthorization();

            // POST /api/empleados
            group.MapPost("/",
                async (EmpleadoService service, HttpContext context, [FromBody] EmpleadoRequest request) =>
                {
                    if (request == null)
                        return Results.BadRequest(new { mensaje = "No se recibieron datos del empleado." });

                    // El usuario que ejecuta la acción sale del token, no del cuerpo.
                    request.Usuario = context.User.Identity?.Name ?? request.Usuario;

                    var result = await service.RegistrarEmpleadoAsync(request);

                    if (!result.Exito)
                    {
                        if (result.Mensaje.Contains("Ya existe"))
                            return Results.Conflict(new { mensaje = result.Mensaje });

                        if (result.Mensaje.StartsWith("Error técnico"))
                            return Results.Problem(result.Mensaje, statusCode: 500);

                        return Results.BadRequest(new { mensaje = result.Mensaje });
                    }

                    return Results.Created($"/api/empleados/{result.EmpleadoId}", result);
                })
                .WithName("RegistrarEmpleado");

            // GET /api/empleados/health (verifica conexión a EMP)
            group.MapGet("/health",
                async (EmpleadoRepository repository) =>
                {
                    try
                    {
                        var count = await repository.ProbarConexionEmpAsync();
                        return Results.Ok(new { conexion = true, empleados = count });
                    }
                    catch
                    {
                        return Results.Problem(
                            "No se pudo conectar a la base de datos.",
                            statusCode: 503);
                    }
                })
                .WithName("ProbarConexionEmp");
        }
    }
}
