using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using System.Text.RegularExpressions;

namespace PatientAccess.UI.DocumentImagingViews
{

    /// <summary>
    /// FormWebView - Form to host an imbedded web browser ControlView.
    /// </summary>
    [Serializable]
    public class FormWebBrowserView : TimeOutFormView
    {

		#region Constants 

        private const string FORM_TITLE_SCAN_DOCUMENT = "Scan Document";
        private const string FORM_TITLE_VIEW_DOCUMENT = "View Document";
        private string RemovableChars = Regex.Escape(@"^&*%+= '");
		#endregion Constants 

		#region Fields 

        private Container _components = null;
        private string _scanImageUrl = string.Empty;
        private string _viewImageUrl = string.Empty;
        private string _viWebAppKey = string.Empty;
        private string _viWebGuid = string.Empty;
        private CheckBox uiCloseCheckbox;
        private ExtendedWebBrowser uiExtendedWebBrowser;
        private TableLayoutPanel uiTableLayoutPanel;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Build a new instance of this form for the GetInstance method
        /// </summary>
        public FormWebBrowserView()
        {

            // Required for Windows Form Designer support
            InitializeComponent();

            base.EnableThemesOn( this );

        }

		#endregion Constructors 

		#region Properties 

        [Browsable(false)]
        public new Account Model
        {
            private get
            {
                return base.Model as Account;
            }
            set
            {
                base.Model = value;
            }
        }

        /// <summary>
        /// Gets the scan image URL.
        /// </summary>
        /// <value>The scan image URL.</value>
        private string ScanImageURL
        {
            get
            {
                if( this._scanImageUrl == string.Empty && !this.IsInDesignMode )
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._scanImageUrl = broker.GetScanURL();
                }

                return this._scanImageUrl;
            }
        }

        /// <summary>
        /// Gets the view image URL.
        /// </summary>
        /// <value>The view image URL.</value>
        private string ViewImageURL
        {
            get
            {
                if( this._viewImageUrl == string.Empty && !this.IsInDesignMode )
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._viewImageUrl = broker.GetViewURL();
                }

                return this._viewImageUrl;
            }
        }

        /// <summary>
        /// Gets the VI web app key.
        /// </summary>
        /// <value>The VI web app key.</value>
        private string VIWebAppKey
        {
            get
            {
                if( this._viWebAppKey == string.Empty && !this.IsInDesignMode )
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._viWebAppKey = broker.GetVIWebAppKey();
                }

                return this._viWebAppKey;
            }
        }

        /// <summary>
        /// Gets the VI web GUID.
        /// </summary>
        /// <value>The VI web GUID.</value>
        private string VIWebGuid
        {
            get
            {
                if( this._viWebGuid == string.Empty && !this.IsInDesignMode )
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._viWebGuid = broker.GetVIWebGuid();
                }

                return this._viWebGuid;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// ScanDocument - user has requested to scan a document.  Build the URL based on properties of
        /// the Model (Account).
        /// </summary>
        public void ScanDocument()
        {

            this.Text = FORM_TITLE_SCAN_DOCUMENT;
            string pattern = "[" + RemovableChars + "]";
            // format the name as Last First MI
            StringBuilder formattedName = new StringBuilder();
            formattedName.AppendFormat( "{0}+{1}", 
                                        this.Model.Patient.LastName.Trim(),
                                        this.Model.Patient.FirstName.Trim() );

            string middleInitial = 
                this.Model.Patient.MiddleInitial.Trim();

            if( !String.IsNullOrEmpty( middleInitial ) )
            {
                formattedName .AppendFormat( "+{0}", middleInitial);
            }
            string scanningUrl = 
                string.Format( 
                    "{0}?mrNumber={1}&patNumber={2}&hspCd={3}&patName={4}&payor={5}&admitDate={6}&appGUID={7}&appKey={8}",                    
                    this.ScanImageURL,
                    this.Model.Patient.MedicalRecordNumber,
                    this.Model.AccountNumber,
                    this.Model.Facility.Code,
                    formattedName,
                    Regex.Replace(this.Model.PrimaryPayor, pattern, ""),
                    DateTime.Now.ToString( "yyyyMMdd" ),
                    this.VIWebGuid,
                    this.VIWebAppKey);

            this.uiExtendedWebBrowser.Navigate( scanningUrl );
            this.uiExtendedWebBrowser.Focus();
            
        }

        /// <summary>
        /// ViewDocument - user has requested to view a document(s).  Build the URL based on the passed docId(s)  
        /// </summary>
        /// <param name="documentIdentifiers"></param>
        public void ViewDocument( ArrayList documentIdentifiers )
        {

           this.ViewDocument( documentIdentifiers, FORM_TITLE_VIEW_DOCUMENT );

        }

        /// <summary>
        /// Views the document.
        /// </summary>
        /// <param name="documentIdentifiers">The document identifiers.</param>
        /// <param name="title">The title.</param>
        private void ViewDocument( ArrayList documentIdentifiers, string title )
        {

            this.Text = title;

            StringBuilder viewingUrl = new StringBuilder();
            viewingUrl.AppendFormat( 
                    "{0}?hspCode={1}&appGUID={2}&appKey={3}",
                    this.ViewImageURL,
                    this.Model.Facility.Code,
                    this.VIWebGuid,
                    this.VIWebAppKey );

            foreach( string documentId in documentIdentifiers )
            {

                if( !String.IsNullOrEmpty( documentId ) )
                {

                    viewingUrl.AppendFormat( "&docID={0}", documentId );

                }

            }

            this.uiExtendedWebBrowser.Navigate( viewingUrl.ToString() );
            this.uiExtendedWebBrowser.Focus();

        }

        /// <summary>
        /// Dispose - clean up
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {

            if( disposing )
            {

                this.uiExtendedWebBrowser.Dispose();

                if( _components != null )
                {
                    _components.Dispose();
                }

            }
            
            base.Dispose( disposing );

        }

        /// <summary>
        /// Handles the extended web browser navigate error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="PatientAccess.UI.CommonControls.WebBrowserNavigateErrorEventArgs"/> instance containing the event data.</param>
        private void HandleExtendedWebBrowserNavigateError( object sender, WebBrowserNavigateErrorEventArgs eventArgs )
        {

            if( eventArgs.StatusCode >= 400 || eventArgs.StatusCode < 600 )
            {

                MessageBox.Show( UIErrorMessages.DOC_IMG_NO_RESPONSE_MSG, 
                                 "Error",
                                 MessageBoxButtons.OK, 
                                 MessageBoxIcon.Error,
                                 MessageBoxDefaultButton.Button1 );

                this.uiExtendedWebBrowser.Stop();

            }

        }

        /// <summary>
        /// Handles the close checkbox click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleCloseCheckboxClick( object sender, EventArgs eventArgs )
        {
            
            this.Close();

        }

        /// <summary>
        /// Handles the close checkbox key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleCloseCheckboxKeyDown( object sender, KeyEventArgs eventArgs )
        {

            this.Close();

        }

        /// <summary>
        /// Handles the form web browser view closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void HandleFormWebBrowserViewClosing( object sender, CancelEventArgs eventArgs )
        {

            try
            {

                Cursor.Current = Cursors.WaitCursor;
                this.uiExtendedWebBrowser.Dispose();     

            }
            finally
            {

                Cursor.Current = Cursors.Default;
                this.Hide();

            }                        
        
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            ComponentResourceManager resources = new ComponentResourceManager( typeof( FormWebBrowserView ) );
            this.uiTableLayoutPanel = new TableLayoutPanel();
            this.uiCloseCheckbox = new CheckBox();
            this.uiExtendedWebBrowser = new ExtendedWebBrowser();
            this.uiTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel
            // 
            this.uiTableLayoutPanel.ColumnCount = 1;
            this.uiTableLayoutPanel.ColumnStyles.Add( new ColumnStyle() );
            this.uiTableLayoutPanel.Controls.Add( this.uiCloseCheckbox, 0, 1 );
            this.uiTableLayoutPanel.Controls.Add( this.uiExtendedWebBrowser, 0, 0 );
            this.uiTableLayoutPanel.Dock = DockStyle.Fill;
            this.uiTableLayoutPanel.Location = new Point( 0, 0 );
            this.uiTableLayoutPanel.Name = "uiTableLayoutPanel";
            this.uiTableLayoutPanel.RowCount = 2;
            this.uiTableLayoutPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 50F ) );
            this.uiTableLayoutPanel.RowStyles.Add( new RowStyle() );
            this.uiTableLayoutPanel.Size = new Size( 760, 660 );
            this.uiTableLayoutPanel.TabIndex = 0;
            // 
            // uiCloseCheckbox
            // 
            this.uiCloseCheckbox.Anchor = ( (AnchorStyles)( ( AnchorStyles.Bottom | AnchorStyles.Right ) ) );
            this.uiCloseCheckbox.Appearance = Appearance.Button;
            this.uiCloseCheckbox.AutoCheck = false;
            this.uiCloseCheckbox.FlatStyle = FlatStyle.System;
            this.uiCloseCheckbox.Location = new Point( 682, 634 );
            this.uiCloseCheckbox.Name = "uiCloseCheckbox";
            this.uiCloseCheckbox.Size = new Size( 75, 23 );
            this.uiCloseCheckbox.TabIndex = 2;
            this.uiCloseCheckbox.Text = "Close";
            this.uiCloseCheckbox.TextAlign = ContentAlignment.MiddleCenter;
            this.uiCloseCheckbox.Click += this.HandleCloseCheckboxClick;
            this.uiCloseCheckbox.KeyDown += this.HandleCloseCheckboxKeyDown;
            // 
            // uiExtendedWebBrowser
            // 
            this.uiExtendedWebBrowser.Dock = DockStyle.Fill;
            this.uiExtendedWebBrowser.Location = new Point( 3, 3 );
            this.uiExtendedWebBrowser.Margin = new Padding( 3, 3, 3, 0 );
            this.uiExtendedWebBrowser.MinimumSize = new Size( 20, 20 );
            this.uiExtendedWebBrowser.Name = "uiExtendedWebBrowser";
            this.uiExtendedWebBrowser.Size = new Size( 754, 628 );
            this.uiExtendedWebBrowser.TabIndex = 1;
            this.uiExtendedWebBrowser.NavigateError += HandleExtendedWebBrowserNavigateError;
            // 
            // FormWebBrowserView
            // 
            this.AutoScaleDimensions = new SizeF( 6F, 13F );
            this.BackColor = Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new Size( 760, 660 );
            this.Controls.Add( this.uiTableLayoutPanel );
            this.Icon = ( (Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.Location = new Point( 215, 5 );
            this.Name = "FormWebBrowserView";
            this.SizeGripStyle = SizeGripStyle.Show;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "DocumentImagingView";
            this.Closing += new CancelEventHandler( HandleFormWebBrowserViewClosing );
            this.uiTableLayoutPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }

		#endregion Methods 

    }

}
