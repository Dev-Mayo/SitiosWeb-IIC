using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ADMExpedientePersonal.BLL
{
    public class CompaniaBLL
    {
        private readonly CompaniaDAL dal = new CompaniaDAL();

        public List<Compania> Listar()
        {
            return dal.Listar();
        }

        public Compania Obtener(string codigo)
        {
            return dal.Obtener(codigo);
        }

        public void InsertarCompania(Compania item)
        {
            ValidarCompania(item);

            if (dal.Obtener(item.codigo_compania) != null)
                throw new Exception("Ya existe una compañía con ese código.");

            dal.Insertar(item);
        }

        public void ActualizarCompania(Compania item)
        {
            ValidarCompania(item);

            if (dal.Obtener(item.codigo_compania) == null)
                throw new Exception("La compañía no existe.");

            dal.Actualizar(item);
        }

        public void EliminarCompania(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new Exception("Debe seleccionar una compañía.");

            dal.Eliminar(codigo);
        }

        private void ValidarCompania(Compania item)
        {
            if (string.IsNullOrWhiteSpace(item.codigo_compania))
                throw new Exception("Debe ingresar el código de la compañía.");

            if (string.IsNullOrWhiteSpace(item.nombre))
                throw new Exception("Debe ingresar el nombre de la compañía.");

            if (item.nombre.Length > 150)
                throw new Exception("El nombre no puede superar los 150 caracteres.");
        }
    }
}