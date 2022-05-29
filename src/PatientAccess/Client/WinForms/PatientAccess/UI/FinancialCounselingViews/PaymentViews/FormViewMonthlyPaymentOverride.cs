using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for FormViewMonthlyPaymentOverride.
	/// </summary>
	//TODO: Create XML summary comment for FormViewMonthlyPaymentOverride
	[Serializable]
	public class FormViewMonthlyPaymentOverride : TimeOutFormView
	{
		#region Event Handlers        
        private void btnCancel_Click(object sender, EventArgs e)
        {			

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if( ValidateUserRole() )
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }            
            else
            {
                UIColors.SetErrorBgColor( monthlyPaymentOverrideView1.mtbUserID );
                this.monthlyPaymentOverrideView1.mtbPassword.UnMaskedText = "";

                MessageBox.Show( UIErrorMessages.INVALID_PAYMENTOVERRIDE_USER_ERRMSG, UIErrorMessages.LOGIN_FAILURE_MESSAGE_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
        }

        private void monthlyPaymentOverrideView1_CheckUserLoginEntering(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            MonthlyPaymentOverrideView monthlyPaymentOverrideView = args.Context as MonthlyPaymentOverrideView;

            if( monthlyPaymentOverrideView.mtbUserID.TextLength > 0 && monthlyPaymentOverrideView.mtbPassword.TextLength > 0 ) 
            {
                this.btnOk.Enabled = true;
            }
            else
            {
                this.btnOk.Enabled = false;
            }
        }

		#endregion

		#region Methods
        /// <summary>
        /// UpdateView with data from model
        /// </summary>
        public override void UpdateView()
        {
            this.monthlyPaymentOverrideView1.Model = this.Model;
            this.monthlyPaymentOverrideView1.UpdateView();
        }

        /// <summary>
        /// UpdateModel method
        /// </summary>
        public override void UpdateModel()
        {   
			
        }

		#endregion

		#region Properties
        public new Account Model
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

        public PaymentService PaymentService
        {
            private get
            {
                return i_PaymentService;
            }
            set
            {
                i_PaymentService = value;
            }
        }

 		#endregion

		#region Private Methods
        private bool ValidateUserRole()
        {
            bool hasPermission = false;
            string userName = this.monthlyPaymentOverrideView1.mtbUserID.Text;
            string password = this.monthlyPaymentOverrideView1.mtbPassword.Text;
            Facility facility = this.Model.Facility;
            hasPermission = this.PaymentService.HasPermissionToOverrideMonthlyPayment( userName, password, facility );
            return hasPermission;
        }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCancel = new LoggingButton();
			this.btnOk = new LoggingButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.monthlyPaymentOverrideView1 = new PatientAccess.UI.FinancialCounselingViews.PaymentViews.MonthlyPaymentOverrideView();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(374, 218);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.BackColor = System.Drawing.SystemColors.Control;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(290, 219);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.monthlyPaymentOverrideView1);
			this.panel1.Location = new System.Drawing.Point(15, 15);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(433, 194);
			this.panel1.TabIndex = 0;
			// 
			// monthlyPaymentOverrideView1
			// 
			this.monthlyPaymentOverrideView1.BackColor = System.Drawing.Color.White;
			this.monthlyPaymentOverrideView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monthlyPaymentOverrideView1.Location = new System.Drawing.Point(0, 0);
			this.monthlyPaymentOverrideView1.Model = null;
			this.monthlyPaymentOverrideView1.Name = "monthlyPaymentOverrideView1";
			this.monthlyPaymentOverrideView1.Size = new System.Drawing.Size(431, 192);
			this.monthlyPaymentOverrideView1.TabIndex = 0;
			this.monthlyPaymentOverrideView1.CheckUserLoginEntering += new System.EventHandler(this.monthlyPaymentOverrideView1_CheckUserLoginEntering);
			// 
			// FormViewMonthlyPaymentOverride
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(464, 251);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormViewMonthlyPaymentOverride";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Override Number of Monthly Payments";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties


        
        #endregion

		#region Construction and Finalization
		public FormViewMonthlyPaymentOverride()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
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
		private LoggingButton btnCancel;
		private Panel panel1;
		private MonthlyPaymentOverrideView monthlyPaymentOverrideView1;
		private LoggingButton btnOk;
        private PaymentService i_PaymentService;

		#endregion

		#region Constants
		#endregion
	}
}
