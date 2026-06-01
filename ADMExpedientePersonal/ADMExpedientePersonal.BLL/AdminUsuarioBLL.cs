using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System.Collections.Generic;

namespace ADMExpedientePersonal.BLL
{
    public class AdminUsuarioBLL
    {
        private AdminUsuarioDAL adminUsuarioDAL = new AdminUsuarioDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        public List<Usuario> ObtenerUsuarios(string usuarioActual)
        {
            var usuarios = adminUsuarioDAL.ObtenerUsuarios();
            bitacoraBLL.RegistrarBitacora(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = bitacoraBLL.CrearJsonConsulta("Usuarios")
            });
            return usuarios;
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            return adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);
        }

        public List<Rol> ObtenerRoles()
        {
            return adminUsuarioDAL.ObtenerRoles();
        }

        public bool ValidarDuplicados(string username, string correo, int? idExcluir = null)
        {
            return adminUsuarioDAL.ValidarDuplicados(username, correo, idExcluir);
        }

        public int InsertarUsuario(string username, string fullname, string email,
            string password, string roles, string usuarioActual)
        {
            int resultado = adminUsuarioDAL.InsertarUsuario(username, fullname, email, password, roles);

            if (resultado > 0)
            {
                var nuevo = adminUsuarioDAL.ObtenerUsuarios()
                    .Find(u => u.nombreusuario == username);

                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = bitacoraBLL.CrearJsonNuevo(new
                    {
                        nuevo?.id_usuario,
                        nuevo?.nombreusuario,
                        nuevo?.nombre_completo,
                        nuevo?.correo,
                        nuevo?.estado,
                        nuevo?.roles
                        // password NOT included per SEG6 requirement
                    })
                });
            }

            return resultado;
        }

        public int ActualizarUsuario(int idUsuario, string username, string fullname,
            string email, string estado, string roles, string password, string usuarioActual)
        {
            var anterior = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);
            int resultado = adminUsuarioDAL.ActualizarUsuario(
                idUsuario, username, fullname, email, estado, roles, password);

            if (resultado > 0)
            {
                var nuevo = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = bitacoraBLL.CrearJsonActualizacion(
                        new
                        {
                            anterior.id_usuario,
                            anterior.nombreusuario,
                            anterior.nombre_completo,
                            anterior.correo,
                            anterior.estado,
                            anterior.roles
                        },
                        new
                        {
                            nuevo.id_usuario,
                            nuevo.nombreusuario,
                            nuevo.nombre_completo,
                            nuevo.correo,
                            nuevo.estado,
                            nuevo.roles
                        }
                    )
                });
            }

            return resultado;
        }

        public int EliminarUsuario(int idUsuario, string usuarioActual)
        {
            var usuario = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);

            adminUsuarioDAL.EliminarUsuario(idUsuario);

            
            var verificar = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);
            if (verificar == null)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.DELETE,
                    DescripcionJson = bitacoraBLL.CrearJsonEliminacion(new
                    {
                        usuario.id_usuario,
                        usuario.nombreusuario,
                        usuario.nombre_completo,
                        usuario.correo,
                        usuario.estado,
                        usuario.roles
                    })
                });
                return 2;
            }

            return 1;
        }

        public void CambiarEstado(int idUsuario, string nuevoEstado, string usuarioActual)
        {
            var anterior = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);
            adminUsuarioDAL.CambiarEstado(idUsuario, nuevoEstado);
            var nuevo = adminUsuarioDAL.ObtenerUsuarioPorId(idUsuario);

            bitacoraBLL.RegistrarBitacora(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = bitacoraBLL.CrearJsonActualizacion(
                    new { anterior.id_usuario, anterior.nombreusuario, anterior.estado },
                    new { nuevo.id_usuario, nuevo.nombreusuario, nuevo.estado }
                )
            });
        }
    }
}