using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class AccionPersonalService
    {
        private readonly AccionPersonalRepository _repository;

        public AccionPersonalService(AccionPersonalRepository repository)
        {
            _repository = repository;
        }

        public List<AccionPersonal> ObtenerTodos()
        {
            return _repository.ObtenerTodos();
        }

        public AccionPersonal ObtenerPorId(int id)
        {
            return _repository.ObtenerPorId(id);
        }

        public List<Empleado> ObtenerEmpleados()
        {
            return _repository.ObtenerEmpleados();
        }

        public void Insertar(AccionPersonal accion)
        {
            ValidarAccionPersonal(accion);
            _repository.Insertar(accion);
        }

        public void Actualizar(AccionPersonal accion)
        {
            ValidarAccionPersonal(accion);
            _repository.Actualizar(accion);
        }

        public void Eliminar(int id)
        {
            _repository.Eliminar(id);
        }

        private void ValidarAccionPersonal(AccionPersonal accion)
        {
            if (accion.CodigoAccion <= 0)
                throw new Exception("El código de acción es requerido.");

            if (accion.Fecha == default)
                throw new Exception("La fecha es requerida.");

            if (string.IsNullOrWhiteSpace(accion.Descripcion))
                throw new Exception("La descripción es requerida.");

            if (accion.Descripcion.Length > 500)
                throw new Exception("La descripción no puede superar los 500 caracteres.");

            if (accion.EmpleadoId <= 0)
                throw new Exception("Debe seleccionar un empleado.");

            if (accion.JefaturaId <= 0)
                throw new Exception("Debe seleccionar una jefatura.");
        }
    }
}