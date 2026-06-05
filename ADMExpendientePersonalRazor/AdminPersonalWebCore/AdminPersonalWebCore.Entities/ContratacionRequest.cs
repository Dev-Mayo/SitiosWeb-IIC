using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class ContratacionRequest
    {
        public string IdentificacionOferente { get; set; } = string.Empty;
        public int PuestoId { get; set; }
        public int JefaturaId { get; set; }
    }
}
