using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for PaymentView.
	/// </summary>
	[Serializable]
    public class PaymentView : ControlView
	{
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void PaymentView_Enter( object sender, EventArgs e )
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

        private void PaymentView_Load(object sender, EventArgs e)
        {
            int a = 1;
            int b = a + a;    
			this.paymentCalculatorView1.Focus();
        }

        private void previousAccountsWithoutPlanView1_ChangeCursorToDefault(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;       
        }

		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			this.paymentCalculatorView1.Model = this.Model;
			this.paymentCalculatorView1.UpdateView();

			this.paymentCollectedView1.Model = this.Model;
			this.paymentCollectedView1.UpdateView();

			this.previousAccountsWithoutPlanView1.Model = this.Model;
			this.previousAccountsWithoutPlanView1.UpdateView();
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
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

		#region Private Methods

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.paymentCalculatorView1 = new PatientAccess.UI.FinancialCounselingViews.PaymentViews.PaymentCalculatorView();
			this.paymentCollectedView1 = new PatientAccess.UI.FinancialCounselingViews.PaymentViews.PaymentCollectedView();
			this.previousAccountsWithoutPlanView1 = new PatientAccess.UI.FinancialCounselingViews.PaymentViews.PreviousAccountsWithoutPlanView();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// paymentCalculatorView1
			// 
			this.paymentCalculatorView1.BackColor = System.Drawing.Color.White;
			this.paymentCalculatorView1.Location = new System.Drawing.Point(15, 6);
			this.paymentCalculatorView1.Model = null;
			this.paymentCalculatorView1.Name = "paymentCalculatorView1";
			this.paymentCalculatorView1.Size = new System.Drawing.Size(639, 96);
			this.paymentCalculatorView1.TabIndex = 0;
			this.paymentCalculatorView1.TotalCurrentAmtDue = new System.Decimal(new int[] {
																							  0,
																							  0,
																							  0,
																							  0});
			// 
			// paymentCollectedView1
			// 
			this.paymentCollectedView1.BackColor = System.Drawing.Color.White;
			this.paymentCollectedView1.Location = new System.Drawing.Point(8, 126);
			this.paymentCollectedView1.Model = null;
			this.paymentCollectedView1.Name = "paymentCollectedView1";
			this.paymentCollectedView1.PaymentService = null;
			this.paymentCollectedView1.Size = new System.Drawing.Size(479, 220);
			this.paymentCollectedView1.TabIndex = 1;
			// 
			// previousAccountsWithoutPlanView1
			// 
			this.previousAccountsWithoutPlanView1.BackColor = System.Drawing.Color.White;
			this.previousAccountsWithoutPlanView1.Location = new System.Drawing.Point(504, 121);
			this.previousAccountsWithoutPlanView1.Model = null;
			this.previousAccountsWithoutPlanView1.Name = "previousAccountsWithoutPlanView1";
			this.previousAccountsWithoutPlanView1.Size = new System.Drawing.Size(379, 255);
			this.previousAccountsWithoutPlanView1.TabIndex = 2;
			this.previousAccountsWithoutPlanView1.ChangeCursorToDefault += new System.EventHandler(this.previousAccountsWithoutPlanView1_ChangeCursorToDefault);
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label1.Location = new System.Drawing.Point(11, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(981, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "_________________________________________________________________________________" +
				"________________________________________________________________________________" +
				"_____";
			// 
			// PaymentView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.paymentCalculatorView1);
			this.Controls.Add(this.paymentCollectedView1);
			this.Controls.Add(this.previousAccountsWithoutPlanView1);
			this.Name = "PaymentView";
			this.Size = new System.Drawing.Size(989, 378);
            this.Enter += new System.EventHandler(this.PaymentView_Enter);
			this.Load += new System.EventHandler(this.PaymentView_Load);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PaymentView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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
		private PaymentCalculatorView paymentCalculatorView1;
		private PaymentCollectedView paymentCollectedView1;
		private Label label1;
		private PreviousAccountsWithoutPlanView previousAccountsWithoutPlanView1;
		#endregion

        
		#region Constants
		#endregion
	}
}
