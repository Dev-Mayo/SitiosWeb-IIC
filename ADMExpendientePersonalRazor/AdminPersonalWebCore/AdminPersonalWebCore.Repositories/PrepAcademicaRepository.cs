using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AdminPersonalWebCore.Entities;

namespace AdminPersonalWebCore.Repository
{
    public class PrepAcademicaRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public PrepAcademicaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Obtener preparación académica por identificación de oferente.
        /// </summary>
        public async Task<IEnumerable<PreparacionAcad>> ObtenerPreparacionAcadAsync(string identificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var lista = await connection.QueryAsync<PreparacionAcad>(
                    "sp_ObtenerPreparacionAcad",
                    new { p_identificacion = identificacion },
                    commandType: CommandType.StoredProcedure
                );
                return lista;
            }
        }

        /// <summary>
        /// Obtener preparación académica por ID.
        /// </summary>
        public async Task<PreparacionAcad?> ObtenerPreparacionAcadPorIdAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.QueryFirstOrDefaultAsync<PreparacionAcad>(
                    "sp_ObtenerPreparacionAcadPorId",
                    new { p_id = id },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        /// <summary>
        /// Crear nueva preparación académica.
        /// </summary>
        public async Task<int> CrearPreparacionAcadAsync(PreparacionAcad prep)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_CrearPreparacionAcad",
                    new
                    {
                        p_codigo_institucion = prep.CodigoInstitucion,
                        p_identificacion = prep.OferenteId,
                        p_titulo = prep.Titulo,
                        p_fecha_inicio = prep.FechaInicio,
                        p_fecha_fin = prep.FechaFin
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; // 1 = éxito, 0 = error
            }
        }

        /// <summary>
        /// Modificar preparación académica existente.
        /// </summary>
        public async Task<int> ModificarPreparacionAcadAsync(PreparacionAcad prep)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_ModificarPreparacionAcad",
                    new
                    {
                        p_id = prep.Id,
                        p_codigo_institucion = prep.CodigoInstitucion,
                        p_titulo = prep.Titulo,
                        p_fecha_inicio = prep.FechaInicio,
                        p_fecha_fin = prep.FechaFin
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; // 1 = éxito, 0 = error
            }
        }

        /// <summary>
        /// Eliminar preparación académica.
        /// </summary>
        public async Task<int> EliminarPreparacionAcadAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_EliminarPreparacionAcad",
                    new { p_id = id },
                    commandType: CommandType.StoredProcedure
                );
                return result;
                // 1 = eliminado, 0 = no existe, 2 = no permitido
            }
        }
    }
}