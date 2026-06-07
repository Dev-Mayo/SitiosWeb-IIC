using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminParametrosModel : PageModel
    {
        private readonly ParametroService _service;

        public List<Parametro> Parametros { get; set; } = new();

        [BindProperty]
        public Parametro Parametro { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }

        public AdminParametrosModel(ParametroService service)
        {
            _service = service;
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
                    _service.Actualizar(Parametro, UsuarioActual());

                    return RedirectToPage("/GEN/AdminParametros",
                        new { mensaje = "Parámetro actualizado correctamente." });
                }

                _service.Insertar(Parametro, UsuarioActual());

                return RedirectToPage("/GEN/AdminParametros",
                    new { mensaje = "Parámetro registrado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/GEN/AdminParametros",
                    new { mensaje = ex.Message });
            }
        }

        public IActionResult OnPostEliminar(string codigo)
        {
            try
            {
                _service.Eliminar(codigo, UsuarioActual());

                return RedirectToPage("/GEN/AdminParametros",
                    new { mensaje = "Parámetro eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/GEN/AdminParametros",
                    new { mensaje = ex.Message });
            }
        }

        private void CargarDatos()
        {
            int cantidad = _service.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var lista = _service.ObtenerTodos(UsuarioActual());

            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)cantidad);

            Parametros = lista
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