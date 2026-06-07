using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class ParametroService
    {
        private readonly ParametroRepository _repository;
        private readonly BitacoraService _bitacora;

        public ParametroService(
            ParametroRepository repository,
            BitacoraService bitacora)
        {
            _repository = repository;
            _bitacora = bitacora;
        }

        public List<Parametro> ObtenerTodos(string usuario)
        {
            var lista = _repository.ObtenerTodos();

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson =
                    _bitacora.CrearJsonConsulta("Parámetros")
            });

            return lista;
        }

        public Parametro ObtenerPorCodigo(string codigo)
        {
            return _repository.ObtenerPorCodigo(codigo);
        }

        public void Insertar(
            Parametro parametro,
            string usuario)
        {
            Validar(parametro);

            if (_repository.ObtenerPorCodigo(
                parametro.CodigoParametro) != null)
            {
                throw new Exception(
                    "Ya existe un parámetro con ese código.");
            }

            _repository.Insertar(parametro);

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.CREATE,
                DescripcionJson =
                    _bitacora.CrearJsonNuevo(parametro)
            });
        }

        public void Actualizar(
            Parametro parametro,
            string usuario)
        {
            Validar(parametro);

            var anterior =
                _repository.ObtenerPorCodigo(
                    parametro.CodigoParametro);

            if (anterior == null)
                throw new Exception(
                    "El parámetro no existe.");

            _repository.Actualizar(parametro);

            var actual =
                _repository.ObtenerPorCodigo(
                    parametro.CodigoParametro);

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
                    "El parámetro no existe.");

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

        public string ObtenerValor(string codigo)
        {
            var parametro = _repository.ObtenerPorCodigo(codigo);

            if (parametro == null)
                throw new Exception($"No existe el parámetro {codigo}.");

            return parametro.Valor;
        }

        public int ObtenerValorEntero(string codigo)
        {
            var valor = ObtenerValor(codigo);

            if (!int.TryParse(valor, out int resultado))
                throw new Exception($"El parámetro {codigo} debe ser numérico.");

            return resultado;
        }

        public string ObtenerValorODefecto(string codigo, string valorDefault)
        {
            try
            {
                var parametro = _repository.ObtenerPorCodigo(codigo);

                if (parametro == null || string.IsNullOrWhiteSpace(parametro.Valor))
                    return valorDefault;

                return parametro.Valor;
            }
            catch
            {
                return valorDefault;
            }
        }

        public List<string> ObtenerLista(string codigo)
        {
            return ObtenerValor(codigo)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLower())
                .ToList();
        }

        public int ObtenerValorEnteroODefecto(string codigo, int valorDefault)
        {
            try
            {
                var valor = ObtenerValorODefecto(codigo, valorDefault.ToString());

                if (!int.TryParse(valor, out int resultado))
                    return valorDefault;

                return resultado;
            }
            catch
            {
                return valorDefault;
            }
        }

        public List<string> ObtenerListaODefecto(string codigo, List<string> valorDefault)
        {
            try
            {
                var valor = ObtenerValorODefecto(codigo, string.Join(",", valorDefault));

                return valor
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().ToLower())
                    .ToList();
            }
            catch
            {
                return valorDefault;
            }
        }

        private void Validar(Parametro parametro)
        {
            if (string.IsNullOrWhiteSpace(
                parametro.CodigoParametro))
            {
                throw new Exception(
                    "Debe ingresar el código.");
            }

            if (string.IsNullOrWhiteSpace(
                parametro.Valor))
            {
                throw new Exception(
                    "Debe ingresar el valor.");
            }

            if (parametro.Valor.Length > 500)
            {
                throw new Exception(
                    "El valor no puede superar los 500 caracteres.");
            }
        }
    }
}