using MySql.Data.MySqlClient;
using System.Configuration;

public class ConexionEMP
{
    public static MySqlConnection ObtenerConexion()
    {
        return new MySqlConnection(
            ConfigurationManager.ConnectionStrings["EMP"].ConnectionString
        );
    }
}