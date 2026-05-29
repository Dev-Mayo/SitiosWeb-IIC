using ADMExpedientePersonal.BLL;
using System;

namespace ADMExpedientePersonal.EMP
{
    public partial class RequisitosPuestos : System.Web.UI.Page
    {
        private RequisitoPuestoBLL requisitoBLL = new RequisitoPuestoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarRequisitos();
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
    }
}