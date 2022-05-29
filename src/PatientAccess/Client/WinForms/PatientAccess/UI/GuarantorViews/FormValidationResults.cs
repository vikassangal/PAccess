using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Summary description for FormValidationResults.
    /// </summary>
    //TODO: Create XML summary comment for FormValidationResults
    [Serializable]
    public class FormValidationResults : TimeOutFormView
    {
        #region Event Handlers

		private void guarantorValidationResults_EnableOKButton(object sender, EventArgs e)
		{
			LooseArgs args = (LooseArgs)e;
			YesNoFlag yesNo = args.Context as YesNoFlag;

			this.btnOk.Enabled = ( yesNo.Code == YesNoFlag.CODE_YES );
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
            // DEFECT 3909 - added null check
            if (this.Model_Guarantor != null)
            {
                this.Model_Guarantor.DataValidationTicket.ResultsReviewed = true;

                this.Model_Guarantor.Name = this.guarantorValidationResults.GuarantorName;

                ContactPoint cp = new ContactPoint(TypeOfContactPoint.NewMailingContactPointType());
                ContactPoint guarantorContactPoint = this.Model_Guarantor.ContactPointWith(cp.TypeOfContactPoint);

                guarantorContactPoint.Address.Address1 = this.guarantorValidationResults.GuarantorAddress.Address1;
                guarantorContactPoint.Address.Address2 = this.guarantorValidationResults.GuarantorAddress.Address2;
                guarantorContactPoint.Address.City = this.guarantorValidationResults.GuarantorAddress.City;
                guarantorContactPoint.Address.Country = this.guarantorValidationResults.GuarantorAddress.Country;
                guarantorContactPoint.Address.County = this.guarantorValidationResults.GuarantorAddress.County;
                guarantorContactPoint.Address.ZipCode.PostalCode = this.guarantorValidationResults.GuarantorAddress.ZipCode.PostalCode;
                guarantorContactPoint.Address.State = this.guarantorValidationResults.GuarantorAddress.State;

                if (this.guarantorValidationResults.IsValidPhoneNumber())
                {
                    guarantorContactPoint.PhoneNumber = this.guarantorValidationResults.GuarantorPhoneNumber;
                }

                if (this.guarantorValidationResults.IsValidSSN())
                {
                    this.Model_Guarantor.SocialSecurityNumber = this.guarantorValidationResults.GuarantorSSN;

                    if (this.Model_Guarantor.SocialSecurityNumber != null)
                    {
                        this.Model_Guarantor.SocialSecurityNumber.SSNStatus = new SocialSecurityNumberStatus();
                        this.Model_Guarantor.SocialSecurityNumber.SSNStatus.Description = KNOWN;

                    }
                }

                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
			this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
			ContactPoint cp               = new ContactPoint( TypeOfContactPoint.NewMailingContactPointType() );
			ContactPoint sentContactPoint = this.Model_Guarantor.ContactPointWith( cp.TypeOfContactPoint );

			this.guarantorValidationResults.SentContactPoint = sentContactPoint;
			this.guarantorValidationResults.Model_Guarantor = this.Model_Guarantor;
            this.guarantorValidationResults.UpdateView();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {

        }
        #endregion

        #region Properties

        public Guarantor Model_Guarantor
        {
            get
            {
                return (Guarantor)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        #endregion

        #region Private Methods

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.btnOk = new LoggingButton();
			this.btnCancel = new LoggingButton();
			this.guarantorValidationResults = new PatientAccess.UI.GuarantorViews.GuarantorValidationResults();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.BackColor = System.Drawing.SystemColors.Control;
			this.btnOk.Location = new System.Drawing.Point(759, 526);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.Location = new System.Drawing.Point(840, 525);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(71, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// guarantorValidationResults
			// 
			this.guarantorValidationResults.BackColor = System.Drawing.Color.White;
			this.guarantorValidationResults.GuarantorAddress = null;
			this.guarantorValidationResults.GuarantorName = null;
			this.guarantorValidationResults.GuarantorPhoneNumber = null;
			this.guarantorValidationResults.GuarantorSSN = null;
			this.guarantorValidationResults.Location = new System.Drawing.Point(15, 15);
			this.guarantorValidationResults.Model = null;
			this.guarantorValidationResults.Model_Guarantor = null;
			this.guarantorValidationResults.Name = "guarantorValidationResults";
			this.guarantorValidationResults.SentContactPoint = null;
			this.guarantorValidationResults.Size = new System.Drawing.Size(900, 500);
			this.guarantorValidationResults.TabIndex = 0;
			this.guarantorValidationResults.EnableOKButton += new System.EventHandler(this.guarantorValidationResults_EnableOKButton);
			// 
			// FormValidationResults
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.ClientSize = new System.Drawing.Size(930, 558);
			this.Controls.Add(this.guarantorValidationResults);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormValidationResults";
			this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Guarantor Validation Results";
			this.ResumeLayout(false);

		}
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FormValidationResults()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            base.EnableThemesOn( this );
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
        }
        #endregion

        #region Data Elements
		private Container components = null;
        private GuarantorValidationResults      guarantorValidationResults = null;
       
        private LoggingButton btnCancel;
        private LoggingButton btnOk;
		#endregion

        #region Constants

        private const string KNOWN  = "Known";

        #endregion
	}
}
