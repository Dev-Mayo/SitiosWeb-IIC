using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class CompaniaService
    {
        private readonly CompaniaRepository _repository;
        private readonly BitacoraService _bitacora;

        public CompaniaService(
            CompaniaRepository repository,
            BitacoraService bitacora)
        {
            _repository = repository;
            _bitacora = bitacora;
        }

        public List<Compania> ObtenerTodos(string usuario)
        {
            var lista = _repository.ObtenerTodos();

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson =
                    _bitacora.CrearJsonConsulta("Compañías")
            });

            return lista;
        }

        public Compania ObtenerPorCodigo(string codigo)
        {
            return _repository.ObtenerPorCodigo(codigo);
        }

        public void Insertar(
            Compania compania,
            string usuario)
        {
            if (string.IsNullOrWhiteSpace(compania.CodigoCompania))
                throw new Exception("Debe ingresar el código.");

            if (string.IsNullOrWhiteSpace(compania.Nombre))
                throw new Exception("Debe ingresar el nombre.");

            var existe =
                _repository.ObtenerPorCodigo(
                    compania.CodigoCompania);

            if (existe != null)
                throw new Exception(
                    "Ya existe una compañía con ese código.");

            _repository.Insertar(compania);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.CREATE,
                DescripcionJson =
                    _bitacora.CrearJsonNuevo(compania)
            });
        }

        public void Actualizar(
            Compania compania,
            string usuario)
        {
            var anterior =
                _repository.ObtenerPorCodigo(
                    compania.CodigoCompania);

            if (anterior == null)
                throw new Exception(
                    "La compañía no existe.");

            _repository.Actualizar(compania);

            var actual =
                _repository.ObtenerPorCodigo(
                    compania.CodigoCompania);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson =
                    _bitacora.CrearJsonActualizacion(
                        anterior,
                        actual)
            });
        }

        public void Eliminar(
            string codigo,
            string usuario)
        {
            var anterior =
                _repository.ObtenerPorCodigo(codigo);

            if (anterior == null)
                throw new Exception(
                    "La compañía no existe.");

            _repository.Eliminar(codigo);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.DELETE,
                DescripcionJson =
                    _bitacora.CrearJsonEliminacion(
                        anterior)
            });
        }
    }
}