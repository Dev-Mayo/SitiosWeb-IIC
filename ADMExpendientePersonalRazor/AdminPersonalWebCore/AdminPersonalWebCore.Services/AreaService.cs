using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Services
{
    public class AreaService
    {
        private readonly AreaRepository _repo;
        private readonly BitacoraService _bitacora;

        public AreaService(
            AreaRepository repo,
            BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<Area> ObtenerTodos(string usuarioActual)
        {
            try
            {
                var lista = _repo.ObtenerTodos();

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.READ,
                    DescripcionJson = _bitacora.CrearJsonConsulta("Áreas")
                });

                return lista;
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public Area ObtenerPorId(int codigoArea)
        {
            return _repo.ObtenerPorId(codigoArea);
        }

        public List<Empleado> ObtenerEmpleados()
        {
            return _repo.ObtenerEmpleados();
        }

        public void Insertar(Area area, string usuarioActual)
        {
            try
            {
                Validar(area);
                if (_repo.ExisteCodigo(area.CodigoArea))
                    throw new Exception("Ya existe un área con ese código.");
                _repo.Insertar(area);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = _bitacora.CrearJsonNuevo(new
                    {
                        area.CodigoArea,
                        area.Nombre,
                        area.Jefatura
                    })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Actualizar(Area area, string usuarioActual)
        {
            try
            {
                Validar(area);

                var anterior = _repo.ObtenerPorId(area.CodigoArea);

                _repo.Actualizar(area);

                var nuevo = _repo.ObtenerPorId(area.CodigoArea);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = _bitacora.CrearJsonActualizacion(
                        new
                        {
                            anterior.CodigoArea,
                            anterior.Nombre,
                            anterior.Jefatura
                        },
                        new
                        {
                            nuevo.CodigoArea,
                            nuevo.Nombre,
                            nuevo.Jefatura
                        })
                });
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public void Eliminar(int codigoArea, string usuarioActual)
        {
            try
            {
                var area = _repo.ObtenerPorId(codigoArea);

                if (area == null)
                    throw new Exception("El área indicada no existe.");

                _repo.Eliminar(codigoArea);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.DELETE,
                    DescripcionJson = _bitacora.CrearJsonEliminacion(new
                    {
                        area.CodigoArea,
                        area.Nombre,
                        area.Jefatura
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

        private void Validar(Area area)
        {
            if (area == null)
                throw new Exception("Debe indicar la información del área.");

            if (area.CodigoArea <= 0)
                throw new Exception("Debe indicar un código de área válido.");

            if (string.IsNullOrWhiteSpace(area.Nombre))
                throw new Exception("El nombre del área es requerido.");

            if (area.Nombre.Length > 100)
                throw new Exception("El nombre del área no puede superar los 100 caracteres.");

            if (!Regex.IsMatch(area.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new Exception("El nombre del área solo debe contener letras y espacios.");

            if (area.Jefatura <= 0)
                throw new Exception("Debe seleccionar una jefatura.");
        }
    }
}