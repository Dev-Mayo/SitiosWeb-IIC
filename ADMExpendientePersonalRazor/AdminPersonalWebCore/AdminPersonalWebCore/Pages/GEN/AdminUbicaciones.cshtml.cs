using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminUbicacionesModel : PageModel
    {
        private readonly UbicacionService _service;
        private readonly ParametroService _parametroService;

        public List<Provincia> Provincias { get; set; } = new();
        public List<Canton> Cantones { get; set; } = new();
        public List<Distrito> Distritos { get; set; } = new();
        public List<Ubicacion> Ubicaciones { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }


        [BindProperty]
        public IFormFile Archivo { get; set; }

        public string Mensaje { get; set; }

        public AdminUbicacionesModel(
            UbicacionService service,
            ParametroService parametroService)
        {
            _service = service;
            _parametroService = parametroService;
        }

        public void OnGet(string mensaje = null)
        {
            Mensaje = mensaje;
            CargarListas();
        }

        private void CargarListas()
        {
            int cantidad = _parametroService.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var lista = _service.ObtenerUbicaciones();

            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)cantidad);

            Ubicaciones = lista
                .Skip((PaginaActual - 1) * cantidad)
                .Take(cantidad)
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Archivo == null || Archivo.Length == 0)
                    throw new Exception("Debe seleccionar un archivo.");

                int maxMb = _parametroService.ObtenerValorEnteroODefecto(
                    "TAMANO_MAX_ARCHIVO_MB",
                    10
                );

                long maxBytes = maxMb * 1024L * 1024L;

                if (Archivo.Length > maxBytes)
                    throw new Exception($"El archivo no puede superar {maxMb} MB.");

                var extension = Path.GetExtension(Archivo.FileName).ToLower();

                var extensionesPermitidas = _parametroService.ObtenerListaODefecto(
                    "EXTENSIONES_UBICACION",
                    new List<string> { ".csv", ".txt" }
                );

                if (!extensionesPermitidas.Contains(extension))
                    throw new Exception("El tipo de archivo no está permitido.");

                var ubicaciones = new List<UbicacionCarga>();

                using var reader = new StreamReader(
                    Archivo.OpenReadStream(),
                    Encoding.UTF8
                );

                bool primeraLinea = true;

                while (!reader.EndOfStream)
                {
                    var linea = await reader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    if (primeraLinea)
                    {
                        primeraLinea = false;

                        if (linea.Contains("CodigoProvincia"))
                            continue;
                    }

                    var partes = linea.Split(',');

                    if (partes.Length < 6)
                        throw new Exception("El archivo no tiene el formato correcto.");

                    ubicaciones.Add(new UbicacionCarga
                    {
                        CodigoProvincia = partes[0].Trim(),
                        Provincia = partes[1].Trim(),
                        CodigoCanton = partes[2].Trim(),
                        Canton = partes[3].Trim(),
                        CodigoDistrito = partes[4].Trim(),
                        Distrito = partes[5].Trim()
                    });
                }

                _service.CargarUbicaciones(ubicaciones, UsuarioActual());

                return RedirectToPage("/GEN/AdminUbicaciones",
                    new { mensaje = "Carga de ubicaciones realizada correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/GEN/AdminUbicaciones",
                    new { mensaje = ex.Message });
            }
        }

        private string UsuarioActual()
        {
            return HttpContext.Session.GetString("NombreUsuario")
                   ?? User.Identity?.Name
                   ?? "Desconocido";
        }
    }
}