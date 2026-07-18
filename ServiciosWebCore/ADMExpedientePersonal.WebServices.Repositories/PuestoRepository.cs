using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Repositories
{
    public class PuestoRepository
    {
        private readonly IDbConnectionFactory _factory;

        public PuestoRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<PuestoDto> ListarDisponibles()
        {
            var resultado = new List<PuestoDto>();

            using (IDbConnection conn = _factory.CreateConnection("EMP"))
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_listar_puestos_disponibles";
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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
