using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<BitacoraDisplay> ObtenerBitacoras(string usuario, string descripcion, string orden)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<BitacoraDisplay>(
                    "sp_listar_bitacoras",
                    new
                    {
                        p_usuario = string.IsNullOrEmpty(usuario) ? null : usuario,
                        p_descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion,
                        p_orden = string.IsNullOrEmpty(orden) ? "fecha_desc" : orden
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

    }
}
