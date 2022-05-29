using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for PaymentCalculatorView.
	/// </summary>
	[Serializable]
	public class PaymentCalculatorView : ControlView
	{
		#region Event Handlers
		private void cmbPercentage_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.CalculatePercentDue();
		}
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			this.CheckFinancialClass();

			this.lblTotalCurrentAmtDue.Text = this.Model.TotalCurrentAmtDue.ToString( "n", i_NumberFormat );
			this.lblRequestedPayment.Text = this.Model.RequestedPayment.ToString( "n", i_NumberFormat );
			this.lblCurrentAmtPercent.Text = this.Model.TotalCurrentAmtDue.ToString( "n", i_NumberFormat );
			this.cmbPercentage.SelectedIndex = 0;
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

		public decimal TotalCurrentAmtDue
		{
			get
			{
				return i_TotalCurrentAmtDue;
			}
			set
			{
				i_TotalCurrentAmtDue = value;
			}
		}
		#endregion

		#region Private Methods
		private void CalculatePercentDue()
		{
			decimal	calcCurrentAmtDue = 1010;
			int		percentage;

			percentage = Convert.ToInt16( this.cmbPercentage.SelectedItem.ToString() );
			calcCurrentAmtDue = ( ( this.Model.TotalCurrentAmtDue * percentage ) / 100 );
			this.lblCurrentAmtPercent.Text =  calcCurrentAmtDue.ToString( "n", i_NumberFormat );
		}

		private void CheckFinancialClass()
		{
            if( this.Model.FinancialClass != null )
			{
				if( IsUninsured( this.Model.FinancialClass ) )
				{
					this.lblPatient.Text = "Uninsured Patient";
				}
				else
				{
					this.lblPatient.Text = "Insured Patient";
				}
			}
		}

		private static bool IsUninsured( FinancialClass aFinancialClass )
		{
			ArrayList unInsuredFinancialClasses = new ArrayList();
			//TODO: Obtain these values from a broker
			unInsuredFinancialClasses.Add( "70" );
			unInsuredFinancialClasses.Add( "72" );
			unInsuredFinancialClasses.Add( "73" );
			unInsuredFinancialClasses.Add( "96" );

			return unInsuredFinancialClasses.Contains( aFinancialClass.Code );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gbxCalculator = new System.Windows.Forms.GroupBox();
			this.lblCurrentAmtPercent = new System.Windows.Forms.Label();
			this.cmbPercentage = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lblRequestedPayment = new System.Windows.Forms.Label();
			this.lblTotalCurrentAmtDue = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblPatient = new System.Windows.Forms.Label();
			this.gbxCalculator.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbxCalculator
			// 
			this.gbxCalculator.Controls.Add(this.lblCurrentAmtPercent);
			this.gbxCalculator.Controls.Add(this.cmbPercentage);
			this.gbxCalculator.Controls.Add(this.label1);
			this.gbxCalculator.Location = new System.Drawing.Point(268, 28);
			this.gbxCalculator.Name = "gbxCalculator";
			this.gbxCalculator.Size = new System.Drawing.Size(364, 60);
			this.gbxCalculator.TabIndex = 1;
			this.gbxCalculator.TabStop = false;
			this.gbxCalculator.Text = "Payment calculator";
			// 
			// lblCurrentAmtPercent
			// 
			this.lblCurrentAmtPercent.Location = new System.Drawing.Point(244, 27);
			this.lblCurrentAmtPercent.Name = "lblCurrentAmtPercent";
			this.lblCurrentAmtPercent.Size = new System.Drawing.Size(109, 17);
			this.lblCurrentAmtPercent.TabIndex = 0;
			this.lblCurrentAmtPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbPercentage
			// 
			this.cmbPercentage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPercentage.Items.AddRange(new object[] {
															   "100",
															   "95",
															   "90",
															   "85",
															   "80",
															   "75",
															   "70",
															   "65",
															   "60",
															   "55",
															   "50",
															   "45",
															   "40",
															   "35",
															   "30",
															   "25",
															   "20",
															   "15",
															   "10",
															   "5"});
			this.cmbPercentage.Location = new System.Drawing.Point(11, 23);
			this.cmbPercentage.Name = "cmbPercentage";
			this.cmbPercentage.Size = new System.Drawing.Size(52, 21);
			this.cmbPercentage.TabIndex = 1;
			this.cmbPercentage.SelectedIndexChanged += new System.EventHandler(this.cmbPercentage_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(64, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(180, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "%    of total current amount due = $";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Requested payment:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(135, 14);
			this.label3.TabIndex = 0;
			this.label3.Text = "Total current amount due:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(135, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(9, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "$";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(135, 29);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(9, 12);
			this.label5.TabIndex = 0;
			this.label5.Text = "$";
			// 
			// lblRequestedPayment
			// 
			this.lblRequestedPayment.Location = new System.Drawing.Point(146, 9);
			this.lblRequestedPayment.Name = "lblRequestedPayment";
			this.lblRequestedPayment.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lblRequestedPayment.Size = new System.Drawing.Size(100, 17);
			this.lblRequestedPayment.TabIndex = 0;
			// 
			// lblTotalCurrentAmtDue
			// 
			this.lblTotalCurrentAmtDue.Location = new System.Drawing.Point(146, 29);
			this.lblTotalCurrentAmtDue.Name = "lblTotalCurrentAmtDue";
			this.lblTotalCurrentAmtDue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lblTotalCurrentAmtDue.Size = new System.Drawing.Size(100, 17);
			this.lblTotalCurrentAmtDue.TabIndex = 6;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.lblRequestedPayment);
			this.panel1.Controls.Add(this.lblTotalCurrentAmtDue);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Location = new System.Drawing.Point(4, 35);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(255, 52);
			this.panel1.TabIndex = 7;
			// 
			// lblPatient
			// 
			this.lblPatient.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((System.Byte)(0)));
			this.lblPatient.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(153)), ((System.Byte)(0)));
			this.lblPatient.Location = new System.Drawing.Point(3, 4);
			this.lblPatient.Name = "lblPatient";
			this.lblPatient.Size = new System.Drawing.Size(190, 26);
			this.lblPatient.TabIndex = 8;
			this.lblPatient.Text = "Insured Patient";
			// 
			// PaymentCalculatorView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.lblPatient);
			this.Controls.Add(this.gbxCalculator);
			this.Controls.Add(this.panel1);
			this.Name = "PaymentCalculatorView";
			this.Size = new System.Drawing.Size(639, 93);
			this.gbxCalculator.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PaymentCalculatorView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
			this.panel1.BackColor = Color.FromArgb(((Byte)(255)), ((Byte)(192)), ((Byte)(98)));
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
		private GroupBox gbxCalculator;
		private Label label1;
		private ComboBox cmbPercentage;
		private Label lblCurrentAmtPercent;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label lblRequestedPayment;
		private Label lblTotalCurrentAmtDue;
		private decimal i_TotalCurrentAmtDue;
		private NumberFormatInfo i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
		private Panel panel1;
		private Label lblPatient;
		#endregion

		#region Constants
		#endregion
	}
}
