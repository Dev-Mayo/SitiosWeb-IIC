using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.GEN
{
    public partial class AdminCompanias : System.Web.UI.Page
    {
        private readonly CompaniaBLL companiaBLL = new CompaniaBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/SEG/Login.aspx?msg=login");
                return;
            }

            if (!IsPostBack)
            {
                CargarCompanias();
            }
        }

        private void CargarCompanias()
        {
            gvCompanias.DataSource = companiaBLL.Listar();
            gvCompanias.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            hfCodigoCompania.Value = "";
            txtCodigo.Enabled = true;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowCompania", "showCompaniaModal();", true);
        }

        protected void gvCompanias_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompanias.PageIndex = e.NewPageIndex;
            CargarCompanias();
        }

        protected void gvCompanias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string codigo = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                AbrirEditar(codigo);
            }
            else if (e.CommandName == "Eliminar")
            {
                hfCodigoCompania.Value = codigo;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
            }
        }

        private void AbrirEditar(string codigo)
        {
            LimpiarFormulario();

            var compania = companiaBLL.Obtener(codigo);

            if (compania == null)
            {
                MostrarMensaje("No se encontró la compañía seleccionada.");
                return;
            }

            hfCodigoCompania.Value = compania.codigo_compania;
            txtCodigo.Text = compania.codigo_compania;
            txtNombre.Text = compania.nombre;

            txtCodigo.Enabled = false;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowCompania", "showCompaniaModal();", true);
        }

        protected void btnGuardarCompania_Click(object sender, EventArgs e)
        {
            try
            {
                var compania = new Compania
                {
                    codigo_compania = txtCodigo.Text.Trim(),
                    nombre = txtNombre.Text.Trim()
                };

                if (string.IsNullOrEmpty(hfCodigoCompania.Value))
                {
                    companiaBLL.InsertarCompania(compania);
                    MostrarMensaje("Compañía registrada correctamente.");
                }
                else
                {
                    companiaBLL.ActualizarCompania(compania);
                    MostrarMensaje("Compañía actualizada correctamente.");
                }

                CargarCompanias();

                ScriptManager.RegisterStartupScript(this, GetType(), "HideCompania", "hideCompaniaModal();", true);
            }
            catch (Exception ex)
            {
                lblMensajeError.Text = ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowCompania", "showCompaniaModal();", true);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = hfCodigoCompania.Value;

                companiaBLL.EliminarCompania(codigo);

                CargarCompanias();

                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);

                MostrarMensaje("Compañía eliminada correctamente.");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
                MostrarMensaje(ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            lblMensajeError.Text = "";
        }

        private void MostrarMensaje(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }
    }
}