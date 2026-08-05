using ADMExpedientePersonal.Api.Auth.Services;
using ADMExpedientePersonal.Api.Auth.Models;
using ADMExpedientePersonal.Api.Auth.Models;
using ADMExpedientePersonal.Api.Auth.Security;
using Microsoft.AspNetCore.Mvc;

namespace ADMExpedientePersonal.Api.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/auth")
                .WithTags("Auth")
                .RequireCors("ReactDev");

            // POST /api/auth/login (público)
            group.MapPost("/login",
                async (
                    [FromServices] AutenticacionService service,
                    [FromServices] JwtSettings jwtSettings,
                    [FromBody] AutenticacionRequest request) =>
                {
                    var result = await service.AutenticarAsync(request);

                    if (!result.Success)
                    {
                        return result.Bloqueado
                            ? Results.Json(new { mensaje = result.Mensaje }, statusCode: 423)
                            : Results.Json(new { mensaje = result.Mensaje }, statusCode: 401);
                    }

                    var expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpireMinutes);
                    var token = JwtTokenGenerator.GenerarToken(
                        jwtSettings,
                        result.IdUsuario,
                        result.Usuario,
                        result.NombreCompleto,
                        expires);

                    return Results.Ok(new LoginResponse
                    {
                        Exito = true,
                        Mensaje = "OK",
                        IdUsuario = result.IdUsuario,
                        NombreCompleto = result.NombreCompleto,
                        Usuario = result.Usuario,
                        Token = token,
                        ExpiraEn = expires
                    });
                })
                .WithName("Login")
                .AllowAnonymous();

            // GET /api/auth/me (requiere token)
            group.MapGet("/me",
                (HttpContext context) =>
                {
                    var id = context.User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var usuario = context.User.Identity?.Name;

                    return Results.Ok(new { idUsuario = id, usuario });
                })
                .WithName("GetCurrentUser")
                .RequireAuthorization();
        }
    }
}
