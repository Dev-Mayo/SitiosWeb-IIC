using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;

namespace ADMExpedientePersonal.BLL
{
    public class ConcursoBLL
    {
        private readonly ConcursoDAL dal = new ConcursoDAL();

        public List<Concurso> Listar()
        {
            return dal.Listar();
        }

        public Concurso Obtener(string codigo)
        {
            return dal.Obtener(codigo);
        }

        public void InsertarConcurso(Concurso item)
        {
            Validar(item);

            if (dal.Obtener(item.codigo_concurso) != null)
                throw new Exception("Ya existe un concurso con ese código.");

            item.estado = "Vigente";
            dal.Insertar(item);
        }

        public void ActualizarConcurso(Concurso item)
        {
            Validar(item);

            if (dal.Obtener(item.codigo_concurso) == null)
                throw new Exception("El concurso no existe.");

            dal.Actualizar(item);
        }

        public void EliminarConcurso(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new Exception("Debe seleccionar un concurso.");

            dal.Eliminar(codigo);
        }

        public void CambiarEstadoConcurso(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new Exception("Debe seleccionar un concurso.");

            dal.CambiarEstado(codigo);
        }

        private void Validar(Concurso item)
        {
            if (string.IsNullOrWhiteSpace(item.codigo_concurso))
                throw new Exception("Debe ingresar el código del concurso.");

            if (string.IsNullOrWhiteSpace(item.nombre))
                throw new Exception("Debe ingresar el nombre del concurso.");

            if (item.fecha_fin < item.fecha_inicio)
                throw new Exception("La fecha fin debe ser mayor o igual a la fecha inicio.");
        }
    }
}