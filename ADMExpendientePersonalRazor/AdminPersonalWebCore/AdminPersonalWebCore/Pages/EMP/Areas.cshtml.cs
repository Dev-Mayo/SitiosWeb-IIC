using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class AreasModel : PageModel
    {
        private readonly AreaService _service;

        public AreasModel(AreaService service)
        {
            _service = service;
        }

        public List<Area> Areas { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();

        [BindProperty]
        public Area Area { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 10;

        public void OnGet(int pagina = 1)
        {
            if (TempData["MensajeExito"] != null)
                MensajeExito = TempData["MensajeExito"]!.ToString();

            if (TempData["MensajeError"] != null)
                MensajeError = TempData["MensajeError"]!.ToString();

            CargarDatos(pagina);
        }

        public IActionResult OnPostCrear(int codigoArea, string nombre, int jefatura) //crear porst 
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var area = new Area
                {
                    CodigoArea = codigoArea,
                    Nombre = nombre,
                    Jefatura = jefatura
                };

                _service.Insertar(area, usuarioActual);

                TempData["MensajeExito"] = "El área ha sido registrada correctamente.";

                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEditar(int codigoArea, string nombre, int jefatura) //editar por código de área, no por id
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var area = new Area
                {
                    CodigoArea = codigoArea,
                    Nombre = nombre,
                    Jefatura = jefatura
                };

                _service.Actualizar(area, usuarioActual);

                TempData["MensajeExito"] = "Área actualizada correctamente.";

                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEliminar(int codigoArea) //eliminar por código de área, no por id
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.Eliminar(codigoArea, usuarioActual);

                TempData["MensajeExito"] = "Área eliminada correctamente.";

                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }
        private void CargarDatos(int pagina = 1)
        {
            var usuarioActual = ObtenerUsuarioActual();

            var listaCompleta = _service.ObtenerTodos(usuarioActual);

            Empleados = _service.ObtenerEmpleados();

            PaginaActual = pagina;

            TotalPaginas =
                (int)Math.Ceiling(listaCompleta.Count / (double)TamanoPagina);

            Areas = listaCompleta
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();
        }

        private string ObtenerUsuarioActual()
        {
            var usuario = Request.Query["u"].ToString();

            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = HttpContext.Session.GetString("usuario")
                       ?? HttpContext.Session.GetString("nombreusuario")
                       ?? HttpContext.Session.GetString("Usuario")
                       ?? User.Identity?.Name
                       ?? "UsuarioDesconocido";
            }

            return usuario;
        }
    }
}