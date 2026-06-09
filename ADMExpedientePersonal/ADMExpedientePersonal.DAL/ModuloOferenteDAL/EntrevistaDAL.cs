using ADMExpedientePersonal.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ADMExpedientePersonal.DAL.ModuloOferenteDAL
{
    public class EntrevistaDAL
    {
        private string connectionStringOFE = ConfigurationManager.ConnectionStrings["OFE"].ConnectionString;
        private string connectionStringEMP = ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;

        
        /// <summary>
        ///     Obtiene solo el nombre y identificacion de los oferentes ************DEBERIA MOVERLO LUEGO A  LA SECCION DE EMPLEADOS DAL
        /// </summary>
        /// <returns>Lista de empleados con solo nombre e id </returns>
        public List<EmpleadoTemporal> ObtenerNombreEmpleados()
        {

            using (var db = new MySqlConnection(connectionStringEMP))
            {

                var empleados = new List<EmpleadoTemporal>();
                using (var conn = new MySqlConnection(connectionStringEMP))
                using (var cmd = new MySqlCommand("sp_obtener_nombre_empleados", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(new EmpleadoTemporal
                            {
                                EmpleadoId = Convert.ToInt32(reader["empleado_id"]),
                                NombreEmpleado = reader["nombre_completo"].ToString()
                            });
                        }
                    }
                }
                return empleados;
            }
        }

        // Obtener entrevistas
        public List<Entrevista> ObtenerEntrevistas(int? EntrevistaId = null)
        {
            var entrevistas = new List<Entrevista>();

            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("sp_ObtenerEntrevistas", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                // Parámetro de entrada
                if (!EntrevistaId.HasValue)
                    cmd.Parameters.AddWithValue("@p_EntrevistaId", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@p_EntrevistaId", EntrevistaId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entrevistas.Add(new Entrevista
                        {
                            EntrevistaId = reader.GetInt32("entrevista_id"),
                            OferenteIdentificacion = reader.GetString("oferente_id"),
                            EmpleadoId = reader.GetInt32("empleado_id"),
                            FechaEntrevista = reader.GetDateTime("fecha_entrevista"),
                            Estado = reader.GetString("estado")
                        });
                    }
                }
            }
            return entrevistas;
        }

        // Crear entrevista
        /// <summary>
        /// Crea una nueva entrevista para un oferente. El estado inicial siempre será "Pendiente"
        /// </summary>
        /// <param name="entrevista"> Necesita: OferenteIdentificacion, EmpleadoId, FechaEntrevista </param>
        /// <returns> 1:Exito </returns>
        public int CrearEntrevista(Entrevista entrevista)
        {
            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("sp_CrearEntrevista", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_identificacion", entrevista.OferenteIdentificacion);
                cmd.Parameters.AddWithValue("@p_empleado_id", entrevista.EmpleadoId);
                cmd.Parameters.AddWithValue("@p_fecha", entrevista.FechaEntrevista);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // devuelve 1 si éxito
            }
        }

        // Modificar entrevista
        /// <summary>
        /// Modifica la fecha o el empleado de una entrevista existente. No se puede modificar el oferente ni el estado desde aquí.
        /// </summary>
        /// <param name="entrevista"> Necesita: EntrevistaId, EmpleadoId, FechaEntrevista </param>
        /// <returns> 1:Exito, 0:Fallos </returns>
        public int ModificarEntrevista(Entrevista entrevista)
        {
            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("ModificarEntrevista", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_entrevista_id", entrevista.EntrevistaId);
                cmd.Parameters.AddWithValue("@p_empleado_id", entrevista.EmpleadoId);
                cmd.Parameters.AddWithValue("@p_fecha", entrevista.FechaEntrevista);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // 1 si éxito, 0 si no
            }
        }

        // Eliminar entrevista
        /// <summary>
        /// Elimina una entrevista existente.
        /// </summary>
        /// <param name="entrevistaId">ID de la entrevista a eliminar</param>
        /// <returns> 1:Exito, 0:Fallo </returns>
        public int EliminarEntrevista(int entrevistaId)
        {
            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("sp_EliminarEntrevista", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_entrevista_id", entrevistaId);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // 1 si éxito, 0 si no
            }
        }

        // Cambiar estado
        /// <summary>
        /// Cambia el estado de una entrevista existente. El nuevo estado debe ser "Pendiente", "Realizada" o "Eliminada".
        /// </summary>
        /// <param name="entrevistaId">ID de la entrevista a modificar</param>
        /// <returns> 1:Exito, 0:Fallo </returns>
        public int CambiarEstadoEntrevista(int EntrevistaId)
        {
            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("sp_ModificarEstadoEntrevista", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_EntrevistaId", EntrevistaId);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // 1 si éxito, 0 si no
            }
        }
    }
}
