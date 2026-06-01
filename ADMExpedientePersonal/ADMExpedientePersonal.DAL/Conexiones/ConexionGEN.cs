using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConexionGEN
{
    public static MySqlConnection ObtenerConexion()
    {
        return new MySqlConnection(
            ConfigurationManager.ConnectionStrings["GEN"].ConnectionString
        );
    }
}