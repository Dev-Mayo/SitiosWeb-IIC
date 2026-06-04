using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml;

namespace AdminPersonalWebCore.Services
{
    public class BitacoraService
    {
        private readonly BitacoraRepository _repo;
        public BitacoraService(BitacoraRepository repo) => _repo = repo;

        public void Registrar(Bitacora bitacora) => _repo.Registrar(bitacora);

        public List<BitacoraDisplay> ObtenerBitacoras(string usuarioActual,
            string filtroUsuario = null, string filtroDesc = null, string orden = "fecha_desc")
        {
            var lista = _repo.ObtenerBitacoras(filtroUsuario, filtroDesc, orden);
            Registrar(new Bitacora
            {
                Usuario = usuarioActual,
                Accion = AccionBitacora.READ,
                DescripcionJson = CrearJsonConsulta("Bitácoras")
            });
            return lista;
        }

        public string CrearJsonError(string msg) =>
            new JObject { ["Error"] = $"Error: {msg}" }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonNuevo(object obj) =>
            new JObject { ["NuevoRegistro"] = JObject.FromObject(obj) }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonActualizacion(object anterior, object actual) =>
            new JObject
            {
                ["RegistroAnterior"] = JObject.FromObject(anterior),
                ["RegistroActual"] = JObject.FromObject(actual)
            }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonEliminacion(object obj) =>
            new JObject { ["RegistroEliminado"] = JObject.FromObject(obj) }.ToString(Newtonsoft.Json.Formatting.None);

        public string CrearJsonConsulta(string elemento) =>
            new JObject { ["Consulta"] = $"El usuario consulta {elemento}" }.ToString(Newtonsoft.Json.Formatting.None);
    }
}