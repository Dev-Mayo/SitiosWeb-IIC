using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class RequisitoPuestoRepository
    {
        private readonly string _connectionString;

        public RequisitoPuestoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<RequisitoPuesto> ObtenerTodos()
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    requisito_id,
                    nombre
                FROM requisitos_puestos
                ORDER BY requisito_id DESC;";

            return db.Query<RequisitoPuesto>(sql).ToList();
        }

        public RequisitoPuesto ObtenerPorId(int id)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    requisito_id,
                    nombre
                FROM requisitos_puestos
                WHERE requisito_id = @id;";

            return db.QueryFirstOrDefault<RequisitoPuesto>(sql, new { id });
        }

        public void Insertar(RequisitoPuesto requisito)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                INSERT INTO requisitos_puestos (nombre)
                VALUES (@nombre);";

            db.Execute(sql, requisito);
        }

        public void Actualizar(RequisitoPuesto requisito)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                UPDATE requisitos_puestos
                SET nombre = @nombre
                WHERE requisito_id = @requisito_id;";

            db.Execute(sql, requisito);
        }

        public void Eliminar(int id)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
                DELETE FROM requisitos_puestos
                WHERE requisito_id = @id;";

            db.Execute(sql, new { id });
        }

        public bool ExisteNombre(string nombre, int? requisitoIdExcluir = null)
        {
            using var db = new MySqlConnection(_connectionString);

            string sql = @"
        SELECT COUNT(1)
        FROM requisitos_puestos
        WHERE LOWER(TRIM(nombre)) = LOWER(TRIM(@nombre))
          AND (@requisitoIdExcluir IS NULL OR requisito_id <> @requisitoIdExcluir);";

            return db.ExecuteScalar<int>(sql, new
            {
                nombre,
                requisitoIdExcluir
            }) > 0;
        }
    }
}