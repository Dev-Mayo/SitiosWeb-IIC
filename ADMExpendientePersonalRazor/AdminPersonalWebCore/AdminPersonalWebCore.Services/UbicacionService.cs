using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;

namespace AdminPersonalWebCore.Services
{
    public class UbicacionService
    {
        private readonly UbicacionRepository _repository;
        private readonly BitacoraService _bitacora;

        public UbicacionService(
            UbicacionRepository repository,
            BitacoraService bitacora)
        {
            _repository = repository;
            _bitacora = bitacora;
        }

        public void CargarUbicaciones(
            List<UbicacionCarga> ubicaciones,
            string usuario)
        {
            if (ubicaciones == null || ubicaciones.Count == 0)
                throw new Exception("El archivo no contiene datos.");

            foreach (var item in ubicaciones)
            {
                Validar(item);
                _repository.GuardarUbicacion(item);
            }

            _bitacora.Registrar(new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.CREATE,
                DescripcionJson = _bitacora.CrearJsonConsulta(
                    "Se realizó la carga de información de provincias, cantones y distritos")
            });
        }

        public List<Provincia> ObtenerProvincias()
        {
            return _repository.ObtenerProvincias();
        }

        public List<Canton> ObtenerCantones()
        {
            return _repository.ObtenerCantones();
        }

        public List<Distrito> ObtenerDistritos()
        {
            return _repository.ObtenerDistritos();
        }

        public List<Ubicacion> ObtenerUbicaciones()
        {
            return _repository.ObtenerUbicaciones();
        }

        private void Validar(UbicacionCarga item)
        {
            if (string.IsNullOrWhiteSpace(item.CodigoProvincia))
                throw new Exception("El código de provincia es requerido.");

            if (string.IsNullOrWhiteSpace(item.Provincia))
                throw new Exception("El nombre de provincia es requerido.");

            if (string.IsNullOrWhiteSpace(item.CodigoCanton))
                throw new Exception("El código de cantón es requerido.");

            if (string.IsNullOrWhiteSpace(item.Canton))
                throw new Exception("El nombre de cantón es requerido.");

            if (string.IsNullOrWhiteSpace(item.CodigoDistrito))
                throw new Exception("El código de distrito es requerido.");

            if (string.IsNullOrWhiteSpace(item.Distrito))
                throw new Exception("El nombre de distrito es requerido.");
        }
    }
}