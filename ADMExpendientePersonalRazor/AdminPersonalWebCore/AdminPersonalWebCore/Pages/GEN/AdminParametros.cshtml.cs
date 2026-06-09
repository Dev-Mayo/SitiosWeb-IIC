using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminParametrosModel : PageModel
    {
        private readonly ParametroService _service;
        private readonly AuthService _authService;

        public List<Parametro> Parametros { get; set; } = new();

        [BindProperty]
        public Parametro Parametro { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "u")]
        public string UsuarioActual { get; set; }

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }


        public AdminParametrosModel(
            ParametroService service,
            AuthService authService)
        {
            _service = service;
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
                    _service.Actualizar(Parametro, UsuarioActual);

                    return RedirectToPage(new
                    {
                        u = UsuarioActual,
                        mensaje = "Parámetro actualizado correctamente."
                    });
                }

                _service.Insertar(Parametro, UsuarioActual);

                return RedirectToPage(new
                {
                    u = UsuarioActual,
                    mensaje = "Parámetro registrado correctamente."
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
                    mensaje = "Parámetro eliminado correctamente."
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
            int cantidad = _service.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var lista = _service.ObtenerTodos(UsuarioActual);

            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)cantidad);

            Parametros = lista
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