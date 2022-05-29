using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    public partial class BenefitsResponseConfirmation : TimeOutFormView
    {
        #region Event Handlers

        private void btnAccept_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Yes;

            Coverage coverage = this.Model as Coverage;

            coverage.DataValidationTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewAcceptedStatus();

            // if the radio buttons were enabled, pull the selected constraints (in- or out-of-network) and plunk on the coverage

            if( this.panel1.Enabled )
            {
                if( this.rbInNetwork.Checked )
                {
                    coverage.SetCoverageConstraints( this.Model_BenefitsResponse.GetInNetworkConstraint() );
                }
                else
                {
                    coverage.SetCoverageConstraints( this.Model_BenefitsResponse.GetOutOfNetworkConstraint() );
                }
            }
            else
            {
                if( this.Model_BenefitsResponse.CoverageConstraintsCollection.Count > 0 )
                {
                    coverage.SetCoverageConstraints( this.Model_BenefitsResponse.CoverageConstraintsCollection[0] as CoverageConstraints );
                }
            }
        }

        private void btnReject_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.No;
            ( this.Model as Coverage ).DataValidationTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewRejectedStatus();
        }

        private void btnDecideLater_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.No;
            ( this.Model as Coverage ).DataValidationTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewDeferredStatus();
        }

        private void btnMoreDetails_Click( object sender, EventArgs e )
        {
            this.details.Model = this.Model_BenefitsResponse;
            this.details.UpdateView();
            this.details.ShowDialog();
        }

        private void rbInNetwork_CheckedChanged( object sender, EventArgs e )
        {
            if( rbInNetwork.Checked || rbOutOfNetwork.Checked )
            {
                this.btnAccept.Enabled = true;
            }
            
        }

        private void rbOutOfNetwork_CheckedChanged( object sender, EventArgs e )
        {
            if( rbInNetwork.Checked || rbOutOfNetwork.Checked )
            {
                this.btnAccept.Enabled = true;
            }
        }

        private string formatDOB( string inDOB )
        {
            if( inDOB.Length != 8 )
            {
                return inDOB;
            }
            else
            {
                string outDOB = inDOB.Substring( 4, 2 ) + "/" + inDOB.Substring( 6, 2 ) + "/" + inDOB.Substring( 0, 4 );
                return outDOB;
            }
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            base.UpdateView();
            
            // build out the labels

            this.lblDVInsuredNameValue.Text         = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredName;
            this.lblDVDOBValue.Text                 = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredDOB;
            this.lblDVPayorValue.Text               = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponsePayorName;
            this.lblDVSubscriberIDValue.Text        = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseSubscriberID;

            this.lblSubmittedInsuredNameValue.Text  = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.RequestInsuredName;
            this.lblSubmittedDOBValue.Text          = this.formatDOB( this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.RequestInsuredDOB );
            this.lblSubmittedPayorValue.Text        = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.RequestPayorName;
            this.lblSubmittedSubscriberIDValue.Text = this.Model_BenefitsResponse.ReturnedDataValidationTicket.BenefitsResponse.RequestSubscriberID;
          
            // determine the member if lable (if any) to display

            Coverage aCoverage = this.Model as Coverage;

            if( aCoverage.GetType() == typeof( CommercialCoverage ) ||
                aCoverage.GetType() == typeof( GovernmentOtherCoverage ) ||
                aCoverage.GetType() == typeof( OtherCoverage ) )
            {
                this.lblSubmittedSubscriberID.Text = CERT_SSN_ID;
                this.lblDVSubscriberID.Text = CERT_SSN_ID;
            }
            else if( aCoverage.GetType() == typeof( GovernmentMedicaidCoverage ) )
            {
                this.lblSubmittedSubscriberID.Text = POLICY_CIN;
                this.lblDVSubscriberID.Text = POLICY_CIN;
            }
            else if( aCoverage.GetType() == typeof( GovernmentMedicareCoverage ) )
            {
                this.lblSubmittedSubscriberID.Text = HIC_NUMBER;
                this.lblDVSubscriberID.Text = HIC_NUMBER;
            }           
            else
            {
                this.lblSubmittedSubscriberID.Text = string.Empty;
                this.lblDVSubscriberID.Text = string.Empty;
            }

            // determine if the response contains both in-network and out-of-network

            if( this.Model_BenefitsResponse.CoverageConstraintsCollection.Count > 1 )
            {                
                this.panel1.Enabled     = true;
                this.btnAccept.Enabled  = false;
                this.rbInNetwork.Checked = false;
            }           
        }

        #endregion

        #region Properties

        // TLG 07/17/2007
        // hack because the reponse is not a property on the DataValidationTicket as it should be
        // REFACTOR!!!

        public BenefitsValidationResponse Model_BenefitsResponse
        {
            private get
            {
                return this.i_Model_BenefitsResponse;
            }
            set
            {
                i_Model_BenefitsResponse = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public BenefitsResponseConfirmation()
        {
            InitializeComponent();
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements

        VerificationResponseDetails                             details = new VerificationResponseDetails();
        BenefitsValidationResponse                              i_Model_BenefitsResponse;

        #endregion

        #region Constants

        private const string    CERT_SSN_ID = "Cert/SSN/ID:",
                                POLICY_CIN  = "Policy/CIN:",
                                HIC_NUMBER  = "HIC number:";

        #endregion        
    }
}