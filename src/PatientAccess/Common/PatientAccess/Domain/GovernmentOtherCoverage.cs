using System;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class GovernmentOtherCoverage : CoverageForCommercialOther
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoveragePayorData()
        {
            this.TrackingNumber                         = string.Empty;
            this.BenefitsCategoryDetails                = new BenefitsCategoryDetails();
            this.TypeOfCoverage                         = string.Empty;
            this.RemoveAuthorization();
        }

        public override void RemoveCoverageVerificationData()
        {

            base.RemoveCoverageVerificationData();

            this.BenefitsCategoryDetails = new BenefitsCategoryDetails();

        }

        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays                          = 0;
            this.Authorization.AuthorizationNumber      = string.Empty;
            this.Authorization.NumberOfDaysAuthorized   = 0;
        }
        public override string GenerateFusNotes()
        {
            string fusNotes = string.Empty;
            return fusNotes;
        }

        public override void SetCoverageConstraints( CoverageConstraints constraints )
        {
            base.SetCoverageConstraints( constraints );
            if( constraints != null )
            {
                SetBenefitsDetails( );
            }
            
        }

        public bool ConstraintHasChangedFor( string propertyName )
        {

            return this.Constraints.HasChangedFor( propertyName );

        }

        public override Account InsertFusNotesInto( Account account, Coverage originalCoverage )
        {
            FusNoteFactory fac = new FusNoteFactory();

            if( !this.AddElectronicVerificationFusNoteTo( account, originalCoverage ) )
            {
                if( this.HaveVerificationFieldsChanged )
                {
                    this.AddManualVerificationFusNoteTo( account, originalCoverage );
                }
            }

            if( this.WriteBenefitsVerifiedFUSNote )
            {
                // Activity Code for Benefits Verification notes
                fac.AddRBVCANoteTo( account, this, originalCoverage );
            }

            if (this.HaveAuthorizationFieldsChanged)
            {
                // Activity Code for Authorization Required notes
                fac.AddRARRANoteTo( account, this, originalCoverage );
            }

            return account;
        }

        #endregion

        #region Properties

        protected override bool HaveVerificationFieldsChanged
        {
            get
            {

                return base.HaveVerificationFieldsChanged ||
                    this.Constraints.HasChangedFor( "TypeOfCoverage" ) ||
                    this.Constraints.HasChangedFor( "EligibilityPhone" ) ||
                    this.Constraints.HasChangedFor( "InsuranceCompanyRepName" ) ||
                    this.Constraints.HasChangedFor( "EffectiveDateForInsured" ) ||
                    this.Constraints.HasChangedFor( "TerminationDateForInsured" ) ||
                    this.BenefitsCategoryDetails.HaveVerificationDetailsChanged;

            }
        }

        public override string AssociatedNumber   // overriding property
        {
            get 
            {
                return i_AssociatedNumber;
            }
        }

        public BenefitsCategoryDetails BenefitsCategoryDetails
        {
            get
            {
                return i_BenefitsCategoryDetails;
            }
            set
            {
                i_BenefitsCategoryDetails = value;
            }
        }

        public string TypeOfCoverage
        {
            get
            {
                return Constraints.TypeOfCoverage;
            }
            set
            {
                Constraints.TypeOfCoverage = value;
            }
        }

        public override string EligibilityPhone
        {
            get
            {
                return Constraints.EligibilityPhone;
            }
            set
            {
                Constraints.EligibilityPhone = value;
                base.EligibilityPhone = value;
            }
        }

        public override string InsuranceCompanyRepName
        {
            get
            {
                return Constraints.InsuranceCompanyRepName;
            }
            set
            {
                Constraints.InsuranceCompanyRepName = value;
                base.InsuranceCompanyRepName = value;
            }
        }

        public override DateTime EffectiveDateForInsured
        {
            get
            {
                return Constraints.EffectiveDateForInsured;
            }
            set
            {
                Constraints.EffectiveDateForInsured = value;
                base.EffectiveDateForInsured = value;
            }
        }

        public override DateTime TerminationDateForInsured
        {
            get
            {
                return Constraints.TerminationDateForInsured;
            }
            set
            {
                Constraints.TerminationDateForInsured = value;
                base.TerminationDateForInsured = value;
            }
        }
        #endregion

        #region Private Methods

        private void SetBenefitsDetails( )
        {
            if( this.BenefitsCategoryDetails == null )
            {
                this.BenefitsCategoryDetails = new BenefitsCategoryDetails( );
            }

            this.BenefitsCategoryDetails.Deductible             = Constraints.Deductible;
            this.BenefitsCategoryDetails.DeductibleDollarsMet   = Constraints.DeductibleDollarsMet;
            this.BenefitsCategoryDetails.CoInsurance            = Constraints.CoInsurance;
            this.BenefitsCategoryDetails.OutOfPocket            = Constraints.OutOfPocket;
            this.BenefitsCategoryDetails.OutOfPocketDollarsMet  = Constraints.OutOfPocketDollarsMet;
            this.BenefitsCategoryDetails.CoPay                  = Constraints.CoPay;
        }
        #endregion

        #region Private Properties


        private GovernmentOtherConstraints Constraints
        {
            get
            {
                return (GovernmentOtherConstraints)i_CoverageConstraints;
            }
        }
	
	

        #endregion

        #region Construction and Finalization
        public GovernmentOtherCoverage(): base()
        {
            i_CoverageConstraints = new GovernmentOtherConstraints( );
        }
        #endregion

        #region Data Elements
        private string i_TypeOfCoverage = String.Empty;
        private BenefitsCategoryDetails i_BenefitsCategoryDetails = new BenefitsCategoryDetails();
      
        #endregion

        #region Constants
        #endregion
    }
}


