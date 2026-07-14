using System;

namespace ADMExpedientePersonal.WebServices.Entities.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string nombreusuario { get; set; }
        public string nombre_completo { get; set; }
        public string correo { get; set; }
        public string estado { get; set; }
    }
}