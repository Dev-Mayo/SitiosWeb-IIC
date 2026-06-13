using AdminPersonalWebCore.Entities;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Repository
{
    public class ModuloRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ModuloRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Modulo> ObtenerTodos()
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT
                    id_modulo,
                    nombre_modulo
                FROM modulos
                ORDER BY id_modulo DESC;";

            return db.Query<Modulo>(sql).ToList();
        }

        public Modulo ObtenerPorId(int idModulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT
                    id_modulo,
                    nombre_modulo
                FROM modulos
                WHERE id_modulo = @idModulo;";

            return db.QueryFirstOrDefault<Modulo>(sql, new { idModulo });
        }

        public int Insertar(Modulo modulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                INSERT INTO modulos
                    (nombre_modulo)
                VALUES
                    (@nombre_modulo);

                SELECT LAST_INSERT_ID();";

            return db.ExecuteScalar<int>(sql, modulo);
        }

        public void Actualizar(Modulo modulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                UPDATE modulos
                SET nombre_modulo = @nombre_modulo
                WHERE id_modulo = @id_modulo;";

            db.Execute(sql, modulo);
        }

        public void Eliminar(int idModulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                DELETE FROM modulos
                WHERE id_modulo = @idModulo;";

            db.Execute(sql, new { idModulo });
        }

        public List<Rol> ObtenerRoles()
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT
                    id_rol,
                    nombre_rol
                FROM roles
                ORDER BY nombre_rol ASC;";

            return db.Query<Rol>(sql).ToList();
        }

        public List<int> ObtenerRolesPorModulo(int idModulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT id_rol
                FROM roles_modulos
                WHERE id_modulo = @idModulo;";

            return db.Query<int>(sql, new { idModulo }).ToList();
        }

        public void ActualizarRolesModulo(int idModulo, List<int> rolesSeleccionados)
        {
            using var db = _connectionFactory.CreateConnection("SEG");
            db.Open();

            using var transaction = db.BeginTransaction();

            string eliminarSql = @"
                DELETE FROM roles_modulos
                WHERE id_modulo = @idModulo;";

            db.Execute(eliminarSql, new { idModulo }, transaction);

            string insertarSql = @"
                INSERT INTO roles_modulos
                    (id_rol, id_modulo)
                VALUES
                    (@idRol, @idModulo);";

            foreach (int idRol in rolesSeleccionados)
            {
                db.Execute(insertarSql, new { idRol, idModulo }, transaction);
            }

            transaction.Commit();
        }

        public bool TieneRolesAsignados(int idModulo)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT COUNT(1)
                FROM roles_modulos
                WHERE id_modulo = @idModulo;";

            return db.ExecuteScalar<int>(sql, new { idModulo }) > 0;
        }

        public bool ExisteNombre(string nombreModulo, int? idModuloExcluir = null)
        {
            using var db = _connectionFactory.CreateConnection("SEG");

            string sql = @"
                SELECT COUNT(1)
                FROM modulos
                WHERE LOWER(TRIM(nombre_modulo)) = LOWER(TRIM(@nombreModulo))
                  AND (@idModuloExcluir IS NULL OR id_modulo <> @idModuloExcluir);";

            return db.ExecuteScalar<int>(sql, new
            {
                nombreModulo,
                idModuloExcluir
            }) > 0;
        }
    }
}