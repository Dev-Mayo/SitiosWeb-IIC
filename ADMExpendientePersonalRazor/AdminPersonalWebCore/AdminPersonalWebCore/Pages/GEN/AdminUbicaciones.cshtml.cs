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
        private readonly AuthService _authService;

        public List<Ubicacion> Ubicaciones { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }

        [BindProperty(SupportsGet = true, Name = "u")]
        public string UsuarioActual { get; set; }

        [BindProperty]
        public IFormFile Archivo { get; set; }

        public string Mensaje { get; set; }

        public AdminUbicacionesModel(
            UbicacionService service,
            ParametroService parametroService,
            AuthService authService)
        {
            _service = service;
            _parametroService = parametroService;
            _authService = authService;
        }

        public IActionResult OnGet(string mensaje = null)
        {
            var check = ValidarSession();
            if (check != null) return check;

            Mensaje = mensaje;
            CargarListas();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var check = ValidarSession();
            if (check != null) return check;

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

                _service.CargarUbicaciones(ubicaciones, UsuarioActual);

                return RedirectToPage(new
                {
                    u = UsuarioActual,
                    mensaje = "Carga de ubicaciones realizada correctamente."
                });
            }
            catch (Exception ex)
            {
                return RedirectToPage(new
                {
                    u = UsuarioActual,
                    mensaje = ex.Message
                });
            }
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

        private IActionResult? ValidarSession()
        {
            var check = CheckSession();
            if (check != null) return check;

            var usuario = _authService.ObtenerPorNombre(UsuarioActual);

            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");

            return null;
        }

        private IActionResult? CheckSession()
        {
            if (string.IsNullOrWhiteSpace(UsuarioActual))
                return Redirect("/SEG/Login?msg=login");

            return null;
        }
    }
}