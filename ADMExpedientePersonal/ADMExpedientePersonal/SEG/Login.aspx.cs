using ADMExpedientePersonal.BLL;
using System;
using System.Web.UI;

namespace ADMExpedientePersonal.SEG
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["msg"] == "login")
            {
                pnlMensaje.Visible = true;
                divMensaje.InnerText = "Por favor inicie sesión para utilizar el sistema.";
                divMensaje.Attributes["class"] = "alert alert-warning";
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            AuthBLL auth = new AuthBLL();
            var result = auth.Login(txtUsuario.Text.Trim(), txtPassword.Text.Trim());

            if (!result.success)
            {
                pnlMensaje.Visible = true;
                divMensaje.InnerText = result.mensaje;
                divMensaje.Attributes["class"] = "alert alert-danger";

                if (result.mensaje.Contains("bloqueado"))
                {
                    pnlBloqueado.Visible = true;
                }
                return;
            }

            Session["Usuario"] = result.usuario;
            Session["NombreUsuario"] = result.usuario.nombre_completo;
            Session["IdUsuario"] = result.usuario.id_usuario;

            Response.Redirect($"~/SEG/Bienvenida.aspx?u={result.usuario.nombreusuario}");
        }
    }
}