using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class ModulosModel : PageModel
    {
        private readonly ModuloService _service;

        public ModulosModel(ModuloService service)
        {
            _service = service;
        }

        public List<Modulo> Modulos { get; set; } = new();
        public List<Rol> Roles { get; set; } = new();
        public List<int> RolesSeleccionadosEdicion { get; set; } = new();

        [BindProperty]
        public Modulo Modulo { get; set; } = new();

        [BindProperty]
        public List<int> RolesSeleccionados { get; set; } = new();

        public string MensajeExito { get; set; } = string.Empty;
        public string MensajeError { get; set; } = string.Empty;

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public const int TamanoPagina = 10;

        public void OnGet(int? pagina, int? editarId, string? exito, string? error)
        {
            CargarDatos(pagina ?? 1);

            if (editarId.HasValue)
            {
                Modulo = _service.ObtenerPorId(editarId.Value) ?? new Modulo();
                RolesSeleccionadosEdicion = _service.ObtenerRolesPorModulo(editarId.Value);
            }

            MensajeExito = exito ?? string.Empty;
            MensajeError = error ?? string.Empty;
        }

        public IActionResult OnPostGuardar()
        {
            string usuarioActual = ObtenerUsuarioActual();

            try
            {
                if (Modulo.id_modulo == 0)
                {
                    _service.Insertar(Modulo, RolesSeleccionados, usuarioActual);

                    return RedirectToPage(new
                    {
                        u = usuarioActual,
                        exito = "El módulo ha sido registrado correctamente."
                    });
                }

                _service.Actualizar(Modulo, RolesSeleccionados, usuarioActual);

                return RedirectToPage(new
                {
                    u = usuarioActual,
                    exito = "El módulo ha sido actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                CargarDatos(PaginaActual);
                MensajeError = ex.Message;
                return Page();
            }
        }

        public IActionResult OnPostEliminar(int idModulo)
        {
            string usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.Eliminar(idModulo, usuarioActual);

                return RedirectToPage(new
                {
                    u = usuarioActual,
                    exito = "El módulo ha sido eliminado correctamente."
                });
            }
            catch (Exception ex)
            {
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    error = ex.Message
                });
            }
        }

        private void CargarDatos(int pagina)
        {
            string usuarioActual = ObtenerUsuarioActual();

            var lista = _service.ObtenerTodos(usuarioActual);

            PaginaActual = pagina;
            TotalPaginas = (int)Math.Ceiling(lista.Count / (double)TamanoPagina);

            Modulos = lista
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();

            Roles = _service.ObtenerRoles();
        }

        private string ObtenerUsuarioActual()
        {
            var usuario = Request.Query["u"].ToString();

            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = HttpContext.Session.GetString("usuario")
                       ?? HttpContext.Session.GetString("nombreusuario")
                       ?? User.Identity?.Name
                       ?? "UsuarioDesconocido";
            }

            return usuario;
        }
    }
}