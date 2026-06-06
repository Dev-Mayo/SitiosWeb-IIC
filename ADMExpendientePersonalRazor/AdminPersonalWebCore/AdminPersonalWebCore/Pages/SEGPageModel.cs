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


        //===============================================================================================================================
        //Agregado por Rafa para facilidad de ediciones redirecionadas y tener un id mas facil de mantener y protegido
        private const string SessionKeyIdGeneral = "IdGeneral";
        public string IdGeneral
        {
            get => HttpContext.Session.GetString(SessionKeyIdGeneral) ?? "";
            set => HttpContext.Session.SetString(SessionKeyIdGeneral, value);
        }
        // Limpia IdGeneral cuando salgas de la página que lo usó
        protected void LimpiarIdGeneral() =>
            HttpContext.Session.Remove(SessionKeyIdGeneral);

        // Verifica si tiene valor, sirve para validar segun situación
        protected bool TieneIdGeneral =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyIdGeneral));

    }
}