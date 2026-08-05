namespace ADMExpedientePersonal.Api.Empleados.Models
{
    public class EmpleadoRequest
    {
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int PuestoId { get; set; }
        public List<string> Correos { get; set; }
        public List<string> Telefonos { get; set; }
        public string Usuario { get; set; }
    }
}
