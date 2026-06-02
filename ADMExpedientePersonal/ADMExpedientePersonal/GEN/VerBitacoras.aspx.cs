using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Web.UI;

namespace ADMExpedientePersonal.GEN
{
    public partial class VerBitacoras : System.Web.UI.Page
    {
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    CargarBitacoras();
            }
            catch (Exception ex)
            {
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        private void CargarBitacoras()
        {
            try
            {
                string username = Request.QueryString["u"];
                AuthBLL authBLL = new AuthBLL();
                var usuarioActual = authBLL.ObtenerUsuarioPorNombre(username);
                string nombreCompleto = usuarioActual?.nombre_completo ?? username;

                
                string filtroUsr = txtFiltroUsuario.Text.Trim();
                string filtroDesc = txtFiltroDescripcion.Text.Trim();
                string orden = ddlOrden.SelectedValue;

                gvBitacoras.DataSource = bitacoraBLL.ObtenerBitacoras(
                    nombreCompleto, filtroUsr, filtroDesc, orden);
                gvBitacoras.DataBind();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar bitácoras: " + ex.Message);
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {
                gvBitacoras.PageIndex = 0;
                CargarBitacoras();
            }
            catch (Exception ex)
            {
                MostrarError("Error al filtrar: " + ex.Message);
            }
        }

        protected void gvBitacoras_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            try
            {
                gvBitacoras.PageIndex = e.NewPageIndex;
                CargarBitacoras();
            }
            catch (Exception ex)
            {
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }
    }
}