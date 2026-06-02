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

    // GEN1
    public class BitacoraDisplay
    {
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public string usuario { get; set; }
        public string accion { get; set; }
        public string descripcion { get; set; }
    }
}
