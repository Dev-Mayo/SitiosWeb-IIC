using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;

namespace ADMExpedientePersonal.WebServices.Services
{
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _repository;

        public EmpleadoService(EmpleadoRepository repository)
        {
            _repository = repository;
        }

        public EmpleadoResponse RegistrarEmpleado(EmpleadoRequest request)
        {
            try
            {
                if (request == null)
                    return Error("No se recibieron datos del empleado.");

                if (string.IsNullOrWhiteSpace(request.Identificacion))
                    return Error("La identificación es requerida.");

                if (string.IsNullOrWhiteSpace(request.TipoIdentificacion))
                    return Error("El tipo de identificación es requerido.");

                if (string.IsNullOrWhiteSpace(request.NombreCompleto))
                    return Error("El nombre completo es requerido.");

                if (request.FechaNacimiento == default(DateTime))
                    return Error("La fecha de nacimiento es requerida.");

                if (request.PuestoId <= 0)
                    return Error("El puesto es requerido.");

                if (request.Telefonos == null || request.Telefonos.Count == 0)
                    return Error("Debe indicar al menos un teléfono.");

                if (_repository.ExisteEmpleado(request.Identificacion))
                    return Error("Ya existe un empleado con esa identificación.");

                int empleadoId = _repository.RegistrarEmpleado(request);

                return new EmpleadoResponse
                {
                    Exito = true,
                    Mensaje = "Empleado creado con éxito.",
                    EmpleadoId = empleadoId
                };
            }
            catch (Exception ex)
            {
                return Error("Error técnico al registrar el empleado: " + ex.Message);
            }
        }

        public int ProbarConexionEMP()
        {
            return _repository.ProbarConexionEMP();
        }

        private EmpleadoResponse Error(string mensaje)
        {
            return new EmpleadoResponse
            {
                Exito = false,
                Mensaje = mensaje,
                EmpleadoId = 0
            };
        }
    }
}