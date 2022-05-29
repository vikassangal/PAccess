using System;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    public partial class VerificationResponseDetails : TimeOutFormView
    {
        public override void UpdateView()
        {
            base.UpdateView();

            BenefitsValidationResponse bvr = this.Model as BenefitsValidationResponse;
            this.richTextBox1.Text = bvr.PayorMessage;
        }
        public VerificationResponseDetails()
        {
            InitializeComponent();
        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            this.Close();
        }
    }
}