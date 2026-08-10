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

        public async Task<PuestosDisponiblesResponse> ListarPuestosDisponiblesAsync(
            int pagina,
            int tamanoPagina
        )
        {
            try
            {
                var puestos = await _repository.ListarDisponiblesAsync(
                    pagina,
                    tamanoPagina
                );

                var totalRegistros =
                    await _repository.ContarDisponiblesAsync();

                var totalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)tamanoPagina
                );

                return new PuestosDisponiblesResponse
                {
                    Success = true,
                    Mensaje = "OK",
                    Puestos = puestos,
                    Pagina = pagina,
                    TamanoPagina = tamanoPagina,
                    TotalRegistros = totalRegistros,
                    TotalPaginas = totalPaginas
                };
            }
            catch (Exception ex)
            {
                return new PuestosDisponiblesResponse
                {
                    Success = false,
                    Mensaje = "Error técnico: " + ex.Message,
                    Puestos = new List<PuestoDto>(),
                    Pagina = pagina,
                    TamanoPagina = tamanoPagina,
                    TotalRegistros = 0,
                    TotalPaginas = 0
                };
            }
        }
    }
}