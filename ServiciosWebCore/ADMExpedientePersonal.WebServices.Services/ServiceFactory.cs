using ADMExpedientePersonal.WebServices.Repositories.Data;

// Alias para diferenciar los servicios de negocio de los servicios WCF
using EmpleadoBusinessService =
    ADMExpedientePersonal.WebServices.Services.EmpleadoService;

using DetalleOferenteBusinessService =
    ADMExpedientePersonal.WebServices.Services.DetalleOferenteService;

using PuestoBusinessService =
    ADMExpedientePersonal.WebServices.Services.PuestoService;

using OferenteBusinessService =
    ADMExpedientePersonal.WebServices.Services.OferenteService;

using AutenticacionBusinessService =
    ADMExpedientePersonal.WebServices.Services.AutenticacionService;

namespace ADMExpedientePersonal.WebServices.Common
{
    public static class ServiceFactory
    {
        public static EmpleadoBusinessService CrearEmpleadoService()
        {
            var empleadoRepository =
                RepositoryFactory.CrearEmpleadoRepository();

            var bitacoraRepository =
                RepositoryFactory.CrearBitacoraRepository();

            return new EmpleadoBusinessService(
                empleadoRepository,
                bitacoraRepository
            );
        }

        public static DetalleOferenteBusinessService CrearDetalleOferenteService()
        {
            var detalleOferenteRepository =
                RepositoryFactory.CrearDetalleOferenteRepository();

            var bitacoraRepository =
                RepositoryFactory.CrearBitacoraRepository();

            return new DetalleOferenteBusinessService(
                detalleOferenteRepository,
                bitacoraRepository
            );
        }

        public static PuestoBusinessService CrearPuestoService()
        {
            var puestoRepository =
                RepositoryFactory.CrearPuestoRepository();

            return new PuestoBusinessService(
                puestoRepository
            );
        }

        public static OferenteBusinessService CrearOferenteService()
        {
            var oferenteRepository =
                RepositoryFactory.CrearOferenteRepository();

            var bitacoraRepository =
                RepositoryFactory.CrearBitacoraRepository();

            return new OferenteBusinessService(
                oferenteRepository,
                bitacoraRepository
            );
        }
        public static AutenticacionBusinessService CrearAutenticacionService()
        {
            var autenticacionRepository =
                RepositoryFactory.CrearAutenticacionRepository();

            return new AutenticacionBusinessService(
                autenticacionRepository
            );
        }
    }
}