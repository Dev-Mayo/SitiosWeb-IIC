using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AdminPersonalWebCore.Repository
{
    public class ConcursoRepository
    {
        private readonly string _connectionString;

        public ConcursoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Concurso> ObtenerTodos()
        {
            using var db = new MySqlConnection(_connectionString);

            return db.Query<Concurso>(
                "SP_OFE_CONCURSO_LISTAR",
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public Concurso ObtenerPorCodigo(string codigo)
        {
            using var db = new MySqlConnection(_connectionString);

            return db.QueryFirstOrDefault<Concurso>(
                "SP_OFE_CONCURSO_OBTENER",
                new { p_codigo_concurso = codigo },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Insertar(Concurso concurso)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_OFE_CONCURSO_INSERTAR",
                new
                {
                    p_codigo_concurso = concurso.CodigoConcurso,
                    p_nombre = concurso.Nombre,
                    p_fecha_inicio = concurso.FechaInicio,
                    p_fecha_fin = concurso.FechaFin,
                    p_estado = concurso.Estado
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Actualizar(Concurso concurso)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_OFE_CONCURSO_ACTUALIZAR",
                new
                {
                    p_codigo_concurso = concurso.CodigoConcurso,
                    p_nombre = concurso.Nombre,
                    p_fecha_inicio = concurso.FechaInicio,
                    p_fecha_fin = concurso.FechaFin,
                    p_estado = concurso.Estado
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Eliminar(string codigo)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_OFE_CONCURSO_ELIMINAR",
                new { p_codigo_concurso = codigo },
                commandType: CommandType.StoredProcedure
            );
        }

        public void CambiarEstado(string codigo)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_OFE_CONCURSO_CAMBIAR_ESTADO",
                new { p_codigo_concurso = codigo },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}