using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
{
    public enum AccionBitacora { CREATE, READ, UPDATE, DELETE, ERROR }

    public class Bitacora
    {
        public string Usuario { get; set; }
        public AccionBitacora Accion { get; set; }
        public string DescripcionJson { get; set; }
    }
}
