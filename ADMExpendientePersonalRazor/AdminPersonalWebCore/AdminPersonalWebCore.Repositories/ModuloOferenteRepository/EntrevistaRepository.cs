using AdminPersonalWebCore.Entities;
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
    public class EntrevistaRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public EntrevistaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Obtiene solo el nombre e identificación de los empleados.
        /// </summary>
        public async Task<IEnumerable<Empleado>> ObtenerNombreEmpleadosAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection("EMP"))
            {
                var empleados = await connection.QueryAsync<Empleado>(
                    "sp_obtener_nombre_empleados",
                    commandType: CommandType.StoredProcedure
                );
                return empleados;
            }
        }



        /// <summary>
        /// Obtener entrevistas (todas o por ID).
        /// </summary>
        public async Task<IEnumerable<Entrevista>> ObtenerEntrevistasAsync(int? entrevistaId = null)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var entrevistas = await connection.QueryAsync<Entrevista>(
                    "sp_ObtenerEntrevistas",
                    new { p_EntrevistaId = entrevistaId ?? (object)DBNull.Value },
                    commandType: CommandType.StoredProcedure
                );
                return entrevistas;
            }
        }

        /// <summary>
        /// Crear nueva entrevista (estado inicial = Pendiente).
        /// </summary>
        public async Task<int> CrearEntrevistaAsync(Entrevista entrevista)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_CrearEntrevista",
                    new
                    {
                        p_identificacion = entrevista.OferenteIdentificacion,
                        p_empleado_id = entrevista.EmpleadoId,
                        p_fecha = entrevista.FechaEntrevista
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        /// <summary>
        /// Modificar fecha o empleado de una entrevista existente.
        /// </summary>
        public async Task<int> ModificarEntrevistaAsync(Entrevista entrevista)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "ModificarEntrevista",
                    new
                    {
                        p_entrevista_id = entrevista.EntrevistaId,
                        p_empleado_id = entrevista.EmpleadoId,
                        p_fecha = entrevista.FechaEntrevista
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        /// <summary>
        /// Eliminar entrevista.
        /// </summary>
        public async Task<int> EliminarEntrevistaAsync(int entrevistaId)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_EliminarEntrevista",
                    new { p_entrevista_id = entrevistaId },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        /// <summary>
        /// Cambiar estado de entrevista.
        /// </summary>
        public async Task<int> CambiarEstadoEntrevistaAsync(int entrevistaId)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_ModificarEstadoEntrevista",
                    new { p_EntrevistaId = entrevistaId },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
    }
}
