using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminCompaniasModel : PageModel
    {
        private readonly CompaniaService _service;

        public List<Compania> Companias { get; set; } = new();

        [BindProperty]
        public Compania Compania { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        public AdminCompaniasModel(CompaniaService service)
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
            Companias = _service.ObtenerTodos(UsuarioActual())
                                .Take(10)
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