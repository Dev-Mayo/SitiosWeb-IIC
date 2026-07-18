using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Services
{
    public class PuestoService
    {
        private readonly PuestoRepository _repository;

        public PuestoService(PuestoRepository repository)
        {
            _repository = repository;
        }

        public PuestosDisponiblesResponse ListarPuestosDisponibles()
        {
            try
            {
                var puestos = _repository.ListarDisponibles();
                return new PuestosDisponiblesResponse
                {
                    Success = true,
                    Mensaje = "OK",
                    Puestos = puestos
                };
            }
            catch (Exception ex)
            {
                return new PuestosDisponiblesResponse
                {
                    Success = false,
                    Mensaje = "Error técnico: " + ex.Message,
                    Puestos = new List<PuestoDto>()
                };
            }
        }
    }
}
