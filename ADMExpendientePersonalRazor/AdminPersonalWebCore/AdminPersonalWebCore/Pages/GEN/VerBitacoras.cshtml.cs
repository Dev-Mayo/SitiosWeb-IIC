using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Pages;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class VerBitacorasModel : SecurePageModel
    {
        private readonly BitacoraService _svc;
        public VerBitacorasModel(BitacoraService svc) => _svc = svc;

        public List<BitacoraDisplay> Bitacoras { get; set; } = new();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public const int PageSize = 100;

        public string FiltroUsuario { get; set; }
        public string FiltroDescripcion { get; set; }
        public string Orden { get; set; } = "fecha_desc";
        public string MensajeError { get; set; }

        public IActionResult OnGet(string filtroUsuario, string filtroDescripcion,
            string orden, int page = 1)
        {
            var check = CheckSession();
            if (check != null) return check;

            FiltroUsuario = filtroUsuario;
            FiltroDescripcion = filtroDescripcion;
            Orden = orden ?? "fecha_desc";
            PaginaActual = page;

            try
            {
                var todas = _svc.ObtenerBitacoras(
                    NombreCompleto, filtroUsuario, filtroDescripcion, Orden);

                TotalRegistros = todas.Count;
                TotalPaginas = (int)Math.Ceiling(todas.Count / (double)PageSize);
                Bitacoras = todas
                    .Skip((PaginaActual - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                MensajeError = "Error al cargar bitácoras: " + ex.Message;
            }

            return Page();
        }

        public static string FormatearDescripcion(string json)
        {
            if (string.IsNullOrEmpty(json)) return "-";
            try
            {
                var obj = JObject.Parse(json);

                if (obj.ContainsKey("Consulta"))
                    return obj["Consulta"].ToString();

                if (obj.ContainsKey("Error"))
                    return $"<span class='text-danger'>{obj["Error"]}</span>";

                if (obj.ContainsKey("NuevoRegistro"))
                {
                    var data = obj["NuevoRegistro"] as JObject;
                    return "<strong>Nuevo registro:</strong><br/>" + FormatearObjeto(data);
                }

                if (obj.ContainsKey("RegistroEliminado"))
                {
                    var data = obj["RegistroEliminado"] as JObject;
                    return "<strong>Registro eliminado:</strong><br/>" + FormatearObjeto(data);
                }

                if (obj.ContainsKey("RegistroAnterior") && obj.ContainsKey("RegistroActual"))
                {
                    var anterior = obj["RegistroAnterior"] as JObject;
                    var actual = obj["RegistroActual"] as JObject;
                    return "<strong>Antes:</strong><br/>" + FormatearObjeto(anterior) +
                           "<strong>Después:</strong><br/>" + FormatearObjeto(actual);
                }

                return json;
            }
            catch
            {
                return json;
            }
        }

        private static string FormatearObjeto(JObject obj)
        {
            if (obj == null) return "-";
            var sb = new System.Text.StringBuilder();
            sb.Append("<ul class='mb-1 ps-3' style='font-size:0.82rem'>");
            foreach (var prop in obj.Properties())
            {
                if (prop.Name.ToLower().Contains("password")) continue;
                sb.Append($"<li><strong>{prop.Name}:</strong> {prop.Value}</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}