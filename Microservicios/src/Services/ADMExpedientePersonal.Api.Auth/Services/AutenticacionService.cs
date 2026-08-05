using ADMExpedientePersonal.Api.Auth.Models;
using ADMExpedientePersonal.Api.Auth.Models;
using ADMExpedientePersonal.Api.Auth.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace ADMExpedientePersonal.Api.Auth.Services
{
    public class AutenticacionService
    {
        private readonly AutenticacionRepository _repository;
        private readonly IMemoryCache _cache;

        public AutenticacionService(
            AutenticacionRepository repository,
            IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        private string GetKey(string username) => $"login_attempts_{username}";

        public async Task<AutenticacionResponse> AutenticarAsync(
            AutenticacionRequest request)
        {
            try
            {
                if (request == null ||
                    string.IsNullOrWhiteSpace(request.Usuario) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return Error("Usuario y/o contraseña incorrectos.");
                }

                // 1. Verificar si está bloqueado
                var userCheck = await _repository.ObtenerPorNombreAsync(request.Usuario);
                if (userCheck != null && userCheck.estado == "Bloqueado")
                    return Bloqueado();

                // 2. Validar credenciales via SP
                var usuario = await _repository.ValidarCredencialesAsync(request);

                if (usuario == null)
                {
                    // 3. Incrementar intentos fallidos
                    string key = GetKey(request.Usuario);

                    if (!_cache.TryGetValue(key, out int intentos))
                        intentos = 0;

                    intentos++;

                    _cache.Set(key, intentos, TimeSpan.FromMinutes(15));

                    // 4. Bloquear tras 3 intentos
                    if (intentos >= 3 && userCheck != null)
                    {
                        await _repository.BloquearUsuarioAsync(userCheck.id_usuario);
                        _cache.Remove(key);
                        return Bloqueado();
                    }

                    return Error("Usuario y/o contraseña incorrectos.");
                }

                // 5. Éxito
                _cache.Remove(GetKey(request.Usuario));

                return new AutenticacionResponse
                {
                    Success = true,
                    Mensaje = "OK",
                    NombreCompleto = usuario.nombre_completo,
                    IdUsuario = usuario.id_usuario,
                    Usuario = string.IsNullOrWhiteSpace(usuario.nombreusuario)
                        ? request.Usuario
                        : usuario.nombreusuario
                };
            }
            catch (Exception ex)
            {
                return Error("Error técnico: " + ex.Message);
            }
        }

        private AutenticacionResponse Error(string mensaje)
        {
            return new AutenticacionResponse
            {
                Success = false,
                Mensaje = mensaje,
                Bloqueado = false
            };
        }

        private AutenticacionResponse Bloqueado()
        {
            return new AutenticacionResponse
            {
                Success = false,
                Mensaje = "Usuario bloqueado.",
                Bloqueado = true
            };
        }
    }
}
