using ADMExpedientePersonal.DAL.ModuloOferenteDAL;
using ADMExpedientePersonal.Entities;
using ADMExpedientePersonal.Entities.ModuloOferenteEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ADMExpedientePersonal.BLL.ModuloOferenteBLL
{
    public class PrepAcademicaBLL
    {
        private PrepAcademicaDAL prepAcademicaDAL = new PrepAcademicaDAL();
        private InstEducativaBLL instEducativaBLL = new InstEducativaBLL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();
        private int resultado;

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
        { //Para evitar repetir codigo al crear bitacoras, y ya valida para solo registrar resultados exitosos (1)

            bitacoraBLL.GenericoCrearBitacora(Usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles); // Llamar al método genérico
        }

        public List<InstEducativa> ObtenerInstituciones(string usuario)
        {
            return instEducativaBLL.ObtenerInstituciones(usuario);
        }

        public List<PreparacionAcad> ObtenerPreparacionAcad(string identificacion, string usuario)
        {
            var lista = new List<PreparacionAcad>();
            GenericoCrearBitacora(usuario, 3, 1, detalles: "PrepAcademica");
            return prepAcademicaDAL.ObtenerPreparacionAcad(identificacion);
        }

        public PreparacionAcad ObtenerPreparacionAcadPorId(int id, string usuario)
        {
            GenericoCrearBitacora(usuario, 3, 1, detalles: "PrepAcademica");
            return prepAcademicaDAL.ObtenerPreparacionAcadPorId(id);
        }


        /// <summary>
        /// Crea una nueva preparación académica para un oferente, validando campos obligatorios, formato de fechas y longitud del título.
        /// </summary>
        /// <param name="prep">Objeto tipo PreparacionAcad que contiene la información de la preparación académica.</param>
        /// <param name="usuario">Nombre del usuario que realiza la acción.</param>
        /// <returns>Resultado de la operación: 0 = falló, 1 = éxito, 2 = error en campos obligatorios o incorrectos, 3 = error en fechas, 4 = error en longitud del título.</returns>
        public int CrearPreparacionAcad(PreparacionAcad prep, string usuario)
        {
            if (string.IsNullOrEmpty(prep.CodigoInstitucion) || string.IsNullOrEmpty(prep.OferenteId)
                || string.IsNullOrEmpty(prep.Titulo) || prep.FechaInicio == default || prep.FechaFin == default)
                return 2; // Error: campos obligatorios o incorrectos

            if (prep.FechaFin < prep.FechaInicio)
                return 3; // Error: fecha fin anterior a fecha inicio

            if (prep.Titulo.Length > 100 || !Regex.IsMatch(prep.Titulo, @"^[a-zA-Z\s]+$"))
                return 4; // Error: título menor a 100 caracteres y solo caracteres
            resultado = prepAcademicaDAL.CrearPreparacionAcad(prep);
            GenericoCrearBitacora(usuario, 0, resultado, objetoOriginal: prep);
            return resultado;
        }

        /// <summary>
        /// Modifica una preparación académica existente para un oferente, validando campos obligatorios, formato de fechas y longitud del título.
        /// </summary>
        /// <param name="prep">Objeto tipo PreparacionAcad que contiene la información de la preparación académica.</param>
        /// <param name="usuario">Nombre del usuario que realiza la acción.</param>
        /// <returns>Resultado de la operación: 0 = falló, 1 = éxito, 2 = error en campos obligatorios o incorrectos, 3 = error en fechas, 4 = error en longitud del título.</returns>
        public int ModificarPreparacionAcad(PreparacionAcad prep, string usuario)
        {
            var anterior = ObtenerPreparacionAcadPorId(prep.Id, usuario);
            if (string.IsNullOrEmpty(prep.CodigoInstitucion) || prep.Id <= 0
                || string.IsNullOrEmpty(prep.Titulo) || prep.FechaInicio == default || prep.FechaFin == default)
                return 2; // Error: campos obligatorios o incorrectos

            if (prep.FechaFin < prep.FechaInicio)
                return 3; // Error: fecha fin anterior a fecha inicio

            if (prep.Titulo.Length > 100)
                return 4; // Error: título menor a 100 caracteres
            resultado = prepAcademicaDAL.ModificarPreparacionAcad(prep);
            GenericoCrearBitacora(usuario, 1, resultado, prep, anterior);
            return resultado;
        }

        /// <summary>
        ///     Elimina una preparación académica existente, validando que no esté asociada a un oferente y registrando la acción en la bitácora.
        /// </summary>
        /// <param name="id">ID de la preparación académica a eliminar.</param>
        /// <param name="usuario">Nombre del usuario que realiza la acción.</param>
        /// <returns>Resultado de la operación: 0 = falló, 1 = eliminado, 2 = no permitido (asociado a oferente)</returns>
        public int EliminarPreparacionAcad(int id, string usuario)
        {
            var anterior = ObtenerPreparacionAcadPorId(id, usuario);
            resultado = prepAcademicaDAL.EliminarPreparacionAcad(id);
            GenericoCrearBitacora(usuario, accion: 2, resultado, anterior);
            return resultado;
        }
    }
}
