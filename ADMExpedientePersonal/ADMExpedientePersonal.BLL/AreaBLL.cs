using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ADMExpedientePersonal.BLL
{
    public class AreaBLL
    {
        private AreaDAL areaDAL = new AreaDAL();

        public List<Area> ObtenerTodos()
        {
            return areaDAL.ObtenerTodos();
        }

        public Area ObtenerPorId(int id)
        {
            return areaDAL.ObtenerPorId(id);
        }

        public void Insertar(Area area)
        {
            ValidarArea(area);
            areaDAL.Insertar(area);
        }

        public void Actualizar(Area area)
        {
            ValidarArea(area);
            areaDAL.Actualizar(area);
        }

        public void Eliminar(int id)
        {
            areaDAL.Eliminar(id);
        }

        private void ValidarArea(Area area)
        {
            if (area == null)
                throw new Exception("El área no puede ser nula.");

            if (area.codigo_area <= 0)
                throw new Exception("El código del área es obligatorio y debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(area.nombre))
                throw new Exception("El nombre del área es obligatorio.");

            if (area.nombre.Length > 100)
                throw new Exception("El nombre del área no puede superar los 100 caracteres.");

            if (!Regex.IsMatch(area.nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new Exception("El nombre del área solo puede contener letras y espacios.");

            if (area.jefatura <= 0)
                throw new Exception("Debe seleccionar una jefatura.");
        }

        public List<Empleado> ObtenerJefaturas()
        {
            return areaDAL.ObtenerJefaturas();
        }
    }
}