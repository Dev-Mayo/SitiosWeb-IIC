using ADMExpedientePersonal.BLL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ADMExpedientePersonal
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string path = Request.Url.AbsolutePath.ToLower();
                if (!path.Contains("login.aspx"))
                {
                    string username = Request.QueryString["u"];
                    if (string.IsNullOrEmpty(username))
                    {
                        Response.Redirect("~/SEG/Login.aspx?msg=login");
                        return;
                    }

                    AuthBLL bll = new AuthBLL();
                    var usuario = bll.ObtenerUsuarioPorNombre(username);
                    if (usuario == null)
                    {
                        Response.Redirect("~/SEG/Login.aspx?msg=login");
                        return;
                    }

                    lblUsuario.Text = usuario.nombre_completo;
                    CargarMenu(usuario.id_usuario, username);
                }
            }
        }

        private void CargarMenu(int usuarioId, string username)
        {
            // First hide all menu links
            lnkInicio.Visible = false;
            lnkRoles.Visible = false;
            lnkPantallas.Visible = false;
            lnkUsuarios.Visible = false;
            lnkOferentes.Visible = false;
            lnkConcursos.Visible = false;
            lnkEntrevista.Visible = false;
            lnkOfeEmpleado.Visible = false;
            lnkPuestos.Visible = false;
            lnkAreas.Visible = false;
            lnkAccPersonal.Visible = false;
            lnkBitacora.Visible = false;
            lnkParametros.Visible = false;
            lnkCompanias.Visible = false;
            lnkCargarUbi.Visible = false;
            lnkInsEdu.Visible = false;

            // Then show only what the SP returns
            MenuBLL bll = new MenuBLL();
            var modulos = bll.ObtenerModulosPorUsuario(usuarioId);

            foreach (var mod in modulos)
            {
                switch (mod.nombre_modulo)
                {
                    case "Inicio":
                        lnkInicio.Visible = true;
                        lnkInicio.NavigateUrl = $"~/SEG/Bienvenida.aspx?u={username}";
                        break;
                    case "Roles":
                        lnkRoles.Visible = true;
                        lnkRoles.NavigateUrl = $"~/SEG/AdminRoles.aspx?u={username}";
                        break;
                    case "Pantallas":
                        lnkPantallas.Visible = true;
                        lnkPantallas.NavigateUrl = $"~/SEG/AdminPantallas.aspx?u={username}";
                        break;
                    case "Usuarios":
                        lnkUsuarios.Visible = true;
                        lnkUsuarios.NavigateUrl = $"~/SEG/AdminUsuarios.aspx?u={username}";
                        break;
                    case "Oferentes":
                        lnkOferentes.Visible = true;
                        lnkOferentes.NavigateUrl = $"~/OFE/MainOferentes.aspx?u={username}";
                        break;
                    case "Concursos":
                        lnkConcursos.Visible = true;
                        lnkConcursos.NavigateUrl = $"~/OFE/RegistroConcursos.aspx?u={username}";
                        break;
                    case "Entrevistas":
                        lnkEntrevista.Visible = true;
                        lnkEntrevista.NavigateUrl = $"~/OFE/AgendarEntrevista.aspx?u={username}";
                        break;
                    case "Contratar empleado":
                        lnkOfeEmpleado.Visible = true;
                        lnkOfeEmpleado.NavigateUrl = $"~/EMP/Oferente-a-Empleado.aspx?u={username}";
                        break;
                    case "Puestos":
                        lnkPuestos.Visible = true;
                        lnkPuestos.NavigateUrl = $"~/EMP/AdminPuestos.aspx?u={username}";
                        break;
                    case "Áreas":
                        lnkAreas.Visible = true;
                        lnkAreas.NavigateUrl = $"~/EMP/AdminAreas.aspx?u={username}";
                        break;
                    case "Acciones Personal":
                        lnkAccPersonal.Visible = true;
                        lnkAccPersonal.NavigateUrl = $"~/EMP/AdminAccionesPersonal.aspx?u={username}";
                        break;
                    case "Bitácora":
                        lnkBitacora.Visible = true;
                        lnkBitacora.NavigateUrl = $"~/GEN/VerBitacoras.aspx?u={username}";
                        break;
                    case "Parámetros":
                        lnkParametros.Visible = true;
                        lnkParametros.NavigateUrl = $"~/GEN/AdminParametros.aspx?u={username}";
                        break;
                    case "Compañías":
                        lnkCompanias.Visible = true;
                        lnkCompanias.NavigateUrl = $"~/GEN/AdminCompanias.aspx?u={username}";
                        break;
                    case "Ubicaciones":
                        lnkCargarUbi.Visible = true;
                        lnkCargarUbi.NavigateUrl = $"~/GEN/AdminUbicaciones.aspx?u={username}";
                        break;
                    case "Inst. Educativas":
                        lnkInsEdu.Visible = true;
                        lnkInsEdu.NavigateUrl = $"~/GEN/AdminInsEducativas.aspx?u={username}";
                        break;
                }
            }

            // Inicio is always visible
            lnkInicio.Visible = true;
            lnkInicio.NavigateUrl = $"~/SEG/Bienvenida.aspx?u={username}";
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SEG/Login.aspx");
        }
    }
}