using System;

namespace ADMExpedientePersonal.Entities
{
    public class Area
    {
        public int codigo_area { get; set; }
        public string nombre { get; set; }
        public int jefatura { get; set; }

        public string nombre_jefatura { get; set; }
    }
}