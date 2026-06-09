using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.DAL.ModuloOferenteDAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.BLL.ModuloOferenteBLL
{
    public class EntrevistaBLL
    {
        private EntrevistaDAL entrevistaDAL = new EntrevistaDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        private int resultado; //para guardar los resultados de las operaciones y no declarar a cada momento

        public void GenericoCrearBitacora(string Usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null) // accion: 0=insertar, 1=actualizar, 2=eliminar, 3=consultar, 4=error
        { //Para evitar repetir codigo al crear bitacoras, y ya valida para solo registrar resultados exitosos
            bitacoraBLL.GenericoCrearBitacora(Usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles); // Llamar al método genérico
        }

        public List<EmpleadoTemporal> ObtenerNombreEmpleados() //******************
        {
            return entrevistaDAL.ObtenerNombreEmpleados();
        }

        // Obtener entrevistas
        public List<Entrevista> ObtenerEntrevistas(string Usuario, int? EntrevistaId = null)
        {
            List<Entrevista> entrevistas = entrevistaDAL.ObtenerEntrevistas(EntrevistaId);
            GenericoCrearBitacora(Usuario, 3, 1, detalles: "Entrevistas");
            return entrevistas;
        }

        public Entrevista ObtenerEntrevista(string Usuario, int EntrevistaId) //Recuperar una entrevista específica, se registra la consulta en la bitácora
        {
            Entrevista entrevista = entrevistaDAL.ObtenerEntrevistas(EntrevistaId).FirstOrDefault();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: $"Entrevista {EntrevistaId}");
            return entrevista;
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
            if (string.IsNullOrEmpty(entrevista.OferenteIdentificacion) || entrevista.EmpleadoId <= 0 || entrevista.FechaEntrevista == default)
            {
                return 2; // Datos inválidos
            }
            if (entrevista.FechaEntrevista < DateTime.Now)
            {
                return 3; // Fecha inválida
            }
            resultado = entrevistaDAL.CrearEntrevista(entrevista);
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
            var anterior = entrevistaDAL.ObtenerEntrevistas(entrevista.EntrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            resultado = entrevistaDAL.ModificarEntrevista(entrevista);
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
            var anterior = entrevistaDAL.ObtenerEntrevistas(entrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            resultado = entrevistaDAL.EliminarEntrevista(entrevistaId);
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
            var anterior = entrevistaDAL.ObtenerEntrevistas(EntrevistaId).FirstOrDefault(); // Obtener datos anteriores para la bitácora
            var nuevoEstado = anterior;
            nuevoEstado.Estado = "Realizada";
            resultado = entrevistaDAL.CambiarEstadoEntrevista(EntrevistaId);
            GenericoCrearBitacora(usuario, 1, resultado, nuevoEstado, anterior);
            return resultado;
        }

    }
}
