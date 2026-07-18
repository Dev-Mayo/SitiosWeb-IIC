using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;

namespace ADMExpedientePersonal.WebServices.Services
{
    public class DetalleOferenteService
    {
        private readonly DetalleOferenteRepository _repository;

        public DetalleOferenteService(
            DetalleOferenteRepository repository)
        {
            _repository = repository;
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