using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class RequisitoPuestoDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;

        public List<RequisitoPuesto> ObtenerTodos()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        requisito_id,
                        nombre
                    FROM requisitos_puestos
                    ORDER BY requisito_id DESC";

                return db.Query<RequisitoPuesto>(sql).ToList();
            }
        }
   
        public RequisitoPuesto ObtenerPorId(int id)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT 
                requisito_id,
                nombre
            FROM requisitos_puestos
            WHERE requisito_id = @id";

                return db.Query<RequisitoPuesto>(sql, new { id }).FirstOrDefault();
            }
        }

         public void Insertar(RequisitoPuesto requisito)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            INSERT INTO requisitos_puestos (nombre)
            VALUES (@nombre)";

                db.Execute(sql, requisito);
            }
    }

        public void Actualizar(RequisitoPuesto requisito)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            UPDATE requisitos_puestos
            SET nombre = @nombre
            WHERE requisito_id = @requisito_id";

                db.Execute(sql, requisito);
            }
        }

        public void Eliminar(int id)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
            DELETE FROM requisitos_puestos
            WHERE requisito_id = @id";

                db.Execute(sql, new { id });
            }
        }
    }// RequisitoPuestoDAL
}