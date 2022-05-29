using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.FinancialCounselingViews.CreditReportViews;
using User = Extensions.SecurityService.Domain.User;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for CreditAdviceView.
	/// </summary>
	//TODO: Create XML summary comment for CreditAdviceView
	[Serializable]
	public class CreditAdviceView : ControlView
	{
		#region Event Handlers
		private void btnViewCreditReport_Click(object sender, EventArgs e)
		{
			ViewCreditReport();
		}
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			PopulateCreditAdvice();
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{
		}
		#endregion

		#region Properties
		public Account Model_Account
		{
			private get
			{
				return (Account)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

		public bool IsInsuredPatient
		{
			private get
			{
				return i_isInsuredPatient;
			}
			set
			{
				i_isInsuredPatient = value;
			}
		}
		#endregion

		#region Private Methods
		
		private void PopulateCreditAdvice()
		{
			if( IsCreditResultsAvalible() && isUserRoleValidForCreditAdviceView() )
			{ 
				// Begin CR0217 

				Model_Account.Guarantor.CreditReport.IsInsured = IsInsuredPatient;
				
				tbCreditAdvice.Enabled = true;
				decimal TotalCurrentAmountDue = Model_Account.TotalCurrentAmtDue;

				tbCreditAdvice.Text = 
					Model_Account.Guarantor.CreditReport.GetCreditAdvice( 
						TotalCurrentAmountDue );
 
                btnViewCreditReport.Enabled = true;
				// End CR0217 
			}
			else
			{
                tbCreditAdvice.Text = "(Not available. If not already completed, perform Guarantor validation.)";
				btnViewCreditReport.Enabled = false;
				tbCreditAdvice.Enabled = false;
			}
		}

        private bool isUserRoleValidForCreditAdviceView()
        {
            User secFrameworkUser = Domain.User.GetCurrent().SecurityUser;

            bool result = false;

            foreach( Role role in secFrameworkUser.Roles() )
            {
                if( role.Name == "FinancialUser" ||
                    role.Name == "RegistrationAdministrator" )
                {
                    result = true;
                    break;
                }               
            }
            return result;
        }

		private bool IsCreditResultsAvalible()
		{
			bool result = false;
			if( this.Model_Account.Guarantor.DataValidationTicket != null )
			{
				if( this.Model_Account.Guarantor.DataValidationTicket.ResultsAvailable == true )
				{
					result = true;
				}
			}
			return result;
		}

		private void ViewCreditReport()
		{
			if( this.Model_Account.Guarantor.CreditReport != null )
			{
				FormViewCreditReport formViewCreditReport = new FormViewCreditReport();
				formViewCreditReport.Model = Model_Account.Guarantor;
				formViewCreditReport.UpdateView();

				try
				{
					formViewCreditReport.ShowDialog( this );
				}
				finally
				{
					formViewCreditReport.Dispose();
				}
			}
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblCreditAdvice = new System.Windows.Forms.Label();
			this.tbCreditAdvice = new System.Windows.Forms.TextBox();
			this.btnViewCreditReport = new LoggingButton();
			this.SuspendLayout();
			// 
			// lblCreditAdvice
			// 
			this.lblCreditAdvice.Location = new System.Drawing.Point(10, 4);
			this.lblCreditAdvice.Name = "lblCreditAdvice";
			this.lblCreditAdvice.Size = new System.Drawing.Size(73, 13);
			this.lblCreditAdvice.TabIndex = 0;
			this.lblCreditAdvice.Text = "Credit advice:";
			// 
			// tbCreditAdvice
			// 
			this.tbCreditAdvice.BackColor = System.Drawing.Color.White;
			this.tbCreditAdvice.Location = new System.Drawing.Point(3, 22);
			this.tbCreditAdvice.Multiline = true;
			this.tbCreditAdvice.Name = "tbCreditAdvice";
			this.tbCreditAdvice.ReadOnly = true;
			this.tbCreditAdvice.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbCreditAdvice.Size = new System.Drawing.Size(219, 114);
			this.tbCreditAdvice.TabIndex = 1;
			this.tbCreditAdvice.Text = "";
			// 
			// btnViewCreditReport
			// 
			this.btnViewCreditReport.Location = new System.Drawing.Point(237, 22);
			this.btnViewCreditReport.Name = "btnViewCreditReport";
			this.btnViewCreditReport.Size = new System.Drawing.Size(121, 23);
			this.btnViewCreditReport.TabIndex = 2;
			this.btnViewCreditReport.Text = "&View Credit Report";
			this.btnViewCreditReport.Click += new System.EventHandler(this.btnViewCreditReport_Click);
			// 
			// CreditAdviceView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.btnViewCreditReport);
			this.Controls.Add(this.tbCreditAdvice);
			this.Controls.Add(this.lblCreditAdvice);
			this.Name = "CreditAdviceView";
			this.Size = new System.Drawing.Size(361, 140);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Construction and Finalization
		public CreditAdviceView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
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
		}
		#endregion

		#region Data Elements
		private Container components = null;
		private Label lblCreditAdvice;
		private TextBox tbCreditAdvice;
		private LoggingButton btnViewCreditReport;
		//private CreditReport i_CreditReport;
		private bool  i_isInsuredPatient;
		//      private CreditAdvice i_CreditAdvice;
		//      private CreditScore i_CreditScore;
		#endregion

		#region Constants

		#endregion
	}
}
