using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.BLL.ModuloOferenteBLL;
using ADMExpedientePersonal.Entities;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace ADMExpedientePersonal.OFE
{
    public partial class AgendarEntrevista : System.Web.UI.Page
    {
        private EntrevistaBLL entrevistaBLL = new EntrevistaBLL();
        private OferenteBLL oferenteBLL = new OferenteBLL();

        private AuthBLL AuthBLL = new AuthBLL();
        private string usuario = "Desconocido"; // Variable para almacenar el nombre de usuario
        private int resultado;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CargarEntrevistas();
                }
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        public void reportarFallos(Exception mensaje)
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                entrevistaBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: ("Error" + mensaje));
                MostrarMensaje("Error inesperado: " + mensaje);
            }
            catch (Exception ex)
            {
                entrevistaBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: "Error no controlado: " + ex);
                MostrarMensaje("Error no controlado: " + ex);
            }
        }

        private void CargarEntrevistas()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                gvEntrevistas.DataSource = entrevistaBLL.ObtenerEntrevistas(usuario);
                gvEntrevistas.DataBind();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void gvEntrevistas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvEntrevistas.PageIndex = e.NewPageIndex;
                CargarEntrevistas(); // vuelve a enlazar los datos al GridView
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            litMensajeModal.Text = mensaje;

            // Actualiza SOLO el panel del modal
            upModalMensaje.Update();

            // Lanza el script para mostrar el modal
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }



        protected void gvEntrevistas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                hfEntrevistaId.Value = e.CommandArgument.ToString();
                if (e.CommandName == "Editar")
                {
                    lblMensajeError.Text = "";
                    AbrirModalEditar();
                }
                else if (e.CommandName == "Eliminar")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEliminar", "showEliminarModal();", true);
                }
                else if (e.CommandName == "CambiarEstado")
                {
                    cambiarEstado();
                }
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        private void cambiarEstado()
        {
            this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
            resultado = entrevistaBLL.CambiarEstadoEntrevista(int.Parse(hfEntrevistaId.Value), usuario);
            if (resultado != 1)
            {
                MostrarMensaje("Estado ya cambiado o falló al cambiarlo");
                return;
            }
            CargarEntrevistas();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            lblMensajeError.Text = "";
            hfAccion.Value = "0";
            txtEntrevistaId.Text = "0";
            txtFechaEntrevista.Text = "";

            ddlEmpleados.DataSource = entrevistaBLL.ObtenerNombreEmpleados(); 
            ddlEmpleados.DataBind();
            ddlOferentes.DataSource = oferenteBLL.ObtenerNombreOferentes();
            ddlOferentes.DataBind();

            ddlOferentes.Enabled = true; // Permitir editar el campo de identificación del oferente al crear una nueva entrevista

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        private void AbrirModalEditar()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                hfAccion.Value = "1";
                int EntrevistaId = int.Parse(hfEntrevistaId.Value);
                var entrevista = entrevistaBLL.ObtenerEntrevista(usuario, EntrevistaId);
                ddlOferentes.Enabled = false; // No permitir editar el campo de identificación del oferente al editar una entrevista existente
                if (entrevista != null)
                {
                    ddlEmpleados.DataSource = entrevistaBLL.ObtenerNombreEmpleados();
                    ddlEmpleados.DataBind();
                    ddlOferentes.DataSource = oferenteBLL.ObtenerNombreOferentes();
                    ddlOferentes.DataBind();

                    // Campos simples
                    txtEntrevistaId.Text = entrevista.EntrevistaId.ToString();
                    ddlOferentes.Text = entrevista.OferenteIdentificacion;
                    ddlEmpleados.SelectedValue = entrevista.EmpleadoId.ToString();
                    txtFechaEntrevista.Text = entrevista.FechaEntrevista.ToString("yyyy-MM-dd");
                }
                else
                {
                    MostrarMensaje("No se encontró la entrevista seleccionada.");
                    return;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void btnGuardarEntrevista_Click(object sender, EventArgs e)
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                int Accion = Convert.ToInt32(hfAccion.Value);

                if (string.IsNullOrEmpty(txtEntrevistaId.Text) || ddlEmpleados.SelectedIndex == -1
                    || ddlOferentes.SelectedIndex == -1 || string.IsNullOrEmpty(txtFechaEntrevista.Text))
                {
                    lblMensajeError.Text = "Todos los campos son requeridos.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                if (!DateTime.TryParse(txtFechaEntrevista.Text, out _))
                {
                    lblMensajeError.Text = "La fecha de entrevista no es válida.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }

                var entrevista = new Entrevista
                {
                    EntrevistaId = Accion == 0 ? 0 : int.Parse(txtEntrevistaId.Text),
                    EmpleadoId = int.Parse(ddlEmpleados.SelectedValue),
                    OferenteIdentificacion = ddlOferentes.SelectedValue,
                    FechaEntrevista = DateTime.Parse(txtFechaEntrevista.Text)
                };

                if (Accion == 0) // Nuevo
                {
                    this.resultado = entrevistaBLL.InsertarEntrevista(entrevista, usuario);
                    if (resultado == 2)
                    {
                        lblMensajeError.Text = "Verifica todos los espacios y datos ingresados.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado == 3)
                    {
                        lblMensajeError.Text = "La fecha de la entrevista no puede ser anterior a la fecha actual.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado == 1)
                    {
                        MostrarMensaje("La entrevista ha sido registrada correctamente.");
                    }
                    else
                    {
                        lblMensajeError.Text = "Error desconocido al registrar la entrevista";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }
                else // Editar
                {
                    this.resultado = entrevistaBLL.ModificarEntrevista(entrevista, usuario);
                    if (resultado == 2)
                    {
                        lblMensajeError.Text = "Verifica todos los espacios y datos ingresados.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado == 3)
                    {
                        lblMensajeError.Text = "La fecha de la entrevista no puede ser anterior a la fecha actual.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado != 1 && resultado != 2 && resultado != 3)
                    {
                        lblMensajeError.Text = "Error desconocido al modificar la entrevista";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }
                CargarEntrevistas();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                string entrevistaId = hfEntrevistaId.Value.ToString();

                if (string.IsNullOrEmpty(entrevistaId))
                {
                    MostrarMensaje("No se reconocio el id de la entrevista");
                    return;
                }

                int resultado = entrevistaBLL.EliminarEntrevista(int.Parse(entrevistaId), usuario);
                if (resultado == 1)
                {
                    MostrarMensaje("Eliminado correctamente");
                }
                else
                {
                    MostrarMensaje("No se ha eliminado");
                }
                CargarEntrevistas();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
            finally
            {
                // Cerrar el modal
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEliminar", "hideEliminarModal();", true);
            }
        }
    }
}