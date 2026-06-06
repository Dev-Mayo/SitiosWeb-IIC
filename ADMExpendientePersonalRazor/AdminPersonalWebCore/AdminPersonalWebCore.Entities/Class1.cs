using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class UbicacionCarga
    {
        public string CodigoProvincia { get; set; }
        public string Provincia { get; set; }

        public string CodigoCanton { get; set; }
        public string Canton { get; set; }

        public string CodigoDistrito { get; set; }
        public string Distrito { get; set; }
    }
}