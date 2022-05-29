using System;
using System.Text;

namespace PatientAccess
{
    public partial class Diag : System.Web.UI.Page
    {

        protected void Page_Load( object sender, EventArgs e )
        {
            /////////////////////////////////////////////////////////////////////////////
            // Display a message on the web form showing if the request was successful //
            /////////////////////////////////////////////////////////////////////////////
            ServerStatus.Text = "<font face=\"Courier New\" size=\"2\" >Connection to <b>" + Request.ServerVariables["SERVER_NAME"] + "</b> successful.</font>";

            //////////////////////////////////////////////////////////////////////////
            // Build out a string on server values to write to the response stream. //
            //////////////////////////////////////////////////////////////////////////
            var builder = new StringBuilder();

            builder.Append( "<font face=\"Courier New\" size=\"2\" >" );
            builder.Append( "<br><b>IIS User:</b> " + System.Security.Principal.WindowsIdentity.GetCurrent().Name );
            builder.Append( "<br><b>Machine Name:</b> " + Environment.MachineName );

            builder.Append( "<br><b>Host Name:</b> " + System.Net.Dns.GetHostName() );
            builder.Append( "<br><b>Local Host:</b> " + Request.ServerVariables["LOCAL_ADDR"] );
            builder.Append( "<br><b>OS:</b> " + Environment.OSVersion );
            builder.Append( "<br><b>IP Addresses:</b> " );

            var ipAddresses = string.Empty;

            foreach ( System.Net.IPAddress i in System.Net.Dns.GetHostEntry( System.Net.Dns.GetHostName() ).AddressList )
            {
                ipAddresses += i + ", ";
            }

            ipAddresses = ipAddresses.Substring( 0, ipAddresses.Trim().Length - 1 );
            builder.Append( ipAddresses );

            builder.Append( "<br />" );
            builder.Append( "<br />" );
            builder.Append( "<b>HOST INFO</b><br />" );
            builder.Append( "---------" );

            builder.Append( "<br><b>HTTP Host:</b> " + Request.ServerVariables["HTTP_HOST"] );
            builder.Append( "<br><b>Server Name:</b> " + Request.ServerVariables["SERVER_NAME"] );
            builder.Append( "<br><b>User Host Address:</b> " + Request.UserHostAddress );
            builder.Append( "<br><b>User Host Name:</b> " + Request.UserHostName );
            builder.Append( "<br><b>Request Method:</b> " + Request.ServerVariables["REQUEST_METHOD"] );
            builder.Append( "<br><b>Server Port:</b> " + Request.ServerVariables["SERVER_PORT"] );
            builder.Append( "<br><b>Server Protocol:</b> " + Request.ServerVariables["SERVER_PROTOCOL"] );
            builder.Append( "<br><b>Server Software:</b> " + Request.ServerVariables["SERVER_SOFTWARE"] );

            builder.Append( "<br />" );

            builder.Append( "<br><b>Host Domain:</b> " + AppDomain.CurrentDomain.FriendlyName );
            builder.Append( "<br><b>Host Assembly Version:</b> " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version );
            builder.Append( "<br><b>Host Code Base:</b> " + System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase );
            builder.Append( "<br><b>Host .NET Framework Version:</b> " + Environment.Version );
            builder.Append( "<br><b>Host OS Version:</b> " + Environment.OSVersion );

            builder.Append( "<br />" );

            builder.Append( "<br><b>Application Path:</b> " + Request.ApplicationPath );
            builder.Append( "<br><b>Physical Application Path:</b> " + Request.PhysicalApplicationPath );
            builder.Append( "<br><b>Path:</b> " + Request.Path );
            builder.Append( "<br><b>Physical Path:</b> " + Request.PhysicalPath );

            builder.Append( "<br />" );
            builder.Append( "<br />" );
            builder.Append( "<strong>CLIENT INFO</strong><br />" );
            builder.Append( "-----------" );
            builder.Append( "<br><b>Operating System:</b> " + Request.Browser.Platform );
            builder.Append( "<br><b>Browser:</b> " + Request.Browser.Browser );
            builder.Append( "<br><b>Browser Version:</b> " + Request.Browser.Version );
            builder.Append( "<br><b>Current Execution File Path:</b> " + Request.CurrentExecutionFilePath );
            builder.Append( "<br><b>File Path:</b> " + Request.FilePath );
            builder.Append( "<br><b>Is Authenticated:</b> " + Request.IsAuthenticated );
            builder.Append( "<br><b>Is Secure:</b> " + Request.IsSecureConnection );
            builder.Append( "<br />" );
            builder.Append( "<br><b>Url:</b> " + Request.Url );
            builder.Append( "<br><b>Raw Url:</b> " + Request.RawUrl );
            builder.Append( "<br><b>Referrer:</b> " + "<a href=\"" + Request.ServerVariables["HTTP_REFERER"] + "\">" + Request.ServerVariables["HTTP_REFERER"] + "</a>" );
            builder.Append( "</font>" );

            ServerInfo.Text = builder.ToString();
        }
    }
}