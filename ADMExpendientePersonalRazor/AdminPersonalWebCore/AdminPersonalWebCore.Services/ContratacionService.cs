using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class ContratacionService
    {
        private readonly ContratacionRepository _repo;
        private readonly BitacoraService _bitacora;

        public ContratacionService(
            ContratacionRepository repo,
            BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<Oferente> ObtenerOferentesDisponibles(string usuarioActual)
        {
            try
            {
                var lista = _repo.ObtenerOferentesDisponibles();

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.READ,
                    DescripcionJson = _bitacora.CrearJsonConsulta("Oferentes disponibles para contratación")
                });

                return lista;
            }
            catch (Exception ex)
            {
                RegistrarError(usuarioActual, ex.Message);
                throw;
            }
        }

        public List<Puesto> ObtenerPuestos()
        {
            return _repo.ObtenerPuestos();
        }

        public List<Empleado> ObtenerJefaturas()
        {
            return _repo.ObtenerJefaturas();
        }

        public int ContratarEmpleado(ContratacionRequest request, string usuarioActual)
        {
            try
            {
                Validar(request);

                int empleadoId = _repo.ContratarEmpleado(request);

                _bitacora.Registrar(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = _bitacora.CrearJsonNuevo(new
                    {
                        EmpleadoId = empleadoId,
                        request.IdentificacionOferente,
                        request.PuestoId,
                        request.JefaturaId,
                        AccionPersonal = "Contratación de empleado"
                    })
                });

                return empleadoId;
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

        private void Validar(ContratacionRequest request)
        {
            if (request == null)
                throw new Exception("Debe indicar la información de contratación.");

            if (string.IsNullOrWhiteSpace(request.IdentificacionOferente))
                throw new Exception("Debe seleccionar un oferente.");

            if (request.PuestoId <= 0)
                throw new Exception("Debe seleccionar un puesto.");

            if (request.JefaturaId <= 0)
                throw new Exception("Debe seleccionar la jefatura que aprueba.");
        }

     
    }
}