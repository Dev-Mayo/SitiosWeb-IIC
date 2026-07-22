using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories;
using System;
using System.Text;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

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

                // Identificación
                if (string.IsNullOrWhiteSpace(request.Identificacion))
                    return Error("La identificación es requerida.");

                request.Identificacion = request.Identificacion.Trim();

                if (!Regex.IsMatch(request.Identificacion, @"^\d+$"))
                    return Error("La identificación solamente puede contener números.");

                if (request.Identificacion.Length < 9 ||
                    request.Identificacion.Length > 12)
                {
                    return Error(
                        "La identificación debe contener entre 9 y 12 dígitos."
                    );
                }

                // Tipo de identificación
                if (string.IsNullOrWhiteSpace(request.TipoIdentificacion))
                    return Error("El tipo de identificación es requerido.");

                request.TipoIdentificacion =
                    request.TipoIdentificacion.Trim();

                if (!TipoIdentificacionValido(request.TipoIdentificacion))
                {
                    return Error(
                        "El tipo de identificación indicado no es válido."
                    );
                }

                // Nombre
                if (string.IsNullOrWhiteSpace(request.NombreCompleto))
                    return Error("El nombre completo es requerido.");

                request.NombreCompleto =
                    request.NombreCompleto.Trim();

                if (request.NombreCompleto.Length < 3)
                {
                    return Error(
                        "El nombre completo debe contener al menos 3 caracteres."
                    );
                }

                if (request.NombreCompleto.Length > 150)
                {
                    return Error(
                        "El nombre completo no puede superar los 150 caracteres."
                    );
                }

                if (!NombreValido(request.NombreCompleto))
                {
                    return Error(
                        "El nombre completo solamente puede contener letras, " +
                        "espacios, guiones y apóstrofes."
                    );
                }

                // Fecha de nacimiento
                if (request.FechaNacimiento == default(DateTime))
                    return Error("La fecha de nacimiento es requerida.");

                if (request.FechaNacimiento.Date > DateTime.Today)
                {
                    return Error(
                        "La fecha de nacimiento no puede ser una fecha futura."
                    );
                }

                if (CalcularEdad(request.FechaNacimiento) < 18)
                {
                    return Error(
                        "El empleado debe ser mayor de edad."
                    );
                }

                // Puesto
                if (request.PuestoId <= 0)
                    return Error("El puesto es requerido.");

                // Teléfonos
                if (request.Telefonos == null ||
                    request.Telefonos.Count == 0)
                {
                    return Error("Debe indicar al menos un teléfono.");
                }

                foreach (string telefono in request.Telefonos)
                {
                    if (string.IsNullOrWhiteSpace(telefono))
                        return Error("No se permiten teléfonos vacíos.");

                    if (!TelefonoValido(telefono))
                    {
                        return Error(
                            $"El teléfono '{telefono}' debe contener " +
                            "exactamente 8 dígitos."
                        );
                    }
                }

                // Normaliza los teléfonos antes de guardarlos
                request.Telefonos = request.Telefonos
                    .Select(NormalizarTelefono)
                    .Distinct()
                    .ToList();

                // Correos
                if (request.Correos == null ||
                    request.Correos.Count == 0)
                {
                    return Error("Debe indicar al menos un correo electrónico.");
                }

                foreach (string correo in request.Correos)
                {
                    if (string.IsNullOrWhiteSpace(correo))
                        return Error("No se permiten correos vacíos.");

                    if (!CorreoValido(correo))
                    {
                        return Error(
                            $"El correo electrónico '{correo}' no tiene " +
                            "un formato válido."
                        );
                    }

                    if (correo.Length > 150)
                    {
                        return Error(
                            "El correo electrónico no puede superar " +
                            "los 150 caracteres."
                        );
                    }
                }

                // Normaliza los correos
                request.Correos = request.Correos
                    .Select(correo => correo.Trim().ToLowerInvariant())
                    .Distinct()
                    .ToList();

                // Usuario
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return Error("El usuario que realiza la acción es requerido.");

                request.Usuario = request.Usuario.Trim();

                // Duplicados
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
                        : "Empleado creado con éxito, pero no se pudo " +
                          "registrar la bitácora.",
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
        private bool NombreValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            return Regex.IsMatch(
                nombre,
                @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s'-]+$"
            );
        }

        private bool CorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            try
            {
                string correoLimpio = correo.Trim();

                MailAddress direccion =
                    new MailAddress(correoLimpio);

                return direccion.Address == correoLimpio;
            }
            catch
            {
                return false;
            }
        }
        private bool TelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            string telefonoLimpio =
                NormalizarTelefono(telefono);

            return Regex.IsMatch(
                telefonoLimpio,
                @"^\d{8}$"
            );
        }

        private bool TipoIdentificacionValido(
    string tipoIdentificacion)
        {
            string[] tiposPermitidos =
            {
        "Nacional",
        "Extranjero",
        "DIMEX",
        "Pasaporte"
    };

            return tiposPermitidos.Any(
                tipo => tipo.Equals(
                    tipoIdentificacion,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            int edad =
                DateTime.Today.Year -
                fechaNacimiento.Year;

            if (fechaNacimiento.Date >
                DateTime.Today.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }

        private string NormalizarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return string.Empty;

            return telefono
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Trim();
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