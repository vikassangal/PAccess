using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for UnInsuredAmountDueView.
	/// </summary>
	//TODO: Create XML summary comment for UnInsuredAmountDueView
	[Serializable]
	public class UnInsuredAmountDueView : ControlView
	{
		#region Event Handlers
        private void mtbEstimate_Enter(object sender, EventArgs e)
        {
            decimal estimate = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbEstimate );	
            if( estimate == 0 )
            {
                this.mtbEstimate.UnMaskedText = "0.00";
            }
            else
            {
                this.mtbEstimate.UnMaskedText = estimate.ToString();
            }           
        }

        private void mtbEstimate_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( this.mtbEstimate );                           
        }

		private void mtbEstimate_TextChanged(object sender, EventArgs e)
		{
            this.CalculateRequestedPayment();     
		}

		private void cbxNoLiability_CheckedChanged(object sender, EventArgs e)
		{
			if( this.cbxNoLiability.Checked )
			{
				this.DisableLiability();
				this.Model.HasNoLiability = true;
			}
			else
			{
				this.EnableLiability();
				this.Model.HasNoLiability = false;
			}
		}

		private void rbYes_CheckedChanged(object sender, EventArgs e)
		{
		    this.ResourceListProvided.SetYes();
		}

		private void rbNo_CheckedChanged(object sender, EventArgs e)
		{
		    this.ResourceListProvided.SetNo();
		}
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			this.mtbEstimate.Text = this.EstimatedAmountDue.ToString( "###,###,##0.00" );
			this.cbxNoLiability.Checked = this.Model.HasNoLiability;
		    if( this.ResourceListProvided.Code == YesNoFlag.CODE_YES )
		    {
		        this.rbYes.Checked = true;
		    }
		    else
		    {
		        this.rbNo.Checked = true;
		    }

            this.CalculateRequestedPayment();

            this.cbxNoLiability.Focus();
		}

        public void CalculateRequestedPayment()
        {
            i_NumberFormat.CurrencySymbol = "";
            i_EstimatedAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbEstimate );	            
            i_RequestedPaymentAmount = ( i_EstimatedAmount + Convert.ToDecimal( this.AccountsWithoutPlanTotal ) );

            this.lblRequestedPaymentAmt.Text = i_RequestedPaymentAmount.ToString( "$#,###,###,##0.00" );

            this.RequestedPayment = Convert.ToDecimal( this.i_RequestedPaymentAmount );
            this.EstimatedAmountDue = i_EstimatedAmount;
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

		public decimal EstimatedAmountDue
		{
			get
			{
				return i_EstimatedAmountDue;
			}
			set
			{
				i_EstimatedAmountDue = value;
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
        public YesNoFlag ResourceListProvided
        {
            get
            {
                return i_ResourceListProvided;
            }
            set
            {
                i_ResourceListProvided = value;
            }
        }
		#endregion

		#region Private Methods
		private void DisableLiability()
		{
			this.mtbEstimate.Text = "0.00";
			this.mtbEstimate.Enabled = false;
            
            this.CalculateRequestedPayment();
		}

		private void EnableLiability()
		{
			this.mtbEstimate.Enabled = true;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.gbxAmountDue = new System.Windows.Forms.GroupBox();
            this.mtbEstimate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxNoLiability = new System.Windows.Forms.CheckBox();
            this.lblEstimate = new System.Windows.Forms.Label();
            this.lblRequestedPayment = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblResourceList = new System.Windows.Forms.Label();
            this.rbYes = new System.Windows.Forms.RadioButton();
            this.rbNo = new System.Windows.Forms.RadioButton();
            this.lblRequestedPaymentAmt = new System.Windows.Forms.Label();
            this.gbxAmountDue.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxAmountDue
            // 
            this.gbxAmountDue.Controls.Add(this.mtbEstimate);
            this.gbxAmountDue.Controls.Add(this.label1);
            this.gbxAmountDue.Controls.Add(this.cbxNoLiability);
            this.gbxAmountDue.Controls.Add(this.lblEstimate);
            this.gbxAmountDue.Location = new System.Drawing.Point(5, 7);
            this.gbxAmountDue.Name = "gbxAmountDue";
            this.gbxAmountDue.Size = new System.Drawing.Size(231, 74);
            this.gbxAmountDue.TabIndex = 0;
            this.gbxAmountDue.TabStop = false;
            this.gbxAmountDue.Text = "Current amount due (group 3)";
            // 
            // mtbEstimate
            // 
            this.mtbEstimate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbEstimate.KeyPressExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbEstimate.Location = new System.Drawing.Point(135, 40);
            this.mtbEstimate.Mask = "";
            this.mtbEstimate.MaxLength = 14;
            this.mtbEstimate.Name = "mtbEstimate";
            this.mtbEstimate.Size = new System.Drawing.Size(84, 20);
            this.mtbEstimate.TabIndex = 2;
            this.mtbEstimate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbEstimate.ValidationExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbEstimate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEstimate_Validating);
            this.mtbEstimate.TextChanged += new System.EventHandler(this.mtbEstimate_TextChanged);
            this.mtbEstimate.Enter += new System.EventHandler(this.mtbEstimate_Enter);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(114, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "$";
            // 
            // cbxNoLiability
            // 
            this.cbxNoLiability.Location = new System.Drawing.Point(11, 20);
            this.cbxNoLiability.Name = "cbxNoLiability";
            this.cbxNoLiability.Size = new System.Drawing.Size(145, 18);
            this.cbxNoLiability.TabIndex = 1;
            this.cbxNoLiability.Text = "Patient has no liability";
            this.cbxNoLiability.CheckedChanged += new System.EventHandler(this.cbxNoLiability_CheckedChanged);
            // 
            // lblEstimate
            // 
            this.lblEstimate.Location = new System.Drawing.Point(9, 43);
            this.lblEstimate.Name = "lblEstimate";
            this.lblEstimate.Size = new System.Drawing.Size(51, 14);
            this.lblEstimate.TabIndex = 0;
            this.lblEstimate.Text = "Estimate:";
            // 
            // lblRequestedPayment
            // 
            this.lblRequestedPayment.Location = new System.Drawing.Point(15, 100);
            this.lblRequestedPayment.Name = "lblRequestedPayment";
            this.lblRequestedPayment.Size = new System.Drawing.Size(110, 14);
            this.lblRequestedPayment.TabIndex = 0;
            this.lblRequestedPayment.Text = "Requested payment:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Sum of groups 2 and 3";
            // 
            // lblResourceList
            // 
            this.lblResourceList.Location = new System.Drawing.Point(15, 145);
            this.lblResourceList.Name = "lblResourceList";
            this.lblResourceList.Size = new System.Drawing.Size(119, 16);
            this.lblResourceList.TabIndex = 0;
            this.lblResourceList.Text = "Resource list provided:";
            // 
            // rbYes
            // 
            this.rbYes.Location = new System.Drawing.Point(134, 145);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(43, 14);
            this.rbYes.TabIndex = 2;
            this.rbYes.Text = "Yes";
            this.rbYes.CheckedChanged += new System.EventHandler(this.rbYes_CheckedChanged);
            // 
            // rbNo
            // 
            this.rbNo.Location = new System.Drawing.Point(180, 145);
            this.rbNo.Name = "rbNo";
            this.rbNo.Size = new System.Drawing.Size(43, 14);
            this.rbNo.TabIndex = 3;
            this.rbNo.Text = "No";
            this.rbNo.CheckedChanged += new System.EventHandler(this.rbNo_CheckedChanged);
            // 
            // lblRequestedPaymentAmt
            // 
            this.lblRequestedPaymentAmt.Location = new System.Drawing.Point(119, 101);
            this.lblRequestedPaymentAmt.Name = "lblRequestedPaymentAmt";
            this.lblRequestedPaymentAmt.Size = new System.Drawing.Size(101, 13);
            this.lblRequestedPaymentAmt.TabIndex = 0;
            this.lblRequestedPaymentAmt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // UnInsuredAmountDueView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblRequestedPaymentAmt);
            this.Controls.Add(this.rbNo);
            this.Controls.Add(this.rbYes);
            this.Controls.Add(this.lblResourceList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblRequestedPayment);
            this.Controls.Add(this.gbxAmountDue);
            this.Name = "UnInsuredAmountDueView";
            this.Size = new System.Drawing.Size(244, 172);
            this.gbxAmountDue.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public UnInsuredAmountDueView()
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
		private GroupBox gbxAmountDue;
		private Label lblEstimate;
		private CheckBox cbxNoLiability;
		private Label label1;
		private Label lblRequestedPayment;
		private Label label2;
		private Label lblResourceList;
		private RadioButton rbYes;
		private MaskedEditTextBox mtbEstimate;
		private RadioButton rbNo;
		private decimal i_AccountsWithoutPlanTotal;
		private CultureInfo i_US = new CultureInfo("en-US");
		private NumberFormatInfo i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
		private decimal i_EstimatedAmount;
		private Label lblRequestedPaymentAmt;
		private decimal i_RequestedPaymentAmount;
		private decimal i_EstimatedAmountDue;
		private decimal i_RequestedPayment;
        private YesNoFlag i_ResourceListProvided;
		#endregion

		#region Constants
		#endregion
	}
}
