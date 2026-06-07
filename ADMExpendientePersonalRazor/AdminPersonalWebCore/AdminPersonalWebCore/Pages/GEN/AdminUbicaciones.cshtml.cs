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
        public List<Provincia> Provincias { get; set; } = new();
        public List<Canton> Cantones { get; set; } = new();
        public List<Distrito> Distritos { get; set; } = new();

        [BindProperty]
        public IFormFile Archivo { get; set; }

        public string Mensaje { get; set; }

        public AdminUbicacionesModel(UbicacionService service)
        {
            _service = service;
        }

        public void OnGet(string mensaje = null)
        {
            Mensaje = mensaje;
            CargarListas();
        }
        private void CargarListas()
        {
            Provincias = _service.ObtenerProvincias();
            Cantones = _service.ObtenerCantones();
            Distritos = _service.ObtenerDistritos();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Archivo == null || Archivo.Length == 0)
                    throw new Exception("Debe seleccionar un archivo.");

                var extension = Path.GetExtension(Archivo.FileName).ToLower();

                if (extension != ".csv" && extension != ".txt")
                    throw new Exception("Solo se permiten archivos CSV o TXT.");

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