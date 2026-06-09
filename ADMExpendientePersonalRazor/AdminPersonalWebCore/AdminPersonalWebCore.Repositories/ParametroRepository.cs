using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AdminPersonalWebCore.Repository
{
    public class ParametroRepository
    {
        private readonly string _connectionString;

        public ParametroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Parametro> ObtenerTodos()
        {
            using var db = new MySqlConnection(_connectionString);

            return db.Query<Parametro>(
                "SP_GEN_PARAMETRO_LISTAR",
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public Parametro ObtenerPorCodigo(string codigo)
        {
            using var db = new MySqlConnection(_connectionString);

            return db.QueryFirstOrDefault<Parametro>(
                "SP_GEN_PARAMETRO_OBTENER",
                new { p_codigo_parametro = codigo },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Insertar(Parametro parametro)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_GEN_PARAMETRO_INSERTAR",
                new
                {
                    p_codigo_parametro = parametro.CodigoParametro,
                    p_valor = parametro.Valor
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Actualizar(Parametro parametro)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_GEN_PARAMETRO_ACTUALIZAR",
                new
                {
                    p_codigo_parametro = parametro.CodigoParametro,
                    p_valor = parametro.Valor
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Eliminar(string codigo)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_GEN_PARAMETRO_ELIMINAR",
                new { p_codigo_parametro = codigo },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}