using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.DAL
{
    public class MenuDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["SEG"].ConnectionString;
        public List<Modulo> ObtenerModulosPorUsuario(int idUsuario)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                return conn.Query<Modulo>(
                    "sp_obtener_modulos_por_usuario",
                    new { p_id_usuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }
    }
}
