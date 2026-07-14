using AdminPersonalWebCore.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class MenuRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MenuRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Modulo> ObtenerModulosPorUsuario(int idUsuario)
        {
            using var db = _connectionFactory.CreateConnection("SEG");
            return db.Query<Modulo>("sp_obtener_modulos_por_usuario",
                new { p_id_usuario = idUsuario },
                commandType: CommandType.StoredProcedure).ToList();
        }
    }
}