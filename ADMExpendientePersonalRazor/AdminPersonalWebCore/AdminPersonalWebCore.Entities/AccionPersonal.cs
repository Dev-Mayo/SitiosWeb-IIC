using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class AccionPersonal
    {
        public int AccionId { get; set; }
        public int CodigoAccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int EmpleadoId { get; set; }
        public int JefaturaId { get; set; }

        public string? NombreEmpleado { get; set; }
        public string? NombreJefatura { get; set; }
    }
}
