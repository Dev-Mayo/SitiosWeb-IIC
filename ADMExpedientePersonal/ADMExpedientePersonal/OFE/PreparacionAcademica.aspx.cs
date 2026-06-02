using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.BLL.ModuloOferenteBLL;
using ADMExpedientePersonal.Entities;
using ADMExpedientePersonal.Entities.ModuloOferenteEntities;
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
    public partial class PreparacionAcademica : System.Web.UI.Page
    {
        private PrepAcademicaBLL preparacionAcadBLL = new PrepAcademicaBLL();
        private AuthBLL AuthBLL = new AuthBLL();
        private string usuario = "Desconocido"; // Variable para almacenar el nombre de usuario
        private int resultado;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CargarPreparacionAcademica();
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
                preparacionAcadBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: ("Error" + mensaje));
                MostrarMensaje("Error inesperado: " + mensaje);
            }
            catch (Exception ex)
            {
                preparacionAcadBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: "Error no controlado: " + ex);
                MostrarMensaje("Error no controlado: " + ex);
            }
        }

        private void CargarPreparacionAcademica()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                gvPreparacionAcad.DataSource = preparacionAcadBLL.ObtenerPreparacionAcad(Request.QueryString["id"], usuario);
                gvPreparacionAcad.DataBind();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void gvPreparacionAcad_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvPreparacionAcad.PageIndex = e.NewPageIndex;
                CargarPreparacionAcademica(); // vuelve a enlazar los datos al GridView
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



        protected void gvPreparacionAcad_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                hfPreparacionAcadId.Value = e.CommandArgument.ToString();
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
                reportarFallos(ex);
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            lblMensajeError.Text = "";
            hfAccion.Value = "0";
            txtOferente.Text = Request.QueryString["id"];
            txtPreparacionAcadId.Text = "0";
            txtTitulo.Text = "";

            ddlInstituciones.DataSource = preparacionAcadBLL.ObtenerInstituciones(usuario); 
            ddlInstituciones.DataBind();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        private void AbrirModalEditar()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                hfAccion.Value = "1";
                int prepAcadId = int.Parse(hfPreparacionAcadId.Value);
                var preparacionAcad = preparacionAcadBLL.ObtenerPreparacionAcadPorId(prepAcadId, usuario);
                lblMensajeError.Text = "";

                if (preparacionAcad != null)
                {                
                    txtPreparacionAcadId.Text = preparacionAcad.Id.ToString();
                    txtOferente.Text = preparacionAcad.OferenteId;
                    txtTitulo.Text = preparacionAcad.Titulo;
                    txtFechaInicio.Text = preparacionAcad.FechaInicio.ToString("yyyy-MM-dd");
                    txtFechaFin.Text = preparacionAcad.FechaFin.ToString("yyyy-MM-dd");

                    ddlInstituciones.DataSource = preparacionAcadBLL.ObtenerInstituciones(usuario);

                    ddlInstituciones.SelectedValue = preparacionAcad.CodigoInstitucion; 
                    ddlInstituciones.DataBind();
                }
                else
                {
                    MostrarMensaje("No se encontró la preparación académica seleccionada.");
                    return;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void btnGuardarPreparacionAcad_Click(object sender, EventArgs e)
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                int Accion = Convert.ToInt32(hfAccion.Value);

                if (string.IsNullOrEmpty(txtPreparacionAcadId.Text) || ddlInstituciones.SelectedIndex == -1
                    || string.IsNullOrEmpty(txtTitulo.Text) || string.IsNullOrEmpty(txtFechaInicio.Text) || string.IsNullOrEmpty(txtFechaFin.Text))
                {
                    lblMensajeError.Text = "Todos los campos son requeridos.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                if (!DateTime.TryParse(txtFechaInicio.Text, out _) || !DateTime.TryParse(txtFechaFin.Text, out _))
                {
                    lblMensajeError.Text = "La fecha de inicio o fin no es válida.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                if (txtTitulo.Text.Length > 100 || !Regex.IsMatch(txtTitulo.Text, @"^[a-zA-Z\s]+$"))
                {
                    lblMensajeError.Text = "El título no puede superar los 100 caracteres y solo permite letras y espacios.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }


                var prepAcad = new PreparacionAcad
                {
                    Id = int.Parse(txtPreparacionAcadId.Text),
                    CodigoInstitucion = ddlInstituciones.SelectedValue,
                    Titulo = txtTitulo.Text,
                    OferenteId = txtOferente.Text,
                    FechaInicio = DateTime.Parse(txtFechaInicio.Text),
                    FechaFin = DateTime.Parse(txtFechaFin.Text)
                };

                if (Accion == 0) // Nuevo
                {
                    this.resultado = preparacionAcadBLL.CrearPreparacionAcad(prepAcad, usuario);
                }
                else // Editar
                {
                    this.resultado = preparacionAcadBLL.ModificarPreparacionAcad(prepAcad, usuario);
                }

                if (resultado == 2)
                {
                    lblMensajeError.Text = "Verifica todos los espacios y datos ingresados.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                else if (resultado == 3)
                {
                    lblMensajeError.Text = "Error en las fechas, recuerde que la fecha incial debe ser menor a la final";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                else if (resultado == 4)
                {
                    lblMensajeError.Text = "El titulo no puede superar los 100 caracteres y solo permite letras y espacios";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                if (resultado == 1)
                {
                    MostrarMensaje(Accion == 0 ? "Registrado correctamente" : "Modificado correctamente");
                }
                else
                {
                    lblMensajeError.Text = "Error desconocido al registrar la preparación académica";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                CargarPreparacionAcademica();
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
                string preparacionAcadId = hfPreparacionAcadId.Value.ToString();
                if (string.IsNullOrEmpty(preparacionAcadId))
                {
                    MostrarMensaje("No se reconocio el id de la preparación académica");
                    return;
                }

                int resultado = preparacionAcadBLL.EliminarPreparacionAcad(int.Parse(preparacionAcadId), usuario);
                if (resultado == 1)
                {
                    MostrarMensaje("Eliminado correctamente");
                }
                else if (resultado == 2)
                {
                    MostrarMensaje("No se puede eliminar un \r\nregistro con datos relacionados.");
                }
                else
                {
                    MostrarMensaje("No se ha eliminado");
                }
                CargarPreparacionAcademica();
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