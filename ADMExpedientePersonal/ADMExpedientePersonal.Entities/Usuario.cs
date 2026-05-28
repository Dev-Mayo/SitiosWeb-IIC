using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
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
