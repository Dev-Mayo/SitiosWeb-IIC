using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMExpedientePersonal.Entities.ModuloOferenteEntities;

namespace ADMExpedientePersonal.DAL.ModuloOferenteDAL
{
    public class PrepAcademicaDAL
    {
        public List<PreparacionAcad> ObtenerPreparacionAcad(string identificacion)
        {
            var lista = new List<PreparacionAcad>();

            using (var conn = ConexionOFE.ObtenerConexion())
            using (var cmd = new MySqlCommand("sp_ObtenerPreparacionAcad", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_identificacion", identificacion);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new PreparacionAcad
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            CodigoInstitucion = reader["codigo_institucion"].ToString(),
                            Institucion = reader["institucion"].ToString(),
                            Titulo = reader["titulo"].ToString(),
                            FechaInicio = reader["fecha_inicio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fecha_inicio"]),
                            FechaFin = reader["fecha_fin"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fecha_fin"])
                        });
                    }
                }
            }
            return lista;
        }

        public PreparacionAcad ObtenerPreparacionAcadPorId(int id)
        {
            using (var conn = ConexionOFE.ObtenerConexion())
            using (var cmd = new MySqlCommand("sp_ObtenerPreparacionAcadPorId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PreparacionAcad
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            CodigoInstitucion = reader["codigo_institucion"].ToString(),
                            Institucion = reader["institucion"].ToString(),
                            OferenteId = reader["oferente_id"].ToString(),
                            Titulo = reader["titulo"].ToString(),
                            FechaInicio = reader["fecha_inicio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fecha_inicio"]),
                            FechaFin = reader["fecha_fin"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fecha_fin"])
                        };
                    }
                }
            }
            return null; // si no existe el registro
        }


        public int CrearPreparacionAcad(PreparacionAcad prep)
        {
            using (var conn = ConexionOFE.ObtenerConexion())
            using (var cmd = new MySqlCommand("sp_CrearPreparacionAcad", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_codigo_institucion", prep.CodigoInstitucion);
                cmd.Parameters.AddWithValue("@p_identificacion", prep.OferenteId);
                cmd.Parameters.AddWithValue("@p_titulo", prep.Titulo);
                cmd.Parameters.AddWithValue("@p_fecha_inicio", prep.FechaInicio);
                cmd.Parameters.AddWithValue("@p_fecha_fin", prep.FechaFin);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result); // 1 = éxito, 0 = error
            }
        }

        public int ModificarPreparacionAcad(PreparacionAcad prep)
        {
            using (var conn = ConexionOFE.ObtenerConexion())
            using (var cmd = new MySqlCommand("sp_ModificarPreparacionAcad", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id", prep.Id);
                cmd.Parameters.AddWithValue("@p_codigo_institucion", prep.CodigoInstitucion);
                cmd.Parameters.AddWithValue("@p_titulo", prep.Titulo);
                cmd.Parameters.AddWithValue("@p_fecha_inicio", prep.FechaInicio);
                cmd.Parameters.AddWithValue("@p_fecha_fin", prep.FechaFin);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result); // 1 = éxito, 0 = error
            }
        }

        public int EliminarPreparacionAcad(int id)
        {
            using (var conn = ConexionOFE.ObtenerConexion())
            using (var cmd = new MySqlCommand("sp_EliminarPreparacionAcad", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id", id);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
                // 1 = eliminado, 0 = no existe, 2 = no permitido (asociado a oferente)
            }
        }

    }
}
