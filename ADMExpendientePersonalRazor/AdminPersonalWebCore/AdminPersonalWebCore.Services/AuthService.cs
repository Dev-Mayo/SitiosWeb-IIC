using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace AdminPersonalWebCore.Services
{
    public class AuthService
    {
        private readonly UsuarioRepository _repo;
        private readonly IMemoryCache _cache;

        public AuthService(UsuarioRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        private string GetKey(string username) => $"login_attempts_{username}";

        public (bool success, string mensaje, Usuario usuario) Login(string username, string password)
        {
            var userCheck = _repo.ObtenerPorNombre(username);
            if (userCheck != null && userCheck.estado == "Bloqueado")
                return (false, "Usuario bloqueado.", null);

            var user = _repo.Login(username, password);

            if (user == null)
            {
                string key = GetKey(username);
                _cache.TryGetValue(key, out int intentos);
                intentos++;
                _cache.Set(key, intentos, TimeSpan.FromMinutes(15));

                if (intentos >= 3 && userCheck != null)
                {
                    _repo.BloquearUsuario(userCheck.id_usuario);
                    _cache.Remove(key);
                    return (false, "Usuario bloqueado.", null);
                }

                return (false, "Usuario y/o contraseña incorrectos.", null);
            }

            _cache.Remove(GetKey(username));
            return (true, "OK", user);
        }

        public Usuario ObtenerPorNombre(string username) => _repo.ObtenerPorNombre(username);
    }
}