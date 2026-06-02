using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
{
    public class Oferente
    {
        public string identificacion { get; set; }
        public string tipo_identificacion { get; set; } // Cédula, DIMEX, Pasaporte
        public string nombre_completo { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public int contratado {  get; set; }

        // Listas para múltiples valores
        public List<string> email { get; set; } = new List<string>();
        public List<string> telefono { get; set; } = new List<string>();
        public List<string> codigo_concurso { get; set; } = new List<string>();

        // Propiedades calculadas para mostrar (convertir a datos para ver directamente
        public string EmailDisplay => email != null && email.Any() ? string.Join(", ", email) : "";
        public string TelefonoDisplay => telefono != null && telefono.Any() ? string.Join(", ", telefono) : "";
        public string ConcursoDisplay => codigo_concurso != null && codigo_concurso.Any() ? string.Join(", ", codigo_concurso) : "";



        public bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(tipo_identificacion))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(nombre_completo))
            {
                return false;
            }
            if (fecha_nacimiento == default)
            {
                return false;
            }
            if (contratado < 0 || contratado > 1)
            {
                return false;
            }
            if (email == null || !email.Any())
            {
                return false;
            }
            if (telefono == null || !telefono.Any())
                return false;
            if (codigo_concurso == null || !codigo_concurso.Any())
                return false;
            if (fecha_nacimiento > DateTime.Now)
                return false;
            if (email.Any(e => !IsValidEmail(e)))
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
