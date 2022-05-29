using System;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    public partial class DebugBenResponseCatDetails : UserControl
    {
        private TimePeriodFlag blankTimePeriodFlag;
        private TimePeriodFlag yearTimePeriodFlag;
        private TimePeriodFlag visitTimePeriodFlag;
        private YesNoFlag blankYesNoFlag;
        private YesNoFlag yesYesNoFlag;
        private YesNoFlag noYesNoFlag;


        void DebugBenResponseCatDetails_Load( object sender, EventArgs e )
        {
        
        }

        private void PopulateDeductibleMetComboBox()
        {
            cmbDeductibleMet.Items.Add( blankYesNoFlag );
            cmbDeductibleMet.Items.Add( yesYesNoFlag );
            cmbDeductibleMet.Items.Add( noYesNoFlag );
        }

        private void PopulateDeductibleTimePeriodComboBox()
        {
            cmbTimePeriod.Items.Add( blankTimePeriodFlag );
            cmbTimePeriod.Items.Add( yearTimePeriodFlag );
            cmbTimePeriod.Items.Add( visitTimePeriodFlag );
        }

        private void PopulateMaxBenefitMetComboBox()
        {
            cmbLifetimeMaxBenefitMet.Items.Add( blankYesNoFlag );
            cmbLifetimeMaxBenefitMet.Items.Add( yesYesNoFlag );
            cmbLifetimeMaxBenefitMet.Items.Add( noYesNoFlag );
        }

        private void PopulateMaxBenefitPerVisitComboBox()
        {
            cmbMaxBenefitPerVisitMet.Items.Add( blankYesNoFlag );
            cmbMaxBenefitPerVisitMet.Items.Add( yesYesNoFlag );
            cmbMaxBenefitPerVisitMet.Items.Add( noYesNoFlag );
        }

        private void PopulateOutOfPocketMetComboBox()
        {
            cmbOutOfPocketMet.Items.Add( blankYesNoFlag );
            cmbOutOfPocketMet.Items.Add( yesYesNoFlag );
            cmbOutOfPocketMet.Items.Add( noYesNoFlag );
        }

        private void PopulateWaiveIfAdmittedComboBox()
        {
            cmbCoPayWaiveIfAdmitted.Items.Add( blankYesNoFlag );
            cmbCoPayWaiveIfAdmitted.Items.Add( yesYesNoFlag );
            cmbCoPayWaiveIfAdmitted.Items.Add( noYesNoFlag );
        }


        public BenefitsCategoryDetails Model;

        public DebugBenResponseCatDetails()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            blankTimePeriodFlag = new TimePeriodFlag();
            blankTimePeriodFlag.SetBlank();
            yearTimePeriodFlag = new TimePeriodFlag();
            yearTimePeriodFlag.SetYear();
            visitTimePeriodFlag = new TimePeriodFlag();
            visitTimePeriodFlag.SetVisit();

            blankYesNoFlag = new YesNoFlag();
            blankYesNoFlag.SetBlank();
            yesYesNoFlag = new YesNoFlag();
            yesYesNoFlag.SetYes();
            noYesNoFlag = new YesNoFlag();
            noYesNoFlag.SetNo();

            PopulateDeductibleTimePeriodComboBox();
            PopulateDeductibleMetComboBox();
            PopulateOutOfPocketMetComboBox();
            PopulateWaiveIfAdmittedComboBox();
            PopulateMaxBenefitMetComboBox();
            PopulateMaxBenefitPerVisitComboBox();  

            this.lblCategory.Text = this.Model.BenefitCategory.Description;

            this.UpdateBenefitsCategoryControls( this.Model );
        }

        private void UpdateBenefitsCategoryControls( BenefitsCategoryDetails bcd )
        {
            if( bcd != null )
            {
                cmbTimePeriod.SelectedItem = bcd.TimePeriod;
                cmbDeductibleMet.SelectedItem = bcd.DeductibleMet;
                cmbOutOfPocketMet.SelectedItem = bcd.OutOfPocketMet;
                cmbCoPayWaiveIfAdmitted.SelectedItem = bcd.WaiveCopayIfAdmitted;
                cmbLifetimeMaxBenefitMet.SelectedItem = bcd.RemainingLifetimeValueMet;
                cmbMaxBenefitPerVisitMet.SelectedItem = bcd.RemainingBenefitPerVisitsMet;

                if( bcd.Deductible != -1 )
                {
                    mtbDeductible.Text = bcd.Deductible.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbDeductible.UnMaskedText = string.Empty;
                }

                if( bcd.CoPay != -1 )
                {
                    mtbCoPayAmount.Text = bcd.CoPay.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbCoPayAmount.UnMaskedText = string.Empty;
                }

                if( bcd.CoInsurance != -1 )
                {
                    mtbCoInsuranceAmount.Text = bcd.CoInsurance.ToString();
                }
                else
                {
                    mtbCoInsuranceAmount.UnMaskedText = string.Empty;
                }

                if( bcd.AfterOutOfPocketPercent != -1 )
                {
                    mtbPercentOutOfPocket.Text = bcd.AfterOutOfPocketPercent.ToString();
                }
                else
                {
                    mtbPercentOutOfPocket.UnMaskedText = string.Empty;
                }

                if( bcd.DeductibleDollarsMet != -1 )
                {
                    mtbDeductibleDollarsMet.Text = bcd.DeductibleDollarsMet.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbDeductibleDollarsMet.UnMaskedText = string.Empty;
                }

                if( bcd.OutOfPocket != -1 )
                {
                    mtbOutOfPocketAmount.Text = bcd.OutOfPocket.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbOutOfPocketAmount.UnMaskedText = string.Empty;
                }

                if( bcd.LifeTimeMaxBenefit != -1 )
                {
                    mtbMaxBenefitAmount.Text = bcd.LifeTimeMaxBenefit.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbMaxBenefitAmount.UnMaskedText = string.Empty;
                }

                if( bcd.RemainingLifetimeValue != -1 )
                {
                    this.mtbRemainingLifetimeValue.Text = bcd.RemainingLifetimeValue.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbRemainingLifetimeValue.UnMaskedText = string.Empty;
                }

                if( bcd.MaxBenefitPerVisit != -1 )
                {
                    mtbMaxBenefitPerVisit.Text = bcd.MaxBenefitPerVisit.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbMaxBenefitPerVisit.UnMaskedText = string.Empty;
                }

                if( bcd.RemainingBenefitPerVisits != -1 )
                {
                    this.mtbRemainingBenefitPerVisit.Text = bcd.RemainingBenefitPerVisits.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbRemainingBenefitPerVisit.UnMaskedText = string.Empty;
                }

                if( bcd.OutOfPocketDollarsMet != -1 )
                {
                    mtbOutOfPocketDollarsMet.Text = bcd.OutOfPocketDollarsMet.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbOutOfPocketDollarsMet.UnMaskedText = string.Empty;
                }

                if( mtbNumberVisitsPerYear.Enabled
                    && bcd.VisitsPerYear != -1 )
                {
                    mtbNumberVisitsPerYear.Text = bcd.VisitsPerYear.ToString();
                }
                else
                {
                    mtbNumberVisitsPerYear.UnMaskedText = string.Empty;
                }
            }
        }

    }
}
