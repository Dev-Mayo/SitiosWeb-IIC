using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using System;

// Alias para evitar conflicto con el nombre, porque se llama igual al que tengo en el proyecto.services
using EmpleadoBusinessService = ADMExpedientePersonal.WebServices.Services.EmpleadoService;

namespace ADMExpedientePersonal.WebServices
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly EmpleadoBusinessService _service;

        public EmpleadoService()
        {
            var empleadoRepository = RepositoryFactory.CrearEmpleadoRepository();
            var bitacoraRepository = RepositoryFactory.CrearBitacoraRepository();

            _service = new EmpleadoBusinessService(
                empleadoRepository,
                bitacoraRepository
            );
        }

        public EmpleadoResponse RegistrarEmpleado(EmpleadoRequest request)
        {
            return _service.RegistrarEmpleado(request);
        }

        public int ProbarConexionEMP()
        {
            return _service.ProbarConexionEMP();
        }
    }
}