using ADMExpedientePersonal.Api.Puestos.Models;
using ADMExpedientePersonal.Api.Puestos.Data;
using MySqlConnector;
using System.Data;

namespace ADMExpedientePersonal.Api.Puestos.Repositories
{
    public class PuestoRepository
    {
        private readonly IDbConnectionFactory _factory;

        public PuestoRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<PuestoDto>> ListarDisponiblesAsync()
        {
            var resultado = new List<PuestoDto>();

            using (var conn = (MySqlConnection)_factory.CreateConnection("EMP"))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("sp_listar_puestos_disponibles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resultado.Add(new PuestoDto
                            {
                                PuestoId = reader.GetInt32(reader.GetOrdinal("puesto_id")),
                                Nombre = reader["nombre"].ToString(),
                                Salario = reader.GetDecimal(reader.GetOrdinal("salario")),
                                Jefe = reader["nombre_jefe"] == DBNull.Value ? "Sin asignar" : reader["nombre_jefe"].ToString(),
                                Disponible = Convert.ToBoolean(reader["disponible"])
                            });
                        }
                    }
                }
            }
            return resultado;
        }
    }
}
