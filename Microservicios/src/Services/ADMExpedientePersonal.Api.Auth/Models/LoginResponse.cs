namespace ADMExpedientePersonal.Api.Auth.Models
{
    public class LoginResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public DateTime ExpiraEn { get; set; }
    }
}
