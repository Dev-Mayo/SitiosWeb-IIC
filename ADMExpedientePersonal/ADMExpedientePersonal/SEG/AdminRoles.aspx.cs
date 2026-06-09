using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.SEG
{
    public partial class AdminRoles : System.Web.UI.Page
    {
        private AdminRolBLL adminRolBLL = new AdminRolBLL();
        private AuthBLL AuthBLL = new AuthBLL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    CargarRoles();
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }

        private void CargarRoles()
        {
            try
            {
                string username = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                gvRoles.DataSource = adminRolBLL.ObtenerRoles(username);
                gvRoles.DataBind();
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }

        protected void gvRoles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRoles.PageIndex = e.NewPageIndex;
                CargarRoles(); // vuelve a enlazar los datos al GridView
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }


        private void MostrarError(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }


        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfIdRol.Value = e.CommandArgument.ToString();
                if (e.CommandName == "Editar")
                {
                    lblMensajeError.Text = "";
                    AbrirModalEditar();
                }
                else if (e.CommandName == "Eliminar")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
                }
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            lblMensajeError.Text = "";
            hfAccion.Value = "0";
            txtRolNombre.Text = "";
            txtIDRol.Text = "Automatico";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        private void AbrirModalEditar()
        {
            try
            {
                hfAccion.Value = "1";
                int id_rol = int.Parse(hfIdRol.Value);
                var rol = adminRolBLL.ObtenerRol(id_rol: id_rol);
                if (rol != null)
                {
                    txtRolNombre.Text = rol.nombre_rol;
                    txtIDRol.Text = rol.id_rol.ToString();
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }


        protected void btnGuardarRol_Click(object sender, EventArgs e)
        {
            try
            {
                int Accion = Convert.ToInt32(hfAccion.Value);
                string nombre_rol = txtRolNombre.Text;

                if (adminRolBLL.ValidarDuplicados(nombre_rol))
                {
                    lblMensajeError.Text = "El nombre del rol ya existe.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }

                // Expresión regular: solo letras (incluyendo acentos y ñ) y espacios
                Regex regex = new Regex("^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$");
                if (string.IsNullOrEmpty(nombre_rol))
                {
                    lblMensajeError.Text = "El nombre del rol es requerido.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                else if (!regex.IsMatch(nombre_rol))
                {
                    lblMensajeError.Text = "Solo se permiten letras y espacios.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                else if (nombre_rol.Length > 40)
                {
                    lblMensajeError.Text = "El nombre del rol no puede superar los 40 caracteres.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                else
                {
                    if (Accion == 0) //crear
                    {
                        string username = Request.QueryString["u"];
                        var usuario = AuthBLL.ObtenerUsuarioPorNombre(username);
                        adminRolBLL.InsertarRol(nombre_rol, usuario.nombre_completo);
                    }
                    else // editar
                    {
                        int id_rol = int.Parse(hfIdRol.Value);
                        string username = Request.QueryString["u"];
                        var usuario = AuthBLL.ObtenerUsuarioPorNombre(username);
                        adminRolBLL.ActualizarRol(id_rol, nombre_rol, usuario.nombre_completo);
                    }
                    CargarRoles();

                    ScriptManager.RegisterStartupScript(this, GetType(), "HideModal", "hideModal();", true);
                }
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(hfIdRol.Value);
                string username = Request.QueryString["u"];
                var usuario = AuthBLL.ObtenerUsuarioPorNombre(username);

                int resultado = adminRolBLL.eliminarRol(id, usuario.nombre_completo);
                if (resultado == 2)
                {
                    MostrarError("Rol eliminado correctamente.");
                }
                else if (resultado == 0)
                {
                    MostrarError("No se puede eliminar un registro con datos relacionados.");
                }
                else
                {
                    MostrarError("No se ha eliminado");
                }
                CargarRoles();
            }
            catch (Exception ex)
            {
                bitacoraBLL.RegistrarBitacora(new Bitacora
                {
                    Usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo,
                    Accion = AccionBitacora.ERROR,
                    DescripcionJson = bitacoraBLL.CrearJsonError("Error inesperado: " + ex)
                });
                MostrarError("Error inesperado: " + ex);
            }
            finally
            {
                // Cerrar el modal
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
            }
        }
    }
}