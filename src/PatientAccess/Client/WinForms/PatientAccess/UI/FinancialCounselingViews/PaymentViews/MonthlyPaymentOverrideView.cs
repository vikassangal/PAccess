using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for MonthlyPaymentOverrideView.
	/// </summary>
	//TODO: Create XML summary comment for MonthlyPaymentOverrideView
	[Serializable]
	public class MonthlyPaymentOverrideView : ControlView
	{
		#region Event Handlers

        public event EventHandler CheckUserLoginEntering;

        private void cmbNumberMonthlyPayments_SelectedIndexChanged(object sender, EventArgs e)
        {

            this.Model.NumberOfMonthlyPayments = Convert.ToInt16( this.cmbNumberMonthlyPayments.SelectedItem.ToString() );

            this.lblMonthlyPayment.Text = this.Model.MonthlyPayment.ToString( "n", i_NumberFormat );
        }

        private void lblNetBalance_TextChanged(object sender, EventArgs e)
        {
            this.PopulateNumberOfMonthlyPayments( this.Model.BalanceDue );
        }

        private void mtbUserID_TextChanged(object sender, EventArgs e)
        {
            this.CheckUserLoginEntering( this, new LooseArgs( this ) );
        }

        private void mtbPassword_TextChanged(object sender, EventArgs e)
        {
        
            this.CheckUserLoginEntering( this, new LooseArgs( this ) );
        }



		#endregion

		#region Methods
        /// <summary>
        /// UpdateView with data from model
        /// </summary>
        public override void UpdateView()
        {
//            decimal	netBalance;
            
//            netBalance = ( this.Model.TotalCurrentAmtDue - this.Model.AmountCollected );            
//            this.lblNetBalance.Text = netBalance.ToString( "###,###,##0.00" );
            this.lblNetBalance.Text = this.Model.BalanceDue.ToString( "###,###,##0.00" );

            this.cmbNumberMonthlyPayments.SelectedIndex = cmbNumberMonthlyPayments.FindString( this.Model.NumberOfMonthlyPayments.ToString() );
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
            private get
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
        private void PopulateNumberOfMonthlyPayments( decimal balanceDue )
        {
            int numberOfMonths = 0;
            int range = 0;

            if( balanceDue <= 0m )
            {
                numberOfMonths = 0;
            }
            else if( balanceDue > 0m && balanceDue <= 2500m )
            {
                //max of 24 monthly payments
                numberOfMonths = 24;
            }
            else if( balanceDue > 2500 )
            {
                //max of 48 monthly payments
                numberOfMonths = 48;
            }

            this.cmbNumberMonthlyPayments.Items.Clear();

            do
            {
                this.cmbNumberMonthlyPayments.Items.Add( range );
                range++;
            } while ( range <= numberOfMonths );

        }


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblMonthlyPayment = new System.Windows.Forms.Label();
			this.cmbNumberMonthlyPayments = new System.Windows.Forms.ComboBox();
			this.lblNetBalance = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.mtbUserID = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.mtbPassword = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblMonthlyPayment);
			this.groupBox1.Controls.Add(this.cmbNumberMonthlyPayments);
			this.groupBox1.Controls.Add(this.lblNetBalance);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(12, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(404, 114);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Payment terms for current account";
			// 
			// lblMonthlyPayment
			// 
			this.lblMonthlyPayment.Location = new System.Drawing.Point(186, 81);
			this.lblMonthlyPayment.Name = "lblMonthlyPayment";
			this.lblMonthlyPayment.Size = new System.Drawing.Size(108, 17);
			this.lblMonthlyPayment.TabIndex = 5;
			this.lblMonthlyPayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbNumberMonthlyPayments
			// 
			this.cmbNumberMonthlyPayments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbNumberMonthlyPayments.Location = new System.Drawing.Point(176, 51);
			this.cmbNumberMonthlyPayments.Name = "cmbNumberMonthlyPayments";
			this.cmbNumberMonthlyPayments.Size = new System.Drawing.Size(43, 21);
			this.cmbNumberMonthlyPayments.TabIndex = 0;
			this.cmbNumberMonthlyPayments.SelectedIndexChanged += new System.EventHandler(this.cmbNumberMonthlyPayments_SelectedIndexChanged);
			// 
			// lblNetBalance
			// 
			this.lblNetBalance.Location = new System.Drawing.Point(187, 28);
			this.lblNetBalance.Name = "lblNetBalance";
			this.lblNetBalance.Size = new System.Drawing.Size(106, 16);
			this.lblNetBalance.TabIndex = 3;
			this.lblNetBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblNetBalance.TextChanged += new System.EventHandler(this.lblNetBalance_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 14);
			this.label3.TabIndex = 1;
			this.label3.Text = "Monthly payment:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(18, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(155, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Number of monthly payments:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Net balance:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(170, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(11, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "$";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(170, 82);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(11, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "$";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(18, 143);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(84, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "eTenet User ID:";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(18, 168);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(57, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "Password:";
			// 
			// mtbUserID
			// 
			this.mtbUserID.AcceptsReturn = true;
			this.mtbUserID.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.mtbUserID.Location = new System.Drawing.Point(110, 139);
			this.mtbUserID.Mask = "";
			this.mtbUserID.MaxLength = 34;
			this.mtbUserID.Name = "mtbUserID";
			this.mtbUserID.Size = new System.Drawing.Size(214, 20);
			this.mtbUserID.TabIndex = 3;
			this.mtbUserID.TextChanged += new System.EventHandler(this.mtbUserID_TextChanged);
			this.mtbUserID.Enter += new System.EventHandler(this.mtbUserID_Enter);
			// 
			// mtbPassword
			// 
			this.mtbPassword.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.mtbPassword.Location = new System.Drawing.Point(110, 165);
			this.mtbPassword.Mask = "";
			this.mtbPassword.MaxLength = 100;
			this.mtbPassword.Name = "mtbPassword";
			this.mtbPassword.PasswordChar = '*';
			this.mtbPassword.Size = new System.Drawing.Size(214, 20);
			this.mtbPassword.TabIndex = 4;
			this.mtbPassword.TextChanged += new System.EventHandler(this.mtbPassword_TextChanged);
			// 
			// MonthlyPaymentOverrideView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.mtbPassword);
			this.Controls.Add(this.mtbUserID);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.groupBox1);
			this.Name = "MonthlyPaymentOverrideView";
			this.Size = new System.Drawing.Size(437, 197);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public MonthlyPaymentOverrideView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );

			this.cmbNumberMonthlyPayments.Focus();
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
        public MaskedEditTextBox mtbUserID;
        public MaskedEditTextBox mtbPassword;

		private Container components = null;
		private GroupBox groupBox1;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label lblNetBalance;
		private ComboBox cmbNumberMonthlyPayments;
		private Label lblMonthlyPayment;
		private Label label6;
		private Label label7;
		private Label label4;
		private Label label5;
    	private NumberFormatInfo i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
		#endregion

        private void mtbUserID_Enter(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( mtbUserID );
            this.Refresh();
        }

		#region Constants
		#endregion
	}
}
