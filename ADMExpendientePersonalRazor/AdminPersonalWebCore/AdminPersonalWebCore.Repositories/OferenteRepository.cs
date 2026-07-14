using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Repository
{
    public class OferenteRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public OferenteRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Oferente>> ObtenerOferentesAsync(string identificacion = null)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var parametros = new { p_identificacion = identificacion };

                var lookup = new Dictionary<string, Oferente>();

                var oferentes = await connection.QueryAsync<Oferente, string, string, int?, Oferente>(
                    "sp_obtener_oferentes",
                    (oferente, email, telefono, concurso) =>
                    {
                        if (!lookup.TryGetValue(oferente.Identificacion, out var entry))
                        {
                            entry = oferente;
                            entry.Email = new List<string>();
                            entry.Telefono = new List<string>();
                            entry.CodigoConcurso = new List<string>();
                            lookup.Add(entry.Identificacion, entry);
                        }

                        if (!string.IsNullOrEmpty(email) && !entry.Email.Contains(email))
                            entry.Email.Add(email);

                        if (!string.IsNullOrEmpty(telefono) && !entry.Telefono.Contains(telefono))
                            entry.Telefono.Add(telefono);

                        if (concurso.HasValue && !entry.CodigoConcurso.Contains(concurso.Value.ToString()))
                            entry.CodigoConcurso.Add(concurso.Value.ToString());

                        return entry;
                    },
                    parametros,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Email,Telefono,CodigoConcurso"
                );

                return lookup.Values.ToList();
            }
        }

        /// <summary>
        ///     Obtiene solo el nombre y identificacion de los oferentes
        /// </summary>
        /// <returns>Lista de oferentes con solo nombre e identificacion</returns>
        /// 
        public async Task<IEnumerable<Oferente>> ObtenerNombreOferentesAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var oferentes = await connection.QueryAsync<Oferente>(
                    "sp_obtener_nombre_oferentes",
                    commandType: CommandType.StoredProcedure
                );

                return oferentes;
            }
        }

        /// <summary>
        /// Gestiona un oferente.
        /// </summary>
        /// <param name="accion"> Acción a realizar: 0 = insertar, 1 = Modificar 2 = Eliminar </param>
        /// <param name="oferente">Objeto con la información del oferente. Si va a eliminar solo importa la identificacion</param>
        /// <returns>0 = fallo, 1 = éxito, 2 = ya asignado, 3 = ya existe</returns>
        /// 
        public async Task<int> GestionarOferenteAsync(int accion, Oferente oferente)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var parametros = new
                {
                    accion = accion,
                    p_identificacion = oferente.Identificacion,
                    p_tipo_identificacion = oferente.TipoIdentificacion,
                    p_nombre_completo = oferente.NombreCompleto,
                    p_fecha_nacimiento = oferente.FechaNacimiento,
                    p_contratado = oferente.Contratado,
                    p_emails = oferente.EmailDisplay,
                    p_telefonos = oferente.TelefonoDisplay,
                    p_concursos = oferente.ConcursoDisplay
                };

                var resultado = await connection.QuerySingleAsync<int>(
                    "sp_GestionarOferente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return resultado;
            }
        }

        public async Task<IEnumerable<ConcursoTemporal>> ObtenerConcursosAsync(string identificacion = null)
        {
            using (var connection = _dbConnectionFactory.CreateConnection("OFE"))
            {
                var parametros = new { p_identificacion = identificacion };

                var concursos = await connection.QueryAsync<ConcursoTemporal>(
                    "sp_ObtenerConcursos",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return concursos;
            }
        }
    }
}
