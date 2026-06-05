using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml;

namespace AdminPersonalWebCore.Services
{
    public class BitacoraService
    {
        private readonly BitacoraRepository _repo;
        public BitacoraService(BitacoraRepository repo) => _repo = repo;

        public void Registrar(Bitacora bitacora) => _repo.Registrar(bitacora);

        public List<BitacoraDisplay> ObtenerBitacoras(string usuarioActual,
            string filtroUsuario = null, string filtroDesc = null, string orden = "fecha_desc")
        {
            var lista = _repo.ObtenerBitacoras(filtroUsuario, filtroDesc, orden);
            Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = CrearJsonConsulta("Bitácoras")
            });
            return lista;
        }

        public string CrearJsonError(string msg) =>
            new JObject { ["Error"] = $"Error: {msg}" }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonNuevo(object obj) =>
            new JObject { ["NuevoRegistro"] = JObject.FromObject(obj) }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonActualizacion(object anterior, object actual) =>
            new JObject
            {
                ["RegistroAnterior"] = JObject.FromObject(anterior),
                ["RegistroActual"] = JObject.FromObject(actual)
            }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonEliminacion(object obj) =>
            new JObject { ["RegistroEliminado"] = JObject.FromObject(obj) }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonConsulta(string elemento) =>
            new JObject { ["Consulta"] = $"El usuario consulta {elemento}" }.ToString(Newtonsoft.Json.Formatting.None);


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
           // if (resultado == 1) //si cumplio lo esperado / Se puede comentar para no saturar de bittacoras de prueba
             //   Registrar(bitacora);
        }
    }
}