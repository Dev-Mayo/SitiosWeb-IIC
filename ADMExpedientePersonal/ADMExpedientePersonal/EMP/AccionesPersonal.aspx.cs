using ADMExpedientePersonal.BLL;
using System;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class AccionesPersonal : System.Web.UI.Page
    {
        private AccionPersonalBLL accionBLL = new AccionPersonalBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarAcciones();

                if (Session["Mensaje"] != null)
                {
                    lblMensaje.Text = Session["Mensaje"].ToString();
                    Session["Mensaje"] = null;
                }
            }
        }

        private void CargarAcciones()
        {
            gvAccionesPersonal.DataSource = accionBLL.ObtenerTodos();
            gvAccionesPersonal.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("AccionPersonalForm.aspx?u=" + usuario);
        }

        protected void gvAccionesPersonal_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAccionesPersonal.PageIndex = e.NewPageIndex;
            CargarAcciones();
        }

        protected void gvAccionesPersonal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int accionId = Convert.ToInt32(e.CommandArgument);
            string usuario = Request.QueryString["u"];

            if (e.CommandName == "EditarAccion")
            {
                Response.Redirect("AccionPersonalForm.aspx?id=" + accionId + "&u=" + usuario);
            }

            if (e.CommandName == "EliminarAccion")
            {
                try
                {
                    accionBLL.Eliminar(accionId);
                    Session["Mensaje"] = "La acción de personal ha sido eliminada correctamente.";
                    Response.Redirect("AccionesPersonal.aspx?u=" + usuario);
                }
                catch
                {
                    lblError.Text = "No se puede eliminar un registro con datos relacionados.";
                }
            }
        }
    }
}