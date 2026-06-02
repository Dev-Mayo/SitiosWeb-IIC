using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ADMExpedientePersonal.GEN
{
    public partial class AdminInsEducativas : System.Web.UI.Page
    {
        private InstEducativaBLL bll = new InstEducativaBLL();
        private AuthBLL authBLL = new AuthBLL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    CargarInstituciones();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        private void CargarInstituciones()
        {
            string username = Request.QueryString["u"];
            var usuario = authBLL.ObtenerUsuarioPorNombre(username);
            gvInstituciones.DataSource = bll.ObtenerInstituciones(usuario.nombre_completo);
            gvInstituciones.DataBind();
        }

        protected void gvInstituciones_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            try
            {
                gvInstituciones.PageIndex = e.NewPageIndex;
                CargarInstituciones();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            hfAccion.Value = "0";
            hfCodigo.Value = "";
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtCodigo.Enabled = true;
            lblMensajeError.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void gvInstituciones_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            try
            {
                hfCodigo.Value = e.CommandArgument.ToString();

                if (e.CommandName == "Editar")
                {
                    hfAccion.Value = "1";
                    var inst = bll.ObtenerPorCodigo(hfCodigo.Value);
                    if (inst != null)
                    {
                        txtCodigo.Text = inst.codigo_institucion;
                        txtNombre.Text = inst.nombre;
                        txtCodigo.Enabled = false;
                        lblMensajeError.Text = "";
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
                else if (e.CommandName == "Eliminar")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = txtCodigo.Text.Trim();
                string nombre = txtNombre.Text.Trim();

                if (!Validar(codigo, nombre)) return;

                string username = Request.QueryString["u"];
                var usuario = authBLL.ObtenerUsuarioPorNombre(username);

                if (hfAccion.Value == "0") // Nuevo
                {
                    if (bll.ValidarDuplicado(codigo, nombre))
                    {
                        lblMensajeError.Text = "El código o nombre ya existe.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    bll.Insertar(codigo, nombre, usuario.nombre_completo);
                }
                else // Editar
                {
                    if (bll.ValidarDuplicado(codigo, nombre, codigo))
                    {
                        lblMensajeError.Text = "El nombre ya existe en otra institución.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    bll.Actualizar(codigo, nombre, usuario.nombre_completo);
                }

                CargarInstituciones();
                ScriptManager.RegisterStartupScript(this, GetType(), "HideModal", "hideModal();", true);
                MostrarMensajeExito("Institución guardada correctamente.");
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al guardar: " + ex.Message);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = hfCodigo.Value;
                string username = Request.QueryString["u"];
                var usuario = authBLL.ObtenerUsuarioPorNombre(username);

                bll.Eliminar(codigo, usuario.nombre_completo);
                CargarInstituciones();
                MostrarMensajeExito("Institución eliminada correctamente.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No se puede eliminar"))
                    MostrarError(ex.Message);
                else
                {
                    RegistrarError(ex);
                    MostrarError("Error al eliminar: " + ex.Message);
                }
                CargarInstituciones();
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
            }
        }


        private bool Validar(string codigo, string nombre)
        {
            Regex regexSoloLetras = new Regex(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$");

            if (string.IsNullOrEmpty(codigo))
            { lblMensajeError.Text = "El código es requerido."; ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true); return false; }

            if (string.IsNullOrEmpty(nombre))
            { lblMensajeError.Text = "El nombre es requerido."; ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true); return false; }

            if (nombre.Length > 150)
            { lblMensajeError.Text = "El nombre no puede superar los 150 caracteres."; ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true); return false; }

            if (!regexSoloLetras.IsMatch(nombre))
            { lblMensajeError.Text = "El nombre solo puede contener letras."; ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true); return false; }

            return true;
        }

        private void MostrarError(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }

        private void MostrarMensajeExito(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }

        private void RegistrarError(Exception ex)
        {
            bitacoraBLL.RegistrarBitacora(new Bitacora
            {
                Usuario = Request.QueryString["u"],
                Accion = AccionBitacora.ERROR,
                DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
            });
        }
    }
}