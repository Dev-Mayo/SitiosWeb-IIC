using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



        public bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(Identificacion))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(TipoIdentificacion))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                return false;
            }
            if (FechaNacimiento == default)
            {
                return false;
            }
            if (Email == null || !Email.Any())
            {
                return false;
            }
            if (Telefono == null || !Telefono.Any())
                return false;
            if (CodigoConcurso == null || !CodigoConcurso.Any())
                return false;
            if (FechaNacimiento > DateTime.Now)
                return false;
            if (Email.Any(e => !IsValidEmail(e)))
                return false;

            return true;
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
