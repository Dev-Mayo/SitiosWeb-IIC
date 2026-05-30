using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ADMExpedientePersonal.BLL
{
    public class RequisitoPuestoBLL
    {
        private RequisitoPuestoDAL requisitoDAL = new RequisitoPuestoDAL();

        public List<RequisitoPuesto> ObtenerTodos()
        {
            return requisitoDAL.ObtenerTodos();
        }

        public RequisitoPuesto ObtenerPorId(int id)
        {
            return requisitoDAL.ObtenerPorId(id);
        }

        public void Insertar(RequisitoPuesto requisito)
        {
            ValidarRequisito(requisito);
            requisitoDAL.Insertar(requisito);
        }

        public void Actualizar(RequisitoPuesto requisito)
        {
            ValidarRequisito(requisito);
            requisitoDAL.Actualizar(requisito);
        }

        public void Eliminar(int id)
        {
            requisitoDAL.Eliminar(id);
        }

        private void ValidarRequisito(RequisitoPuesto requisito)
        {
            if (requisito == null)
                throw new Exception("El requisito no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(requisito.nombre))
                throw new Exception("El nombre del requisito es obligatorio.");

            if (requisito.nombre.Length > 100)
                throw new Exception("El nombre del requisito no puede superar los 100 caracteres.");

            if (!Regex.IsMatch(requisito.nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new Exception("El nombre del requisito solo puede contener letras y espacios.");
        }
    }
}