using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Entities.Entities.Models;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class OferenteRepository
    {
        private readonly IDbConnectionFactory _factory;

        public OferenteRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<OferenteDto> ObtenerPorPuesto(int codigoPuesto)
        {
            var resultado = new List<OferenteDto>();

            using (IDbConnection conn = _factory.CreateConnection("OFE"))
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_obtener_oferentes_por_puesto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var param = cmd.CreateParameter();
                    param.ParameterName = "p_codigo_puesto";
                    param.Value = codigoPuesto;
                    cmd.Parameters.Add(param);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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