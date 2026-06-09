using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace ADMExpedientePersonal
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void Application_EndRequest(object sender, EventArgs e)
        {
            foreach (string key in Response.Cookies.Keys)
            {
                if (key == "ASP.NET_SessionId")
                {
                    Response.Cookies[key].Secure = true;
                    Response.Cookies[key].HttpOnly = true;
                    Response.Cookies[key].SameSite = SameSiteMode.Lax;
                }
            }
        }
    }
}