using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Collections.Generic;

namespace AdminPersonalWebCore.Services
{
    public class InstEducativaService
    {
        private readonly InstEducativaRepository _repo;
        private readonly BitacoraService _bitacora;

        public InstEducativaService(InstEducativaRepository repo, BitacoraService bitacora)
        {
            _repo = repo;
            _bitacora = bitacora;
        }

        public List<InstEducativa> ObtenerTodas(string usuarioActual)
        {
            var lista = _repo.ObtenerTodas();
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacora.CrearJsonConsulta("Instituciones Educativas")
            });
            return lista;
        }

        public InstEducativa ObtenerPorCodigo(string codigo) => _repo.ObtenerPorCodigo(codigo);
        public bool ValidarDuplicado(string codigo, string nombre, string excluir = null) =>
            _repo.ValidarDuplicado(codigo, nombre, excluir);

        public void Insertar(string codigo, string nombre, string usuarioActual)
        {
            _repo.Insertar(codigo, nombre);
            var nueva = _repo.ObtenerPorCodigo(codigo);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.CREATE,
                DescripcionJson = _bitacora.CrearJsonNuevo(nueva)
            });
        }

        public void Actualizar(string codigo, string nombre, string usuarioActual)
        {
            var anterior = _repo.ObtenerPorCodigo(codigo);
            _repo.Actualizar(codigo, nombre);
            var nueva = _repo.ObtenerPorCodigo(codigo);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.UPDATE,
                DescripcionJson = _bitacora.CrearJsonActualizacion(anterior, nueva)
            });
        }

        public void Eliminar(string codigo, string usuarioActual)
        {
            var inst = _repo.ObtenerPorCodigo(codigo);
            _repo.Eliminar(codigo);
            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = _bitacora.CrearJsonEliminacion(inst)
            });
        }
    }
}