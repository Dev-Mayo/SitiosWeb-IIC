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
            gvRequisitos.DataSource = requisitoBLL.ObtenerTodos();
            gvRequisitos.DataBind();
        }
    }
}