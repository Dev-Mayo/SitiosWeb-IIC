using ADMExpedientePersonal.WebServices.Entities.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADMExpedientePersonal.WebServices.Services
{
    public class OferenteService
    {
        private readonly OferenteRepository _repository;

        public OferenteService(OferenteRepository repository)
        {
            _repository = repository;
        }

        public OferentesPorPuestoResponse ObtenerOferentesPorPuesto(OferentesPorPuestoRequest request)
        {
            try
            {
                if (request == null || request.CodigoPuesto <= 0)
                {
                    return Error("Debe indicar un código de puesto válido.");
                }

                var oferentes = _repository.ObtenerPorPuesto(request.CodigoPuesto);

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