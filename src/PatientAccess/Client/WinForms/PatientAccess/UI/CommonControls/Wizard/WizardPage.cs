using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls.Wizard
{
    /// <summary>
    /// WizardPage - a container for the components that comprise a page in the wizard.  Holds a WizardLinksControl,
    /// a WizardMessageContro, and a WizardButtonsControl.  The content of the page is added to the pnlWizardPageBody 
    /// panel
    /// </summary>
    [Serializable]
	
    public class WizardPage : ControlView
	{
        #region Event Handlers

        private void WizardPage_Load(object sender, EventArgs e)
        {            
            this.MyWizardLinks.MyWizardPage = this;
            this.MyWizardButtons.MyWizardPage = this;
            this.MyWizardMessages.MyWizardPage = this;            
        }

        #endregion

        #region Methods

        /// <summary>
        ///  UpdateModel
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();
        }

        /// <summary>
        /// UpdateView
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView ();
        }

        public virtual void ResetPage()
        {
        }

        #endregion

        #region Properties

        public bool IsCurrentPage
        {
            get
            {
                return this.i_IsCurrentPage;
            }
            set
            {
                this.i_IsCurrentPage = value;
            }
        }

        public bool ShowLink
        {
            get
            {
                return i_ShowLink;
            }
            set
            {
                i_ShowLink = value;                
            }
        }

        public string LinkName
        {
            get
            {
                return i_LinkName;
            }
            set
            {
                i_LinkName = value;
            }
        }

        public string PageName
        {
            get
            {
                return i_PageName;
            }
            set
            {
                i_PageName = value;
            }
        }

        public WizardContainer MyWizardContainer
        {
            get
            {
                return i_MyWizardContainer;
            }
            set
            {
                i_MyWizardContainer = value;
            }
        }

        public WizardLinksControl MyWizardLinks
        {
            get
            {
                return i_MyWizardLinks;
            }
            set
            {
                i_MyWizardLinks = value;
            }
        }

        public WizardButtonsControl MyWizardButtons
        {
            get
            {
                return i_MyWizardButtons;
            }
            set
            {
                i_MyWizardButtons = value;
            }
        }

        public WizardMessageControl MyWizardMessages
        {
            get
            {
                return i_MyWizardMessages;
            }
            set
            {
                i_MyWizardMessages = value;
            }
        }

        public Panel WizardPageBody
        {
            get
            {
                return this.pnlWizardPageBody;
            }
        }

        public Account Model_Account
        {
            get
            {
                return this.Model as Account;
            }
            set
            {
                this.Model = value;
            }
        }

        public bool CanNavigate
        {
            get
            {
                return i_CanNavigate;
            }
            set
            {
                i_CanNavigate = value;
            }
        }

        public bool HasSummary
        {
            get
            {
                return i_HasSummary;
            }
            set
            {
                i_HasSummary = value;
            }
        }

        #endregion

        #region Private Methods

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.i_MyWizardLinks = new PatientAccess.UI.CommonControls.Wizard.WizardLinksControl();
            this.i_MyWizardMessages = new PatientAccess.UI.CommonControls.Wizard.WizardMessageControl();
            this.i_MyWizardButtons = new PatientAccess.UI.CommonControls.Wizard.WizardButtonsControl();
            this.pnlWizardPageBody = new System.Windows.Forms.Panel();
            this.pnlWizardPageBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // i_MyWizardLinks
            // 
            this.i_MyWizardLinks.BackColor = System.Drawing.Color.White;
            this.i_MyWizardLinks.Location = new System.Drawing.Point(23, 9);
            this.i_MyWizardLinks.Model = null;
            this.i_MyWizardLinks.MyWizardPage = null;
            this.i_MyWizardLinks.Name = "i_MyWizardLinks";
            this.i_MyWizardLinks.Size = new System.Drawing.Size(684, 25);
            this.i_MyWizardLinks.TabIndex = 1;
            // 
            // i_MyWizardMessages
            // 
            this.i_MyWizardMessages.FontStyle1 = System.Drawing.FontStyle.Regular;
            this.i_MyWizardMessages.FontStyle2 = System.Drawing.FontStyle.Regular;
            this.i_MyWizardMessages.Location = new System.Drawing.Point(0, 7);
            this.i_MyWizardMessages.Message1 = null;
            this.i_MyWizardMessages.Message2 = null;
            this.i_MyWizardMessages.Model = null;
            this.i_MyWizardMessages.MyWizardPage = null;
            this.i_MyWizardMessages.Name = "i_MyWizardMessages";
            this.i_MyWizardMessages.Size = new System.Drawing.Size(680, 40);
            this.i_MyWizardMessages.TabIndex = 0;
            this.i_MyWizardMessages.TabStop = false;
            this.i_MyWizardMessages.TextColor1 = System.Drawing.Color.Black;
            this.i_MyWizardMessages.TextColor2 = System.Drawing.Color.Black;
            this.i_MyWizardMessages.TextFont1 = "Microsoft Sans Serif";
            this.i_MyWizardMessages.TextFont2 = "Microsoft Sans Serif";
            this.i_MyWizardMessages.TextSize1 = 8.25;
            this.i_MyWizardMessages.TextSize2 = 8.25;
            // 
            // i_MyWizardButtons
            // 
            this.i_MyWizardButtons.BackColor = System.Drawing.Color.White;
            this.i_MyWizardButtons.Location = new System.Drawing.Point(26, 617);
            this.i_MyWizardButtons.Model = null;
            this.i_MyWizardButtons.MyWizardPage = null;
            this.i_MyWizardButtons.Name = "i_MyWizardButtons";
            this.i_MyWizardButtons.Size = new System.Drawing.Size(684, 24);
            this.i_MyWizardButtons.TabIndex = 3;
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.BackColor = System.Drawing.Color.White;
            this.pnlWizardPageBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlWizardPageBody.Controls.Add(this.i_MyWizardMessages);
            this.pnlWizardPageBody.Location = new System.Drawing.Point(24, 49);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Size = new System.Drawing.Size(684, 559);
            this.pnlWizardPageBody.TabIndex = 2;
            this.pnlWizardPageBody.TabStop = true;
            // 
            // WizardPage
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.pnlWizardPageBody);
            this.Controls.Add(this.i_MyWizardButtons);
            this.Controls.Add(this.i_MyWizardLinks);
            this.Name = "WizardPage";
            this.Size = new System.Drawing.Size(740, 650);
            this.Load += new System.EventHandler(this.WizardPage_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WizardPage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public WizardPage( Account anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );

            this.Model_Account = anAccount;
        }

        public WizardPage( WizardContainer wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            this.i_MyWizardContainer    = wizardContainer;

            EnableThemesOn( this );
        }

        public WizardPage( WizardContainer wizardContainer, Account anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            this.i_MyWizardContainer    = wizardContainer;

            EnableThemesOn( this );

            this.Model_Account = anAccount;
        }

        public WizardPage( string pageName, WizardContainer wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            this.i_PageName             = pageName;
            this.i_MyWizardContainer    = wizardContainer;

            EnableThemesOn( this );
        }

        public WizardPage( string pageName, WizardContainer wizardContainer, Account anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            this.i_PageName             = pageName;
            this.i_MyWizardContainer    = wizardContainer;

            EnableThemesOn( this );

            this.Model_Account = anAccount;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements

        private IContainer                    components = null;

        public Panel                           pnlWizardPageBody;

        private string                                              i_PageName = string.Empty;
        private string                                              i_LinkName = string.Empty;

        private bool                                                i_ShowLink = true;
        private bool                                                i_IsCurrentPage = false;
        private bool                                                i_CanNavigate = false;
        private bool                                                i_HasSummary = false;
        
        private WizardContainer                                     i_MyWizardContainer = null;        
        private WizardLinksControl                                  i_MyWizardLinks = null;
        private WizardButtonsControl                                i_MyWizardButtons = null;
        private WizardMessageControl                                i_MyWizardMessages = null;

        #endregion

        #region Constants
        #endregion

    }
}



