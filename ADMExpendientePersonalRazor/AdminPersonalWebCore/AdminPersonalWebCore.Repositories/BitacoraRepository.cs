using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class BitacoraRepository
    {
        private readonly string _conn;
        public BitacoraRepository(string conn) => _conn = conn;

        public void Registrar(Bitacora bitacora)
        {
            using var db = new MySqlConnection(_conn);
            db.Open();
            string sql = "INSERT INTO bitacoras(usuario,accion,descripcion) VALUES(@u,@a,@d)";
            using var cmd = new MySqlCommand(sql, db);
            cmd.Parameters.AddWithValue("@u", bitacora.Usuario);
            cmd.Parameters.AddWithValue("@a", bitacora.Accion.ToString());
            cmd.Parameters.AddWithValue("@d", bitacora.DescripcionJson);
            cmd.ExecuteNonQuery();
        }

        public List<BitacoraDisplay> ObtenerBitacoras(string usuario, string descripcion, string orden)
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<BitacoraDisplay>("sp_listar_bitacoras",
                new
                {
                    p_usuario = string.IsNullOrEmpty(usuario) ? null : usuario,
                    p_descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion,
                    p_orden = string.IsNullOrEmpty(orden) ? "fecha_desc" : orden
                },
                commandType: CommandType.StoredProcedure).ToList();
        }
    }
}