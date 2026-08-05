namespace ADMExpedientePersonal.Api.Empleados.Models
{
    public class EmpleadoResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int EmpleadoId { get; set; }

        public int CodigoEstado { get; set; }
    }
}
