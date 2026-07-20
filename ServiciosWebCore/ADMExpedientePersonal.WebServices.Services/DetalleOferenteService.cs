using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;

namespace ADMExpedientePersonal.WebServices.Services
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

        public DetalleOferenteResponse ObtenerDetalleOferente(
            DetalleOferenteRequest request)
        {
            try
            {
                if (request == null)
                    return Error(
                        "No se recibieron datos para consultar el oferente."
                    );

                if (string.IsNullOrWhiteSpace(request.Identificacion))
                    return Error(
                        "La identificación del oferente es requerida."
                    );

                var oferente =
                    _repository.ObtenerDetalleOferente(request);

                if (oferente == null)
                    return Error(
                        "No se encontró un oferente con la identificación indicada."
                    );

                RegistrarConsultaEnBitacora(request);

                oferente.Exito = true;
                oferente.Mensaje =
                    "Detalle del oferente obtenido correctamente.";

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

        private void RegistrarConsultaEnBitacora(
            DetalleOferenteRequest request)
        {
            try
            {
                string descripcionJson =
                    "{"
                    + "\"Consulta\":\"El usuario consulta el detalle del oferente\","
                    + "\"Identificacion\":\""
                    + EscaparJson(request.Identificacion)
                    + "\""
                    + "}";

                _bitacoraRepository.Registrar(
                    ObtenerUsuario(request.Usuario),
                    "READ",
                    descripcionJson
                );
            }
            catch
            {
                // No impedir que el servicio responda
                // si falla el registro de la bitácora.
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