using System.ComponentModel;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for InsuranceDetails.
    /// </summary>
    public class InsuranceDetails : TimeOutFormView
    {
        #region Event Handlers


        #endregion

        #region Methods

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
           this.insuranceDetailsView.UpdateView();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        #endregion

        #region Properties 

        #endregion

        #region Private Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.insuranceDetailsView = new PatientAccess.UI.InsuranceViews.InsuranceDetailsView();
            this.SuspendLayout();
            // 
            // insuranceDetailsView
            // 
            this.insuranceDetailsView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.insuranceDetailsView.Location = new System.Drawing.Point(1, 1);
            this.insuranceDetailsView.Model = null;
            this.insuranceDetailsView.Name = "insuranceDetailsView";
            this.insuranceDetailsView.Size = new System.Drawing.Size(910, 555);
            this.insuranceDetailsView.TabIndex = 0;
            // 
            // InsuranceDetails
            //             
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(912, 556);
            this.Controls.Add(this.insuranceDetailsView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsuranceDetails";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Insurance Details";
            this.Closing +=new CancelEventHandler(InsuranceDetails_Closing);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public InsuranceDetails()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Form variables
        /// </summary>
        /// 
        private Container                         components = null;
        public InsuranceDetailsView     insuranceDetailsView;

//        private Account                         i_Account;
        
        #endregion

        #region Constants
 
        #endregion

        private void InsuranceDetails_Closing(object sender, CancelEventArgs e)
        {
            AccountView.GetInstance().StartBenefitsResponsePollTimer();

            if( RuleEngine.GetInstance().AccountHasFailedError() )
            {
                this.insuranceDetailsView.CancelThis();
                //e.Cancel = true;
            }

            // start a timer on the AccountView to poll for results so that the To Do list can be updated

            this.Dispose();
        }
    }
}
