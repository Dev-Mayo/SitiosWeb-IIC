using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.SEG
{
    public partial class Bienvenida : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/SEG/Login.aspx?msg=login");
                return;
            }

            if (!IsPostBack)
            {
                lblNombreUsuario.Text = Session["NombreUsuario"].ToString();
            }
        }
    }
}