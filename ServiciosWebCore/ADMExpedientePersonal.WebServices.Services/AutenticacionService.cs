using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;
using System.Runtime.Caching;

namespace ADMExpedientePersonal.WebServices.Services
{
    public class AutenticacionService
    {
        private readonly AutenticacionRepository _repository;
        private readonly MemoryCache _cache = MemoryCache.Default;

        public AutenticacionService(AutenticacionRepository repository)
        {
            _repository = repository;
        }

        private string GetKey(string username) => $"login_attempts_{username}";

        public AutenticacionResponse Autenticar(AutenticacionRequest request)
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
                var userCheck = _repository.ObtenerPorNombre(request.Usuario);
                if (userCheck != null && userCheck.estado == "Bloqueado")
                    return Error("Usuario bloqueado.");

                // 2. Validar credenciales via SP
                var usuario = _repository.ValidarCredenciales(request);

                if (usuario == null)
                {
                    // 3. Incrementar intentos fallidos
                    string key = GetKey(request.Usuario);

                    int intentos = 0;

                    var cachedValue = _cache.Get(key);
                    if (cachedValue != null)
                    {
                        intentos = (int)cachedValue;
                    }

                    intentos++;

                    _cache.Set(key, intentos, new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
                    });

                    // 4. Bloquear tras 3 intentos
                    if (intentos >= 3 && userCheck != null)
                    {
                        _repository.BloquearUsuario(userCheck.id_usuario);
                        _cache.Remove(key);
                        return Error("Usuario bloqueado.");
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
                    IdUsuario = usuario.id_usuario
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
                Mensaje = mensaje
            };
        }
    }
}