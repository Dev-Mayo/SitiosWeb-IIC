using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.BLL
{
    public class BitacoraBLL
    {
        BitacoraDAL bitacoraDAL = new BitacoraDAL();

        public void RegistrarBitacora(Bitacora bitacora) //Recibe el objeto entities con todos los datos (aca el json ya debe ir formateado)
        {
            if (bitacora == null) throw new ArgumentNullException(nameof(bitacora));
            if (string.IsNullOrWhiteSpace(bitacora.Usuario)) throw new ArgumentException("Usuario requerido para bitacora.");
            
            bitacoraDAL.RegistrarBitacora(bitacora); //persistir los datos
        }

        // Retorna true si es JSON válido
        public static bool JsonValido(string json) //Por si hacen un json manual lo pueden validar rapido, auqnue es mejor usar los metodos para crearlos
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public List<BitacoraDisplay> ObtenerBitacoras(string usuarioActual,
            string filtroUsuario = null, string filtroDescripcion = null, string orden = "fecha_desc")
        {
            var bitacoras = bitacoraDAL.ObtenerBitacoras(filtroUsuario, filtroDescripcion, orden);

            RegistrarBitacora(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = CrearJsonConsulta("Bitácoras")
            });

            return bitacoras;
        }

        /// <summary>
        /// Metodo generico para llamadas mas directas y crear las bitacoras
        /// </summary>
        /// <param name="Usuario"> Quien hace la accion</param>
        /// <param name="accion">Tipo de accion: 0=insertar, 1=actualizar, 2=eliminar, 3=consultar, 4=error</param>
        /// <param name="resultado"> Solo se registrar si se pasa un resultado de 1 (exito)</param>
        /// <param name="objetoOriginal">Objeto original involucrado en la accion (nuevo registro, registro eliminado)</param>
        /// <param name="objetoAnterior">Objeto anterior solo en caso de actualizacion</param>
        /// <param name="detalles">Detalles adicionales de la accion (Para consultar se pasa ejemplo "Oferente" y si es error solo el error)</param>
        public void GenericoCrearBitacora(string Usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null) // accion: 0=insertar, 1=actualizar, 2=eliminar, 3=consultar, 4=error
        { //Para evitar repetir codigo al crear bitacoras, y ya valida para solo registrar resultados exitosos
            Bitacora bitacora = new Bitacora();
            bitacora.Usuario = Usuario;

            if (accion == 0)
            {
                bitacora.Accion = AccionBitacora.CREATE;
                bitacora.DescripcionJson = CrearJsonNuevo(objetoOriginal);
            }
            else if (accion == 1)
            {
                bitacora.Accion = AccionBitacora.UPDATE;
                bitacora.DescripcionJson = CrearJsonActualizacion(objetoAnterior, objetoOriginal);
            }
            else if (accion == 2)
            {
                bitacora.Accion = AccionBitacora.DELETE;
                bitacora.DescripcionJson = CrearJsonEliminacion(objetoOriginal);
            }
            else if (accion == 3)
            {
                bitacora.Accion = AccionBitacora.READ;
                bitacora.DescripcionJson = CrearJsonConsulta(detalles);
            }
            else // ERROR u otras acciones
            {
                bitacora.Accion = AccionBitacora.ERROR;
                bitacora.DescripcionJson = CrearJsonError(detalles);
            }
            if (resultado == 1) //si cumplio lo esperado / Se puede comentar para no saturar de bittacoras de prueba
                RegistrarBitacora(bitacora);
        }

        #region Métodos para crear JSON de bitácora
        /// <summary>
        /// Metodos dedicados para crear los json y seguir mismo formato en la bitacora
        /// Unos metodos reciben objetos, otros solo mensajes, dependiendo del tipo de operacion (CRUD) que se registre en la bitacora
        /// Reciben cualquier tipo de objeto y el metodo lo convierte a JSON con estructura definida
        /// Todos retornan un string con el JSON listo para guardar en el objeto bitacora y enviarlo a RegistrarBitacora
        /// </summary>

        //Para ERROR, solo el mensaje de error
        public string CrearJsonError(string mensajeError)
        {
            var json = new JObject
            {
                ["Error"] = $"Error: {mensajeError}"
            };
            return json.ToString(Formatting.None);
        }

        // Para CREATE, solo el nuevo registro
        public string CrearJsonNuevo(object nuevoRegistro)
        {
            var json = new JObject
            {
                ["NuevoRegistro"] = JObject.FromObject(nuevoRegistro)
            };
            return json.ToString(Formatting.None);
        }

        // Para UPDATE, registro anterior y actual
        public string CrearJsonActualizacion(object registroAnterior, object registroActual)
        {
            var json = new JObject
            {
                ["RegistroAnterior"] = JObject.FromObject(registroAnterior),
                ["RegistroActual"] = JObject.FromObject(registroActual)
            };
            return json.ToString(Formatting.None);
        }

        // Para DELETE, solo el eliminado
        public string CrearJsonEliminacion(object registroEliminado)
        {
            var json = new JObject
            {
                ["RegistroEliminado"] = JObject.FromObject(registroEliminado)
            };
            return json.ToString(Formatting.None);
        }

        // Para READ, mensaje simple
        public string CrearJsonConsulta(string elemento)
        {
            var json = new JObject
            {
                ["Consulta"] = $"El usuario consulta {elemento}"
            };
            return json.ToString(Formatting.None);
        }

        #endregion Metodos para crear JSON de bitácora
    }
}
        
