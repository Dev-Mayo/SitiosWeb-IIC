using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class CompaniaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CompaniaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Compania> ObtenerTodos()
        {
            using var db = _connectionFactory.CreateConnection("GEN");

            return db.Query<Compania>(
                "SP_GEN_COMPANIA_LISTAR",
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public Compania ObtenerPorCodigo(string codigo)
        {
            using var db = _connectionFactory.CreateConnection("GEN");

            return db.QueryFirstOrDefault<Compania>(
                "SP_GEN_COMPANIA_OBTENER",
                new
                {
                    p_codigo_compania = codigo
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Insertar(Compania compania)
        {
            using var db = _connectionFactory.CreateConnection("GEN");

            db.Execute(
                "SP_GEN_COMPANIA_INSERTAR",
                new
                {
                    p_codigo_compania = compania.CodigoCompania,
                    p_nombre = compania.Nombre
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Actualizar(Compania compania)
        {
            using var db = _connectionFactory.CreateConnection("GEN");

            db.Execute(
                "SP_GEN_COMPANIA_ACTUALIZAR",
                new
                {
                    p_codigo_compania = compania.CodigoCompania,
                    p_nombre = compania.Nombre
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Eliminar(string codigo)
        {
            using var db = _connectionFactory.CreateConnection("GEN");

            db.Execute(
                "SP_GEN_COMPANIA_ELIMINAR",
                new
                {
                    p_codigo_compania = codigo
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}