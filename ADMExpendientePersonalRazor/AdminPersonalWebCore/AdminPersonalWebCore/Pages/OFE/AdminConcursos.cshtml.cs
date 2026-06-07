using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class AdminConcursosModel : PageModel
    {
        private readonly ConcursoService _service;

        public List<Concurso> Concursos { get; set; } = new();

        [BindProperty]
        public Concurso Concurso { get; set; } = new();

        [BindProperty]
        public bool EsEdicion { get; set; }

        public string Mensaje { get; set; }

        public AdminConcursosModel(ConcursoService service)
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
                    _service.Actualizar(Concurso, UsuarioActual());
                    return RedirectToPage("/OFE/AdminConcursos",
                        new { mensaje = "Concurso actualizado correctamente." });
                }

                _service.Insertar(Concurso, UsuarioActual());
                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = "Concurso registrado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = ex.Message });
            }
        }

        public IActionResult OnPostEliminar(string codigo)
        {
            try
            {
                _service.Eliminar(codigo, UsuarioActual());

                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = "Concurso eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = ex.Message });
            }
        }

        public IActionResult OnPostCambiarEstado(string codigo)
        {
            try
            {
                _service.CambiarEstado(codigo, UsuarioActual());

                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = "Estado del concurso actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/OFE/AdminConcursos",
                    new { mensaje = ex.Message });
            }
        }

        private void CargarDatos()
        {
            Concursos = _service.ObtenerTodos(UsuarioActual())
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