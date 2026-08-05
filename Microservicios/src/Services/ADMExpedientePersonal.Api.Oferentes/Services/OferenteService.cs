using ADMExpedientePersonal.Api.Oferentes.Models;
using ADMExpedientePersonal.Api.Oferentes.Models;
using ADMExpedientePersonal.Api.Oferentes.Repositories;

namespace ADMExpedientePersonal.Api.Oferentes.Services
{
    public class OferenteService
    {
        private readonly OferenteRepository _repository;
        private readonly BitacoraRepository _bitacoraRepository;

        public OferenteService(
            OferenteRepository repository,
            BitacoraRepository bitacoraRepository)
        {
            _repository = repository;
            _bitacoraRepository = bitacoraRepository;
        }

        public async Task<OferentesPorPuestoResponse> ObtenerOferentesPorPuestoAsync(
            int codigoPuesto,
            string usuario)
        {
            try
            {
                if (codigoPuesto <= 0)
                {
                    return Error("Debe indicar un código de puesto válido.");
                }

                var oferentes =
                    await _repository.ObtenerPorPuestoAsync(codigoPuesto);

                await RegistrarConsultaEnBitacoraAsync(codigoPuesto, usuario);

                return new OferentesPorPuestoResponse
                {
                    Success = true,
                    Mensaje = "OK",
                    Oferentes = oferentes
                };
            }
            catch (Exception ex)
            {
                return Error("Error técnico: " + ex.Message);
            }
        }

        private async Task RegistrarConsultaEnBitacoraAsync(
            int codigoPuesto,
            string usuario)
        {
            try
            {
                string descripcionJson =
                    "{"
                    + "\"Consulta\":"
                    + "\"El usuario consulta oferentes disponibles\","
                    + "\"PuestoId\":"
                    + codigoPuesto
                    + "}";

                await _bitacoraRepository.RegistrarAsync(
                    ObtenerUsuario(usuario),
                    "READ",
                    descripcionJson
                );
            }
            catch
            {
                // Un fallo en la bitácora no debe romper el servicio.
            }
        }

        private string ObtenerUsuario(string usuario)
        {
            return string.IsNullOrWhiteSpace(usuario)
                ? "USUARIO_NO_IDENTIFICADO"
                : usuario;
        }

        private OferentesPorPuestoResponse Error(string mensaje)
        {
            return new OferentesPorPuestoResponse
            {
                Success = false,
                Mensaje = mensaje,
                Oferentes = new List<OferenteDto>()
            };
        }
    }
}
