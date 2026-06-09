using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class Compania
    {
        [Required]
        public string CodigoCompania { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}
