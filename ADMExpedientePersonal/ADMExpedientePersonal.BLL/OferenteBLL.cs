using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.BLL
{
    public class OferenteBLL
    {
        private OferentesDAL oferentesDAL = new OferentesDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        private int resultado; //para guardar los resultados de las operaciones y no declarar a cada momento

        public void GenericoCrearBitacora(string Usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null) // accion: 0=insertar, 1=actualizar, 2=eliminar, 3=consultar, 4=error
        { //Para evitar repetir codigo al crear bitacoras, y ya valida para solo registrar resultados exitosos
            Bitacora bitacora = new Bitacora();
            bitacora.Usuario = Usuario;

            if (accion == 0)
            {
                bitacora.Accion = AccionBitacora.CREATE;
                bitacora.DescripcionJson = bitacoraBLL.CrearJsonNuevo(objetoOriginal);
            }
            else if (accion == 1)
            {
                bitacora.Accion = AccionBitacora.UPDATE;
                bitacora.DescripcionJson = bitacoraBLL.CrearJsonActualizacion(objetoAnterior, objetoOriginal);
            }
            else if (accion == 2)
            {
                bitacora.Accion = AccionBitacora.DELETE;
                bitacora.DescripcionJson = bitacoraBLL.CrearJsonEliminacion(objetoOriginal);
            }
            else if (accion == 3)
            {
                bitacora.Accion = AccionBitacora.READ;
                bitacora.DescripcionJson = bitacoraBLL.CrearJsonConsulta(detalles);
            }
            else // ERROR u otras acciones
            {
                bitacora.Accion = AccionBitacora.ERROR;
                bitacora.DescripcionJson = bitacoraBLL.CrearJsonError(detalles);
            }
            //if (resultado == 1) //si cumplio lo esperado
              //  bitacoraBLL.RegistrarBitacora(bitacora);
        }

        public List<Oferente> ObtenerOferentes(string Usuario) //Recuperar todos los ofertentes para mostrar, se registra la consulta en la bitácora
        {
            List<Oferente> oferentes = oferentesDAL.ObtenerOferentes();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: "Entrevistas");
            return oferentes;
        }
        public Oferente ObtenerOferente(string Usuario, string identificacion) //Recuperar un oferente específico, se registra la consulta en la bitácora
        {
            Oferente oferente = oferentesDAL.ObtenerOferentes(identificacion).FirstOrDefault();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: $"Oferente con ID {identificacion}");
            return oferente;
        }

        public int InsertarOferente(Oferente oferente, string Usuario) //Registrar un nuevo oferente, se registra la creación en la bitácora
        {
            if (oferente.ValidarDatos() == false) {
                return 4; // Datos inválidos
            }
            resultado = oferentesDAL.GestionarOferente(0, oferente);
            GenericoCrearBitacora(Usuario, 0, resultado, objetoOriginal: oferente); // Registrar en bitacora solo si es exitosa la creación
            return resultado;
        }

        public int ActualizarOferente(Oferente oferente, string Usuario) //Actualizar un oferente existente, se registra la actualización en la bitácora
        {
            if (oferente.ValidarDatos() == false) {
                return 4; // Datos inválidos
            }
            Oferente oferenteAnterior = oferentesDAL.ObtenerOferentes(identificacion: oferente.identificacion).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            resultado = oferentesDAL.GestionarOferente(1, oferente);
            GenericoCrearBitacora(Usuario, 1, resultado, objetoOriginal: oferente, objetoAnterior: oferenteAnterior); // Registrar en bitacora solo si es exitosa la actualización
            return resultado;
        }

        public int EliminarOferente(Oferente oferente, string Usuario) //Eliminar un oferente, se registra la eliminación en la bitácora
        {
            resultado = oferentesDAL.GestionarOferente(2, oferente);
            GenericoCrearBitacora(Usuario, 2, resultado, objetoOriginal: oferente); // Registrar en bitacora solo si es exitosa la eliminación
            return resultado;
        }

        public List<ConcursoTemporal> ObtenerConcursos(string Usuario, string identificacion = null) //Recuperar concursos temporales para mostrar, se registra la consulta en la bitácora
        {
            List<ConcursoTemporal> concursos = oferentesDAL.ObtenerConcursos(identificacion);
            //GenericoCrearBitacora(Usuario, 3, 1, detalles: "Concursos Temporales");
            return concursos;
        }

        // Obtener entrevistas
        public List<Entrevista> ObtenerEntrevistas(string Usuario, int? EntrevistaId = null)
        {
            List<Entrevista> entrevistas = oferentesDAL.ObtenerEntrevistas(EntrevistaId);
            GenericoCrearBitacora(Usuario, 3, 1, detalles: "Entrevistas");
            return entrevistas;
        }

        public Entrevista ObtenerEntrevista(string Usuario, int EntrevistaId) //Recuperar una entrevista específica, se registra la consulta en la bitácora
        {
            Entrevista entrevista = oferentesDAL.ObtenerEntrevistas(EntrevistaId).FirstOrDefault();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: $"Entrevista {EntrevistaId}");
            return entrevista;
        }

        public List<Oferente> ObtenerNombreOferentes()
        {
            return oferentesDAL.ObtenerNombreOferentes();
        }
        public List<EmpleadoTemporal> ObtenerNombreEmpleados()
        {
            return oferentesDAL.ObtenerNombreEmpleados();
        }

        // Crear entrevista
        /// <summary>
        /// Crea una nueva entrevista para un oferente. El estado inicial siempre será "Pendiente"
        /// </summary>
        /// <param name="entrevista"> Necesita: OferenteIdentificacion, EmpleadoId, FechaEntrevista </param>
        /// <param name="usuario">Nombre del usuario que realiza la acción</param>
        /// <returns> 1:Exito </returns>
        public int InsertarEntrevista(Entrevista entrevista, string usuario)   
        {
            if (string.IsNullOrEmpty(entrevista.OferenteIdentificacion) || entrevista.EmpleadoId <= 0 || entrevista.FechaEntrevista == default) { 
                return 2; // Datos inválidos
            }
            if (entrevista.FechaEntrevista < DateTime.Now) {
                return 3; // Fecha inválida
            }
            resultado = oferentesDAL.CrearEntrevista(entrevista);
            GenericoCrearBitacora(usuario, 0, resultado, objetoOriginal: entrevista);
            return resultado;
        }

        // Modificar entrevista
        /// <summary>
        /// Modifica la fecha o el empleado de una entrevista existente. No se puede modificar el oferente ni el estado desde aquí.
        /// </summary>
        /// <param name="entrevista"> Necesita: EntrevistaId, EmpleadoId, FechaEntrevista </param>
        /// <param name="usuario">Nombre del usuario que realiza la acción</param>
        /// <returns> 1:Exito, 0:Fallos </returns>
        public int ModificarEntrevista(Entrevista entrevista, string usuario)
        {
            if (string.IsNullOrEmpty(entrevista.OferenteIdentificacion) || entrevista.EmpleadoId <= 0 || entrevista.FechaEntrevista == default)
            {
                return 2; // Datos inválidos
            }
            if (entrevista.FechaEntrevista < DateTime.Now)
            {
                return 3; // Fecha inválida
            }
            var anterior = oferentesDAL.ObtenerEntrevistas(entrevista.EntrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            resultado = oferentesDAL.ModificarEntrevista(entrevista);
            GenericoCrearBitacora(usuario, 1, resultado, entrevista, anterior);
            return resultado;
        }

        // Eliminar entrevista
        /// <summary>
        /// Elimina una entrevista existente.
        /// </summary>
        /// <param name="entrevistaId">ID de la entrevista a eliminar</param>
        /// <param name="usuario">Nombre del usuario que realiza la acción</param>
        /// <returns> 1:Exito, 0:Fallo </returns>
        public int EliminarEntrevista(int entrevistaId, string usuario)
        {
            var anterior = oferentesDAL.ObtenerEntrevistas(entrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            resultado = oferentesDAL.EliminarEntrevista(entrevistaId);
            GenericoCrearBitacora(usuario, 2, resultado, objetoOriginal: anterior);
            return resultado;
        }

        // Cambiar estado
        /// <summary>
        /// Cambia el estado de una entrevista existente. El nuevo estado debe ser "Pendiente", "Realizada" o "Eliminada".
        /// </summary>
        /// <param name="entrevista">Objeto entrevista que contiene el ID de entrevista y el nuevo estado</param>
        /// <param name="usuario">Nombre del usuario que realiza la acción</param>
        /// <returns> 1:Exito, 0:Fallo </returns>
        public int CambiarEstadoEntrevista(int EntrevistaId, string usuario)
        {
            var anterior = oferentesDAL.ObtenerEntrevistas(EntrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            var nuevoEstado = anterior;
            nuevoEstado.Estado = "Realizada";
            resultado = oferentesDAL.CambiarEstadoEntrevista(EntrevistaId);
            GenericoCrearBitacora(usuario, 1, resultado, nuevoEstado, anterior);
            return resultado;
        }

    }
}
