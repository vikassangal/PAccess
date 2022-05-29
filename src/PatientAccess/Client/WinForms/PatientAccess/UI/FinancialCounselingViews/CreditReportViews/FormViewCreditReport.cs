using System;
using System.ComponentModel;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

//using PatientAccess.BrokerInterfaces;


namespace PatientAccess.UI.FinancialCounselingViews.CreditReportViews
{
	/// <summary>
	/// Summary description for FormViewCreditReport.
	/// </summary>
	//TODO: Create XML summary comment for FormViewCreditReport
	[Serializable]
	public class FormViewCreditReport : TimeOutFormView
	{
		#region Event Handlers
		#endregion

		#region Methods
		public override void UpdateView()
		{
			if( this.Model != null )
			{
				creditReportView1.Model = this.Model;
				creditReportView1.UpdateView();
			}
		}
		#endregion

		#region Properties
		public new Guarantor Model
		{
			private get
			{
				return (Guarantor)base.Model;
			}
			set
			{
				base.Model = value;
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
            this.creditReportView1 = new PatientAccess.UI.FinancialCounselingViews.CreditReportViews.CreditReportView();
            this.btnOk = new LoggingButton();
            this.SuspendLayout();
            // 
            // creditReportView1
            // 
            this.creditReportView1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.creditReportView1.Location = new System.Drawing.Point(-2, 0);
            this.creditReportView1.Model = null;
            this.creditReportView1.Name = "creditReportView1";
            this.creditReportView1.Size = new System.Drawing.Size(875, 490);
            this.creditReportView1.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(793, 493);
            this.btnOk.Name = "btnOk";
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            // 
            // FormViewCreditReport
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(879, 522);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.creditReportView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormViewCreditReport";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Credit Report";
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public FormViewCreditReport()
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
		private CreditReportView creditReportView1;
		private LoggingButton btnOk;
//		private CreditReport i_CreditReport;
		#endregion

		#region Constants
		#endregion
	}
}
