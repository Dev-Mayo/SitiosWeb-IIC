using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminCompaniasModel : PageModel
    {
        private readonly CompaniaService _service;
        private readonly ParametroService _parametroService;

        public List<Compania> Companias { get; set; } = new();

        [BindProperty]
        public Compania Compania { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }
        public AdminCompaniasModel(
            CompaniaService service,
            ParametroService parametroService)
        {
            _service = service;
            _parametroService = parametroService;
        }

        public void OnGet(string mensaje = null)
        {
            Mensaje = mensaje;
            CargarDatos();
        }

        public IActionResult OnPostGuardar()
        {
            try
            {
                if (EsEdicion)
                {
                    _service.Actualizar(Compania, UsuarioActual());
                    return RedirectToPage("/GEN/AdminCompanias",
                        new { mensaje = "Compañía actualizada correctamente." });
                }

                _service.Insertar(Compania, UsuarioActual());
                return RedirectToPage("/GEN/AdminCompanias",
                    new { mensaje = "Compañía registrada correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/GEN/AdminCompanias",
                    new { mensaje = ex.Message });
            }
        }

        public IActionResult OnPostEliminar(string codigo)
        {
            try
            {
                _service.Eliminar(codigo, UsuarioActual());

                return RedirectToPage("/GEN/AdminCompanias",
                    new { mensaje = "Compañía eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/GEN/AdminCompanias",
                    new { mensaje = ex.Message });
            }
        }

        private void CargarDatos()
        {
            int cantidad = _parametroService.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var lista = _service.ObtenerTodos(UsuarioActual());

            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)cantidad);

            Companias = lista
                .Skip((PaginaActual - 1) * cantidad)
                .Take(cantidad)
                .ToList();
        }

        private string UsuarioActual()
        {
            return HttpContext.Session.GetString("NombreUsuario")
                   ?? User.Identity?.Name
                   ?? "Desconocido";
        }
    }
}