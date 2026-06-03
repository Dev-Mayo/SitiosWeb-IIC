using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class MenuRepository
    {
        private readonly string _conn;
        public MenuRepository(string conn) => _conn = conn;

        public List<Modulo> ObtenerModulosPorUsuario(int idUsuario)
        {
            using var db = new MySqlConnection(_conn);
            return db.Query<Modulo>("sp_obtener_modulos_por_usuario",
                new { p_id_usuario = idUsuario },
                commandType: CommandType.StoredProcedure).ToList();
        }
    }
}