using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class ContratarEmpleadoModel : PageModel
    {
        private readonly ContratacionService _service;

        public ContratarEmpleadoModel(ContratacionService service)
        {
            _service = service;
        }

        [BindProperty]
        public ContratacionRequest Contratacion { get; set; } = new();

        public List<Oferente> Oferentes { get; set; } = new();
        public List<Puesto> Puestos { get; set; } = new();
        public List<Empleado> Jefaturas { get; set; } = new();

        public string MensajeExito { get; set; } = string.Empty;
        public string MensajeError { get; set; } = string.Empty;

        public void OnGet()
        {
            CargarDatos();
        }

        public IActionResult OnPost()
        {
            string usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.ContratarEmpleado(Contratacion, usuarioActual);

                TempData["MensajeExito"] = "Empleado creado con éxito";

                return RedirectToPage(new { u = usuarioActual });
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                CargarDatos();
                return Page();
            }
        }

        private void CargarDatos()
        {
            string usuarioActual = ObtenerUsuarioActual();

            Oferentes = _service.ObtenerOferentesDisponibles(usuarioActual);
            Puestos = _service.ObtenerPuestos();
            Jefaturas = _service.ObtenerJefaturas();

            if (TempData["MensajeExito"] != null)
            {
                MensajeExito = TempData["MensajeExito"]!.ToString()!;
            }
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