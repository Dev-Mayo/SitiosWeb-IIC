namespace ADMExpedientePersonal.Api.Oferentes.Security
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; } = 60;
    }
}
