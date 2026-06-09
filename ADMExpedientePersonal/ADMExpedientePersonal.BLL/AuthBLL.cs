using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Runtime.Caching;

namespace ADMExpedientePersonal.BLL
{
    public class AuthBLL
    {
        private UsuarioDAL usuarioDAL = new UsuarioDAL();
        private MemoryCache cache = MemoryCache.Default;

        private string GetKey(string username) => $"login_attempts_{username}";

        public (bool success, string mensaje, Usuario usuario) Login(string username, string password)
        {

            var userCheck = usuarioDAL.ObtenerUsuario(username);
            if (userCheck != null && userCheck.estado == "Bloqueado")
                return (false, "Usuario bloqueado.", null);


            var user = usuarioDAL.Login(username, password);

            if (user == null)
            {

                string key = GetKey(username);
                int intentos = (int)(cache.Get(key) ?? 0);
                intentos++;
                cache.Set(key, intentos, DateTimeOffset.Now.AddMinutes(15));


                if (intentos >= 3 && userCheck != null)
                {
                    usuarioDAL.BloquearUsuario(userCheck.id_usuario);
                    cache.Remove(key);
                    return (false, "Usuario bloqueado.", null);
                }

                return (false, "Usuario y/o contraseña incorrectos.", null);
            }


            string successKey = GetKey(username);
            cache.Remove(successKey);

            return (true, "OK", user);
        }

        public Usuario ObtenerUsuarioPorNombre(string username)
        {
            return usuarioDAL.ObtenerUsuario(username);
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            return usuarioDAL.ObtenerUsuarioPorId(idUsuario);
        }
    }
}