using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Services
{
    public class AdminUsuarioService
    {
        private readonly AdminUsuarioRepository _repo;
        private readonly BitacoraService _bitacora;

        public AdminUsuarioService(AdminUsuarioRepository repo, BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<Usuario> ObtenerUsuarios(string usuarioActual)
        {
            var lista = _repo.ObtenerUsuarios();
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacora.CrearJsonConsulta("Usuarios")
            });
            return lista;
        }

        public Usuario ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public List<Rol> ObtenerRoles() => _repo.ObtenerRoles();
        public bool ValidarDuplicados(string username, string correo, int? idExcluir = null) =>
            _repo.ValidarDuplicados(username, correo, idExcluir);

        public void Insertar(string username, string fullname, string email,
            string password, string roles, string usuarioActual)
        {
            _repo.Insertar(username, fullname, email, password, roles);
            var nuevo = _repo.ObtenerUsuarios().FirstOrDefault(u => u.nombreusuario == username);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.CREATE,
                DescripcionJson = _bitacora.CrearJsonNuevo(new
                {
                    nuevo?.id_usuario,
                    nuevo?.nombreusuario,
                    nuevo?.nombre_completo,
                    nuevo?.correo,
                    nuevo?.estado,
                    nuevo?.roles
                })
            });
        }

        public void Actualizar(int idUsuario, string username, string fullname,
            string email, string estado, string roles, string password, string usuarioActual)
        {
            var anterior = _repo.ObtenerPorId(idUsuario);
            _repo.Actualizar(idUsuario, username, fullname, email, estado, roles, password);
            var nuevo = _repo.ObtenerPorId(idUsuario);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(
                    new { anterior.id_usuario, anterior.nombreusuario, anterior.nombre_completo, anterior.correo, anterior.estado, anterior.roles },
                    new { nuevo.id_usuario, nuevo.nombreusuario, nuevo.nombre_completo, nuevo.correo, nuevo.estado, nuevo.roles }
                )
            });
        }

        public void Eliminar(int idUsuario, string usuarioActual)
        {
            var usuario = _repo.ObtenerPorId(idUsuario);
            _repo.Eliminar(idUsuario);
            var verificar = _repo.ObtenerPorId(idUsuario);
            if (verificar == null)
                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.DELETE,
                    DescripcionJson = _bitacora.CrearJsonEliminacion(new
                    {
                        usuario.id_usuario,
                        usuario.nombreusuario,
                        usuario.nombre_completo,
                        usuario.correo,
                        usuario.estado,
                        usuario.roles
                    })
                });
        }

        public void CambiarEstado(int idUsuario, string estado, string usuarioActual)
        {
            var anterior = _repo.ObtenerPorId(idUsuario);
            _repo.CambiarEstado(idUsuario, estado);
            var nuevo = _repo.ObtenerPorId(idUsuario);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(
                    new { anterior.id_usuario, anterior.nombreusuario, anterior.estado },
                    new { nuevo.id_usuario, nuevo.nombreusuario, nuevo.estado }
                )
            });
        }
    }
}