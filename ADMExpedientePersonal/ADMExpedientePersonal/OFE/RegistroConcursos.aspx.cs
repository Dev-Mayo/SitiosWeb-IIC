using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.OFE
{
    public partial class RegistroConcursos : System.Web.UI.Page
    {
        private readonly ConcursoBLL concursoBLL = new ConcursoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/SEG/Login.aspx?msg=login");
                return;
            }

            if (!IsPostBack)
            {
                CargarConcursos();
            }
        }

        private void CargarConcursos()
        {
            gvConcursos.DataSource = concursoBLL.Listar();
            gvConcursos.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            hfCodigoConcurso.Value = "";
            txtCodigo.Enabled = true;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowConcurso", "showConcursoModal();", true);
        }

        protected void gvConcursos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvConcursos.PageIndex = e.NewPageIndex;
            CargarConcursos();
        }

        protected void gvConcursos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string codigo = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                AbrirEditar(codigo);
            }
            else if (e.CommandName == "Eliminar")
            {
                hfCodigoConcurso.Value = codigo;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
            }
            else if (e.CommandName == "Estado")
            {
                try
                {
                    concursoBLL.CambiarEstadoConcurso(codigo);
                    CargarConcursos();
                    MostrarMensaje("Estado del concurso actualizado correctamente.");
                }
                catch (Exception ex)
                {
                    MostrarMensaje(ex.Message);
                }
            }
        }

        private void AbrirEditar(string codigo)
        {
            LimpiarFormulario();

            var concurso = concursoBLL.Obtener(codigo);

            if (concurso == null)
            {
                MostrarMensaje("No se encontró el concurso seleccionado.");
                return;
            }

            hfCodigoConcurso.Value = concurso.codigo_concurso;
            txtCodigo.Text = concurso.codigo_concurso;
            txtNombre.Text = concurso.nombre;
            txtFechaInicio.Text = concurso.fecha_inicio.ToString("yyyy-MM-dd");
            txtFechaFin.Text = concurso.fecha_fin.ToString("yyyy-MM-dd");

            txtCodigo.Enabled = false;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowConcurso", "showConcursoModal();", true);
        }

        protected void btnGuardarConcurso_Click(object sender, EventArgs e)
        {
            try
            {
                var concurso = new Concurso
                {
                    codigo_concurso = txtCodigo.Text.Trim(),
                    nombre = txtNombre.Text.Trim(),
                    fecha_inicio = DateTime.Parse(txtFechaInicio.Text),
                    fecha_fin = DateTime.Parse(txtFechaFin.Text),
                    estado = "Vigente"
                };

                if (string.IsNullOrEmpty(hfCodigoConcurso.Value))
                {
                    concursoBLL.InsertarConcurso(concurso);
                    MostrarMensaje("Concurso registrado correctamente.");
                }
                else
                {
                    var concursoActual = concursoBLL.Obtener(hfCodigoConcurso.Value);
                    concurso.estado = concursoActual != null ? concursoActual.estado : "Vigente";

                    concursoBLL.ActualizarConcurso(concurso);
                    MostrarMensaje("Concurso actualizado correctamente.");
                }

                CargarConcursos();

                ScriptManager.RegisterStartupScript(this, GetType(), "HideConcurso", "hideConcursoModal();", true);
            }
            catch (Exception ex)
            {
                lblMensajeError.Text = ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowConcurso", "showConcursoModal();", true);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = hfCodigoConcurso.Value;

                concursoBLL.EliminarConcurso(codigo);

                CargarConcursos();

                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);

                MostrarMensaje("Concurso eliminado correctamente.");
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
            txtFechaInicio.Text = "";
            txtFechaFin.Text = "";
            lblMensajeError.Text = "";
        }

        private void MostrarMensaje(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }
    }
}