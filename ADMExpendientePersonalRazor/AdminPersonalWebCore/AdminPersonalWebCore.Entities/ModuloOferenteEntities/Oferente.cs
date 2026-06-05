using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities.ModuloOferenteEntities
{
    public class Oferente
    {
        public string Identificacion { get; set; } = string.Empty;
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int Contratado { get; set; } = 0;

        // Listas para múltiples valores
        public List<string> Email { get; set; } = new List<string>();
        public List<string> Telefono { get; set; } = new List<string>();
        public List<string> CodigoConcurso { get; set; } = new List<string>();

        // Propiedades calculadas para mostrar (convertir a datos para ver directamente o enviar separados por comas)
        public string EmailDisplay => Email != null && Email.Any() ? string.Join(", ", Email) : "";
        public string TelefonoDisplay => Telefono != null && Telefono.Any() ? string.Join(", ", Telefono) : "";
        public string ConcursoDisplay => CodigoConcurso != null && CodigoConcurso.Any() ? string.Join(", ", CodigoConcurso) : "";


        // Errores acumulados tras la validación
        public List<string> Errores { get; private set; } = new List<string>();

        // Patrones requeridos para cada tipo de identificación
        private static readonly Dictionary<string, (Regex regex, string msg)> patrones = new Dictionary<string, (Regex, string)>(StringComparer.OrdinalIgnoreCase)
    {
        { "Cedula",    (new Regex(@"^\d{9}$", RegexOptions.Compiled), "Cédula: exactamente 9 dígitos.") },
        { "Dimex",     (new Regex(@"^\d{12}$", RegexOptions.Compiled), "DIMEX: exactamente 12 dígitos.") },
        { "Pasaporte", (new Regex(@"^[A-Za-z0-9]{6,9}$", RegexOptions.Compiled), "Pasaporte: 6-9 caracteres alfanuméricos.") }
    };

        /// <summary>
        /// Valida todos los datos del oferente. Devuelve true si pasa todas las validaciones.
        /// En caso de fallo, la lista Errores contendrá los mensajes explicativos.
        /// </summary>
        public bool ValidarDatos()
        {
            Errores.Clear();

            // Campos obligatorios básicos
            if (string.IsNullOrWhiteSpace(Identificacion))
                Errores.Add("La identificación es obligatoria.");

            if (string.IsNullOrWhiteSpace(TipoIdentificacion))
                Errores.Add("El tipo de identificación es obligatorio.");

            if (string.IsNullOrWhiteSpace(NombreCompleto))
                Errores.Add("El nombre completo es obligatorio.");

            if (FechaNacimiento == default)
                Errores.Add("La fecha de nacimiento es obligatoria.");

            if (FechaNacimiento > DateTime.Now)
                Errores.Add("La fecha de nacimiento no puede ser mayor a la fecha actual.");

            // Mínimos: al menos 1 correo y 1 teléfono
            if (Email == null || !Email.Any())
                Errores.Add("Debe tener al menos 1 correo electrónico.");

            if (Telefono == null || !Telefono.Any())
                Errores.Add("Debe tener al menos 1 teléfono.");

            // Validar correos
            if (Email != null && Email.Any())
            {
                var invalidEmails = Email.Where(e => string.IsNullOrWhiteSpace(e) || !IsValidEmail(e)).ToList();
                if (invalidEmails.Any())
                    Errores.Add("Uno o más correos electrónicos no son válidos: " + string.Join(", ", invalidEmails));
            }

            // Validar teléfonos
            if (Telefono == null || !Telefono.Any())
            {
                Errores.Add("Debe tener al menos un teléfono.");
            }

            // Validar tipo de identificación y patrón correspondiente
            if (!string.IsNullOrWhiteSpace(TipoIdentificacion))
            {
                if (!patrones.ContainsKey(TipoIdentificacion))
                {
                    Errores.Add("Tipo de identificación inválido. Valores permitidos: Cedula, Dimex, Pasaporte.");
                }
                else
                {
                    var (regex, msg) = patrones[TipoIdentificacion];
                    if (string.IsNullOrWhiteSpace(Identificacion) || !regex.IsMatch(Identificacion))
                    {
                        Errores.Add(msg);
                    }
                }
            }

            // Resultado final
            return !Errores.Any();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}