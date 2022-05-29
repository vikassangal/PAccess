using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for SpanCode.
    /// </summary>
    //TODO: Create XML summary comment for SpanCode
    [Serializable]
    public class SpanCode : CodedReferenceValue
    {

		#region Constants 

        private const string BENEFIT_ELIGIBILITY_PERIOD = "73";
        private const string FIRST_LAST_VISIT = "72";
        public const string NONCOVERED_LEVEL_OF_CARE = "74";
        private const string PATIENT_LIABILITY = "76";
        public const string PRIOR_STAY_DATES = "71";
        private const string PRO_UR_APPROVED_STAY_DATE = "M0";
        private const string PROVIDER_LIABILITY_PERIOD = "77";
        public const string QUALIFYING_STAY_DATES = "70";
        private const string SNF_LEVEL_OF_CARE = "75";
        private const string SNF_PRIOR_STAY_DATES = "78";

		#endregion Constants 

		#region Constructors 

        public SpanCode( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public SpanCode( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public SpanCode()
        {
        }

		#endregion Constructors 

		#region Properties 

        public bool IsBenefitEligibilityPeriod
        {
            get
            {
                return this.Code.Equals( BENEFIT_ELIGIBILITY_PERIOD );
            }
        }

        public bool IsFirstLastVisit
        {
            get
            {
                return this.Code.Equals( FIRST_LAST_VISIT );
            }
        }

        public bool IsNoncoveredLevelOfCare
        {
            get
            {
                return this.Code.Equals( NONCOVERED_LEVEL_OF_CARE );
            }
        }

        public bool IsPatientLiability
        {
            get
            {
                return this.Code.Equals( PATIENT_LIABILITY );
            }
        }

        public bool IsPriorStayDates
        {
            get
            {
                return this.Code.Equals( PRIOR_STAY_DATES );
            }

        }

        public bool IsProUrApprovedStayDate
        {
            get
            {
                return this.Code.Equals( PRO_UR_APPROVED_STAY_DATE );
            }
        }

        public bool IsProviderLiabilityPeriod
        {
            get
            {
                return this.Code.Equals( PROVIDER_LIABILITY_PERIOD );
            }
        }

        public bool IsQualifyingStayDate
        {
            get
            {
                return this.Code.Equals( QUALIFYING_STAY_DATES );
            }
        }

        public bool IsSnfLevelOfCare
        {
            get
            {
                return this.Code.Equals( SNF_LEVEL_OF_CARE );
            }
        }

        public bool  IsSnfPriorStayDates
        {
            get
            {
                return this.Code.Equals( SNF_PRIOR_STAY_DATES );
            }
        }

		#endregion Properties 

		#region Methods 

        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }

		#endregion Methods 

    }
}
