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


namespace ADMExpedientePersonal.DAL
{
    public class OferentesDAL
    {
        private string connectionStringOFE = ConfigurationManager.ConnectionStrings["OFE"].ConnectionString;
        private string connectionStringEMP = ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;

        public List<Oferente> ObtenerOferentes(string identificacion = null)
        {

            using (var db = new MySqlConnection(connectionStringOFE))
            {
                var parametros = new { p_identificacion = identificacion };

                var lookup = new Dictionary<string, Oferente>(); //para evitar duplicados cuando se devuelven multiples datos de un oferente

                var oferentes = db.Query<Oferente, string, string, int?, Oferente>(
                    "sp_obtener_oferentes",
                    (oferente, email, telefono, concurso) =>
                    {
                        if (!lookup.TryGetValue(oferente.identificacion, out var entry))
                        {
                            entry = oferente;
                            entry.email = new List<string>();
                            entry.telefono = new List<string>();
                            entry.codigo_concurso = new List<string>();
                            lookup.Add(entry.identificacion, entry);
                        }

                        if (!string.IsNullOrEmpty(email) && !entry.email.Contains(email))
                            entry.email.Add(email);

                        if (!string.IsNullOrEmpty(telefono) && !entry.telefono.Contains(telefono))
                            entry.telefono.Add(telefono);

                        if (concurso.HasValue && !entry.codigo_concurso.Contains(concurso.Value.ToString()))
                            entry.codigo_concurso.Add(concurso.Value.ToString());

                        return entry;
                    },
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "email,telefono,codigo_concurso"
                ).Distinct().ToList();

                return lookup.Values.ToList();
            }
        }

        /// <summary>
        ///     Obtiene solo el nombre y identificacion de los oferentes
        /// </summary>
        /// <returns>Lista de oferentes con solo nombre e identificacion</returns>
        public List<Oferente> ObtenerNombreOferentes()
        {

            using (var db = new MySqlConnection(connectionStringOFE))
            {

                var oferentes = new List<Oferente>();
                using (var conn = new MySqlConnection(connectionStringOFE))
                using (var cmd = new MySqlCommand("sp_obtener_nombre_oferentes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oferentes.Add(new Oferente
                            {
                                identificacion = reader["identificacion"].ToString(),
                                nombre_completo = reader["nombre_completo"].ToString()
                            });
                        }
                    }
                }
                return oferentes;
            }
        }

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

        /// <summary>
        /// Gestiona un oferente.
        /// </summary>
        /// <param name="accion"> Acción a realizar: 0 = insertar, 1 = Modificar 2 = Eliminar </param>
        /// <param name="oferente">Objeto con la información del oferente. Si va a eliminar solo importa la identificacion</param>
        /// <returns>0 = fallo, 1 = éxito, 2 = ya asignado, 3 = ya existe</returns>
        public int GestionarOferente(int accion, Oferente oferente)
        {
            using (var db = new MySqlConnection(connectionStringOFE))
            {
                var parametros = new
                {
                    accion,
                    p_identificacion = oferente.identificacion,
                    p_tipo_identificacion = oferente.tipo_identificacion,
                    p_nombre_completo = oferente.nombre_completo,
                    p_fecha_nacimiento = oferente.fecha_nacimiento,
                    p_contratado = oferente.contratado,
                    p_emails = oferente.EmailDisplay,       // lista separada por comas
                    p_telefonos = oferente.TelefonoDisplay, // lista separada por comas
                    p_concursos = oferente.ConcursoDisplay  // lista separada por comas
                };

                // Ejecuta el SP y obtiene el código de resultado
                var resultado = db.QuerySingle<int>(
                    "sp_GestionarOferente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return resultado; // 0:fallo, 1:exito, 2:ya asignado, 3:ya existe
            }
        }

        public List<ConcursoTemporal> ObtenerConcursos(string identificacion = null)
        {
            var concursos = new List<ConcursoTemporal>();

            using (var conn = new MySqlConnection(connectionStringOFE))
            using (var cmd = new MySqlCommand("sp_ObtenerConcursos", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de entrada
                if (string.IsNullOrEmpty(identificacion))
                    cmd.Parameters.AddWithValue("@p_identificacion", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@p_identificacion", identificacion);


                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        concursos.Add(new ConcursoTemporal
                        {
                            codigo_concurso = reader["codigo_concurso"].ToString(),
                            nombre = reader["nombre"].ToString(),
                            fecha_inicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                            fecha_fin = reader.GetDateTime(reader.GetOrdinal("fecha_fin"))
                        });
                    }
                }
            }

            return concursos;
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
