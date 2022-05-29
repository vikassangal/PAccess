using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PatientAccess.AppServer
{

	/// <summary>
	/// Summary description for _Default.
	/// </summary>
    public class Default : Page
    {
		#region Constants 

        private const string DOWNLOAD_ENABLED = "DownloadEnabled";

	    private const string DOWNLOAD_URL = "ClientDownloadURL";

	    private const string DOWNLOAD_PATH = "ClientDownloadPath";

	    private const string DOWNLOADER_ERROR_MSG = "An error has occured during the execution of the Patient Access Installation Wizard.  Please click <b><a href='{0}' title='Click here to download the file(s) manually'>here</a></b> to download the file(s) manually, or contact the Tenet Help Desk for assistance.";

		#endregion Constants 

		#region Fields 

        protected HtmlGenericControl chkIAgree;
        protected CompareValidator comp_pa_email;
        protected DropDownList ddlAppList;
        protected Panel Done;
        protected Panel Download;
        protected Panel Errors;
        protected Panel GatherInfo;
        bool _downloadEnabled = true;
        string _downloadPath = string.Empty;
        string _downloadURL = string.Empty;
        protected Literal lblErrMsg;
        protected Panel License;
        protected Repeater lstMessages;
        protected Literal ltrErrorMsg;
        protected Literal ltrVersion;
        // arraylist of InstallerMessages. We construct this on every page request to only keep track of the errors
        // that have occurred during this web request. We don't store it in viewstate because we only want the errors
        // that have happened on each page request
        private ArrayList messages;
        protected Button Next;
        protected Button Previous;
        protected HtmlGenericControl req_pa_acknowledge_license;
        protected RequiredFieldValidator req_pa_email;
        protected RequiredFieldValidator req_pa_fname;
        protected RequiredFieldValidator req_pa_lname;
        protected RequiredFieldValidator req_pa_phone;
        protected RequiredFieldValidator reqAppList;
        protected RequiredFieldValidator reqAppName;
        protected Panel SchemaExists;
        protected Panel SelectApp;
        protected TextBox txtConfirmEmail;
        protected TextBox txtEmail;
        protected TextBox txtFirstName;
        protected TextBox txtLastName;
        protected TextBox txtPhone;
        protected Panel Welcome;

		#endregion Fields 

		#region Enums 

        private enum WizardPanel 
        {
            Welcome,
            SelectApp,
            Download,
            Done,
            Errors,
        }

		#endregion Enums 

		#region Constructors 

        /// <summary>
        /// Default Page Constructor.
        /// </summary>
        public Default()
        {
            this._downloadEnabled = bool.Parse( ConfigurationManager.AppSettings[DOWNLOAD_ENABLED] );
            this._downloadURL = ConfigurationManager.AppSettings[DOWNLOAD_URL];
            this._downloadPath = ConfigurationManager.AppSettings[DOWNLOAD_PATH];
        }

		#endregion Constructors 

		#region Properties 

	    private WizardPanel CurrentWizardPanel 
        {
            get 
            {
                if( ViewState["WizardPanel"] != null )
                    return (WizardPanel)ViewState["WizardPanel"];

                return WizardPanel.Welcome;
            }
            set 
            {
                ViewState["WizardPanel"] = value;		
            }
        }

		#endregion Properties 

		#region Methods 

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {    
            this.Load += new System.EventHandler(this.Page_Load);
            this.Next.Click += new System.EventHandler(this.Next_Click);
            this.Previous.Click += new System.EventHandler(this.Previous_Click);
        }


        private bool ListApps( out string errorMessage )
        {
            try 
            {
                if ( true )
                {
                    DataSet dsAppList = new DataSet();
                    DataTable dtAppList = new DataTable();

                    dsAppList.ReadXml(HttpContext.Current.Server.MapPath("Downloads.xml"), XmlReadMode.InferSchema);
                    dtAppList = dsAppList.Tables[0];

                    ddlAppList.Items.Clear();

                    ddlAppList.DataSource = dsAppList;
                    ddlAppList.DataValueField = "InstallFile";
                    ddlAppList.DataTextField = "Name";
                    ddlAppList.DataBind();

                    // Add blank entry in list
                    ddlAppList.Items.Insert(0, string.Empty);
                    ddlAppList.SelectedIndex = 0;

                    errorMessage = string.Empty;
            		
                    return true;
                }
            }
            catch( Exception e ) 
            {
                errorMessage = e.Message;
                ReportException("ListApps", e );
                return false;
            }
        }


        private void Next_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
        
            switch( CurrentWizardPanel ) 
            {
                case WizardPanel.Welcome:
                    if (ListApps(out errorMessage)) 
                    {
                        SetActivePanel( WizardPanel.SelectApp, SelectApp );
                    } 
                    else 
                    {
                        lstMessages.DataSource = messages;
                        lstMessages.DataBind();
                        SetActivePanel( WizardPanel.Errors, Errors );
                    }
                    break;

                case WizardPanel.SelectApp:
                    if( Page.IsValid ) 
                    {
                        ltrVersion.Text = ddlAppList.SelectedItem.ToString();
                        SetActivePanel( WizardPanel.Download, Download );
                    }
                    break;

                case WizardPanel.Download:
                    if ( SecureFileDownload( ddlAppList.SelectedValue ) ) 
                    {
                        SetActivePanel( WizardPanel.Done, Done );
                    }
                    else 
                    {
                        lstMessages.DataSource = messages;
                        lstMessages.DataBind();
                        SetActivePanel( WizardPanel.Errors, Errors );
                    }
                    break;

                case WizardPanel.Done:
                    break;
            }
        }


        private void Page_Load(object sender, EventArgs e)
        {
            // Put user code to initialize the page here
            // We use the installer enabled flag to prevent someone from accidentally 
            // downloading the installer while the application is being versioned. 
            if (!this._downloadEnabled) 
            {
                Response.Write("<h1>The Patient Access Installation Wizard is currently disabled.</h1>");
                Response.Flush();
                Response.End();
            }
            else 
            {
                messages = new ArrayList();

                if (!Page.IsPostBack)
                    SetActivePanel (WizardPanel.Welcome, Welcome);
            }
        }


        private void Previous_Click(object sender, EventArgs e)
        {
            switch (CurrentWizardPanel) 
            {
                case WizardPanel.Welcome:
                    break;

                case WizardPanel.SelectApp:
                    SetActivePanel (WizardPanel.Welcome, Welcome);
                    break;

                case WizardPanel.Download:
                    SetActivePanel (WizardPanel.SelectApp, SelectApp);
                    break;
        		
                case WizardPanel.Done:
                    SetActivePanel (WizardPanel.Download, Download);
                    break;
            }
        }


	    private void ReportException( string module, Exception e ) 
        {
            ReportException( module, e.Message );
        }


	    private void ReportException( string module, string message ) 
        {
            messages.Add( new InstallerMessage( module, message ));
        }


        private bool SecureFileDownload( string inFile )
        {
            bool returnValue = false;
            try
            {
                string FileNamePath = string.Concat( this._downloadPath, inFile );

                FileInfo downloadFile = new FileInfo( FileNamePath );

                if ( downloadFile.Exists )
                {
                    Response.Clear();
                    Response.AddHeader( "Content-Disposition", "attachment; filename=" + downloadFile.Name );
                    Response.AddHeader( "Content-Length", downloadFile.Length.ToString() );
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile( downloadFile.FullName );
                    Response.End();
                }
                else
                {
                   ReportException( "SecureFileDownload", "File does not exist." );
                }
            }
            catch(Exception e ) 
            {
                switch( e.Message )
                {
                    case "Thread was being aborted.":
                        // Ignore...
                        returnValue = true;
                        break;
                    default:
                    {
                        ReportException( "SecureFileDownload", e );
                        break;
                    }
                }
            }
            return returnValue;
        }


        /// <summary>
        /// Sets the active panel.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="controlToShow">The control to show.</param>
        void SetActivePanel (WizardPanel panel, Control controlToShow) 
        {

            Panel currentPanel = FindControl(CurrentWizardPanel.ToString()) as Panel;
            if( currentPanel != null )
                currentPanel.Visible = false;
        	
            switch( panel ) 
            {
                case WizardPanel.Welcome:
                    Previous.Enabled = false;
                    License.Visible = false;
                    Previous.Text = "< Previous";
                    Next.Text = "Next >";
                    break;
                case WizardPanel.SelectApp:
                    Previous.Enabled = true;
                    Next.Enabled = true;
                    Previous.Text = "< Previous";
                    Next.Text = "Next >";
                    break;
                case WizardPanel.Download:
                    Previous.Enabled = true;
                    Next.Enabled = true;
                    Previous.Text = "< Previous";
                    Next.Text = "Download";
                    break;
                case WizardPanel.Done:
                    Next.Enabled = false;
                    Previous.Enabled = true;
                    Previous.Text = "< Previous";
                    Next.Text = "Next >";
                    break;
                case WizardPanel.Errors:
                    ltrErrorMsg.Text = String.Format( DOWNLOADER_ERROR_MSG, 
                                                      this._downloadURL );
                    Previous.Enabled = false;
                    Next.Enabled = false;
                    Previous.Text = "< Previous";
                    Next.Text = "Next >";
                    break;
                default:
                    Previous.Enabled = true;
                    Next.Enabled = true;
                    Previous.Text = "< Previous";
                    Next.Text = "Next >";
                    break;
            }

            controlToShow.Visible = true;
            CurrentWizardPanel = panel;
        }

		#endregion Methods 

		#region Nested Classes 

// Class to encapsulate the module (method) along with the error 
        // message that occurred within the module(method).
        public class InstallerMessage 
        {
		#region Fields 

            public string Message;
            public string Module;

		#endregion Fields 

		#region Constructors 

	            public InstallerMessage( string module, string message ) 
            {
                Module = module;
                Message = message;
            }

		#endregion Constructors 
        }
		#endregion Nested Classes 
    }
}