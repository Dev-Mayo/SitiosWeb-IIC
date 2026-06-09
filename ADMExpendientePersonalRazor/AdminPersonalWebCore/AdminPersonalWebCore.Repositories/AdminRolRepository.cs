using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using Dapper;

namespace AdminPersonalWebCore.Repository
{
    public class AdminRolRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AdminRolRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> RolEnUsoBitAsync(int idRol)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                var result = await connection.QuerySingleAsync<int>(
                    "sp_rol_en_uso_bit",
                    new { p_id_rol = idRol },
                    commandType: CommandType.StoredProcedure
                );
                return result == 1;
            }
        }

        public async Task<int> EliminarRolAsync(int idRol)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                string sql = "DELETE FROM roles WHERE id_rol = @id_rol";
                return await connection.ExecuteAsync(sql, new { id_rol = idRol });
            }
        }

        public async Task<int> InsertarRolAsync(string nombreRol)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                string sql = "INSERT INTO roles (nombre_rol) VALUES (@nombre_rol)";
                return await connection.ExecuteAsync(sql, new { nombre_rol = nombreRol });
            }
        }

        public async Task<int> ActualizarRolAsync(int idRol, string nombreRol)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                string sql = "UPDATE roles SET nombre_rol = @nombre_rol WHERE id_rol = @id_rol";
                return await connection.ExecuteAsync(sql, new { id_rol = idRol, nombre_rol = nombreRol });
            }
        }

        public async Task<IEnumerable<Rol>> ObtenerRolAsync(int? idRol = null, string nombreRol = null)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                if (idRol != null)
                {
                    string sql = "SELECT id_rol, nombre_rol FROM roles WHERE id_rol = @id_rol";
                    return await connection.QueryAsync<Rol>(sql, new { id_rol = idRol });
                }
                else if (nombreRol != null)
                {
                    string sql = "SELECT id_rol, nombre_rol FROM roles WHERE nombre_rol = @nombre_rol";
                    return await connection.QueryAsync<Rol>(sql, new { nombre_rol = nombreRol });
                }
                else
                {
                    string sql = "SELECT id_rol, nombre_rol FROM roles";
                    return await connection.QueryAsync<Rol>(sql);
                }
            }
        }

        public async Task<bool> ValidarDuplicadosAsync(string nombreRol)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("SEG"))
            {
                string sql = "SELECT COUNT(*) FROM roles WHERE LOWER(nombre_rol) = LOWER(@nombre_rol)";
                int count = await connection.ExecuteScalarAsync<int>(sql, new { nombre_rol = nombreRol });
                return count > 0;
            }
        }
    }
}
