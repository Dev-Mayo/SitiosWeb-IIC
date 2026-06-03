using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class BienvenidaModel : PageModel
    {
        private readonly AuthService _authService;

        public BienvenidaModel(AuthService authService)
        {
            _authService = authService;
        }

        public string NombreCompleto { get; set; }

        public IActionResult OnGet()
        {
            string u = Request.Query["u"];

            if (string.IsNullOrEmpty(u))
                return Redirect("/SEG/Login?msg=login");

            // Validate session
            string sessionUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(sessionUser))
                return Redirect("/SEG/Login?msg=login");

            var usuario = _authService.ObtenerPorNombre(u);
            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");

            NombreCompleto = usuario.nombre_completo;
            return Page();
        }
    }
}