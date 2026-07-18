using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using ADMExpedientePersonal.WebServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ADMExpedientePersonal.WebServices
{
    public class OferenteServiceHost : IOferenteService
    {
        private readonly OferenteService _service;

        public OferenteServiceHost()
        {
            _service = new OferenteService(
                RepositoryFactory.CrearOferenteRepository()
            );
        }

        public OferentesPorPuestoResponse ObtenerOferentesPorPuesto(OferentesPorPuestoRequest request)
        {
            return _service.ObtenerOferentesPorPuesto(request);
        }
    }
}
