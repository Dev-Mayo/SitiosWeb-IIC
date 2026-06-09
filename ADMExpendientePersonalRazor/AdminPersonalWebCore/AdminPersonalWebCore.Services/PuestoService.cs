using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class PuestoService
    {
        private readonly PuestoRepository _repository;
        private readonly BitacoraService _bitacora;

        public PuestoService(PuestoRepository repository, BitacoraService bitacora)
        {
            _repository = repository;
            _bitacora = bitacora;
        }

        public List<Puesto> ObtenerTodos(string usuario)
        {
            var lista = _repository.ObtenerTodos();

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacora.CrearJsonConsulta("Puestos")
            });

            return lista;
        }

        public Puesto ObtenerPorId(int puestoId)
        {
            return _repository.ObtenerPorId(puestoId);
        }

        public void Insertar(Puesto puesto, string usuario)
        {
            Validar(puesto);

            _repository.Insertar(puesto);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.CREATE,
                DescripcionJson = _bitacora.CrearJsonNuevo(puesto)
            });
        }

        public void Actualizar(Puesto puesto, string usuario)
        {
            Validar(puesto);

            var anterior = _repository.ObtenerPorId(puesto.PuestoId);

            if (anterior == null)
                throw new Exception("El puesto no existe.");

            _repository.Actualizar(puesto);

            var actual = _repository.ObtenerPorId(puesto.PuestoId);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(anterior, actual)
            });
        }

        public void Eliminar(int puestoId, string usuario)
        {
            var anterior = _repository.ObtenerPorId(puestoId);

            if (anterior == null)
                throw new Exception("El puesto no existe.");

            _repository.Eliminar(puestoId);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = _bitacora.CrearJsonEliminacion(anterior)
            });
        }

        private void Validar(Puesto puesto)
        {
            if (string.IsNullOrWhiteSpace(puesto.Nombre))
                throw new Exception("Debe ingresar el nombre del puesto.");

            if (puesto.Salario <= 0)
                throw new Exception("El salario debe ser mayor a cero.");

            if (puesto.JefePuestoId.HasValue && puesto.PuestoId == puesto.JefePuestoId.Value)
                throw new Exception("Un puesto no puede ser jefatura de sí mismo.");
        }
    }
}