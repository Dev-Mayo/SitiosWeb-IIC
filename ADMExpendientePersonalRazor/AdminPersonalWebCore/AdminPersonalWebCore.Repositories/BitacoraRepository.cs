using AdminPersonalWebCore.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class BitacoraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Registrar(Bitacora bitacora)
        {
            using var db = _connectionFactory.CreateConnection("BIT");
            string sql = "INSERT INTO bitacoras(usuario,accion,descripcion) VALUES(@u,@a,@d)";
            db.Execute(sql, new
            {
                u = bitacora.Usuario,
                a = bitacora.Accion.ToString(),
                d = bitacora.DescripcionJson
            });
        }

        public List<BitacoraDisplay> ObtenerBitacoras(string usuario, string descripcion, string orden)
        {
            using var db = _connectionFactory.CreateConnection("BIT");
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