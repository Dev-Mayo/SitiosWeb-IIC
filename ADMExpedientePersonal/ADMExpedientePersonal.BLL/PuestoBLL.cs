using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;

namespace ADMExpedientePersonal.BLL
{
    public class PuestoBLL
    {
        private readonly PuestoDAL dal = new PuestoDAL();

        public List<Puesto> Listar()
        {
            return dal.Listar();
        }

        public Puesto Obtener(int puestoId)
        {
            return dal.Obtener(puestoId);
        }

        public void InsertarPuesto(Puesto item)
        {
            Validar(item);
            dal.Insertar(item);
        }

        public void ActualizarPuesto(Puesto item)
        {
            if (item.puesto_id <= 0)
                throw new Exception("Debe seleccionar un puesto válido.");

            Validar(item);

            if (dal.Obtener(item.puesto_id) == null)
                throw new Exception("El puesto no existe.");

            dal.Actualizar(item);
        }

        public void EliminarPuesto(int puestoId)
        {
            if (puestoId <= 0)
                throw new Exception("Debe seleccionar un puesto válido.");

            dal.Eliminar(puestoId);
        }

        private void Validar(Puesto item)
        {
            if (string.IsNullOrWhiteSpace(item.nombre))
                throw new Exception("Debe ingresar el nombre del puesto.");

            if (item.nombre.Length > 100)
                throw new Exception("El nombre no puede superar los 100 caracteres.");

            if (item.salario <= 0)
                throw new Exception("El salario debe ser mayor a cero.");

            if (item.jefe_puesto_id.HasValue && item.puesto_id == item.jefe_puesto_id.Value)
                throw new Exception("Un puesto no puede ser jefe de sí mismo.");
        }
    }
}