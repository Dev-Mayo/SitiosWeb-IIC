using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class AdminPuestos : System.Web.UI.Page
    {
        private readonly PuestoBLL puestoBLL = new PuestoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/SEG/Login.aspx?msg=login");
                return;
            }

            if (!IsPostBack)
            {
                CargarPuestos();
            }
        }

        private void CargarPuestos()
        {
            gvPuestos.DataSource = puestoBLL.Listar();
            gvPuestos.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            hfPuestoId.Value = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowPuesto", "showPuestoModal();", true);
        }

        protected void gvPuestos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPuestos.PageIndex = e.NewPageIndex;
            CargarPuestos();
        }

        protected void gvPuestos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int puestoId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                AbrirEditar(puestoId);
            }
            else if (e.CommandName == "Eliminar")
            {
                hfPuestoId.Value = puestoId.ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
            }
        }

        private void AbrirEditar(int puestoId)
        {
            LimpiarFormulario();

            var puesto = puestoBLL.Obtener(puestoId);

            if (puesto == null)
            {
                MostrarMensaje("No se encontró el puesto seleccionado.");
                return;
            }

            hfPuestoId.Value = puesto.puesto_id.ToString();
            txtNombre.Text = puesto.nombre;
            txtSalario.Text = puesto.salario.ToString("0.00");
            txtJefePuestoId.Text = puesto.jefe_puesto_id.HasValue
                ? puesto.jefe_puesto_id.Value.ToString()
                : "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowPuesto", "showPuestoModal();", true);
        }

        protected void btnGuardarPuesto_Click(object sender, EventArgs e)
        {
            try
            {
                decimal salario;

                if (!decimal.TryParse(txtSalario.Text.Trim(), out salario))
                    throw new Exception("Debe ingresar un salario válido.");

                int? jefePuestoId = null;

                if (!string.IsNullOrWhiteSpace(txtJefePuestoId.Text))
                    jefePuestoId = Convert.ToInt32(txtJefePuestoId.Text.Trim());

                var puesto = new Puesto
                {
                    nombre = txtNombre.Text.Trim(),
                    salario = salario,
                    jefe_puesto_id = jefePuestoId
                };

                if (string.IsNullOrEmpty(hfPuestoId.Value))
                {
                    puestoBLL.InsertarPuesto(puesto);
                    MostrarMensaje("Puesto registrado correctamente.");
                }
                else
                {
                    puesto.puesto_id = Convert.ToInt32(hfPuestoId.Value);
                    puestoBLL.ActualizarPuesto(puesto);
                    MostrarMensaje("Puesto actualizado correctamente.");
                }

                CargarPuestos();

                ScriptManager.RegisterStartupScript(this, GetType(), "HidePuesto", "hidePuestoModal();", true);
            }
            catch (Exception ex)
            {
                lblMensajeError.Text = ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowPuesto", "showPuestoModal();", true);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int puestoId = Convert.ToInt32(hfPuestoId.Value);

                puestoBLL.EliminarPuesto(puestoId);

                CargarPuestos();

                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);

                MostrarMensaje("Puesto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
                MostrarMensaje(ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtSalario.Text = "";
            txtJefePuestoId.Text = "";
            lblMensajeError.Text = "";
        }

        private void MostrarMensaje(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }
    }
}