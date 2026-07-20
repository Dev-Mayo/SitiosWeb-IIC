using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;
using System.Text;

namespace ADMExpedientePersonal.WebServices.Services
{
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _repository;
        private readonly BitacoraRepository _bitacoraRepository;

        public EmpleadoService(
            EmpleadoRepository repository,
            BitacoraRepository bitacoraRepository)
        {
            _repository = repository;
            _bitacoraRepository = bitacoraRepository;
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

                if (request.Telefonos == null ||
                    request.Telefonos.Count == 0)
                {
                    return Error("Debe indicar al menos un teléfono.");
                }

                if (_repository.ExisteEmpleado(request.Identificacion))
                {
                    return Error(
                        "Ya existe un empleado con esa identificación."
                    );
                }

                int empleadoId =
                    _repository.RegistrarEmpleado(request);

                bool oferenteActualizado =
                    _repository.MarcarOferenteComoContratado(
                        request.Identificacion
                    );

                if (!oferenteActualizado)
                {
                    return Error(
                        "El empleado fue creado, pero no se pudo marcar " +
                        "el oferente como contratado."
                    );
                }

                bool bitacoraRegistrada =
                    RegistrarBitacoraCreacion(request, empleadoId);

                return new EmpleadoResponse
                {
                    Exito = true,
                    Mensaje = bitacoraRegistrada
                        ? "Empleado creado con éxito."
                        : "Empleado creado con éxito, pero no se pudo registrar la bitácora.",
                    EmpleadoId = empleadoId
                };
            }
            catch (Exception ex)
            {
                RegistrarBitacoraError(request, ex);

                return Error(
                    "Error técnico al registrar el empleado: "
                    + ex.Message
                );
            }
        }

        private bool RegistrarBitacoraCreacion(
            EmpleadoRequest request,
            int empleadoId)
        {
            try
            {
                string descripcionJson =
                    "{"
                    + "\"Mensaje\":\"Empleado creado correctamente\","
                    + "\"EmpleadoId\":" + empleadoId + ","
                    + "\"Identificacion\":\""
                    + EscaparJson(request.Identificacion) + "\","
                    + "\"NombreCompleto\":\""
                    + EscaparJson(request.NombreCompleto) + "\","
                    + "\"PuestoId\":" + request.PuestoId
                    + "}";

                _bitacoraRepository.Registrar(
                    ObtenerUsuario(request),
                    "CREATE",
                    descripcionJson
                );

                return true;
            }
            catch
            {
                /*
                 * El empleado ya fue creado.
                 * Si falla únicamente la bitácora, no se debe indicar
                 * que toda la operación de creación falló.
                 */
                return false;
            }
        }

        private void RegistrarBitacoraError(
            EmpleadoRequest request,
            Exception ex)
        {
            try
            {
                string identificacion =
                    request?.Identificacion ?? string.Empty;

                string nombreCompleto =
                    request?.NombreCompleto ?? string.Empty;

                string descripcionJson =
                    "{"
                    + "\"Mensaje\":\"Error técnico al registrar empleado\","
                    + "\"Identificacion\":\""
                    + EscaparJson(identificacion) + "\","
                    + "\"NombreCompleto\":\""
                    + EscaparJson(nombreCompleto) + "\","
                    + "\"Error\":\""
                    + EscaparJson(ex.Message) + "\""
                    + "}";

                _bitacoraRepository.Registrar(
                    ObtenerUsuario(request),
                    "ERROR",
                    descripcionJson
                );
            }
            catch
            {
                /*
                 * Si también falla la bitácora, se conserva
                 * el error original del registro del empleado.
                 */
            }
        }

        private string ObtenerUsuario(EmpleadoRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Usuario))
            {
                return "USUARIO_NO_IDENTIFICADO";
            }

            return request.Usuario;
        }

        private string EscaparJson(string valor)
        {
            if (valor == null)
                return string.Empty;

            var resultado = new StringBuilder();

            foreach (char caracter in valor)
            {
                switch (caracter)
                {
                    case '"':
                        resultado.Append("\\\"");
                        break;

                    case '\\':
                        resultado.Append("\\\\");
                        break;

                    case '\b':
                        resultado.Append("\\b");
                        break;

                    case '\f':
                        resultado.Append("\\f");
                        break;

                    case '\n':
                        resultado.Append("\\n");
                        break;

                    case '\r':
                        resultado.Append("\\r");
                        break;

                    case '\t':
                        resultado.Append("\\t");
                        break;

                    default:
                        if (caracter < 32)
                        {
                            resultado.AppendFormat(
                                "\\u{0:x4}",
                                (int)caracter
                            );
                        }
                        else
                        {
                            resultado.Append(caracter);
                        }

                        break;
                }
            }

            return resultado.ToString();
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