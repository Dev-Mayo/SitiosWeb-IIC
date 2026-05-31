using ADMExpedientePersonal.BLL;
using System;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class Areas : System.Web.UI.Page
    {
        private AreaBLL areaBLL = new AreaBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarAreas();

                if (Session["Mensaje"] != null)
                {
                    lblMensaje.Text = Session["Mensaje"].ToString();
                    Session["Mensaje"] = null;
                }
            }
        }

        private void CargarAreas()
        {
            gvAreas.DataSource = areaBLL.ObtenerTodos();
            gvAreas.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("AreaForm.aspx?u=" + usuario);
        }

        protected void gvAreas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAreas.PageIndex = e.NewPageIndex;
            CargarAreas();
        }

        protected void gvAreas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarArea")
            {
                string usuario = Request.QueryString["u"];
                Response.Redirect("AreaForm.aspx?id=" + id + "&u=" + usuario);
            }

            if (e.CommandName == "EliminarArea")
            {
                try
                {
                    areaBLL.Eliminar(id);
                    Session["Mensaje"] = "El área ha sido eliminada correctamente.";
                    string usuario = Request.QueryString["u"];
                    Response.Redirect("Areas.aspx?u=" + usuario);
                }
                catch (Exception)
                {
                    lblError.Text = "No se puede eliminar un registro con datos relacionados.";
                }
            }
        }
    }
}