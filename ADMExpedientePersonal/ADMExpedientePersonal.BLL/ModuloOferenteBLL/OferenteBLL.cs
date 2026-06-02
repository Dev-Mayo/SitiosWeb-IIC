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
        { //Para evitar repetir codigo al crear bitacoras, y ya valida para solo registrar resultados exitosos (1)

            bitacoraBLL.GenericoCrearBitacora(Usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles); // Llamar al método genérico
        }

        public List<ConcursoTemporal> ObtenerConcursos(string Usuario, string identificacion = null) //Recuperar concursos temporales para mostrar, se registra la consulta en la bitácora
        {
            List<ConcursoTemporal> concursos = oferentesDAL.ObtenerConcursos(identificacion);
            //GenericoCrearBitacora(Usuario, 3, 1, detalles: "Concursos Temporales");
            return concursos;
        }

        public List<Oferente> ObtenerOferentes(string Usuario) //Recuperar todos los ofertentes para mostrar, se registra la consulta en la bitácora
        {
            List<Oferente> oferentes = oferentesDAL.ObtenerOferentes();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: "Oferentes");
            return oferentes;
        }

        public List<Oferente> ObtenerNombreOferentes()
        {
            return oferentesDAL.ObtenerNombreOferentes();
        }

        public Oferente ObtenerOferente(string Usuario, string identificacion) //Recuperar un oferente específico, se registra la consulta en la bitácora
        {
            Oferente oferente = oferentesDAL.ObtenerOferentes(identificacion).FirstOrDefault();
            GenericoCrearBitacora(Usuario, 3, 1, detalles: $"Oferente {identificacion}");
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

    }
}
