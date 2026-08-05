namespace ADMExpedientePersonal.Api.Auth.Models
{
    public class AutenticacionResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public string NombreCompleto { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public bool Bloqueado { get; set; }
    }
}
