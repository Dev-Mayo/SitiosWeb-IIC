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
        //private string conexion = ConfigurationManager.ConnectionStrings["OFE"].ConnectionString;
        //private string connectionStringEMP = ConfigurationManager.ConnectionStrings["EMP"].ConnectionString;

        public List<Oferente> ObtenerOferentes(string identificacion = null)
        {

            using (var db = ConexionOFE.ObtenerConexion())
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

            using (var db = ConexionOFE.ObtenerConexion())
            {

                var oferentes = new List<Oferente>();
                using (var conn = ConexionOFE.ObtenerConexion())
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
        /// Gestiona un oferente.
        /// </summary>
        /// <param name="accion"> Acción a realizar: 0 = insertar, 1 = Modificar 2 = Eliminar </param>
        /// <param name="oferente">Objeto con la información del oferente. Si va a eliminar solo importa la identificacion</param>
        /// <returns>0 = fallo, 1 = éxito, 2 = ya asignado, 3 = ya existe</returns>
        public int GestionarOferente(int accion, Oferente oferente)
        {
            using (var db = ConexionOFE.ObtenerConexion())
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

            using (var conn = ConexionOFE.ObtenerConexion())
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
    }
}
