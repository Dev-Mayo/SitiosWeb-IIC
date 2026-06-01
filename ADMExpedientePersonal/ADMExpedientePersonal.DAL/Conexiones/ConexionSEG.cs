using MySql.Data.MySqlClient;
using System.Configuration;

namespace ADMExpedientePersonal.DAL
{
    public class ConexionSEG
    {
        public static MySqlConnection ObtenerConexion()
        {
            string cadena = ConfigurationManager.ConnectionStrings["SEG"].ConnectionString;
            return new MySqlConnection(cadena);
        }
    }
}