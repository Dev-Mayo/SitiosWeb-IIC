using AdminPersonalWebCore.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class InstEducativaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InstEducativaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<InstEducativa> ObtenerTodas()
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            return db.Query<InstEducativa>("sp_listar_inst_educativas",
                commandType: CommandType.StoredProcedure).ToList();
        }

        public InstEducativa ObtenerPorCodigo(string codigo)
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            return db.Query<InstEducativa>(
                "SELECT codigo_institucion, nombre FROM inst_educativas WHERE codigo_institucion = @codigo",
                new { codigo }).FirstOrDefault();
        }

        public void Insertar(string codigo, string nombre)
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            db.Execute("sp_crear_inst_educativa",
                new { p_codigo = codigo, p_nombre = nombre },
                commandType: CommandType.StoredProcedure);
        }

        public void Actualizar(string codigo, string nombre)
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            db.Execute("sp_actualizar_inst_educativa",
                new { p_codigo = codigo, p_nombre = nombre },
                commandType: CommandType.StoredProcedure);
        }

        public void Eliminar(string codigo)
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            db.Execute("sp_eliminar_inst_educativa",
                new { p_codigo = codigo },
                commandType: CommandType.StoredProcedure);
        }

        public bool ValidarDuplicado(string codigo, string nombre, string codigoExcluir = null)
        {
            using var db = _connectionFactory.CreateConnection("GEN");
            return db.ExecuteScalar<int>(@"
                SELECT COUNT(*) FROM inst_educativas
                WHERE (codigo_institucion = @codigo OR LOWER(nombre) = LOWER(@nombre))
                AND (@codigoExcluir IS NULL OR codigo_institucion != @codigoExcluir)",
                new { codigo, nombre, codigoExcluir }) > 0;
        }
    }
}