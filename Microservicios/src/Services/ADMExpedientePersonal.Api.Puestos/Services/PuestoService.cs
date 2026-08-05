using ADMExpedientePersonal.Api.Puestos.Models;
using ADMExpedientePersonal.Api.Puestos.Models;
using ADMExpedientePersonal.Api.Puestos.Repositories;

namespace ADMExpedientePersonal.Api.Puestos.Services
{
    public class PuestoService
    {
        private readonly PuestoRepository _repository;

        public PuestoService(PuestoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PuestosDisponiblesResponse> ListarPuestosDisponiblesAsync()
        {
            try
            {
                var puestos = await _repository.ListarDisponiblesAsync();
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
