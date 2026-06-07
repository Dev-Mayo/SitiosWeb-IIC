using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class ConcursoService
    {
        private readonly ConcursoRepository _repository;
        private readonly BitacoraService _bitacora;
        private readonly ParametroService _parametroService;

        public ConcursoService(ConcursoRepository repository, BitacoraService bitacora, ParametroService parametroService)
        {
            _repository = repository;
            _bitacora = bitacora;
            _parametroService = parametroService;

        }

        public List<Concurso> ObtenerTodos(string usuario)
        {
            var lista = _repository.ObtenerTodos();

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacora.CrearJsonConsulta("Concursos")
            });

            return lista;
        }

        public Concurso ObtenerPorCodigo(string codigo)
        {
            return _repository.ObtenerPorCodigo(codigo);
        }

        public void Insertar(Concurso concurso, string usuario)
        {
            Validar(concurso);

            if (_repository.ObtenerPorCodigo(concurso.CodigoConcurso) != null)
                throw new Exception("Ya existe un concurso con ese código.");

            var estadoDefault = _parametroService.ObtenerValorODefecto(
                "ESTADO_CONCURSO_DEFAULT",
                "Vigente"
            );

            if (estadoDefault != "Vigente" && estadoDefault != "Vencido")
                estadoDefault = "Vigente";

            concurso.Estado = estadoDefault;

            _repository.Insertar(concurso);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.CREATE,
                DescripcionJson = _bitacora.CrearJsonNuevo(concurso)
            });
        }

        public void Actualizar(Concurso concurso, string usuario)
        {
            Validar(concurso);

            var anterior = _repository.ObtenerPorCodigo(concurso.CodigoConcurso);

            if (anterior == null)
                throw new Exception("El concurso no existe.");

            concurso.Estado = anterior.Estado;

            _repository.Actualizar(concurso);

            var actual = _repository.ObtenerPorCodigo(concurso.CodigoConcurso);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(anterior, actual)
            });
        }

        public void Eliminar(string codigo, string usuario)
        {
            var anterior = _repository.ObtenerPorCodigo(codigo);

            if (anterior == null)
                throw new Exception("El concurso no existe.");

            if (_repository.TieneOferentesAsociados(codigo))
                throw new Exception("No se puede eliminar el concurso porque tiene oferentes asociados.");

            _repository.Eliminar(codigo);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = _bitacora.CrearJsonEliminacion(anterior)
            });
        }

        public void CambiarEstado(string codigo, string usuario)
        {
            var anterior = _repository.ObtenerPorCodigo(codigo);

            if (anterior == null)
                throw new Exception("El concurso no existe.");

            _repository.CambiarEstado(codigo);

            var actual = _repository.ObtenerPorCodigo(codigo);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(anterior, actual)
            });
        }

        private void Validar(Concurso concurso)
        {
            if (string.IsNullOrWhiteSpace(concurso.CodigoConcurso))
                throw new Exception("Debe ingresar el código del concurso.");

            if (string.IsNullOrWhiteSpace(concurso.Nombre))
                throw new Exception("Debe ingresar el nombre del concurso.");

            if (concurso.FechaFin < concurso.FechaInicio)
                throw new Exception("La fecha fin debe ser mayor o igual a la fecha inicio.");
        }
    }
}