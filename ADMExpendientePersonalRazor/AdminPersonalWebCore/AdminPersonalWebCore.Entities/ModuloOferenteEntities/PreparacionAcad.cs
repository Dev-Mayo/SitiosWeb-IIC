using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities.ModuloOferenteEntities
{
    public class PreparacionAcad
    {
        public int Id { get; set; }
        public string CodigoInstitucion { get; set; }
        public string Institucion { get; set; }
        public string OferenteId { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
