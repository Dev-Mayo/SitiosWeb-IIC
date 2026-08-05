using ADMExpedientePersonal.Api.Oferentes.Models;
using ADMExpedientePersonal.Api.Oferentes.Data;
using MySqlConnector;
using System.Data;

namespace ADMExpedientePersonal.Api.Oferentes.Repositories
{
    public class OferenteRepository
    {
        private readonly IDbConnectionFactory _factory;

        public OferenteRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<OferenteDto>> ObtenerPorPuestoAsync(int codigoPuesto)
        {
            var resultado = new List<OferenteDto>();

            using (var conn = (MySqlConnection)_factory.CreateConnection("OFE"))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("sp_obtener_oferentes_por_puesto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_codigo_puesto", codigoPuesto);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resultado.Add(new OferenteDto
                            {
                                Identificacion = reader["identificacion"].ToString(),
                                NombreCompleto = reader["nombre_completo"].ToString()
                            });
                        }
                    }
                }
            }

            return resultado;
        }
    }
}
