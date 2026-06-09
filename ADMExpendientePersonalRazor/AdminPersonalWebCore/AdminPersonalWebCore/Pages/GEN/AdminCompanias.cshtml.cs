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
        private readonly AuthService _authService;

        public List<Compania> Companias { get; set; } = new();

        [BindProperty]
        public Compania Compania { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }

        [BindProperty(SupportsGet = true, Name = "u")]
        public string UsuarioActual { get; set; }

        public AdminCompaniasModel(
            CompaniaService service,
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
            CargarDatos();

            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                if (EsEdicion)
                {
                    _service.Actualizar(Compania, UsuarioActual);

                    return RedirectToPage(new
                    {
                        u = UsuarioActual,
                        mensaje = "Compañía actualizada correctamente."
                    });
                }

                _service.Insertar(Compania, UsuarioActual);

                return RedirectToPage(new
                {
                    u = UsuarioActual,
                    mensaje = "Compañía registrada correctamente."
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

        public IActionResult OnPostEliminar(string codigo)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                _service.Eliminar(codigo, UsuarioActual);

                return RedirectToPage(new
                {
                    u = UsuarioActual,
                    mensaje = "Compañía eliminada correctamente."
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

        private void CargarDatos()
        {
            int cantidad = _parametroService.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var lista = _service.ObtenerTodos(UsuarioActual);

            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)cantidad);

            Companias = lista
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