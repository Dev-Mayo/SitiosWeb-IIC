using ADMExpedientePersonal.BLL;
using System;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class RequisitosPuestos : System.Web.UI.Page
    {
        private RequisitoPuestoBLL requisitoBLL = new RequisitoPuestoBLL();
        private readonly RequisitoPuestoBLL _bll = new RequisitoPuestoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarRequisitos();
                string mensaje=Request.QueryString["msg"];
                if (!string.IsNullOrEmpty(mensaje))
                { 
                lblMensaje.Visible = true;
                    switch (mensaje)
                    { 
                        case "guardado":
                        lblMensaje.Text = "Requisito guardado exitosamente.";
                       
                            break;
                        case "actualizado":
                            lblMensaje.Text = "Requisito actualizado exitosamente.";
                             break;

                        case "eliminado":
                            lblMensaje.Text = "Requisito eliminado exitosamente.";
                            break;

                    }// fin del switch

                }// fin del if para mostrar mensaje
            }
        }

        private void CargarRequisitos()
        {
            try
            {
                gvRequisitos.DataSource = requisitoBLL.ObtenerTodos();
                gvRequisitos.DataBind();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.Write("<h2>Error en RequisitosPuestos</h2>");
                Response.Write("<pre>" + Server.HtmlEncode(ex.ToString()) + "</pre>");
                Response.End();
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("~/EMP/RequisitoPuestoForm.aspx?u=" + usuario);
        }


        // Maneja el evento de comando del GridView para editar un requisito, para editar directo en el gridview con el evento RowEditing, se puede usar el evento RowCommand para redirigir a la página de edición con el ID del requisito seleccionado.
        protected void gvRequisitos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                string usuario = Request.QueryString["u"];
                string id = e.CommandArgument.ToString();

                Response.Redirect("RequisitoPuestoForm.aspx?u=" + usuario + "&id=" + id);
            }
            else if (e.CommandName == "Eliminar")
            { 
                int id = Convert.ToInt32(e.CommandArgument);
                _bll.Eliminar(id);

                string usuario = Request.QueryString["u"];

                Response.Redirect($"RequisitosPuestos.aspx?u={usuario}&msg=eliminado");


                //CargarRequisitos(); ya no ocupo esto gracias al redirect

            }
        }// final del evento RowCommand

        protected void gvRequisitos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRequisitos.PageIndex = e.NewPageIndex;
            CargarRequisitos();
        }// final del evento PageIndexChanging


    }
}