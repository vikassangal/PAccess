using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using log4net;

namespace Hsd.PerotSystems.PatientAccess.Services.ViWeb
{

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start( object sender, EventArgs e )
        {

            log4net.Config.XmlConfigurator.Configure();

        }

        protected void Session_Start( object sender, EventArgs e )
        {

        }

        protected void Application_BeginRequest( object sender, EventArgs e )
        {

        }

        protected void Application_AuthenticateRequest( object sender, EventArgs e )
        {

        }

        protected void Application_Error( object sender, EventArgs e )
        {

        }

        protected void Session_End( object sender, EventArgs e )
        {

        }

        protected void Application_End( object sender, EventArgs e )
        {

        }
    }
}