using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class Area
    {
        public int CodigoArea { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Jefatura { get; set; }

        // Campo para mostrar en pantalla con JOIN
        public string NombreJefatura { get; set; } = string.Empty;
    }
}