using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class AccionPersonalForm : System.Web.UI.Page
    {
        private AccionPersonalBLL accionBLL = new AccionPersonalBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEmpleados();

                if (Request.QueryString["id"] != null)
                {
                    CargarAccion();
                }
            }
        }

        private void CargarEmpleados()
        {
            var empleados = accionBLL.ObtenerEmpleados();

            ddlEmpleado.DataSource = empleados;
            ddlEmpleado.DataTextField = "nombre_completo";
            ddlEmpleado.DataValueField = "empleado_id";
            ddlEmpleado.DataBind();
            ddlEmpleado.Items.Insert(0, new ListItem("-- Seleccione un empleado --", "0"));

            ddlJefatura.DataSource = empleados;
            ddlJefatura.DataTextField = "nombre_completo";
            ddlJefatura.DataValueField = "empleado_id";
            ddlJefatura.DataBind();
            ddlJefatura.Items.Insert(0, new ListItem("-- Seleccione una jefatura --", "0"));
        }

        private void CargarAccion()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            AccionPersonal accion = accionBLL.ObtenerPorId(id);

            if (accion != null)
            {
                txtCodigoAccion.Text = accion.codigo_accion.ToString();
                txtCodigoAccion.Enabled = false;

                txtFecha.Text = accion.fecha.ToString("yyyy-MM-dd");
                txtDescripcion.Text = accion.descripcion;
                ddlEmpleado.SelectedValue = accion.empleado_id.ToString();
                ddlJefatura.SelectedValue = accion.jefatura_id.ToString();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                AccionPersonal accion = new AccionPersonal
                {
                    codigo_accion = Convert.ToInt32(txtCodigoAccion.Text),
                    fecha = Convert.ToDateTime(txtFecha.Text),
                    descripcion = txtDescripcion.Text.Trim(),
                    empleado_id = Convert.ToInt32(ddlEmpleado.SelectedValue),
                    jefatura_id = Convert.ToInt32(ddlJefatura.SelectedValue)
                };

                if (Request.QueryString["id"] != null)
                {
                    accion.accion_id = Convert.ToInt32(Request.QueryString["id"]);
                    accionBLL.Actualizar(accion);
                    Session["Mensaje"] = "La acción de personal ha sido actualizada correctamente.";
                }
                else
                {
                    accionBLL.Insertar(accion);
                    Session["Mensaje"] = "La acción de personal ha sido registrada correctamente.";
                }

                string usuario = Request.QueryString["u"];
                Response.Redirect("AccionesPersonal.aspx?u=" + usuario);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("AccionesPersonal.aspx?u=" + usuario);
        }
    }
}