namespace ADMExpedientePersonal.Api.DetalleOferente.Models
{
    public class DetalleOferenteResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Contratado { get; set; }
        public List<string> Correos { get; set; }
        public List<string> Telefonos { get; set; }
        public List<PreparacionAcademicaResponse> PreparacionAcademica { get; set; }
        public List<ExperienciaLaboralResponse> ExperienciaLaboral { get; set; }
        public List<ConcursoResponse> Concursos { get; set; }
    }
}
