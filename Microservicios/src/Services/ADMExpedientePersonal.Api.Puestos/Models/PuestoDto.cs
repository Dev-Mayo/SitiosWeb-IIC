namespace ADMExpedientePersonal.Api.Puestos.Models
{
    public class PuestoDto
    {
        public int PuestoId { get; set; }
        public string Nombre { get; set; }
        public decimal Salario { get; set; }
        public string Jefe { get; set; }
        public bool Disponible { get; set; }
    }
}
