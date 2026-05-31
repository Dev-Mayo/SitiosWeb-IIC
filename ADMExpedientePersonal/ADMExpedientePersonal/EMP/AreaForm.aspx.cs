using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;
using System;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal.EMP
{
    public partial class AreaForm : System.Web.UI.Page
    {
        private AreaBLL areaBLL = new AreaBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarJefaturas();

                if (Request.QueryString["id"] != null)
                {
                    CargarArea();
                }
            }
        }

        private void CargarJefaturas()
        {
            ddlJefatura.DataSource = areaBLL.ObtenerJefaturas();
            ddlJefatura.DataTextField = "nombre_completo";
            ddlJefatura.DataValueField = "empleado_id";
            ddlJefatura.DataBind();

            ddlJefatura.Items.Insert(0, new ListItem("-- Seleccione una jefatura --", "0"));
        }

        private void CargarArea()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            Area area = areaBLL.ObtenerPorId(id);

            if (area != null)
            {
                txtCodigoArea.Text = area.codigo_area.ToString();
                txtCodigoArea.Enabled = false;

                txtNombre.Text = area.nombre;
                ddlJefatura.SelectedValue = area.jefatura.ToString();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Area area = new Area
                {
                    codigo_area = Convert.ToInt32(txtCodigoArea.Text),
                    nombre = txtNombre.Text.Trim(),
                    jefatura = Convert.ToInt32(ddlJefatura.SelectedValue)
                };

                if (Request.QueryString["id"] != null)
                {
                    areaBLL.Actualizar(area);
                    Session["Mensaje"] = "El área ha sido actualizada correctamente.";
                }
                else
                {
                    areaBLL.Insertar(area);
                    Session["Mensaje"] = "El área ha sido registrada correctamente.";
                }

                string usuario = Request.QueryString["u"];
                Response.Redirect("Areas.aspx?u=" + usuario);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("Areas.aspx?u=" + usuario);
        }
    }
}