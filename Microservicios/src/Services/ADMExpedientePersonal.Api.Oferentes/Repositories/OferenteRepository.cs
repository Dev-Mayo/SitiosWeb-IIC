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

        public async Task<List<OferenteDto>> ObtenerPorPuestoAsync(
      int codigoPuesto,
      int pagina,
      int tamanoPagina)
        {
            var resultado = new List<OferenteDto>();
            var offset = (pagina - 1) * tamanoPagina; // AQUÍ CALCULA EL OFFSET PARA LA PAGINACIÓN ANTES DE ABRUR LA CONEXIÓN

            using var conn =
                (MySqlConnection)_factory.CreateConnection("OFE");

            await conn.OpenAsync();

            using var cmd =
                new MySqlCommand("sp_obtener_oferentes_por_puesto", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
                "p_codigo_puesto",
                codigoPuesto
            ); // Enviar los nuevos parámetros al procedimiento

            cmd.Parameters.AddWithValue(
                "p_offset",
                offset
            );

            cmd.Parameters.AddWithValue(
                "p_tamano_pagina",
                tamanoPagina
            );

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new OferenteDto
                {
                    Identificacion =
                        reader["identificacion"].ToString() ?? string.Empty,

                    NombreCompleto =
                        reader["nombre_completo"].ToString() ?? string.Empty
                });
            }

            return resultado;
        }

        public async Task<int> ContarPorPuestoAsync(int codigoPuesto)
        {
            using var conn =
                (MySqlConnection)_factory.CreateConnection("OFE");

            await conn.OpenAsync();

            using var cmd =
                new MySqlCommand(
                    "sp_contar_oferentes_por_puesto",
                    conn
                );

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
                "p_codigo_puesto",
                codigoPuesto
            );

            var resultado = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(resultado);
        }

        public async Task<bool> ExistePuestoAsync(int codigoPuesto)
        {
            using var conn =
                (MySqlConnection)_factory.CreateConnection("EMP"); // aquí es EMP porque la tabla puestos está en la base EMP.

            await conn.OpenAsync();

            const string sql = @"
        SELECT COUNT(*)
        FROM puestos
        WHERE puesto_id = @codigoPuesto;
    ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@codigoPuesto", codigoPuesto);

            var resultado = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(resultado) > 0;
        }
    }
}
