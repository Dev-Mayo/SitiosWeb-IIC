using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages
{
    public class SecurePageModel : PageModel
    {
        public string UsuarioActual { get; private set; }
        public string NombreCompleto { get; private set; }

        public IActionResult CheckSession()
        {
            string u = Request.Query["u"];
            string sessionUser = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(sessionUser))
                return Redirect("/SEG/Login?msg=login");

            UsuarioActual = u;
            NombreCompleto = HttpContext.Session.GetString("nombre_completo") ?? u;
            return null;
        }
    }
}