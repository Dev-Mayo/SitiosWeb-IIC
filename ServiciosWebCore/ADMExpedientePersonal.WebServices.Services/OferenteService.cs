using ADMExpedientePersonal.WebServices.Entities.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;
using System.Collections.Generic;

namespace ADMExpedientePersonal.WebServices.Services
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

        public OferentesPorPuestoResponse ObtenerOferentesPorPuesto(
            OferentesPorPuestoRequest request)
        {
            try
            {
                if (request == null || request.CodigoPuesto <= 0)
                {
                    return Error(
                        "Debe indicar un código de puesto válido."
                    );
                }

                var oferentes =
                    _repository.ObtenerPorPuesto(request.CodigoPuesto);

                RegistrarConsultaEnBitacora(request);

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

        private void RegistrarConsultaEnBitacora(
            OferentesPorPuestoRequest request)
        {
            try
            {
                string descripcionJson =
                    "{"
                    + "\"Consulta\":"
                    + "\"El usuario consulta oferentes disponibles\","
                    + "\"PuestoId\":"
                    + request.CodigoPuesto
                    + "}";

                _bitacoraRepository.Registrar(
                    ObtenerUsuario(request.Usuario),
                    "READ",
                    descripcionJson
                );
            }
            catch
            {
                // La consulta ya fue realizada.
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