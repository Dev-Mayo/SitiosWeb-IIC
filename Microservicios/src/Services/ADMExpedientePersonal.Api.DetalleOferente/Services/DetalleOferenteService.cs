using ADMExpedientePersonal.Api.DetalleOferente.Models;
using ADMExpedientePersonal.Api.DetalleOferente.Repositories;

namespace ADMExpedientePersonal.Api.DetalleOferente.Services
{
    public class DetalleOferenteService
    {
        private readonly DetalleOferenteRepository _repository;
        private readonly BitacoraRepository _bitacoraRepository;

        public DetalleOferenteService(
            DetalleOferenteRepository repository,
            BitacoraRepository bitacoraRepository)
        {
            _repository = repository;
            _bitacoraRepository = bitacoraRepository;
        }

        public async Task<DetalleOferenteResponse> ObtenerDetalleOferenteAsync(
            string identificacion,
            string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                    return Error("La identificación del oferente es requerida.");

                var oferente =
                    await _repository.ObtenerDetalleOferenteAsync(
                        new ADMExpedientePersonal.Api.DetalleOferente.Models.DetalleOferenteRequest
                        {
                            Identificacion = identificacion
                        });

                if (oferente == null)
                    return Error("No se encontró un oferente con la identificación indicada.");

                await RegistrarConsultaEnBitacoraAsync(identificacion, usuario);

                oferente.Exito = true;
                oferente.Mensaje = "Detalle del oferente obtenido correctamente.";

                return oferente;
            }
            catch (Exception ex)
            {
                return Error(
                    "Error técnico al obtener el detalle del oferente: "
                    + ex.Message
                );
            }
        }

        private async Task RegistrarConsultaEnBitacoraAsync(
            string identificacion,
            string usuario)
        {
            try
            {
                string descripcionJson =
                    "{"
                    + "\"Consulta\":\"El usuario consulta el detalle del oferente\","
                    + "\"Identificacion\":\""
                    + EscaparJson(identificacion)
                    + "\""
                    + "}";

                await _bitacoraRepository.RegistrarAsync(
                    ObtenerUsuario(usuario),
                    "READ",
                    descripcionJson
                );
            }
            catch
            {
                // No impedir que el servicio responda.
            }
        }

        private string ObtenerUsuario(string usuario)
        {
            return string.IsNullOrWhiteSpace(usuario)
                ? "USUARIO_NO_IDENTIFICADO"
                : usuario;
        }

        private string EscaparJson(string valor)
        {
            if (valor == null)
                return string.Empty;

            return valor
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        private DetalleOferenteResponse Error(string mensaje)
        {
            return new DetalleOferenteResponse
            {
                Exito = false,
                Mensaje = mensaje
            };
        }
    }
}
