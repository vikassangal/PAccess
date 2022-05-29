namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    partial class DebugBenResponseCatDetails
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblDollar1 = new System.Windows.Forms.Label();
            this.lblRemainingBenefitPerVisit = new System.Windows.Forms.Label();
            this.lblRemainingLifetimeValue = new System.Windows.Forms.Label();
            this.mtbRemainingBenefitPerVisit = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemainingLifetimeValue = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDeductible = new System.Windows.Forms.Label();
            this.mtbDeductibleDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbMaxBenefitPerVisitMet = new System.Windows.Forms.ComboBox();
            this.lblStaticMaxBenefitVisitMet = new System.Windows.Forms.Label();
            this.mtbMaxBenefitPerVisit = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMaxBenefitVisit = new System.Windows.Forms.Label();
            this.cmbCoPayWaiveIfAdmitted = new System.Windows.Forms.ComboBox();
            this.cmbOutOfPocketMet = new System.Windows.Forms.ComboBox();
            this.cmbLifetimeMaxBenefitMet = new System.Windows.Forms.ComboBox();
            this.mtbMaxBenefitAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbNumberVisitsPerYear = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCoPayAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbPercentOutOfPocket = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOutOfPocketDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOutOfPocketAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticOutOfPocket = new System.Windows.Forms.Label();
            this.mtbCoInsuranceAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticPercent1 = new System.Windows.Forms.Label();
            this.cmbDeductibleMet = new System.Windows.Forms.ComboBox();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.lblStaticMaxBenefit = new System.Windows.Forms.Label();
            this.lblStaticMaxBenefitMet = new System.Windows.Forms.Label();
            this.lblStaticCoPay = new System.Windows.Forms.Label();
            this.lblStaticWaive = new System.Windows.Forms.Label();
            this.lblStaticVisitsPerYear = new System.Windows.Forms.Label();
            this.lblStaticPaymentMet = new System.Windows.Forms.Label();
            this.lblStaticOutOfPocketDollarsMet = new System.Windows.Forms.Label();
            this.lblStaticAfterOutOfPocket = new System.Windows.Forms.Label();
            this.lblStaticPercent2 = new System.Windows.Forms.Label();
            this.lblStaticTime = new System.Windows.Forms.Label();
            this.lblStaticMet = new System.Windows.Forms.Label();
            this.lblStaticDollarsMet = new System.Windows.Forms.Label();
            this.lblStaticCoInsurance = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lineLabelBenefits1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabelBenefits3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabelBenefits2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 617, 170 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 13, 13 );
            this.label1.TabIndex = 47;
            this.label1.Text = "$";
            // 
            // lblDollar1
            // 
            this.lblDollar1.AutoSize = true;
            this.lblDollar1.Location = new System.Drawing.Point( 617, 143 );
            this.lblDollar1.Name = "lblDollar1";
            this.lblDollar1.Size = new System.Drawing.Size( 13, 13 );
            this.lblDollar1.TabIndex = 48;
            this.lblDollar1.Text = "$";
            // 
            // lblRemainingBenefitPerVisit
            // 
            this.lblRemainingBenefitPerVisit.AutoSize = true;
            this.lblRemainingBenefitPerVisit.Location = new System.Drawing.Point( 486, 170 );
            this.lblRemainingBenefitPerVisit.Name = "lblRemainingBenefitPerVisit";
            this.lblRemainingBenefitPerVisit.Size = new System.Drawing.Size( 118, 13 );
            this.lblRemainingBenefitPerVisit.TabIndex = 49;
            this.lblRemainingBenefitPerVisit.Text = "Remaining benefit/visit:";
            // 
            // lblRemainingLifetimeValue
            // 
            this.lblRemainingLifetimeValue.AutoSize = true;
            this.lblRemainingLifetimeValue.Location = new System.Drawing.Point( 486, 143 );
            this.lblRemainingLifetimeValue.Name = "lblRemainingLifetimeValue";
            this.lblRemainingLifetimeValue.Size = new System.Drawing.Size( 124, 13 );
            this.lblRemainingLifetimeValue.TabIndex = 46;
            this.lblRemainingLifetimeValue.Text = "Remaining lifetime value:";
            // 
            // mtbRemainingBenefitPerVisit
            // 
            this.mtbRemainingBenefitPerVisit.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemainingBenefitPerVisit.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingBenefitPerVisit.Location = new System.Drawing.Point( 632, 167 );
            this.mtbRemainingBenefitPerVisit.Mask = "";
            this.mtbRemainingBenefitPerVisit.MaxLength = 14;
            this.mtbRemainingBenefitPerVisit.Name = "mtbRemainingBenefitPerVisit";
            this.mtbRemainingBenefitPerVisit.Size = new System.Drawing.Size( 101, 20 );
            this.mtbRemainingBenefitPerVisit.TabIndex = 66;
            this.mtbRemainingBenefitPerVisit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingBenefitPerVisit.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            // 
            // mtbRemainingLifetimeValue
            // 
            this.mtbRemainingLifetimeValue.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemainingLifetimeValue.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingLifetimeValue.Location = new System.Drawing.Point( 632, 140 );
            this.mtbRemainingLifetimeValue.Mask = "";
            this.mtbRemainingLifetimeValue.MaxLength = 14;
            this.mtbRemainingLifetimeValue.Name = "mtbRemainingLifetimeValue";
            this.mtbRemainingLifetimeValue.Size = new System.Drawing.Size( 101, 20 );
            this.mtbRemainingLifetimeValue.TabIndex = 63;
            this.mtbRemainingLifetimeValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingLifetimeValue.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            // 
            // mtbDeductible
            // 
            this.mtbDeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductible.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductible.Location = new System.Drawing.Point( 318, 14 );
            this.mtbDeductible.Mask = "";
            this.mtbDeductible.MaxLength = 10;
            this.mtbDeductible.Name = "mtbDeductible";
            this.mtbDeductible.Size = new System.Drawing.Size( 60, 20 );
            this.mtbDeductible.TabIndex = 50;
            this.mtbDeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductible.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            // 
            // lblStaticDeductible
            // 
            this.lblStaticDeductible.Location = new System.Drawing.Point( 246, 17 );
            this.lblStaticDeductible.Name = "lblStaticDeductible";
            this.lblStaticDeductible.Size = new System.Drawing.Size( 77, 23 );
            this.lblStaticDeductible.TabIndex = 43;
            this.lblStaticDeductible.Text = "Deductible:  $";
            // 
            // mtbDeductibleDollarsMet
            // 
            this.mtbDeductibleDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductibleDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductibleDollarsMet.Location = new System.Drawing.Point( 682, 14 );
            this.mtbDeductibleDollarsMet.Mask = "";
            this.mtbDeductibleDollarsMet.MaxLength = 10;
            this.mtbDeductibleDollarsMet.Name = "mtbDeductibleDollarsMet";
            this.mtbDeductibleDollarsMet.Size = new System.Drawing.Size( 60, 20 );
            this.mtbDeductibleDollarsMet.TabIndex = 53;
            this.mtbDeductibleDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductibleDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            // 
            // cmbMaxBenefitPerVisitMet
            // 
            this.cmbMaxBenefitPerVisitMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaxBenefitPerVisitMet.Location = new System.Drawing.Point( 801, 171 );
            this.cmbMaxBenefitPerVisitMet.Name = "cmbMaxBenefitPerVisitMet";
            this.cmbMaxBenefitPerVisitMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbMaxBenefitPerVisitMet.TabIndex = 67;
            // 
            // lblStaticMaxBenefitVisitMet
            // 
            this.lblStaticMaxBenefitVisitMet.Location = new System.Drawing.Point( 769, 174 );
            this.lblStaticMaxBenefitVisitMet.Name = "lblStaticMaxBenefitVisitMet";
            this.lblStaticMaxBenefitVisitMet.Size = new System.Drawing.Size( 35, 23 );
            this.lblStaticMaxBenefitVisitMet.TabIndex = 44;
            this.lblStaticMaxBenefitVisitMet.Text = "Met:";
            // 
            // mtbMaxBenefitPerVisit
            // 
            this.mtbMaxBenefitPerVisit.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbMaxBenefitPerVisit.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitPerVisit.Location = new System.Drawing.Point( 361, 171 );
            this.mtbMaxBenefitPerVisit.Mask = "";
            this.mtbMaxBenefitPerVisit.MaxLength = 14;
            this.mtbMaxBenefitPerVisit.Name = "mtbMaxBenefitPerVisit";
            this.mtbMaxBenefitPerVisit.Size = new System.Drawing.Size( 88, 20 );
            this.mtbMaxBenefitPerVisit.TabIndex = 65;
            this.mtbMaxBenefitPerVisit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMaxBenefitPerVisit.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            // 
            // lblStaticMaxBenefitVisit
            // 
            this.lblStaticMaxBenefitVisit.Location = new System.Drawing.Point( 246, 174 );
            this.lblStaticMaxBenefitVisit.Name = "lblStaticMaxBenefitVisit";
            this.lblStaticMaxBenefitVisit.Size = new System.Drawing.Size( 121, 23 );
            this.lblStaticMaxBenefitVisit.TabIndex = 45;
            this.lblStaticMaxBenefitVisit.Text = "Max benefit/visit:       $";
            // 
            // cmbCoPayWaiveIfAdmitted
            // 
            this.cmbCoPayWaiveIfAdmitted.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoPayWaiveIfAdmitted.Location = new System.Drawing.Point( 470, 96 );
            this.cmbCoPayWaiveIfAdmitted.Name = "cmbCoPayWaiveIfAdmitted";
            this.cmbCoPayWaiveIfAdmitted.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoPayWaiveIfAdmitted.TabIndex = 60;
            // 
            // cmbOutOfPocketMet
            // 
            this.cmbOutOfPocketMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutOfPocketMet.Location = new System.Drawing.Point( 438, 56 );
            this.cmbOutOfPocketMet.Name = "cmbOutOfPocketMet";
            this.cmbOutOfPocketMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbOutOfPocketMet.TabIndex = 56;
            // 
            // cmbLifetimeMaxBenefitMet
            // 
            this.cmbLifetimeMaxBenefitMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLifetimeMaxBenefitMet.Location = new System.Drawing.Point( 801, 140 );
            this.cmbLifetimeMaxBenefitMet.Name = "cmbLifetimeMaxBenefitMet";
            this.cmbLifetimeMaxBenefitMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbLifetimeMaxBenefitMet.TabIndex = 64;
            // 
            // mtbMaxBenefitAmount
            // 
            this.mtbMaxBenefitAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.Default;
            this.mtbMaxBenefitAmount.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitAmount.Location = new System.Drawing.Point( 361, 141 );
            this.mtbMaxBenefitAmount.Mask = "";
            this.mtbMaxBenefitAmount.MaxLength = 14;
            this.mtbMaxBenefitAmount.Name = "mtbMaxBenefitAmount";
            this.mtbMaxBenefitAmount.Size = new System.Drawing.Size( 88, 20 );
            this.mtbMaxBenefitAmount.TabIndex = 62;
            this.mtbMaxBenefitAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMaxBenefitAmount.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            // 
            // mtbNumberVisitsPerYear
            // 
            this.mtbNumberVisitsPerYear.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbNumberVisitsPerYear.KeyPressExpression = "^\\d*$";
            this.mtbNumberVisitsPerYear.Location = new System.Drawing.Point( 590, 97 );
            this.mtbNumberVisitsPerYear.Mask = "";
            this.mtbNumberVisitsPerYear.MaxLength = 3;
            this.mtbNumberVisitsPerYear.Name = "mtbNumberVisitsPerYear";
            this.mtbNumberVisitsPerYear.Size = new System.Drawing.Size( 30, 20 );
            this.mtbNumberVisitsPerYear.TabIndex = 61;
            this.mtbNumberVisitsPerYear.ValidationExpression = "^\\d*$";
            // 
            // mtbCoPayAmount
            // 
            this.mtbCoPayAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoPayAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbCoPayAmount.Location = new System.Drawing.Point( 302, 96 );
            this.mtbCoPayAmount.Mask = "";
            this.mtbCoPayAmount.MaxLength = 10;
            this.mtbCoPayAmount.Name = "mtbCoPayAmount";
            this.mtbCoPayAmount.Size = new System.Drawing.Size( 60, 20 );
            this.mtbCoPayAmount.TabIndex = 59;
            this.mtbCoPayAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCoPayAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            // 
            // mtbPercentOutOfPocket
            // 
            this.mtbPercentOutOfPocket.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbPercentOutOfPocket.KeyPressExpression = "^\\d*$";
            this.mtbPercentOutOfPocket.Location = new System.Drawing.Point( 750, 56 );
            this.mtbPercentOutOfPocket.Mask = "";
            this.mtbPercentOutOfPocket.MaxLength = 3;
            this.mtbPercentOutOfPocket.Name = "mtbPercentOutOfPocket";
            this.mtbPercentOutOfPocket.Size = new System.Drawing.Size( 30, 20 );
            this.mtbPercentOutOfPocket.TabIndex = 58;
            this.mtbPercentOutOfPocket.ValidationExpression = "^\\d*$";
            // 
            // mtbOutOfPocketDollarsMet
            // 
            this.mtbOutOfPocketDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOutOfPocketDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketDollarsMet.Location = new System.Drawing.Point( 566, 56 );
            this.mtbOutOfPocketDollarsMet.Mask = "";
            this.mtbOutOfPocketDollarsMet.MaxLength = 10;
            this.mtbOutOfPocketDollarsMet.Name = "mtbOutOfPocketDollarsMet";
            this.mtbOutOfPocketDollarsMet.Size = new System.Drawing.Size( 60, 20 );
            this.mtbOutOfPocketDollarsMet.TabIndex = 57;
            this.mtbOutOfPocketDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            // 
            // mtbOutOfPocketAmount
            // 
            this.mtbOutOfPocketAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbOutOfPocketAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketAmount.Location = new System.Drawing.Point( 334, 56 );
            this.mtbOutOfPocketAmount.Mask = "";
            this.mtbOutOfPocketAmount.MaxLength = 10;
            this.mtbOutOfPocketAmount.Name = "mtbOutOfPocketAmount";
            this.mtbOutOfPocketAmount.Size = new System.Drawing.Size( 60, 20 );
            this.mtbOutOfPocketAmount.TabIndex = 55;
            this.mtbOutOfPocketAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            // 
            // lblStaticOutOfPocket
            // 
            this.lblStaticOutOfPocket.Location = new System.Drawing.Point( 246, 59 );
            this.lblStaticOutOfPocket.Name = "lblStaticOutOfPocket";
            this.lblStaticOutOfPocket.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticOutOfPocket.TabIndex = 31;
            this.lblStaticOutOfPocket.Text = "Out-of-pocket:  $";
            // 
            // mtbCoInsuranceAmount
            // 
            this.mtbCoInsuranceAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoInsuranceAmount.KeyPressExpression = "^\\d*$";
            this.mtbCoInsuranceAmount.Location = new System.Drawing.Point( 826, 14 );
            this.mtbCoInsuranceAmount.Mask = "";
            this.mtbCoInsuranceAmount.MaxLength = 3;
            this.mtbCoInsuranceAmount.Name = "mtbCoInsuranceAmount";
            this.mtbCoInsuranceAmount.Size = new System.Drawing.Size( 30, 20 );
            this.mtbCoInsuranceAmount.TabIndex = 54;
            this.mtbCoInsuranceAmount.ValidationExpression = "^\\d*$";
            // 
            // lblStaticPercent1
            // 
            this.lblStaticPercent1.Location = new System.Drawing.Point( 858, 17 );
            this.lblStaticPercent1.Name = "lblStaticPercent1";
            this.lblStaticPercent1.Size = new System.Drawing.Size( 16, 23 );
            this.lblStaticPercent1.TabIndex = 30;
            this.lblStaticPercent1.Text = "%";
            // 
            // cmbDeductibleMet
            // 
            this.cmbDeductibleMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDeductibleMet.Location = new System.Drawing.Point( 554, 14 );
            this.cmbDeductibleMet.Name = "cmbDeductibleMet";
            this.cmbDeductibleMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbDeductibleMet.TabIndex = 52;
            // 
            // cmbTimePeriod
            // 
            this.cmbTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePeriod.Location = new System.Drawing.Point( 462, 14 );
            this.cmbTimePeriod.Name = "cmbTimePeriod";
            this.cmbTimePeriod.Size = new System.Drawing.Size( 50, 21 );
            this.cmbTimePeriod.TabIndex = 51;
            // 
            // lblStaticMaxBenefit
            // 
            this.lblStaticMaxBenefit.Location = new System.Drawing.Point( 246, 144 );
            this.lblStaticMaxBenefit.Name = "lblStaticMaxBenefit";
            this.lblStaticMaxBenefit.Size = new System.Drawing.Size( 122, 23 );
            this.lblStaticMaxBenefit.TabIndex = 32;
            this.lblStaticMaxBenefit.Text = "Lifetime max benefit:  $";
            // 
            // lblStaticMaxBenefitMet
            // 
            this.lblStaticMaxBenefitMet.Location = new System.Drawing.Point( 769, 141 );
            this.lblStaticMaxBenefitMet.Name = "lblStaticMaxBenefitMet";
            this.lblStaticMaxBenefitMet.Size = new System.Drawing.Size( 35, 23 );
            this.lblStaticMaxBenefitMet.TabIndex = 29;
            this.lblStaticMaxBenefitMet.Text = "Met:";
            // 
            // lblStaticCoPay
            // 
            this.lblStaticCoPay.Location = new System.Drawing.Point( 246, 99 );
            this.lblStaticCoPay.Name = "lblStaticCoPay";
            this.lblStaticCoPay.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticCoPay.TabIndex = 25;
            this.lblStaticCoPay.Text = "Co-pay:  $";
            // 
            // lblStaticWaive
            // 
            this.lblStaticWaive.Location = new System.Drawing.Point( 374, 99 );
            this.lblStaticWaive.Name = "lblStaticWaive";
            this.lblStaticWaive.Size = new System.Drawing.Size( 95, 23 );
            this.lblStaticWaive.TabIndex = 28;
            this.lblStaticWaive.Text = "Waive if admitted:";
            // 
            // lblStaticVisitsPerYear
            // 
            this.lblStaticVisitsPerYear.Location = new System.Drawing.Point( 526, 100 );
            this.lblStaticVisitsPerYear.Name = "lblStaticVisitsPerYear";
            this.lblStaticVisitsPerYear.Size = new System.Drawing.Size( 60, 23 );
            this.lblStaticVisitsPerYear.TabIndex = 27;
            this.lblStaticVisitsPerYear.Text = "Visits/year:";
            // 
            // lblStaticPaymentMet
            // 
            this.lblStaticPaymentMet.Location = new System.Drawing.Point( 406, 59 );
            this.lblStaticPaymentMet.Name = "lblStaticPaymentMet";
            this.lblStaticPaymentMet.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticPaymentMet.TabIndex = 40;
            this.lblStaticPaymentMet.Text = "Met:";
            // 
            // lblStaticOutOfPocketDollarsMet
            // 
            this.lblStaticOutOfPocketDollarsMet.Location = new System.Drawing.Point( 494, 59 );
            this.lblStaticOutOfPocketDollarsMet.Name = "lblStaticOutOfPocketDollarsMet";
            this.lblStaticOutOfPocketDollarsMet.Size = new System.Drawing.Size( 78, 23 );
            this.lblStaticOutOfPocketDollarsMet.TabIndex = 39;
            this.lblStaticOutOfPocketDollarsMet.Text = "Dollars met:  $";
            // 
            // lblStaticAfterOutOfPocket
            // 
            this.lblStaticAfterOutOfPocket.Location = new System.Drawing.Point( 638, 59 );
            this.lblStaticAfterOutOfPocket.Name = "lblStaticAfterOutOfPocket";
            this.lblStaticAfterOutOfPocket.Size = new System.Drawing.Size( 112, 23 );
            this.lblStaticAfterOutOfPocket.TabIndex = 42;
            this.lblStaticAfterOutOfPocket.Text = "% after out-of-pocket:";
            // 
            // lblStaticPercent2
            // 
            this.lblStaticPercent2.Location = new System.Drawing.Point( 782, 59 );
            this.lblStaticPercent2.Name = "lblStaticPercent2";
            this.lblStaticPercent2.Size = new System.Drawing.Size( 16, 23 );
            this.lblStaticPercent2.TabIndex = 41;
            this.lblStaticPercent2.Text = "%";
            // 
            // lblStaticTime
            // 
            this.lblStaticTime.Location = new System.Drawing.Point( 390, 17 );
            this.lblStaticTime.Name = "lblStaticTime";
            this.lblStaticTime.Size = new System.Drawing.Size( 71, 23 );
            this.lblStaticTime.TabIndex = 38;
            this.lblStaticTime.Text = "Time period:";
            // 
            // lblStaticMet
            // 
            this.lblStaticMet.Location = new System.Drawing.Point( 523, 17 );
            this.lblStaticMet.Name = "lblStaticMet";
            this.lblStaticMet.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticMet.TabIndex = 35;
            this.lblStaticMet.Text = "Met:";
            // 
            // lblStaticDollarsMet
            // 
            this.lblStaticDollarsMet.Location = new System.Drawing.Point( 610, 17 );
            this.lblStaticDollarsMet.Name = "lblStaticDollarsMet";
            this.lblStaticDollarsMet.Size = new System.Drawing.Size( 77, 23 );
            this.lblStaticDollarsMet.TabIndex = 34;
            this.lblStaticDollarsMet.Text = "Dollars met:  $";
            // 
            // lblStaticCoInsurance
            // 
            this.lblStaticCoInsurance.Location = new System.Drawing.Point( 754, 17 );
            this.lblStaticCoInsurance.Name = "lblStaticCoInsurance";
            this.lblStaticCoInsurance.Size = new System.Drawing.Size( 76, 23 );
            this.lblStaticCoInsurance.TabIndex = 37;
            this.lblStaticCoInsurance.Text = "Co-insurance:";
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblCategory.Location = new System.Drawing.Point( 13, 35 );
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size( 0, 17 );
            this.lblCategory.TabIndex = 68;
            // 
            // lineLabelBenefits1
            // 
            this.lineLabelBenefits1.Caption = "";
            this.lineLabelBenefits1.Location = new System.Drawing.Point( 246, 31 );
            this.lineLabelBenefits1.Name = "lineLabelBenefits1";
            this.lineLabelBenefits1.Size = new System.Drawing.Size( 664, 18 );
            this.lineLabelBenefits1.TabIndex = 33;
            this.lineLabelBenefits1.TabStop = false;
            // 
            // lineLabelBenefits3
            // 
            this.lineLabelBenefits3.Caption = "";
            this.lineLabelBenefits3.Location = new System.Drawing.Point( 246, 114 );
            this.lineLabelBenefits3.Name = "lineLabelBenefits3";
            this.lineLabelBenefits3.Size = new System.Drawing.Size( 664, 18 );
            this.lineLabelBenefits3.TabIndex = 26;
            this.lineLabelBenefits3.TabStop = false;
            // 
            // lineLabelBenefits2
            // 
            this.lineLabelBenefits2.Caption = "";
            this.lineLabelBenefits2.Location = new System.Drawing.Point( 246, 75 );
            this.lineLabelBenefits2.Name = "lineLabelBenefits2";
            this.lineLabelBenefits2.Size = new System.Drawing.Size( 664, 13 );
            this.lineLabelBenefits2.TabIndex = 36;
            this.lineLabelBenefits2.TabStop = false;
            // 
            // DebugBenResponseCatDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add( this.lblCategory );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.lblDollar1 );
            this.Controls.Add( this.lblRemainingBenefitPerVisit );
            this.Controls.Add( this.lblRemainingLifetimeValue );
            this.Controls.Add( this.mtbRemainingBenefitPerVisit );
            this.Controls.Add( this.mtbRemainingLifetimeValue );
            this.Controls.Add( this.mtbDeductible );
            this.Controls.Add( this.lblStaticDeductible );
            this.Controls.Add( this.mtbDeductibleDollarsMet );
            this.Controls.Add( this.cmbMaxBenefitPerVisitMet );
            this.Controls.Add( this.lblStaticMaxBenefitVisitMet );
            this.Controls.Add( this.mtbMaxBenefitPerVisit );
            this.Controls.Add( this.lblStaticMaxBenefitVisit );
            this.Controls.Add( this.cmbCoPayWaiveIfAdmitted );
            this.Controls.Add( this.cmbOutOfPocketMet );
            this.Controls.Add( this.cmbLifetimeMaxBenefitMet );
            this.Controls.Add( this.mtbMaxBenefitAmount );
            this.Controls.Add( this.mtbNumberVisitsPerYear );
            this.Controls.Add( this.mtbCoPayAmount );
            this.Controls.Add( this.mtbPercentOutOfPocket );
            this.Controls.Add( this.mtbOutOfPocketDollarsMet );
            this.Controls.Add( this.mtbOutOfPocketAmount );
            this.Controls.Add( this.lblStaticOutOfPocket );
            this.Controls.Add( this.mtbCoInsuranceAmount );
            this.Controls.Add( this.lblStaticPercent1 );
            this.Controls.Add( this.cmbDeductibleMet );
            this.Controls.Add( this.cmbTimePeriod );
            this.Controls.Add( this.lineLabelBenefits1 );
            this.Controls.Add( this.lblStaticMaxBenefit );
            this.Controls.Add( this.lblStaticMaxBenefitMet );
            this.Controls.Add( this.lineLabelBenefits3 );
            this.Controls.Add( this.lblStaticCoPay );
            this.Controls.Add( this.lblStaticWaive );
            this.Controls.Add( this.lblStaticVisitsPerYear );
            this.Controls.Add( this.lblStaticPaymentMet );
            this.Controls.Add( this.lblStaticOutOfPocketDollarsMet );
            this.Controls.Add( this.lblStaticAfterOutOfPocket );
            this.Controls.Add( this.lblStaticPercent2 );
            this.Controls.Add( this.lblStaticTime );
            this.Controls.Add( this.lblStaticMet );
            this.Controls.Add( this.lblStaticDollarsMet );
            this.Controls.Add( this.lblStaticCoInsurance );
            this.Controls.Add( this.lineLabelBenefits2 );
            this.Name = "DebugBenResponseCatDetails";
            this.Size = new System.Drawing.Size( 944, 201 );
            this.Load += new System.EventHandler( DebugBenResponseCatDetails_Load );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDollar1;
        private System.Windows.Forms.Label lblRemainingBenefitPerVisit;
        private System.Windows.Forms.Label lblRemainingLifetimeValue;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbRemainingBenefitPerVisit;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbRemainingLifetimeValue;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDeductible;
        private System.Windows.Forms.Label lblStaticDeductible;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDeductibleDollarsMet;
        private System.Windows.Forms.ComboBox cmbMaxBenefitPerVisitMet;
        private System.Windows.Forms.Label lblStaticMaxBenefitVisitMet;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbMaxBenefitPerVisit;
        private System.Windows.Forms.Label lblStaticMaxBenefitVisit;
        private System.Windows.Forms.ComboBox cmbCoPayWaiveIfAdmitted;
        private System.Windows.Forms.ComboBox cmbOutOfPocketMet;
        private System.Windows.Forms.ComboBox cmbLifetimeMaxBenefitMet;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbMaxBenefitAmount;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbNumberVisitsPerYear;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCoPayAmount;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbPercentOutOfPocket;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbOutOfPocketDollarsMet;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbOutOfPocketAmount;
        private System.Windows.Forms.Label lblStaticOutOfPocket;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCoInsuranceAmount;
        private System.Windows.Forms.Label lblStaticPercent1;
        private System.Windows.Forms.ComboBox cmbDeductibleMet;
        private System.Windows.Forms.ComboBox cmbTimePeriod;
        private PatientAccess.UI.CommonControls.LineLabel lineLabelBenefits1;
        private System.Windows.Forms.Label lblStaticMaxBenefit;
        private System.Windows.Forms.Label lblStaticMaxBenefitMet;
        private PatientAccess.UI.CommonControls.LineLabel lineLabelBenefits3;
        private System.Windows.Forms.Label lblStaticCoPay;
        private System.Windows.Forms.Label lblStaticWaive;
        private System.Windows.Forms.Label lblStaticVisitsPerYear;
        private System.Windows.Forms.Label lblStaticPaymentMet;
        private System.Windows.Forms.Label lblStaticOutOfPocketDollarsMet;
        private System.Windows.Forms.Label lblStaticAfterOutOfPocket;
        private System.Windows.Forms.Label lblStaticPercent2;
        private System.Windows.Forms.Label lblStaticTime;
        private System.Windows.Forms.Label lblStaticMet;
        private System.Windows.Forms.Label lblStaticDollarsMet;
        private System.Windows.Forms.Label lblStaticCoInsurance;
        private PatientAccess.UI.CommonControls.LineLabel lineLabelBenefits2;
        private System.Windows.Forms.Label lblCategory;
    }
}
