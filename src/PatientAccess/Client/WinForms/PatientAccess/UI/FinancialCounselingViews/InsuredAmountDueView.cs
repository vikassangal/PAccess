using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for InsuredAmountDueView.
	/// </summary>
	//TODO: Create XML summary comment for InsuredAmountDueView
	[Serializable]
	public class InsuredAmountDueView : ControlView
	{
		#region Event Handlers

		private void mtbDeductible_Enter(object sender, EventArgs e)
		{
			decimal deductible = CommonFormatting.ConvertTextToCurrencyDecimal( mtbDeductible );	
			if( deductible == 0 )
			{
				mtbDeductible.UnMaskedText = "0.00";
			}
			else
			{
				mtbDeductible.UnMaskedText = deductible.ToString();
			}           
		}

		private void mtbCoPay_Enter(object sender, EventArgs e)
		{
			decimal coPay = CommonFormatting.ConvertTextToCurrencyDecimal( mtbCoPay );	
			if( coPay == 0 )
			{
				mtbCoPay.UnMaskedText = "0.00";
			}
			else
			{
				mtbCoPay.UnMaskedText = coPay.ToString();
			}           
		}

        private void mtbDeductible_Validating(object sender, CancelEventArgs e)
        {
            if ( mtbDeductible != null )
            {
                CalculateAndDisplayTotal();

                ShowErrorMessageIfTotalOutOfRange( mtbDeductible );
            }
        }

        private void mtbCoPay_Validating(object sender, CancelEventArgs e)
        {
            if (mtbCoPay != null)
            {
                CalculateAndDisplayTotal(); 
                
                ShowErrorMessageIfTotalOutOfRange( mtbCoPay );
            }
        }

		private void mtbDeductible_TextChanged(object sender, EventArgs e)
		{
            Model.CoverageFor(CoverageOrder.PRIMARY_OID).Deductible = CommonFormatting.ConvertTextToCurrencyDecimal( mtbDeductible );	    
		}

		private void mtbCoPay_TextChanged(object sender, EventArgs e)
		{
            Model.CoverageFor(CoverageOrder.PRIMARY_OID).CoPay = CommonFormatting.ConvertTextToCurrencyDecimal( mtbCoPay );
		}

		private void cbxNoLiability_CheckedChanged(object sender, EventArgs e)
		{
			if( cbxNoLiability.Checked )
			{
				DisableLiability();
			}
			else
			{
				EnableLiability();
			}
			Model.HasNoLiability = cbxNoLiability.Checked;
		}

		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			mtbCoPay.Text = Model.PrimaryCopay.ToString( "#,###,##0.00" );
			mtbDeductible.Text = Model.PrimaryDeductible.ToString( "#,###,##0.00" );
			cbxNoLiability.Checked = Model.HasNoLiability;

            CalculateAndDisplayTotal();

            cbxNoLiability.Focus();
		}

        public void CalculateAndDisplayTotal()
        {
            i_NumberFormat.CurrencySymbol = "";

            var coPayAmount = CommonFormatting.ConvertTextToCurrencyDecimal( mtbCoPay );
            var deductibleAmount = CommonFormatting.ConvertTextToCurrencyDecimal( mtbDeductible );
            i_TotalAmount = ( coPayAmount + deductibleAmount );
            lblTotalAmount.Text = i_TotalAmount.ToString( "##,###,##0.00" );

            i_requestedPaymentAmount = ( i_TotalAmount + Convert.ToDecimal( AccountsWithoutPlanTotal ) );
            lblRequestedPaymentAmt.Text = i_requestedPaymentAmount.ToString( "$#,###,###,##0.00" );
            RequestedPayment = i_requestedPaymentAmount;
            TotalCurrentAmountDue = i_TotalAmount;
        }


        private void ShowErrorMessageIfTotalOutOfRange( Control fromControl )
	    {
            if ( i_TotalAmount > MAX_TOTAL_AMMOUNT_DUE )
	        {
	            UIColors.SetErrorBgColor( fromControl );
	            MessageBox.Show( UIErrorMessages.TOTAL_AMOUNT_EXCEED_MAXVALUE, UIErrorMessages.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
	            fromControl.Focus();
	        }

	        else
	        {
	            UIColors.SetNormalBgColor( mtbDeductible );
	            UIColors.SetNormalBgColor( mtbCoPay );
	        }
	    }

	    /// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{   

		}
		#endregion

		#region Properties
		public new Insurance Model
		{
			private get
			{
				return (Insurance)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

		public decimal TotalCurrentAmountDue
		{
			get
			{
				return i_TotalCurrentAmountDue;
			}
			set
			{
				i_TotalCurrentAmountDue = value;
			}
		}

		public decimal RequestedPayment
		{
			get
			{
				return i_RequestedPayment;
			}
			set
			{
				i_RequestedPayment = value;
			}
		}

		public decimal AccountsWithoutPlanTotal
		{
		    private get
			{
				return i_AccountsWithoutPlanTotal;
			}
			set
			{
				i_AccountsWithoutPlanTotal = value;
			}
		}

		public string InsuredsName
		{
			get
			{
				return i_InsuredsName;
			}
			set
			{
				i_InsuredsName = value;
			}
		}
		#endregion

		#region Private Methods
		private void DisableLiability()
		{
			mtbDeductible.Text = "0.00";
			mtbDeductible.Enabled = false;
			mtbCoPay.Text = "0.00";
			mtbCoPay.Enabled = false;
			lblTotalAmount.Text = "0.00";
            UIColors.SetDisabledDarkBgColor(mtbDeductible);
            UIColors.SetDisabledDarkBgColor(mtbCoPay);
            Model.CoverageFor(CoverageOrder.PRIMARY_OID).Deductible = 0m;
            Model.CoverageFor(CoverageOrder.PRIMARY_OID).CoPay = 0m;

            CalculateAndDisplayTotal();
		}

	    public void DisablePatientHasNoLiability()
	    {
	        cbxNoLiability.Enabled = false;
	    }

	    public void EnablePatientHasNoLiability()
	    {
	        cbxNoLiability.Enabled = true;
	    }

		private void EnableLiability()
		{
			mtbDeductible.Enabled = true;
			mtbCoPay.Enabled = true;
            UIColors.SetNormalBgColor(mtbDeductible);
            UIColors.SetNormalBgColor(mtbCoPay);

		}
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.gbCurrentAmountDue = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mtbCoPay = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblCoPay = new System.Windows.Forms.Label();
            this.lblDeductible = new System.Windows.Forms.Label();
            this.cbxNoLiability = new System.Windows.Forms.CheckBox();
            this.lblRequestedPayment = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblRequestedPaymentAmt = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbCurrentAmountDue.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCurrentAmountDue
            // 
            this.gbCurrentAmountDue.Controls.Add(this.label3);
            this.gbCurrentAmountDue.Controls.Add(this.label2);
            this.gbCurrentAmountDue.Controls.Add(this.label1);
            this.gbCurrentAmountDue.Controls.Add(this.mtbCoPay);
            this.gbCurrentAmountDue.Controls.Add(this.mtbDeductible);
            this.gbCurrentAmountDue.Controls.Add(this.lblTotal);
            this.gbCurrentAmountDue.Controls.Add(this.lblCoPay);
            this.gbCurrentAmountDue.Controls.Add(this.lblDeductible);
            this.gbCurrentAmountDue.Controls.Add(this.cbxNoLiability);
            this.gbCurrentAmountDue.Location = new System.Drawing.Point(5, 7);
            this.gbCurrentAmountDue.Name = "gbCurrentAmountDue";
            this.gbCurrentAmountDue.Size = new System.Drawing.Size(219, 121);
            this.gbCurrentAmountDue.TabIndex = 0;
            this.gbCurrentAmountDue.TabStop = false;
            this.gbCurrentAmountDue.Text = "Current amount due (group 3)";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(103, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 14);
            this.label3.TabIndex = 0;
            this.label3.Text = "$";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(103, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "$";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(103, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "$";
            // 
            // mtbCoPay
            // 
            this.mtbCoPay.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoPay.KeyPressExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";  
            this.mtbCoPay.Location = new System.Drawing.Point(121, 67);
            this.mtbCoPay.Mask = "";
            this.mtbCoPay.MaxLength = 12;
            this.mtbCoPay.Name = "mtbCoPay";
            this.mtbCoPay.Size = new System.Drawing.Size(90, 20);
            this.mtbCoPay.TabIndex = 2;
            this.mtbCoPay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCoPay.ValidationExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";  
            this.mtbCoPay.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCoPay_Validating);
            this.mtbCoPay.TextChanged += new System.EventHandler(this.mtbCoPay_TextChanged);
			this.mtbCoPay.Enter += new System.EventHandler(this.mtbCoPay_Enter);
            // 
            // mtbDeductible
            // 
            this.mtbDeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductible.KeyPressExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";  
            this.mtbDeductible.Location = new System.Drawing.Point(121, 42);
            this.mtbDeductible.Mask = "";
            this.mtbDeductible.MaxLength = 12;
            this.mtbDeductible.Name = "mtbDeductible";
            this.mtbDeductible.Size = new System.Drawing.Size(90, 20);
            this.mtbDeductible.TabIndex = 1;
            this.mtbDeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductible.ValidationExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";    
            this.mtbDeductible.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDeductible_Validating);
            this.mtbDeductible.TextChanged += new System.EventHandler(this.mtbDeductible_TextChanged);
			this.mtbDeductible.Enter += new System.EventHandler(this.mtbDeductible_Enter);
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point(8, 96);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(38, 12);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Total:";
            // 
            // lblCoPay
            // 
            this.lblCoPay.Location = new System.Drawing.Point(8, 70);
            this.lblCoPay.Name = "lblCoPay";
            this.lblCoPay.Size = new System.Drawing.Size(78, 13);
            this.lblCoPay.TabIndex = 0;
            this.lblCoPay.Text = "Co-pay/co-ins:";
            // 
            // lblDeductible
            // 
            this.lblDeductible.Location = new System.Drawing.Point(8, 44);
            this.lblDeductible.Name = "lblDeductible";
            this.lblDeductible.Size = new System.Drawing.Size(62, 15);
            this.lblDeductible.TabIndex = 0;
            this.lblDeductible.Text = "Deductible:";
            // 
            // cbxNoLiability
            // 
            this.cbxNoLiability.Location = new System.Drawing.Point(10, 20);
            this.cbxNoLiability.Name = "cbxNoLiability";
            this.cbxNoLiability.Size = new System.Drawing.Size(135, 17);
            this.cbxNoLiability.TabIndex = 0;
            this.cbxNoLiability.Text = "Patient has no liability";
            this.cbxNoLiability.CheckedChanged += new System.EventHandler(this.cbxNoLiability_CheckedChanged);
            // 
            // lblRequestedPayment
            // 
            this.lblRequestedPayment.Location = new System.Drawing.Point(9, 6);
            this.lblRequestedPayment.Name = "lblRequestedPayment";
            this.lblRequestedPayment.Size = new System.Drawing.Size(110, 14);
            this.lblRequestedPayment.TabIndex = 0;
            this.lblRequestedPayment.Text = "Requested payment:";
            // 
            // lblNote
            // 
            this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblNote.Location = new System.Drawing.Point(9, 22);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(119, 14);
            this.lblNote.TabIndex = 0;
            this.lblNote.Text = "Sum of groups 2 and 3";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.Location = new System.Drawing.Point(127, 103);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(86, 13);
            this.lblTotalAmount.TabIndex = 0;
            this.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblRequestedPaymentAmt
            // 
            this.lblRequestedPaymentAmt.Location = new System.Drawing.Point(116, 6);
            this.lblRequestedPaymentAmt.Name = "lblRequestedPaymentAmt";
            this.lblRequestedPaymentAmt.Size = new System.Drawing.Size(92, 13);
            this.lblRequestedPaymentAmt.TabIndex = 0;
            this.lblRequestedPaymentAmt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblNote);
            this.panel1.Controls.Add(this.lblRequestedPaymentAmt);
            this.panel1.Controls.Add(this.lblRequestedPayment);
            this.panel1.Location = new System.Drawing.Point(5, 131);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(219, 39);
            this.panel1.TabIndex = 2;
            // 
            // InsuredAmountDueView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.gbCurrentAmountDue);
            this.Controls.Add(this.panel1);
            this.Name = "InsuredAmountDueView";
            this.Size = new System.Drawing.Size(392, 171);
            this.gbCurrentAmountDue.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public InsuredAmountDueView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
			panel1.BackColor = Color.FromArgb(((Byte)(255)), ((Byte)(192)), ((Byte)(98)));
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
		private GroupBox gbCurrentAmountDue;
		private CheckBox cbxNoLiability;
		private Label lblDeductible;
		private Label lblCoPay;
		private Label lblTotal;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label lblRequestedPayment;
		private Label lblNote;
		private MaskedEditTextBox mtbCoPay;
		private MaskedEditTextBox mtbDeductible;
		private decimal i_AccountsWithoutPlanTotal;
		private decimal i_TotalAmount;
		private decimal i_requestedPaymentAmount;
		private CultureInfo i_US = new CultureInfo("en-US");
		private NumberFormatInfo i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
		// private string i_InsuranceText;
		private Label lblTotalAmount;
		private Label lblRequestedPaymentAmt;
		private Panel panel1;
		private string i_InsuredsName;
		private decimal i_TotalCurrentAmountDue;
		private decimal i_RequestedPayment;
		#endregion

		#region Constants
        private static decimal MAX_TOTAL_AMMOUNT_DUE = 9999999.99m;
		#endregion
	}
}
