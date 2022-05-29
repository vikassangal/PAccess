using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using log4net;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Dialog box for GuarantorValidationAlerts.
    /// </summary>
    [Serializable]
    public class GuarantorValidationAlerts : TimeOutFormView
    {
        #region Event Handlers
        private void GuarantorValidationAlerts_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void GuarantorValidationAlerts_Closing(object sender, CancelEventArgs e)
        {
            instanceCount = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
			
            if( Model_Guarantor != null )
            {
				this.Cursor = Cursors.WaitCursor;
				lblName.Text = Model_Guarantor.Name.AsFormattedName();

                webBrowser1.Navigate("about:blank", null, null, null);

                // Wait for the control the be initialized and ready.
                while (webBrowser1.ReadyState != WebBrowserReadyState.Complete ||
                        webBrowser1.IsBusy )
                {
                    Application.DoEvents();
                }

                HtmlDocument htmlDoc = webBrowser1.Document;

				htmlDoc.Write("<style type='text/css'>");
                htmlDoc.Write("body{font-family:Arial Narrow;font-size:smaller;}");
                htmlDoc.Write(".header{color:White;background-color:Gray;font-weight:bold;}");
                htmlDoc.Write(".section{cellspacing:0;cellpadding:0;width: 100%;border-color:Gray; border-width:1px; border-style:solid;}");
                htmlDoc.Write(".label{font-weight:bold;}");
                htmlDoc.Write(".subheading{background-color:Silver;border-color:Gray;border-style:solid;border-width:1px;}");
                htmlDoc.Write(".subiteml{border-color:Gray;border-bottom-style:solid;border-left-style:solid;border-width:1px;}");
                htmlDoc.Write(".subitemr{border-color:Gray;border-bottom-style:solid;border-right-style:solid;border-width:1px;}");
                htmlDoc.Write(".subitem{border-color:Gray;border-bottom-style:solid;border-width:1px;}");
                htmlDoc.Write("</style>");
                htmlDoc.Write("<body></body>");

				htmlDoc.Body.InnerHtml = Model_Guarantor.CreditReport.FormatedHawkAlert;
				this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Properties

        private Guarantor Model_Guarantor
        {
            get
            {
                return (Guarantor)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public bool SingletonCreated
        {
            get
            {
                return (instanceCount == 1);
            }
        }
        #endregion

        #region Private Methods


       
        #endregion

        #region Private Properties
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GuarantorValidationAlerts));
			this.lblStaticName = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabAlerts = new System.Windows.Forms.TabPage();
			this.btnCancel = new LoggingButton();
			this.tabControl.SuspendLayout();
			this.tabAlerts.SuspendLayout();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
			// 
			// lblStaticName
			// 
			this.lblStaticName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStaticName.Location = new System.Drawing.Point(14, 14);
			this.lblStaticName.Name = "lblStaticName";
			this.lblStaticName.TabIndex = 0;
			this.lblStaticName.Text = "Guarantor Name:";
			// 
			// lblName
			// 
			this.lblName.Location = new System.Drawing.Point(116, 14);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(481, 23);
			this.lblName.TabIndex = 0;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabAlerts);
			this.tabControl.Location = new System.Drawing.Point(14, 42);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(804, 392);
			this.tabControl.TabIndex = 1;
			// 
			// tabAlerts
			// 
            this.tabAlerts.Controls.Add(this.webBrowser1);
            this.tabAlerts.Location = new System.Drawing.Point(4, 22);
			this.tabAlerts.Name = "tabAlerts";
			this.tabAlerts.Size = new System.Drawing.Size(796, 366);
			this.tabAlerts.TabIndex = 0;
			this.tabAlerts.Text = "Alerts";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(744, 449);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "OK";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.Size = new System.Drawing.Size(796, 366);
            this.webBrowser1.TabIndex = 0;
            // 
			// GuarantorValidationAlerts
			// 
			this.AcceptButton = this.btnCancel;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(826, 486);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.lblStaticName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GuarantorValidationAlerts";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Guarantor Validation Actions";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.GuarantorValidationAlerts_Closing);
			this.Load += new System.EventHandler(this.GuarantorValidationAlerts_Load);
			this.tabControl.ResumeLayout(false);
			this.tabAlerts.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public GuarantorValidationAlerts()
        {
            instanceCount++;

            if( instanceCount == 1 )
            {   // Only allow 1 instance of this dialog
                InitializeComponent();
                //frameOffset = listBox.Font.Height / 3;
                base.EnableThemesOn( this );
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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
        #endregion

        #region Data Elements
        private Container     components = null;
        private LoggingButton         btnCancel;
        private Label          lblName;
        private Label          lblStaticName;
        private TabControl     tabControl;
        private TabPage        tabAlerts;
        private ArrayList        arrayList = new ArrayList();
        private WebBrowser     webBrowser1;    // Only allow 1 instance of this dialog at a time
        private static int instanceCount;

        private static readonly ILog c_log = LogManager.GetLogger( typeof( GuarantorValidationAlerts ) );
        
        #endregion

		#region Constants
		#endregion
    }
}
