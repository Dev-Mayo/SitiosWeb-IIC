using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Services
{
    public class ModuloService
    {
        private readonly ModuloRepository _repo;
        private readonly BitacoraService _bitacora;

        public ModuloService(
            ModuloRepository repo,
            BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<Modulo> ObtenerTodos(string usuarioActual)
        {
            try
            {
                var lista = _repo.ObtenerTodos();

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.READ,
                    DescripcionJson = _bitacora.CrearJsonConsulta("Módulos")
                });

                return lista;
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public Modulo ObtenerPorId(int idModulo)
        {
            return _repo.ObtenerPorId(idModulo);
        }

        public List<Rol> ObtenerRoles()
        {
            return _repo.ObtenerRoles();
        }

        public List<int> ObtenerRolesPorModulo(int idModulo)
        {
            return _repo.ObtenerRolesPorModulo(idModulo);
        }

        public int Insertar(Modulo modulo, List<int> rolesSeleccionados, string usuarioActual)
        {
            try
            {
                Validar(modulo);
                if (_repo.ExisteNombre(modulo.nombre_modulo))
                    throw new Exception("Ya existe un módulo con ese nombre.");

                if (rolesSeleccionados == null || rolesSeleccionados.Count == 0)
                    throw new Exception("Debe seleccionar al menos un rol.");

                int idModulo = _repo.Insertar(modulo);

                _repo.ActualizarRolesModulo(idModulo, rolesSeleccionados);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = _bitacora.CrearJsonNuevo(new
                    {
                        id_modulo = idModulo,
                        modulo.nombre_modulo,
                        rolesSeleccionados
                    })
                });

                return idModulo;
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Actualizar(Modulo modulo, List<int> rolesSeleccionados, string usuarioActual)
        {
            try
            {
                Validar(modulo);
               
                if (modulo.id_modulo <= 0)
                    throw new Exception("Debe indicar un módulo válido.");

                if (rolesSeleccionados == null || rolesSeleccionados.Count == 0)
                    throw new Exception("Debe seleccionar al menos un rol.");

                var anterior = _repo.ObtenerPorId(modulo.id_modulo);

                if (anterior == null)
                    throw new Exception("El módulo indicado no existe.");

                if (_repo.ExisteNombre(modulo.nombre_modulo, modulo.id_modulo)) // aquí cambie el probelma de que no me dejaba actualizar porque capia sobre el mismo nombre, Valida nombres duplicados excluyendo el módulo actual en edición
                    throw new Exception("Ya existe un módulo con ese nombre.");

                var rolesAnteriores = _repo.ObtenerRolesPorModulo(modulo.id_modulo);

                _repo.Actualizar(modulo);
                _repo.ActualizarRolesModulo(modulo.id_modulo, rolesSeleccionados);

                var nuevo = _repo.ObtenerPorId(modulo.id_modulo);
                var rolesNuevos = _repo.ObtenerRolesPorModulo(modulo.id_modulo);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = _bitacora.CrearJsonActualizacion(
                        new
                        {
                            anterior.id_modulo,
                            anterior.nombre_modulo,
                            roles = rolesAnteriores
                        },
                        new
                        {
                            nuevo.id_modulo,
                            nuevo.nombre_modulo,
                            roles = rolesNuevos
                        })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Eliminar(int idModulo, string usuarioActual)
        {
            try
            {
                var modulo = _repo.ObtenerPorId(idModulo);

                if (modulo == null)
                    throw new Exception("El módulo indicado no existe.");

                if (_repo.TieneRolesAsignados(idModulo))
                    throw new Exception("No se puede eliminar un registro con datos relacionados.");

                _repo.Eliminar(idModulo);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.DELETE,
                    DescripcionJson = _bitacora.CrearJsonEliminacion(new
                    {
                        modulo.id_modulo,
                        modulo.nombre_modulo
                    })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);

                if (ex.Message.Contains("foreign key") ||
                    ex.Message.Contains("constraint"))
                {
                    throw new Exception("No se puede eliminar un registro con datos relacionados.");
                }

                throw;
            }
        }

        private void RegistrarError(string usuarioActual, string mensajeError)
        {
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.ERROR,
                DescripcionJson = _bitacora.CrearJsonError(mensajeError)
            });
        }

        private void Validar(Modulo modulo)
        {
            if (modulo == null)
                throw new Exception("Debe indicar la información del módulo.");

            if (string.IsNullOrWhiteSpace(modulo.nombre_modulo))
                throw new Exception("El nombre del módulo es requerido.");

            if (modulo.nombre_modulo.Length > 100)
                throw new Exception("El nombre del módulo no puede superar los 100 caracteres.");

            if (!Regex.IsMatch(modulo.nombre_modulo, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new Exception("El nombre del módulo solo debe contener letras y espacios.");
        }
    }
}