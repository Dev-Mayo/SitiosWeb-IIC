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
    int pagina,
    int tamanoPagina,
    string usuario)
        {
            try
            {
                if (codigoPuesto <= 0)
                    return Error("Debe indicar un código de puesto válido.", 400);

                if (pagina <= 0)
                    return Error("La página debe ser mayor a cero.", 400);

                if (tamanoPagina <= 0 || tamanoPagina > 100)
                    return Error(
                        "El tamaño de página debe estar entre 1 y 100.",
                        400
                    );

                var puestoExiste =
                    await _repository.ExistePuestoAsync(codigoPuesto);

                if (!puestoExiste)
                {
                    return Error(
                        "El puesto indicado no existe.",
                        404
                    );
                }

                var oferentes = await _repository.ObtenerPorPuestoAsync(
                    codigoPuesto,
                    pagina,
                    tamanoPagina
                );

                var totalRegistros =
                    await _repository.ContarPorPuestoAsync(codigoPuesto);

                var totalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)tamanoPagina
                );

                await RegistrarConsultaEnBitacoraAsync(
                    codigoPuesto,
                    usuario
                );

                return new OferentesPorPuestoResponse
                {
                    Success = true,
                    Mensaje = "OK",
                    CodigoEstado = 200,
                    Oferentes = oferentes,
                    Pagina = pagina,
                    TamanoPagina = tamanoPagina,
                    TotalRegistros = totalRegistros,
                    TotalPaginas = totalPaginas
                };
            }
           
            catch (Exception ex)
            {
                // Se captura la excepción para registrar el detalle técnico en la bitácora.
                // Al cliente solo se le devuelve un mensaje genérico por seguridad.

                return Error(
                    "Ocurrió un error interno del servidor.",
                    500
                );
            }

            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error en ObtenerOferentesPorPuestoAsync: {ex}");

            //    return Error(
            //        "Ocurrió un error interno del servidor.",
            //        500
            //    );
            //} este lo usé para comprobar cuál era el error 500 que tenía, ya luego ví que era falta de string de conexión de la emp en el appsettings.json


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

        private OferentesPorPuestoResponse Error(string mensaje, int codigoEstado)
        {
            return new OferentesPorPuestoResponse
            {
                Success = false,
                Mensaje = mensaje,
                CodigoEstado = codigoEstado,
                Oferentes = new List<OferenteDto>()
            };
        }
    }
}
