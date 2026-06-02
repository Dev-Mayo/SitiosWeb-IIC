using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.SEG
{
    public partial class AdminUsuarios : System.Web.UI.Page
    {
        private AdminUsuarioBLL adminUsuarioBLL = new AdminUsuarioBLL();
        private AuthBLL authBLL = new AuthBLL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CargarUsuarios();
                    CargarRoles();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                string username = Request.QueryString["u"];
                var usuario = authBLL.ObtenerUsuarioPorNombre(username);
                gvUsuarios.DataSource = adminUsuarioBLL.ObtenerUsuarios(usuario.nombre_completo);
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al cargar usuarios: " + ex.Message);
            }
        }

        private void CargarRoles()
        {
            try
            {
                var roles = adminUsuarioBLL.ObtenerRoles();
                cblRoles.DataSource = roles;
                cblRoles.DataTextField = "nombre_rol";
                cblRoles.DataValueField = "id_rol";
                cblRoles.DataBind();

                cblRolesEditar.DataSource = roles;
                cblRolesEditar.DataTextField = "nombre_rol";
                cblRolesEditar.DataValueField = "id_rol";
                cblRolesEditar.DataBind();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al cargar roles: " + ex.Message);
            }
        }

        protected void gvUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvUsuarios.PageIndex = e.NewPageIndex;
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormularioNuevo();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        private void LimpiarFormularioNuevo()
        {
            lblMensajeError.Text = "";
            txtUsername.Text = "";
            txtNombreCompleto.Text = "";
            txtCorreo.Text = "";
            txtPassword.Text = "";
            txtConfirmarPassword.Text = "";
            foreach (ListItem item in cblRoles.Items)
                item.Selected = false;
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                hfIdUsuario.Value = e.CommandArgument.ToString();

                if (e.CommandName == "Editar")
                {
                    lblMensajeErrorEditar.Text = "";
                    AbrirModalEditar();
                }
                else if (e.CommandName == "Eliminar")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
                }
                else if (e.CommandName == "Activar")
                {
                    CambiarEstado(Convert.ToInt32(e.CommandArgument), "Activo");
                }
                else if (e.CommandName == "Inactivar")
                {
                    CambiarEstado(Convert.ToInt32(e.CommandArgument), "Inactivo");
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error inesperado: " + ex.Message);
            }
        }

        private void AbrirModalEditar()
        {
            try
            {
                int idUsuario = int.Parse(hfIdUsuario.Value);
                var usuario = adminUsuarioBLL.ObtenerUsuarioPorId(idUsuario);

                if (usuario != null)
                {
                    txtUsernameEditar.Text = usuario.nombreusuario;
                    txtNombreCompletoEditar.Text = usuario.nombre_completo;
                    txtCorreoEditar.Text = usuario.correo;
                    txtPasswordEditar.Text = "";
                    txtConfirmarPasswordEditar.Text = "";
                    ddlEstadoEditar.SelectedValue = usuario.estado;

                    string[] rolesAsignados = (usuario.id_roles ?? "").Split(',');
                    foreach (ListItem item in cblRolesEditar.Items)
                        item.Selected = Array.Exists(rolesAsignados, r => r.Trim() == item.Value);
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModalEditar", "showModalEditar();", true);
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al abrir edición: " + ex.Message);
            }
        }

        private void CambiarEstado(int idUsuario, string nuevoEstado)
        {
            try
            {
                string username = Request.QueryString["u"];
                var usuarioActual = authBLL.ObtenerUsuarioPorNombre(username);
                adminUsuarioBLL.CambiarEstado(idUsuario, nuevoEstado, usuarioActual.nombre_completo);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al cambiar estado: " + ex.Message);
            }
        }

        protected void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeError.Text = "";

                string username = txtUsername.Text.Trim();
                string nombreCompleto = txtNombreCompleto.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string password = txtPassword.Text.Trim();
                string confirmar = txtConfirmarPassword.Text.Trim();
                string roles = ObtenerRolesSeleccionados(cblRoles);

                if (!ValidarCampos(username, nombreCompleto, correo, password, confirmar, roles, lblMensajeError, true))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }

                string usernameActual = Request.QueryString["u"];
                var usuarioActual = authBLL.ObtenerUsuarioPorNombre(usernameActual);
                adminUsuarioBLL.InsertarUsuario(username, nombreCompleto, correo, password, roles, usuarioActual.nombre_completo);

                CargarUsuarios();
                ScriptManager.RegisterStartupScript(this, GetType(), "HideModal", "hideModal();", true);
                MostrarMensajeExito("Usuario creado correctamente.");
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al guardar usuario: " + ex.Message);
            }
        }

        protected void btnGuardarEditar_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeErrorEditar.Text = "";

                int idUsuario = int.Parse(hfIdUsuario.Value);
                string username = txtUsernameEditar.Text.Trim();
                string nombreCompleto = txtNombreCompletoEditar.Text.Trim();
                string correo = txtCorreoEditar.Text.Trim();
                string password = txtPasswordEditar.Text.Trim();
                string confirmar = txtConfirmarPasswordEditar.Text.Trim();
                string estado = ddlEstadoEditar.SelectedValue;
                string roles = ObtenerRolesSeleccionados(cblRolesEditar);

                if (!ValidarCampos(username, nombreCompleto, correo, password, confirmar, roles, lblMensajeErrorEditar, password.Length > 0))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModalEditar", "showModalEditar();", true);
                    return;
                }

                string usernameActual = Request.QueryString["u"];
                var usuarioActual = authBLL.ObtenerUsuarioPorNombre(usernameActual);
                adminUsuarioBLL.ActualizarUsuario(idUsuario, username, nombreCompleto, correo, estado, roles, password, usuarioActual.nombre_completo);

                CargarUsuarios();
                ScriptManager.RegisterStartupScript(this, GetType(), "HideModalEditar", "hideModalEditar();", true);
                MostrarMensajeExito("Usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                MostrarError("Error al actualizar usuario: " + ex.Message);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int idUsuario = int.Parse(hfIdUsuario.Value);
                string username = Request.QueryString["u"];
                var usuarioActual = authBLL.ObtenerUsuarioPorNombre(username);

                int resultado = adminUsuarioBLL.EliminarUsuario(idUsuario, usuarioActual.nombre_completo);
                if (resultado == 2)
                    MostrarMensajeExito("Usuario eliminado correctamente.");
                else
                    MostrarError("No se pudo eliminar el usuario.");

                CargarUsuarios();
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("No se puede eliminar el usuario por tener registros relacionados"))
                    MostrarError(ex.Message);
                else
                {
                    RegistrarError(ex);
                    MostrarError("Error al eliminar usuario: " + ex.Message);
                }
                CargarUsuarios();
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
            }
        }


        private bool ValidarCampos(string username, string nombreCompleto, string correo,
            string password, string confirmar, string roles,
            Label lblError, bool validarPassword)
        {
            Regex regexCorreo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            Regex regexPassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (string.IsNullOrEmpty(username))
            { lblError.Text = "El nombre de usuario es requerido."; return false; }

            if (string.IsNullOrEmpty(nombreCompleto))
            { lblError.Text = "El nombre completo es requerido."; return false; }

            if (string.IsNullOrEmpty(correo) || !regexCorreo.IsMatch(correo))
            { lblError.Text = "Debe ingresar un correo válido."; return false; }

            if (string.IsNullOrEmpty(roles))
            { lblError.Text = "Debe seleccionar al menos un rol."; return false; }

            if (validarPassword)
            {
                if (string.IsNullOrEmpty(password))
                { lblError.Text = "La contraseña es requerida."; return false; }

                if (!regexPassword.IsMatch(password))
                { lblError.Text = "La contraseña debe tener mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales."; return false; }

                if (password != confirmar)
                { lblError.Text = "Las contraseñas no coinciden."; return false; }
            }

            return true;
        }

        private string ObtenerRolesSeleccionados(CheckBoxList cbl)
        {
            var seleccionados = new System.Collections.Generic.List<string>();
            foreach (ListItem item in cbl.Items)
                if (item.Selected) seleccionados.Add(item.Value);
            return string.Join(",", seleccionados);
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