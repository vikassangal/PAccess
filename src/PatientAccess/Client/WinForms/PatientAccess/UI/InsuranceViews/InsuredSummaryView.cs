using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class InsuredSummaryView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void ResetView()
        {
            this.Account        = null;
            this.Model_Coverage = null;

            lblPlanName.Text    = String.Empty;
            lblDateOfBirth.Text = String.Empty;
            lblGender.Text      = String.Empty;
            lblNationalID.Text  = String.Empty;
          //  btnEdit.Enabled     = false;
            UpdateView();
        }

        public override void UpdateView()
        {
           // btnEdit.Enabled = false;

            if( Model_Coverage != null && Model_Coverage.Insured != null )
            {
             //   btnEdit.Enabled  = true;
                lblPlanName.Text = Model_Coverage.Insured.FormattedName;

                if( Model_Coverage.Insured.Sex != null )
                {
                    lblGender.Text = Model_Coverage.Insured.Sex.Description;
                }

                /*******************************************************************************************
                if( Model_Coverage.Insured.SocialSecurityNumber != null &&
                    Model_Coverage.Insured.SocialSecurityNumber.AsFormattedString() != EMPTY_SSN )
                {
                    lblNationalID.Text = Model_Coverage.Insured.SocialSecurityNumber.AsFormattedString();
                }
                ********************************************************************************************/
 
                lblNationalID.Text = Model_Coverage.Insured.NationalID;
                
                if( Model_Coverage.Insured.DateOfBirth.Date != DateTime.MinValue )
                {
                    lblDateOfBirth.Text = Model_Coverage.Insured.DateOfBirth.Date.ToString( "MM/dd/yyyy",
                        DateTimeFormatInfo.InvariantInfo );
                }
            }
        }

        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            private get
            {
                return (Coverage)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
        #endregion

        #region Private Methods
        protected override void Dispose( bool disposing )
        {
            if ( IsHandleCreated )
            {
                Application.DoEvents();
            }

            if ( disposing )
            {
                if ( components != null ) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.grpPlanDetails = new System.Windows.Forms.GroupBox();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.lblStaticDOB = new System.Windows.Forms.Label();
            this.lblNationalID = new System.Windows.Forms.Label();
            this.lblStaticNationallID = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblStaticGender = new System.Windows.Forms.Label();
            this.lblPlanName = new System.Windows.Forms.Label();
            this.lblStaticName = new System.Windows.Forms.Label();
            this.grpPlanDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlanDetails
            // 
            this.grpPlanDetails.Controls.Add(this.lblDateOfBirth);
            this.grpPlanDetails.Controls.Add(this.lblStaticDOB);
            this.grpPlanDetails.Controls.Add(this.lblNationalID);
            this.grpPlanDetails.Controls.Add(this.lblStaticNationallID);
            this.grpPlanDetails.Controls.Add(this.lblGender);
            this.grpPlanDetails.Controls.Add(this.lblStaticGender);
            this.grpPlanDetails.Controls.Add(this.lblPlanName);
            this.grpPlanDetails.Controls.Add(this.lblStaticName);
            this.grpPlanDetails.Location = new System.Drawing.Point(0, -2);
            this.grpPlanDetails.Name = "grpPlanDetails";
            this.grpPlanDetails.Size = new System.Drawing.Size(300, 90);
            this.grpPlanDetails.TabIndex = 0;
            this.grpPlanDetails.TabStop = false;
            this.grpPlanDetails.Text = "Insured";
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.Location = new System.Drawing.Point(194, 43);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(65, 15);
            this.lblDateOfBirth.TabIndex = 7;
            this.lblDateOfBirth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticDOB
            // 
            this.lblStaticDOB.Location = new System.Drawing.Point(158, 43);
            this.lblStaticDOB.Name = "lblStaticDOB";
            this.lblStaticDOB.Size = new System.Drawing.Size(31, 12);
            this.lblStaticDOB.TabIndex = 6;
            this.lblStaticDOB.Text = "DOB:";
            this.lblStaticDOB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNationalID
            // 
            this.lblNationalID.Location = new System.Drawing.Point(72, 64);
            this.lblNationalID.Name = "lblNationalID";
            this.lblNationalID.Size = new System.Drawing.Size(128, 15);
            this.lblNationalID.TabIndex = 5;
            this.lblNationalID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticNationallID
            // 
            this.lblStaticNationallID.Location = new System.Drawing.Point(8, 64);
            this.lblStaticNationallID.Name = "lblStaticNationallID";
            this.lblStaticNationallID.Size = new System.Drawing.Size(65, 12);
            this.lblStaticNationallID.TabIndex = 4;
            this.lblStaticNationallID.Text = "National ID:";
            this.lblStaticNationallID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(53, 43);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(64, 15);
            this.lblGender.TabIndex = 3;
            this.lblGender.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticGender
            // 
            this.lblStaticGender.Location = new System.Drawing.Point(8, 43);
            this.lblStaticGender.Name = "lblStaticGender";
            this.lblStaticGender.Size = new System.Drawing.Size(44, 12);
            this.lblStaticGender.TabIndex = 2;
            this.lblStaticGender.Text = "Gender:";
            this.lblStaticGender.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanName
            // 
            this.lblPlanName.Location = new System.Drawing.Point(52, 20);
            this.lblPlanName.Name = "lblPlanName";
            this.lblPlanName.Size = new System.Drawing.Size(238, 15);
            this.lblPlanName.TabIndex = 1;
            this.lblPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticName
            // 
            this.lblStaticName.Location = new System.Drawing.Point(8, 20);
            this.lblStaticName.Name = "lblStaticName";
            this.lblStaticName.Size = new System.Drawing.Size(37, 12);
            this.lblStaticName.TabIndex = 0;
            this.lblStaticName.Text = "Name:";
            this.lblStaticName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InsuredSummaryView
            // 
            this.Controls.Add(this.grpPlanDetails);
            this.Name = "InsuredSummaryView";
            this.Size = new System.Drawing.Size(300, 90);
            this.grpPlanDetails.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuredSummaryView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
//            this.btnEdit.Enabled = false;
        }

        public InsuredSummaryView( object model ) : this()
        {
            this.Model = model;
            this.UpdateView();
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private GroupBox   grpPlanDetails;
        // Static text labels
        private Label      lblStaticName;
        private Label      lblStaticGender;
        private Label      lblStaticNationallID;
        private Label      lblStaticDOB;
        // Labels which display patient data
        private Label      lblPlanName;
        private Label      lblDateOfBirth;
        private Label      lblGender;
        private Label      lblNationalID;
        

        private Account                         i_Account;
        #endregion

        #region Constants
        private const int                       INSURED_DETAILS_PAGE = 1;
        //private string                          EMPTY_SSN = "   -  -    ";
        #endregion
    }
}