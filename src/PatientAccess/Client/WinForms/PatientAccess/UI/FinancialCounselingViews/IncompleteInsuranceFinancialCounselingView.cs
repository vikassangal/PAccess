using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews
{
    [Serializable]
    public class IncompleteInsuranceFinancialCounselingView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void IncompleteInsuranceFinancialCounselingView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if( warningResult == DialogResult.Yes )
                {
                    if( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model ) );
                    }
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
			this.DisplayErrorMessage();
        }

        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties

        private new Account Model
		{
			get
			{
				return (Account)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}
        #endregion

        public string SelectedTab
        {
            private get
            {
                return i_SelectedTab;
            }
            set
            {
                i_SelectedTab = value;
            }
        }

        #region Private Methods
		private void DisplayErrorMessage()
		{
			this.lblIncompleteInsurance.Text = this.ValidateErrorMessage();
		}

		private string ValidateErrorMessage()
		{
            // if( this.LocatedOnLiabilityTab )
            if( this.SelectedTab == "LIABILITY" )
            {
                return this.CheckLiabilityMessages();
            }
            else if( this.SelectedTab == "PAYMENT" )
            {
                return this.CheckPaymentMessages();
            }
            else
            {
                return UIErrorMessages.DO_NOT_HAVE_PERMISSION; 
            }
		}

		private string CheckPaymentMessages()
		{
            if( this.Model.BillHasDropped )
			{
				return UIErrorMessages.PAYMENT_BILL_HAS_DROPPED;
			}
			else if( this.Model.Insurance.FinancialClass != null &&
				this.Model.Insurance.FinancialClass.Code != String.Empty &&
				this.Model.Insurance.Coverages.Count == 0 )
			{
				return UIErrorMessages.PAYMENT_NO_INSURANCE;
			}
            //note: we fill PrimaryNoLiability property for Insured, and HasNoLiability - for Uninsured => need to checked both
			else if( this.Model.TotalCurrentAmtDue == 0m &&
					 !this.Model.Insurance.HasNoLiability  &&
					 this.Model.TotalPaid  == 0m &&
					 this.Model.NumberOfMonthlyPayments == 0 )
			{
				return UIErrorMessages.LIABILITY_NOT_DETERMINED;
			}
			else
			{
				return UIErrorMessages.NO_LIABILITY_NO_PAYMENT;
			}
		}

		private string CheckLiabilityMessages()
		{
			if( this.Model.FinancialClass == null ||
                this.Model.FinancialClass.Code.Trim() == String.Empty )
			{
				return UIErrorMessages.FINANCIAL_CLASS_MISSING_ERRMSG;
			}
			else if( this.Model.FinancialClass != null &&
					 this.Model.FinancialClass.Code.Trim() != String.Empty &&
					 this.Model.Insurance.Coverages.Count == 0 )
			{
				return UIErrorMessages.FINANCIAL_CLASS_WITH_NO_INSURANCE;
			}
			else if( !(FinancialCouncelingService.IsPatientInsured( this.Model.FinancialClass )) )
			{
				return UIErrorMessages.FINANCIAL_CLASS_NOT_PRECONDITION;
			}
			else 
			{
				return UIErrorMessages.LIABILITY_BILL_HAS_DROPPED;
			}
		}

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
			this.lblIncompleteInsurance = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblIncompleteInsurance
			// 
			this.lblIncompleteInsurance.Location = new System.Drawing.Point(7, 12);
			this.lblIncompleteInsurance.Name = "lblIncompleteInsurance";
			this.lblIncompleteInsurance.Size = new System.Drawing.Size(576, 55);
			this.lblIncompleteInsurance.TabIndex = 0;
			this.lblIncompleteInsurance.Text = "Incomplete Insurance";
			// 
			// IncompleteInsuranceFinancialCounselingView
			// 
			this.Controls.Add(this.lblIncompleteInsurance);
			this.Name = "IncompleteInsuranceFinancialCounselingView";
			this.Size = new System.Drawing.Size(594, 267);
            this.Enter += new System.EventHandler( this.IncompleteInsuranceFinancialCounselingView_Enter );
            this.ResumeLayout( false );

		}
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public IncompleteInsuranceFinancialCounselingView()
        {
            InitializeComponent();
			base.EnableThemesOn( this );
        }

		protected override void Dispose( bool disposing )
		{
			Application.DoEvents();
			if( disposing )
			{
				if ( components != null ) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
        #endregion

        #region Data Elements
        private Container components = null;
		private Label lblIncompleteInsurance;

        private string i_SelectedTab;

        #endregion

        #region Constants
        #endregion
    }
}
