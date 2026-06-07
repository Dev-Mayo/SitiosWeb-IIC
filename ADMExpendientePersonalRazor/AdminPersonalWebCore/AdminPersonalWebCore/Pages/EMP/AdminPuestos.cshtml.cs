using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class AdminPuestosModel : PageModel
    {
        private readonly PuestoService _service;
        private readonly ParametroService _parametroService;

        public List<Puesto> Puestos { get; set; } = new();

        [BindProperty]
        public Puesto Puesto { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }

        public AdminPuestosModel(
            PuestoService service,
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
                    _service.Actualizar(Puesto, UsuarioActual());
                    return RedirectToPage("/EMP/AdminPuestos",
                        new { mensaje = "Puesto actualizado correctamente." });
                }

                _service.Insertar(Puesto, UsuarioActual());
                return RedirectToPage("/EMP/AdminPuestos",
                    new { mensaje = "Puesto registrado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/EMP/AdminPuestos",
                    new { mensaje = ex.Message });
            }
        }

        public IActionResult OnPostEliminar(int puestoId)
        {
            try
            {
                _service.Eliminar(puestoId, UsuarioActual());

                return RedirectToPage("/EMP/AdminPuestos",
                    new { mensaje = "Puesto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/EMP/AdminPuestos",
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

            Puestos = lista
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