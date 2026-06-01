using ADMExpedientePersonal.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class MenuDAL
    {
        public List<Modulo> ObtenerModulosPorUsuario(int idUsuario)
        {
            using (var conn = ConexionSEG.ObtenerConexion())
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