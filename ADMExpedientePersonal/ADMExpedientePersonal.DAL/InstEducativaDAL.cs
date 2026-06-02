using ADMExpedientePersonal.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class InstEducativaDAL
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["GEN"].ConnectionString;

        public List<InstEducativa> ObtenerInstituciones()
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<InstEducativa>(
                    "sp_listar_inst_educativas",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        public int Insertar(string codigo, string nombre)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_crear_inst_educativa",
                    new { p_codigo = codigo, p_nombre = nombre },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public int Actualizar(string codigo, string nombre)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Execute(
                    "sp_actualizar_inst_educativa",
                    new { p_codigo = codigo, p_nombre = nombre },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Eliminar(string codigo)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                db.Execute(
                    "sp_eliminar_inst_educativa",
                    new { p_codigo = codigo },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public InstEducativa ObtenerPorCodigo(string codigo)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                return db.Query<InstEducativa>(
                    "SELECT codigo_institucion, nombre FROM GEN.inst_educativas WHERE codigo_institucion = @codigo",
                    new { codigo }
                ).FirstOrDefault();
            }
        }

        public bool ValidarDuplicado(string codigo, string nombre, string codigoExcluir = null)
        {
            using (var db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(*) FROM GEN.inst_educativas
                    WHERE (codigo_institucion = @codigo OR LOWER(nombre) = LOWER(@nombre))
                    AND (@codigoExcluir IS NULL OR codigo_institucion != @codigoExcluir)";
                return db.ExecuteScalar<int>(sql, new { codigo, nombre, codigoExcluir }) > 0;
            }
        }
    }
}