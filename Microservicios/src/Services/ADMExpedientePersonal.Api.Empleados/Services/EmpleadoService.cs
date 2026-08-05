using ADMExpedientePersonal.Api.Empleados.Models;
using ADMExpedientePersonal.Api.Empleados.Repositories;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace ADMExpedientePersonal.Api.Empleados.Services
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

        public async Task<EmpleadoResponse> RegistrarEmpleadoAsync(
            EmpleadoRequest request)
        {
            try
            {
                // Validación del cuerpo de la solicitud
                if (request == null)
                {
                    return Error(
                        "No se recibieron datos del empleado.",
                        400
                    );
                }

                // Identificación
                if (string.IsNullOrWhiteSpace(request.Identificacion))
                {
                    return Error(
                        "La identificación es requerida.",
                        400
                    );
                }

                request.Identificacion = request.Identificacion.Trim();

                if (!Regex.IsMatch(request.Identificacion, @"^\d+$"))
                {
                    return Error(
                        "La identificación solamente puede contener números.",
                        400
                    );
                }

                if (request.Identificacion.Length < 9 ||
                    request.Identificacion.Length > 12)
                {
                    return Error(
                        "La identificación debe contener entre 9 y 12 dígitos.",
                        400
                    );
                }

                // Nombre completo
                if (string.IsNullOrWhiteSpace(request.NombreCompleto))
                {
                    return Error(
                        "El nombre completo es requerido.",
                        400
                    );
                }

                request.NombreCompleto = request.NombreCompleto.Trim();

                if (request.NombreCompleto.Length < 3)
                {
                    return Error(
                        "El nombre completo debe contener al menos 3 caracteres.",
                        400
                    );
                }

                if (request.NombreCompleto.Length > 150)
                {
                    return Error(
                        "El nombre completo no puede superar los 150 caracteres.",
                        400
                    );
                }

                if (!NombreValido(request.NombreCompleto))
                {
                    return Error(
                        "El nombre completo solamente puede contener letras, espacios, guiones y apóstrofes.",
                        400
                    );
                }

                // Fecha de nacimiento
                if (request.FechaNacimiento == default)
                {
                    return Error(
                        "La fecha de nacimiento es requerida.",
                        400
                    );
                }

                if (request.FechaNacimiento.Date > DateTime.Today)
                {
                    return Error(
                        "La fecha de nacimiento no puede ser una fecha futura.",
                        400
                    );
                }

                if (CalcularEdad(request.FechaNacimiento) < 18)
                {
                    return Error(
                        "El empleado debe ser mayor de edad.",
                        400
                    );
                }

                // Puesto
                if (request.PuestoId <= 0)
                {
                    return Error(
                        "El puesto es requerido.",
                        400
                    );
                }

                // Teléfonos
                if (request.Telefonos == null ||
                    request.Telefonos.Count == 0)
                {
                    return Error(
                        "Debe indicar al menos un teléfono.",
                        400
                    );
                }

                foreach (string telefono in request.Telefonos)
                {
                    if (string.IsNullOrWhiteSpace(telefono))
                    {
                        return Error(
                            "No se permiten teléfonos vacíos.",
                            400
                        );
                    }

                    if (!TelefonoValido(telefono))
                    {
                        return Error(
                            $"El teléfono '{telefono}' debe contener exactamente 8 dígitos.",
                            400
                        );
                    }
                }

                request.Telefonos = request.Telefonos
                    .Select(NormalizarTelefono)
                    .Distinct()
                    .ToList();

                // Correos electrónicos
                if (request.Correos == null ||
                    request.Correos.Count == 0)
                {
                    return Error(
                        "Debe indicar al menos un correo electrónico.",
                        400
                    );
                }

                foreach (string correo in request.Correos)
                {
                    if (string.IsNullOrWhiteSpace(correo))
                    {
                        return Error(
                            "No se permiten correos vacíos.",
                            400
                        );
                    }

                    if (!CorreoValido(correo))
                    {
                        return Error(
                            $"El correo electrónico '{correo}' no tiene un formato válido.",
                            400
                        );
                    }

                    if (correo.Length > 150)
                    {
                        return Error(
                            "El correo electrónico no puede superar los 150 caracteres.",
                            400
                        );
                    }
                }

                request.Correos = request.Correos
                    .Select(correo => correo.Trim().ToLowerInvariant())
                    .Distinct()
                    .ToList();

                // Usuario que realiza la acción
                if (string.IsNullOrWhiteSpace(request.Usuario))
                {
                    return Error(
                        "El usuario que realiza la acción es requerido.",
                        400
                    );
                }

                request.Usuario = request.Usuario.Trim();



                if (!await _repository.ExistePuestoAsync(request.PuestoId))
                {
                    return Error(
                        "El puesto indicado no existe.",
                        404
                    );
                }

                if (!await _repository.ExisteOferenteAsync(request.Identificacion))
                {
                    return Error(
                        "El oferente indicado no existe.",
                        404
                    );
                }

                if (await _repository.EstaContratadoAsync(request.Identificacion))
                {
                    return Error(
                        "El oferente ya fue contratado.",
                        409
                    );
                }

                if (await _repository.ExisteEmpleadoAsync(request.Identificacion))
                {
                    return Error(
                        "Ya existe un empleado con esa identificación.",
                        409
                    );
                }

                // Registrar empleado
                int empleadoId =
                    await _repository.RegistrarEmpleadoAsync(request);

                // Marcar al oferente como contratado
                bool oferenteActualizado =
                    await _repository.MarcarOferenteComoContratadoAsync(
                        request.Identificacion
                    );

                if (!oferenteActualizado)
                {
                    return Error(
                        "El empleado fue creado, pero no se pudo marcar el oferente como contratado.",
                        500
                    );
                }

                // Registrar bitácora
                bool bitacoraRegistrada =
                    await RegistrarBitacoraCreacionAsync(
                        request,
                        empleadoId
                    );

                return new EmpleadoResponse
                {
                    Exito = true,
                    CodigoEstado = 201,
                    Mensaje = bitacoraRegistrada
                        ? "Empleado creado con éxito."
                        : "Empleado creado con éxito, pero no se pudo registrar la bitácora.",
                    EmpleadoId = empleadoId
                };
            }
            
            //catch (Exception ex)
            //{
            //    await RegistrarBitacoraErrorAsync(request, ex);

            //    // No se expone ex.Message al cliente.
            //    return Error(
            //        "Ocurrió un error interno del servidor.",
            //        500
            //    );
            //}
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en RegistrarEmpleadoAsync: {ex}"
                );

                await RegistrarBitacoraErrorAsync(request, ex);

                return Error(
                    "Ocurrió un error interno del servidor.",
                    500
                );
            }
        }

        public async Task<int> ProbarConexionEmpAsync()
        {
            return await _repository.ProbarConexionEmpAsync();
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
                MailAddress direccion = new(correoLimpio);

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

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            int edad =
                DateTime.Today.Year - fechaNacimiento.Year;

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

        private async Task<bool> RegistrarBitacoraCreacionAsync(
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

                await _bitacoraRepository.RegistrarAsync(
                    ObtenerUsuario(request),
                    "CREATE",
                    descripcionJson
                );

                return true;
            }
            catch
            {
                // Un error en la bitácora no revierte la creación.
                return false;
            }
        }

        private async Task RegistrarBitacoraErrorAsync(
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

                await _bitacoraRepository.RegistrarAsync(
                    ObtenerUsuario(request),
                    "ERROR",
                    descripcionJson
                );
            }
            catch
            {
                // Se conserva el error original.
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

        private EmpleadoResponse Error(
            string mensaje,
            int codigoEstado)
        {
            return new EmpleadoResponse
            {
                Exito = false,
                Mensaje = mensaje,
                CodigoEstado = codigoEstado,
                EmpleadoId = 0
            };
        }
    }
}