using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Services
{
    public class RequisitoPuestoService
    {
        private readonly RequisitoPuestoRepository _repo;
        private readonly BitacoraService _bitacora;

        public RequisitoPuestoService(
            RequisitoPuestoRepository repo,
            BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<RequisitoPuesto> ObtenerTodos(string usuarioActual)
        {
            try
            {
                var lista = _repo.ObtenerTodos();

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.READ,
                    DescripcionJson = _bitacora.CrearJsonConsulta("Requisitos de Puestos")
                });

                return lista;
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public RequisitoPuesto ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public void Insertar(RequisitoPuesto requisito, string usuarioActual)
        {
            try
            {
                Validar(requisito);

                _repo.Insertar(requisito);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = _bitacora.CrearJsonNuevo(new
                    {
                        requisito.nombre
                    })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Actualizar(RequisitoPuesto requisito, string usuarioActual)
        {
            try
            {
                Validar(requisito);

                var anterior = _repo.ObtenerPorId(requisito.requisito_id);

                _repo.Actualizar(requisito);

                var nuevo = _repo.ObtenerPorId(requisito.requisito_id);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = _bitacora.CrearJsonActualizacion(
                        new
                        {
                            anterior.requisito_id,
                            anterior.nombre
                        },
                        new
                        {
                            nuevo.requisito_id,
                            nuevo.nombre
                        })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Eliminar(int id, string usuarioActual)
        {
            try
            {
                var requisito = _repo.ObtenerPorId(id);

                _repo.Eliminar(id);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.DELETE,
                    DescripcionJson = _bitacora.CrearJsonEliminacion(new
                    {
                        requisito.requisito_id,
                        requisito.nombre
                    })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
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

        private void Validar(RequisitoPuesto requisito)
        {
            if (requisito == null)
                throw new Exception("Debe indicar la información del requisito.");

            if (string.IsNullOrWhiteSpace(requisito.nombre))
                throw new Exception("El nombre del requisito es requerido.");

            if (requisito.nombre.Length > 100)
                throw new Exception("El nombre del requisito no puede superar los 100 caracteres.");

            if (!Regex.IsMatch(requisito.nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new Exception("El nombre del requisito solo debe contener letras y espacios.");
        }
    }
}