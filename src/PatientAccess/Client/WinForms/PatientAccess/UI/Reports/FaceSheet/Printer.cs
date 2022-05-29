using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;
using log4net;

namespace PatientAccess.UI.Reports.FaceSheet
{
    public class Printer : TimeOutFormView
	{

		#region Fields 

        private static readonly ILog c_log = LogManager.GetLogger( typeof( Printer ) );
        private Container components = null;
        private static Printer _iPrinter = null;
        private WebBrowser webBrowser1;

		#endregion Fields 

		#region Constructors 

        private Printer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

		#endregion Constructors 

		#region Methods 

        public static Printer GetInstance( )
        {
            if( _iPrinter == null )
            {
                _iPrinter = new Printer( );
            }
            return _iPrinter;
        }

        /// 
        /// <param name="accountData"></param>
        /// <param name="templateUrl"></param>
        /// <param name="templateFiller"></param>
        public void Print( IDictionary accountData, string templateUrl, TemplateFiller templateFiller )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var template = GetTemplateFrom( templateUrl );

                templateFiller.FillTemplateWith( accountData, template );
                
                webBrowser1.ShowPrintDialog();
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private HtmlDocument GetTemplateFrom ( string url )
        {
            //This is here incase user clicks print more than one times. This causes defect 34930
            if (webBrowser1.IsBusy)
            {
                webBrowser1.Stop();
            }

            webBrowser1.Navigate( url );

            //Wait until the document load
            while ( webBrowser1.ReadyState != WebBrowserReadyState.Complete ||
                    webBrowser1.IsBusy )
            {
                Application.DoEvents();
            }

            return  webBrowser1.Document; 
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );

            try
            {
                this.webBrowser1.Dispose();
            }
            catch (Exception ex)
            {
                c_log.Error("Failed to Dispose WebBrowser control", ex);
            }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            ResourceManager resources = new ResourceManager(typeof(Printer));
            this.webBrowser1 = new WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new Point(8, 8);
            this.webBrowser1.Size = new Size(888, 432);
            this.webBrowser1.TabIndex = 0;
            // 
            // FaceSheetView
            // 
            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(904, 453);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Printer";
            this.Text = "FaceSheetView";
            this.ResumeLayout(false);

        }

        #endregion Methods 

	}
}
