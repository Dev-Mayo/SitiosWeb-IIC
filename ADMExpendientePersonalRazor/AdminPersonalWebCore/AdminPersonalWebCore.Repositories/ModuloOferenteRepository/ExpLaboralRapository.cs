using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AdminPersonalWebCore.Repository.ModuloOferenteRepository
{
    public class ExpLaboralRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ExpLaboralRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<ExpLaboral>> ObtenerExpLaboralAsync(string oferenteIdentificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var lista = await connection.QueryAsync<ExpLaboral>(
                    "sp_ObtenerExpLaboral",
                    new { p_oferente_identificacion = oferenteIdentificacion },
                    commandType: CommandType.StoredProcedure
                );
                return lista;
            }
        }

        public async Task<ExpLaboral> ObtenerExpLaboralPorId(int Id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var oferente = await connection.QueryFirstOrDefaultAsync<ExpLaboral>(
                    "sp_ObtenerExpLaboralPorId",
                    new { p_Id = Id },
                    commandType: CommandType.StoredProcedure
                );
                return oferente;
            }
        }

        public async Task<int> CrearExpLaboralAsync(ExpLaboral exp)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_CrearExpLaboral",
                    new
                    {
                        p_empresa = exp.Empresa,
                        p_identificacion = exp.OferenteId,
                        p_puesto = exp.Puesto,
                        p_fecha_inicio = exp.FechaInicio,
                        p_fecha_fin = exp.FechaFin
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; // 1 = éxito, 0 = error
            }
        }

        public async Task<int> ModificarExpLaboralAsync(ExpLaboral exp)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_ModificarExpLaboral",
                    new
                    {
                        p_id = exp.Id,
                        p_empresa = exp.Empresa,
                        p_puesto = exp.Puesto,
                        p_fecha_inicio = exp.FechaInicio,
                        p_fecha_fin = exp.FechaFin
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; // 1 = éxito, 0 = error
            }
        }

        public async Task<int> EliminarExpLaboralAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_EliminarExpLaboral",
                    new { p_id = id },
                    commandType: CommandType.StoredProcedure
                );
                return result;
                // 1 = eliminado, 0 = no existe, 2 = asociado a oferente
            }
        }
    }
}