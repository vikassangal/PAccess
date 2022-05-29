using System;
using System.Diagnostics;
using System.Reflection;

namespace PatientAccess.Domain
{
    [Serializable]
    public class GovernmentOtherConstraints : CoverageConstraints
    {
        #region Event Handlers

        #endregion

        #region Methods

        public override void ResetFieldsToDefault( bool resetTracking )
        {

            this.i_EligibilityPhone = string.Empty;
            this.i_InsuranceCompanyRepName = string.Empty;
            this.i_EffectiveDateForInsured = DateTime.MinValue;
            this.i_TypeOfCoverage = string.Empty;
            this.i_BenefitsCategoryDetails = new BenefitsCategoryDetails();
            this.i_TerminationDateForInsured = DateTime.MinValue;

            if( resetTracking )
                this.ResetChangeTracking();

        }

        #endregion

        #region Properties

        public string EligibilityPhone
        {
            get
            {
                return i_EligibilityPhone;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_EligibilityPhone, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string InsuranceCompanyRepName
        {
            get
            {
                return i_InsuranceCompanyRepName;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>(ref this.i_InsuranceCompanyRepName,value, MethodBase.GetCurrentMethod() );
            }
        }

        public DateTime EffectiveDateForInsured
        {
            get
            {
                return i_EffectiveDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>(ref this.i_EffectiveDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string TypeOfCoverage
        {
            get
            {
                return i_TypeOfCoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>(ref this.i_TypeOfCoverage,value, MethodBase.GetCurrentMethod() );
            }
        }

        public BenefitsCategoryDetails BenefitsCategoryDetails
        {
            private get
            {
                return i_BenefitsCategoryDetails;
            }
            set
            {
                i_BenefitsCategoryDetails = value;
            }
        }

        public DateTime TerminationDateForInsured
        {
            get
            {
                return i_TerminationDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_TerminationDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float Deductible
        {
            get
            {
                return BenefitsCategoryDetails.Deductible;
            }
            set
            {
                BenefitsCategoryDetails.Deductible = value;
            }
        }

        public float DeductibleDollarsMet
        {
            get
            {
                return BenefitsCategoryDetails.DeductibleDollarsMet;
            }
            set
            {
                BenefitsCategoryDetails.DeductibleDollarsMet = value;
            }
        }

        public YesNoFlag DeductibleMet
        {
            get
            {
                return BenefitsCategoryDetails.DeductibleMet;
            }
            set
            {
                BenefitsCategoryDetails.DeductibleMet = value;
            }
        }

        public int CoInsurance
        {
            get
            {
                return BenefitsCategoryDetails.CoInsurance;
            }
            set
            {
                BenefitsCategoryDetails.CoInsurance = value;
            }
        }

        public float OutOfPocket
        {
            get
            {
                return BenefitsCategoryDetails.OutOfPocket;
            }
            set
            {
                BenefitsCategoryDetails.OutOfPocket = value;
            }
        }

        public float OutOfPocketDollarsMet
        {
            get
            {
                return BenefitsCategoryDetails.OutOfPocketDollarsMet;
            }
            set
            {
                BenefitsCategoryDetails.OutOfPocketDollarsMet = value;
            }
        }

        public YesNoFlag OutOfPocketMet
        {
            get
            {
                return BenefitsCategoryDetails.OutOfPocketMet;
            }
            set
            {
                BenefitsCategoryDetails.OutOfPocketMet = value;
            }
        }

        public float CoPay
        {
            get
            {
                return BenefitsCategoryDetails.CoPay;
            }
            set
            {
                BenefitsCategoryDetails.CoPay = value;
            }
        }


        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public GovernmentOtherConstraints( )
        {
            this.ResetFieldsToDefault( true );
        }

        #endregion

        #region Field Elements

        private string                      i_EligibilityPhone = string.Empty;
        private string                      i_InsuranceCompanyRepName = string.Empty;
        private DateTime                    i_EffectiveDateForInsured;
        private string                      i_TypeOfCoverage = string.Empty;
        private BenefitsCategoryDetails     i_BenefitsCategoryDetails = new BenefitsCategoryDetails( );
        private DateTime                    i_TerminationDateForInsured;
        #endregion

        #region Constants

        #endregion
    }
}
