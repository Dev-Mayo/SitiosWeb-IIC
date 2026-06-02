using ADMExpedientePersonal.BLL;
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
    public partial class MainOferentes : System.Web.UI.Page
    {
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
                    CargarOferentes();
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
                oferenteBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: ("Error" + mensaje));
                MostrarMensaje("Error inesperado: " + mensaje);
            }
            catch (Exception ex)
            {
                oferenteBLL.GenericoCrearBitacora(usuario, 4, 1, detalles: "Error no controlado: " + ex);
                MostrarMensaje("Error no controlado: " + ex);
            }
        }

        private void CargarOferentes()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                gvOferentes.DataSource = oferenteBLL.ObtenerOferentes(usuario);
                gvOferentes.DataBind();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void gvOferentes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvOferentes.PageIndex = e.NewPageIndex;
                CargarOferentes(); // vuelve a enlazar los datos al GridView
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            litMensajeModal.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMensaje", "showMensajeModal();", true);
        }


        protected void gvOferentes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfIdOferente.Value = e.CommandArgument.ToString();
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
            txtIdentificacion.Text = "";
            txtNombreCompleto.Text = "";
            txtFechaNacimiento.Text = "";
            txtIdentificacion.Enabled = true;

            //Para los concursos, se cargan todos pero no se selecciona ninguno, ya que es un nuevo oferente
            CargarConcursos();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
        }

        protected void btnPrepAcademica_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/OFE/PreparacionAcademica.aspx?u={Request.QueryString["u"]}");
        }

        protected void btnExpLaboral_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/OFE/ExperienciaLaboral.aspx?u={Request.QueryString["u"]}");
        }

        private void AbrirModalEditar()
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                hfAccion.Value = "1";
                string identificacion = hfIdOferente.Value;
                txtIdentificacion.Enabled = false;
                var oferente = oferenteBLL.ObtenerOferente(usuario, identificacion);
                if (oferente != null)
                {
                    CargarOferente(oferente);
                }
                else
                {
                    MostrarMensaje("No se encontró el oferente seleccionado.");
                    return;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        public void CargarConcursos()
        {
            this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
            var concursos = oferenteBLL.ObtenerConcursos(usuario); // devuelve List<Concurso>
            chkConcursos.DataSource = concursos;
            chkConcursos.DataTextField = "nombre"; // lo que se muestra
            chkConcursos.DataValueField = "codigo_concurso"; // el ID que se guarda
            chkConcursos.DataBind();
        }

        private void CargarOferente(Oferente oferente)
        {
            // Campos simples
            txtIdentificacion.Text = oferente.identificacion;
            ddlTipoIdentificacion.SelectedValue = oferente.tipo_identificacion;
            txtNombreCompleto.Text = oferente.nombre_completo;
            txtFechaNacimiento.Text = oferente.fecha_nacimiento.ToString("yyyy-MM-dd");

            // Repeater: correos
            rptCorreos.DataSource = oferente.email;
            rptCorreos.DataBind();

            // Repeater: teléfonos
            rptTelefonos.DataSource = oferente.telefono;
            rptTelefonos.DataBind();


            // concursos
            CargarConcursos();

            // Activar los concursos que ya tiene el usuario
            foreach (var codigo in oferente.codigo_concurso)
            {
                var item = chkConcursos.Items.FindByValue(codigo);
                if (item != null) item.Selected = true;
            }
        }

        protected void ddlTipoIdentificacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlTipoIdentificacion.SelectedValue)
            {
                case "Cedula":
                    // cédula con 9 dígitos
                    revIdentificacion.ValidationExpression = @"^\d{9}$";
                    break;

                case "Dimex":
                    // DIMEX con 12 dígitos
                    revIdentificacion.ValidationExpression = @"^\d{12}$";
                    break;

                case "Pasaporte":
                    // pasaporte con letras y números, 6-12 caracteres
                    revIdentificacion.ValidationExpression = @"^[A-Za-z0-9]{6,9}$";
                    break;
            }
        }

        protected void btnGuardarOferente_Click(object sender, EventArgs e)
        {
            try
            {
                this.usuario = AuthBLL.ObtenerUsuarioPorNombre(Request.QueryString["u"]).nombre_completo;
                int Accion = Convert.ToInt32(hfAccion.Value);

                if (string.IsNullOrEmpty(txtIdentificacion.Text) || string.IsNullOrWhiteSpace(ddlTipoIdentificacion.Text) 
                    || string.IsNullOrEmpty(txtNombreCompleto.Text) || string.IsNullOrEmpty(txtFechaNacimiento.Text)
                    || rptCorreos.Items.Cast<RepeaterItem>().Any(item => string.IsNullOrWhiteSpace((item.FindControl("txtCorreo") as TextBox)?.Text))
                    || rptTelefonos.Items.Cast<RepeaterItem>().Any(item => string.IsNullOrWhiteSpace((item.FindControl("txtTelefono") as TextBox)?.Text))
                    || chkConcursos.Items.Cast<ListItem>().All(i => !i.Selected))
                {
                    lblMensajeError.Text = "Todos los campos son requeridos.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }

                var oferente = new Oferente
                {
                    identificacion = txtIdentificacion.Text.Trim(),
                    tipo_identificacion = ddlTipoIdentificacion.SelectedValue,
                    nombre_completo = txtNombreCompleto.Text.Trim(),
                    fecha_nacimiento = DateTime.Parse(txtFechaNacimiento.Text),
                    email = new List<string>(),
                    telefono = new List<string>(),
                    codigo_concurso = new List<string>()
                };

                // Correos
                foreach (RepeaterItem item in rptCorreos.Items)
                {
                    var txtCorreo = item.FindControl("txtCorreo") as TextBox;
                    if (txtCorreo != null && !string.IsNullOrWhiteSpace(txtCorreo.Text))
                        oferente.email.Add(txtCorreo.Text.Trim());
                }

                // Teléfonos
                foreach (RepeaterItem item in rptTelefonos.Items)
                {
                    var txtTelefono = item.FindControl("txtTelefono") as TextBox;
                    if (txtTelefono != null && !string.IsNullOrWhiteSpace(txtTelefono.Text))
                        oferente.telefono.Add(txtTelefono.Text.Trim());
                }

                // Concursos seleccionados
                oferente.codigo_concurso = chkConcursos.Items
                    .Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
                    .ToList();

                if (Accion == 0) // Nuevo
                {
                    this.resultado = oferenteBLL.InsertarOferente(oferente, usuario);
                    if (resultado == 4)
                    {
                        lblMensajeError.Text = "Todos los datos son obligatorios, ademas de una fecha y correo validos";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    if (resultado == 3)
                    {
                        lblMensajeError.Text = "Todos los datos son obligatorios";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado == 1)
                    {
                        MostrarMensaje("El oferente ha sido registrado correctamente.");
                    }
                    else
                    {
                        lblMensajeError.Text = "Error desconocido al registrar el oferente";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }
                else // Editar
                {
                    this.resultado = oferenteBLL.ActualizarOferente(oferente, usuario);
                    if (resultado == 4)
                    {
                        lblMensajeError.Text = "Todos los datos son obligatorios, ademas de una fecha y correo validos";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                    else if (resultado != 1 && resultado != 4)
                    {
                        lblMensajeError.Text = "Error desconocido al modificar el oferente";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }
                CargarOferentes();
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
                string identificacionOferente = hfIdOferente.Value.ToString();

                Oferente oferenteAntiguo = oferenteBLL.ObtenerOferente(usuario, identificacionOferente);
                if (oferenteAntiguo == null)
                {
                    MostrarMensaje("No se encontró el oferente seleccionado.");
                    return;
                }

                int resultado = oferenteBLL.EliminarOferente(oferenteAntiguo, usuario);
                if (resultado == 2)
                {
                    MostrarMensaje("No se puede eliminar un registro con datos relacionados.");
                }
                else if (resultado == 1)
                {
                    MostrarMensaje("Eliminado correctamente");
                }
                else
                {
                    MostrarMensaje("No se ha eliminado");
                }
                CargarOferentes();
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

        // Métodos para agregar dinámicamente campos en los Repeaters
        protected void btnAgregarCorreo_Click(object sender, EventArgs e)
        {
            try
            {
                var lista = new List<string>();
                foreach (RepeaterItem item in rptCorreos.Items)
                {
                    var txtCorreo = item.FindControl("txtCorreo") as TextBox;
                    if (txtCorreo != null) lista.Add(txtCorreo.Text.Trim());
                }
                lista.Add(""); // nuevo campo vacío
                rptCorreos.DataSource = lista;
                rptCorreos.DataBind();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void btnAgregarTelefono_Click(object sender, EventArgs e)
        {
            try
            {
                var lista = new List<string>();
                foreach (RepeaterItem item in rptTelefonos.Items)
                {
                    var txtTelefono = item.FindControl("txtTelefono") as TextBox;
                    if (txtTelefono != null) lista.Add(txtTelefono.Text.Trim());
                }
                lista.Add("");
                rptTelefonos.DataSource = lista;
                rptTelefonos.DataBind();
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void rptCorreos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EliminarCorreo")
                {
                    var lista = new List<string>();
                    foreach (RepeaterItem item in rptCorreos.Items)
                    {
                        var txtCorreo = item.FindControl("txtCorreo") as TextBox;
                        if (txtCorreo != null) lista.Add(txtCorreo.Text.Trim());
                    }
                    lista.RemoveAt(e.Item.ItemIndex);
                    rptCorreos.DataSource = lista;
                    rptCorreos.DataBind();
                }
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }

        protected void rptTelefonos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EliminarTelefono")
                {
                    var lista = new List<string>();
                    foreach (RepeaterItem item in rptTelefonos.Items)
                    {
                        var txtTelefono = item.FindControl("txtTelefono") as TextBox;
                        if (txtTelefono != null) lista.Add(txtTelefono.Text.Trim());
                    }
                    lista.RemoveAt(e.Item.ItemIndex);
                    rptTelefonos.DataSource = lista;
                    rptTelefonos.DataBind();
                }
            }
            catch (Exception ex)
            {
                reportarFallos(ex);
            }
        }
    }
}