using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class RequisitosPuestosModel : PageModel
    {
        private readonly RequisitoPuestoService _service;
        private readonly ParametroService _parametroService;

        public RequisitosPuestosModel(
            RequisitoPuestoService service,
            ParametroService parametroService)
        {
            _service = service;
            _parametroService = parametroService;
        }

        public List<RequisitoPuesto> Requisitos { get; set; } = new();

        [BindProperty]
        public RequisitoPuesto Requisito { get; set; } = new();

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

        public IActionResult OnPostCrear(string nombre)
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    nombre = nombre
                };

                _service.Insertar(requisito, usuarioActual);

                TempData["MensajeExito"] = "El requisito de puesto ha sido registrado correctamente.";

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

        public IActionResult OnPostEditar(int requisito_id, string nombre)
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    requisito_id = requisito_id,
                    nombre = nombre
                };

                _service.Actualizar(requisito, usuarioActual);

                TempData["MensajeExito"] = "Requisito actualizado correctamente.";

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

        public IActionResult OnPostEliminar(int id)
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.Eliminar(id, usuarioActual);

                TempData["MensajeExito"] = "Requisito eliminado correctamente.";

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

            TamanoPagina = _parametroService.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var listaCompleta = _service.ObtenerTodos(usuarioActual);

            TotalPaginas = (int)Math.Ceiling(listaCompleta.Count / (double)TamanoPagina);

            if (TotalPaginas == 0)
                TotalPaginas = 1;

            if (pagina < 1)
                pagina = 1;

            if (pagina > TotalPaginas)
                pagina = TotalPaginas;

            PaginaActual = pagina;

            Requisitos = listaCompleta
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