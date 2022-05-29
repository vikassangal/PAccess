using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls.Wizard;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// WelcomePage - gives an overview of the MSP wizard; this page is conditionally displayed based on a checkbox
    /// on the MSP2Dialog
    /// </summary>
    [Serializable]
	public class WelcomePage : WizardPage
	{
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// WelcomePage_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WelcomePage_Load(object sender, EventArgs e)
        {
            this.ShowLink                       = false;

            this.MyWizardMessages.Message1      = "Welcome to the Medicare Secondary Payor Form Analysis Wizard!";
            this.MyWizardMessages.TextFont1     = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1     = 11.25;

            this.MyWizardMessages.ShowMessages();

            this.lblHowDoesItWorkText.Text = "The Medicare Secondary Payor (MSP) Form Analysis wizard is designed to present on" +
                "ly those questions that are applicable to the patient's reason for today's visit.  For patients " +
                "receiving Black Lung benefits, obtain the date black lung benefits began, even if the reason for " +
                "today's visit is not due to Black Lung.  As you respond to each question, the wizard will adjust " +
                "and present the next applicable question. \r\n\r\n" +
                "When the primary payor can be predicted, the wizard will end and you will be presented with a summary " +
                "of reasons for the predicted primary payor, based on your responses to the questions asked.";

            this.lblWhatIfIWantText.Text = "This wizard allows you two methods to return to a previous screen:\r\n\r\n" +
                "1. The Back button allows you to return to the last screen you visited.\r\n\r\n" +
                "2. In the Progress Navigator area, located at the top of each set of questions, you may select " +
                "a previous screen in forward or backward sequence order.  The Progress Navigator does not allow you " +
                "to move forward to questions that have not been previously presented or questions that are not applicable " +
                "(based on your response to the questions asked).";

            this.lblWhatIfIveText.Text = "If, while using this wizard, you determine that any information presented " +
                "is not correct, please cancel the wizard, make adjustments in the Patient Access application and then " +
                "restart the wizard.  Canceling the MSP Form Analysis wizard will not save your responses to the questions.  " +
                "There are two methods to cancel the wizard:\r\n\r\n" +
                "1. Each set of questions displays a Cancel button at the bottom of the screen.\r\n\r\n" +
                "2. Edit & Cancel buttons may be displayed based on certain responses."; 
        }

        #endregion

        #region Methods

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );
            this.MyWizardButtons.AddNavigation( "< &Back", string.Empty );
            this.MyWizardButtons.AddNavigation( "&Next >", "SpecialProgramsPage" );    
            this.MyWizardButtons.SetAcceptButton( "&Next >" );
            this.MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            this.MyWizardButtons.SetPanel();
        }

        /// <summary>
        /// Cancel - handle the Cancel button click
        /// </summary>
        private void Cancel()
        {
            this.MyWizardContainer.Cancel();

            if( this.MSPCancelled != null )
            {
                this.MSPCancelled(this, null);
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHowDoesItWork = new System.Windows.Forms.Label();
            this.lblHowDoesItWorkText = new System.Windows.Forms.Label();
            this.lblWhatIfIWant = new System.Windows.Forms.Label();
            this.lblWhatIfIWantText = new System.Windows.Forms.Label();
            this.lblWhatIfIve = new System.Windows.Forms.Label();
            this.lblWhatIfIveText = new System.Windows.Forms.Label();
            this.pnlWizardPageBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.lblWhatIfIWantText);
            this.pnlWizardPageBody.Controls.Add(this.lblWhatIfIWant);
            this.pnlWizardPageBody.Controls.Add(this.lblHowDoesItWork);
            this.pnlWizardPageBody.Controls.Add(this.lblHowDoesItWorkText);
            this.pnlWizardPageBody.Controls.Add(this.lblWhatIfIve);
            this.pnlWizardPageBody.Controls.Add(this.lblWhatIfIveText);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblWhatIfIveText, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblWhatIfIve, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblHowDoesItWorkText, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblHowDoesItWork, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblWhatIfIWant, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblWhatIfIWantText, 0);
            // 
            // lblHowDoesItWork
            // 
            this.lblHowDoesItWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblHowDoesItWork.Location = new System.Drawing.Point(11, 52);
            this.lblHowDoesItWork.Name = "lblHowDoesItWork";
            this.lblHowDoesItWork.Size = new System.Drawing.Size(132, 17);
            this.lblHowDoesItWork.TabIndex = 0;
            this.lblHowDoesItWork.Text = "How does it work?";
            // 
            // lblHowDoesItWorkText
            // 
            this.lblHowDoesItWorkText.Location = new System.Drawing.Point(12, 71);
            this.lblHowDoesItWorkText.Name = "lblHowDoesItWorkText";
            this.lblHowDoesItWorkText.Size = new System.Drawing.Size(659, 98);
            this.lblHowDoesItWorkText.TabIndex = 0;
            // 
            // lblWhatIfIWant
            // 
            this.lblWhatIfIWant.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblWhatIfIWant.Location = new System.Drawing.Point(10, 180);
            this.lblWhatIfIWant.Name = "lblWhatIfIWant";
            this.lblWhatIfIWant.Size = new System.Drawing.Size(344, 15);
            this.lblWhatIfIWant.TabIndex = 0;
            this.lblWhatIfIWant.Text = "What if I want to go back and change my responses?";
            // 
            // lblWhatIfIWantText
            // 
            this.lblWhatIfIWantText.Location = new System.Drawing.Point(10, 199);
            this.lblWhatIfIWantText.Name = "lblWhatIfIWantText";
            this.lblWhatIfIWantText.Size = new System.Drawing.Size(663, 111);
            this.lblWhatIfIWantText.TabIndex = 0;
            // 
            // lblWhatIfIve
            // 
            this.lblWhatIfIve.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblWhatIfIve.Location = new System.Drawing.Point(10, 322);
            this.lblWhatIfIve.Name = "lblWhatIfIve";
            this.lblWhatIfIve.Size = new System.Drawing.Size(376, 15);
            this.lblWhatIfIve.TabIndex = 0;
            this.lblWhatIfIve.Text = "What if I\'ve chosen Medicare as a payor by mistake?";
            // 
            // lblWhatIfIveText
            // 
            this.lblWhatIfIveText.UseMnemonic = false;
            this.lblWhatIfIveText.Location = new System.Drawing.Point(10, 342);
            this.lblWhatIfIveText.Name = "lblWhatIfIveText";
            this.lblWhatIfIveText.Size = new System.Drawing.Size(663, 109);
            this.lblWhatIfIveText.TabIndex = 0;
            // 
            // WelcomePage
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Name = "WelcomePage";
            this.PageName = "WelcomePage";
            this.Load += new System.EventHandler(this.WelcomePage_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WelcomePage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public WelcomePage( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();    
        
            EnableThemesOn( this );
        }

        public WelcomePage( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
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

        private IContainer                components = null;

        private Label                      lblHowDoesItWork;     
        private Label                      lblHowDoesItWorkText;
        private Label                      lblWhatIfIWant;
        private Label                      lblWhatIfIWantText;            
        private Label                      lblWhatIfIve;
        private Label                      lblWhatIfIveText;  
           
        #endregion

        #region Constants
        #endregion

       

		

		
		

    }
}

