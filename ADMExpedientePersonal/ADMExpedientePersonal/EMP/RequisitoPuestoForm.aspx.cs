using System;
using ADMExpedientePersonal.BLL;
using ADMExpedientePersonal.Entities;

namespace ADMExpedientePersonal.EMP
{
    public partial class RequisitoPuestoForm : System.Web.UI.Page
    {
        private readonly RequisitoPuestoBLL _bll = new RequisitoPuestoBLL();
        private int IdRequisito
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        private bool EsEdicion
        {
            get { return IdRequisito > 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (EsEdicion)
                {
                    CargarRequisito();
                }
            }
        }

        private void CargarRequisito()
        {
            try
            {
                RequisitoPuesto requisito = _bll.ObtenerPorId(IdRequisito);

                if (requisito == null)
                {
                    lblMensaje.Text = "No se encontró el requisito seleccionado.";
                    return;
                }

                txtNombre.Text = requisito.nombre;
            }
            catch (Exception)
            {
                lblMensaje.Text = "Ocurrió un error al cargar el requisito.";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    lblMensaje.Text = "Debe ingresar el nombre del requisito.";
                    return;
                }

                RequisitoPuesto requisito = new RequisitoPuesto
                {
                    nombre = nombre
                };

                string usuario = Request.QueryString["u"];

                if (EsEdicion)
                {
                    requisito.requisito_id = IdRequisito;
                    _bll.Actualizar(requisito);

                    Response.Redirect($"RequisitosPuestos.aspx?u={usuario}&msg=actualizado");
                }
                else
                {
                    _bll.Insertar(requisito);

                    Response.Redirect($"RequisitosPuestos.aspx?u={usuario}&msg=guardado"); // el redirect me ayuda a evitar que se duplique el registro si el usuario refresca la página después de guardar
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = ex.Message;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            string usuario = Request.QueryString["u"];
            Response.Redirect("RequisitosPuestos.aspx?u=" + usuario);
        }
    }
}