using ADMExpedientePersonal.Api.Empleados.Models;
using ADMExpedientePersonal.Api.Empleados.Repositories;
using ADMExpedientePersonal.Api.Empleados.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADMExpedientePersonal.Api.Empleados
{
    public static class EmpleadosEndpoints
    {
        public static void MapEmpleadosEndpoints(
            this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/empleados")
                .WithTags("Empleados")
                .RequireCors("ReactDev")
                .RequireAuthorization();

            // POST /api/empleados
            group.MapPost("/",
                async (
                    [FromServices] EmpleadoService service,
                    HttpContext context,
                    [FromBody] EmpleadoRequest? request) =>
                {
                    if (request == null)
                    {
                        return Results.BadRequest(new
                        {
                            exito = false,
                            mensaje = "No se recibieron datos del empleado."
                        });
                    }

                    // El usuario se obtiene del token, no del cuerpo.
                    request.Usuario =
                        context.User.Identity?.Name
                        ?? "USUARIO_NO_IDENTIFICADO";

                    var result =
                        await service.RegistrarEmpleadoAsync(request);

                    if (!result.Exito)
                    {
                        return result.CodigoEstado switch
                        {
                            400 => Results.BadRequest(new
                            {
                                exito = false,
                                mensaje = result.Mensaje
                            }),

                            404 => Results.NotFound(new
                            {
                                exito = false,
                                mensaje = result.Mensaje
                            }),

                            409 => Results.Conflict(new
                            {
                                exito = false,
                                mensaje = result.Mensaje
                            }),

                            _ => Results.Json(
                                new
                                {
                                    exito = false,
                                    mensaje = result.Mensaje
                                },
                                statusCode: 500
                            )
                        };
                    }

                    return Results.Created(
                        $"/api/empleados/{result.EmpleadoId}",
                        new
                        {
                            exito = true,
                            mensaje = result.Mensaje,
                            empleadoId = result.EmpleadoId
                        }
                    );
                })
                .WithName("RegistrarEmpleado");

            // GET /api/empleados/health
            group.MapGet("/health",
                async (
                    [FromServices] EmpleadoRepository repository) =>
                {
                    try
                    {
                        var count =
                            await repository.ProbarConexionEmpAsync();

                        return Results.Ok(new
                        {
                            conexion = true,
                            empleados = count
                        });
                    }
                    catch
                    {
                        return Results.Json(
                            new
                            {
                                conexion = false,
                                mensaje =
                                    "No se pudo conectar a la base de datos."
                            },
                            statusCode: 503
                        );
                    }
                })
                .WithName("ProbarConexionEmp");
        }
    }
}