using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System.Collections.Generic;

namespace ADMExpedientePersonal.BLL
{
    public class InstEducativaBLL
    {
        private InstEducativaDAL dal = new InstEducativaDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        public List<InstEducativa> ObtenerInstituciones(string usuarioActual)
        {
            var lista = dal.ObtenerInstituciones();
            bitacoraBLL.RegistrarBitacora(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = bitacoraBLL.CrearJsonConsulta("Instituciones Educativas")
            });
            return lista;
        }

        public bool ValidarDuplicado(string codigo, string nombre, string codigoExcluir = null)
        {
            return dal.ValidarDuplicado(codigo, nombre, codigoExcluir);
        }

        public int Insertar(string codigo, string nombre, string usuarioActual)
        {
            int resultado = dal.Insertar(codigo, nombre);
            if (resultado > 0)
            {
                var nueva = dal.ObtenerPorCodigo(codigo);
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = bitacoraBLL.CrearJsonNuevo(nueva)
                });
            }
            return resultado;
        }

        public int Actualizar(string codigo, string nombre, string usuarioActual)
        {
            var anterior = dal.ObtenerPorCodigo(codigo);
            int resultado = dal.Actualizar(codigo, nombre);
            if (resultado > 0)
            {
                var nueva = dal.ObtenerPorCodigo(codigo);
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = usuarioActual,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = bitacoraBLL.CrearJsonActualizacion(anterior, nueva)
                });
            }
            return resultado;
        }

        public void Eliminar(string codigo, string usuarioActual)
        {
            var inst = dal.ObtenerPorCodigo(codigo);
            dal.Eliminar(codigo);
            bitacoraBLL.RegistrarBitacora(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = bitacoraBLL.CrearJsonEliminacion(inst)
            });
        }

        public InstEducativa ObtenerPorCodigo(string codigo)
        {
            return dal.ObtenerPorCodigo(codigo);
        }
    }
}