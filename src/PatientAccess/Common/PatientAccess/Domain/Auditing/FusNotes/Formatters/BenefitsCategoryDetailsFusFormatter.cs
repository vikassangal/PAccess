using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for BenefitsCategoryDetailsFusFormatter
    [Serializable]
    [UsedImplicitly]
    public class BenefitsCategoryDetailsFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            return new ArrayList();
        }

        public bool AddFusBenefitsCategoryDetailsTo( BenefitsCategoryDetails benefitsCategoryDetails,
            BenefitsCategoryDetails origBenefitsCategoryDetails, string benefitCategoryName, ArrayList nameValueList)
        {

            ArrayList tempList = new ArrayList();
            bool writeNote = false;

            if( benefitsCategoryDetails.HasChangedFor("Deductible") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.DEDUCTIBLE, benefitsCategoryDetails.Deductible.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("TimePeriod") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.TIME_PERIOD, benefitsCategoryDetails.TimePeriod.Description ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("DeductibleMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.IS_MET, benefitsCategoryDetails.DeductibleMet.Description ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("DeductibleDollarsMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.DOLLAR_MET,
                    benefitsCategoryDetails.DeductibleDollarsMet.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("CoInsurance") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.CO_INSURANCE, benefitsCategoryDetails.CoInsurance.ToString() ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("OutOfPocket" ) )
            {
                tempList.Add( FormatNameValuePair( FusLabel.OUT_OF_POCKET, benefitsCategoryDetails.OutOfPocket.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("OutOfPocketMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.IS_MET, benefitsCategoryDetails.OutOfPocketMet.Description ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("OutOfPocketDollarsMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.DOLLAR_MET, benefitsCategoryDetails.OutOfPocketDollarsMet.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("AfterOutOfPocketPercent") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.PERCENTAGE_OUT_OF_POCKET, benefitsCategoryDetails.AfterOutOfPocketPercent.ToString() ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("CoPay") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.CO_PAY_AMOUNT, benefitsCategoryDetails.CoPay.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("WaiveCopayIfAdmitted"))
            {
                tempList.Add( FormatNameValuePair( FusLabel.WAIVE_COPAY, benefitsCategoryDetails.WaiveCopayIfAdmitted.Description ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("VisitsPerYear") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.NUMBER_VISITS_PER_YEAR, benefitsCategoryDetails.VisitsPerYear.ToString() ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("LifeTimeMaxBenefit") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.LIFETIME_MAX_BENEFIT, benefitsCategoryDetails.LifeTimeMaxBenefit.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("RemainingLifetimeValue") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.REMAINING_LIFETIME_VALUE, benefitsCategoryDetails.RemainingLifetimeValue.ToString("C") ) );
                writeNote = true;
            }   

            if( benefitsCategoryDetails.HasChangedFor("RemainingLifetimeValueMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.IS_MET, benefitsCategoryDetails.RemainingLifetimeValueMet.Description ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("MaxBenefitPerVisit") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.MAX_BENEFIT_PER_VISIT, benefitsCategoryDetails.MaxBenefitPerVisit.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("RemainingBenefitPerVisits") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.REMAINING_BENEFIT_PER_VISITS, benefitsCategoryDetails.RemainingBenefitPerVisits.ToString("C") ) );
                writeNote = true;
            }

            if( benefitsCategoryDetails.HasChangedFor("RemainingBenefitPerVisitsMet") )
            {
                tempList.Add( FormatNameValuePair( FusLabel.IS_MET, benefitsCategoryDetails.RemainingBenefitPerVisitsMet.Description ) );
                writeNote = true;
            }

            if( writeNote )
            {
                nameValueList.Add( benefitCategoryName );
                nameValueList.AddRange( tempList );
            }
            
            return writeNote;

        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BenefitsCategoryDetailsFusFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants                            
        #endregion
    }
}
