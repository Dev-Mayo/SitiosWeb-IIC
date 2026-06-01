using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMExpedientePersonal.Entities;

namespace ADMExpedientePersonal.DAL
{
    public class BitacoraDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["BIT"].ConnectionString;

        public void RegistrarBitacora(string usuario, string accion, string descripcion)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                db.Open();
                string sql = "INSERT INTO bitacoras(usuario,accion,descripcion) VALUES(@u,@a,@d)";
                using (var cmd = new MySqlCommand(sql, db))
                {
                    cmd.Parameters.AddWithValue("@u", usuario);
                    cmd.Parameters.AddWithValue("@a", accion);
                    cmd.Parameters.Add("@d", MySqlDbType.JSON).Value = descripcion;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RegistrarBitacora(Bitacora bitacora)
        {
            if (bitacora == null) throw new ArgumentNullException(nameof(bitacora));

            using (var db = new MySqlConnection(connectionString))
            {
                db.Open();
                string sql = "INSERT INTO bitacoras(usuario,accion,descripcion) VALUES(@u,@a,@d)";
                using (var cmd = new MySqlCommand(sql, db))
                {
                    cmd.Parameters.AddWithValue("@u", bitacora.Usuario);
                    cmd.Parameters.AddWithValue("@a", bitacora.Accion.ToString());
                    cmd.Parameters.AddWithValue("@d", bitacora.DescripcionJson);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
