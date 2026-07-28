using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ADMExpedientePersonal.WebServices
{
    public class PuestoServiceHost : IPuestoService
    {
        private readonly PuestoService _service;

        public PuestoServiceHost()
        {
            _service = ServiceFactory.CrearPuestoService();
        }

        public PuestosDisponiblesResponse ListarPuestosDisponibles()
        {
            return _service.ListarPuestosDisponibles();
        }
    }
}
