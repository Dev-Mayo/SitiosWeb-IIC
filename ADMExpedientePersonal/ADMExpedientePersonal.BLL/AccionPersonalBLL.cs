using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;

namespace ADMExpedientePersonal.BLL
{
    public class AccionPersonalBLL
    {
        private AccionPersonalDAL accionDAL = new AccionPersonalDAL();

        public List<AccionPersonal> ObtenerTodos()
        {
            return accionDAL.ObtenerTodos();
        }

        public AccionPersonal ObtenerPorId(int accionId)
        {
            return accionDAL.ObtenerPorId(accionId);
        }

        public void Insertar(AccionPersonal accion)
        {
            ValidarAccion(accion);
            accionDAL.Insertar(accion);
        }

        public void Actualizar(AccionPersonal accion)
        {
            ValidarAccion(accion);
            accionDAL.Actualizar(accion);
        }

        public void Eliminar(int accionId)
        {
            accionDAL.Eliminar(accionId);
        }

        public List<Empleado> ObtenerEmpleados()
        {
            return accionDAL.ObtenerEmpleados();
        }

        private void ValidarAccion(AccionPersonal accion)
        {
            if (accion.codigo_accion <= 0)
                throw new Exception("Debe indicar un código de acción válido.");

            if (string.IsNullOrWhiteSpace(accion.descripcion))
                throw new Exception("La descripción es requerida.");

            if (accion.descripcion.Length > 500)
                throw new Exception("La descripción no puede superar los 500 caracteres.");

            if (accion.empleado_id <= 0)
                throw new Exception("Debe seleccionar un empleado.");

            if (accion.jefatura_id <= 0)
                throw new Exception("Debe seleccionar una jefatura.");
        }
    }
}   

