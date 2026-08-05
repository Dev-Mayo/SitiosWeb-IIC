namespace ADMExpedientePersonal.Api.DetalleOferente.Models
{
    public class ConcursoResponse
    {
        public int CodigoConcurso { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
    }
}
