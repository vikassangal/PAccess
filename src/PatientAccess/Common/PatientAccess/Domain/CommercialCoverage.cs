using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class CommercialCoverage : CoverageForCommercialOther, ICloneable
    {
        private const string PROPERTY_SERVICEFORPREEXISTINGCONDITION = "ServiceForPreExistingCondition";
        private const string PROPERTY_SERVICEISCOVEREDBENEFIT = "ServiceIsCoveredBenefit";
        private const string PROPERTY_CLAIMSADDRESSVERIFIED = "ClaimsAddressVerified";
        private const string PROPERTY_COORDINATIONOFBENEFITS = "CoordinationOfbenefits";
        private const string PROPERTY_TYPEOFPRODUCT = "TypeOfProduct";
        private const string PROPERTY_PPOPRICINGORBROKER = "PPOPricingOrBroker";
        private const string PROPERTY_FACILITYCONTRACTEDPROVIDER = "FacilityContractedProvider";
        private const string PROPERTY_AUTOINSURANCECLAIMNUMBER = "AutoInsuranceClaimNumber";
        private const string PROPERTY_AUTOMEDPAYCOVERAGE = "AutoMedPayCoverage";
        private const string PROPERTY_TYPEOFVERIFICATIONRULE = "TypeOfVerificationRule";

        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoveragePayorData()
        {
            this.TrackingNumber                         = string.Empty;           
            this.AutoInsuranceClaimNumber               = string.Empty;
            this.AutoMedPayCoverage                     = new YesNoFlag();
            this.ClaimsAddressVerified                  = new YesNoFlag();
            this.CoordinationOfbenefits                 = new YesNoFlag();
            this.FacilityContractedProvider             = new YesNoFlag();
            this.PPOPricingOrBroker                     = string.Empty;
            this.ServiceForPreExistingCondition         = new YesNoFlag();
            this.ServiceIsCoveredBenefit                = new YesNoFlag();
            this.TypeOfProduct                          = new TypeOfProduct();
            this.TypeOfVerificationRule                 = new TypeOfVerificationRule();
            this.RemoveAuthorization();
        }

        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays                          = 0;
            this.Authorization.AuthorizationNumber      = string.Empty;
            this.Authorization.NumberOfDaysAuthorized   = 0;
        }

        public override string GenerateFusNotes( )
        {
            string fusNotes = string.Empty;
            return fusNotes;
        }

        public override void SetCoverageConstraints( CoverageConstraints constraints )
        {
            base.SetCoverageConstraints( constraints );
            if( constraints != null )
            {
                SetBenefitsDetailsCollection( );
            }
        }

        public bool ConstraintHasChangedFor( string propertyName )
        {

            return this.Constraints.HasChangedFor( propertyName );

        }

        public override void RemoveCoverageVerificationData()
        {

            base.RemoveCoverageVerificationData();

            this.ServiceForPreExistingCondition = new YesNoFlag();
            this.ForceUnChangedStatusFor( "ServiceForPreExistingCondition" );
            this.ServiceIsCoveredBenefit = new YesNoFlag();
            this.ForceUnChangedStatusFor( "ServiceIsCoveredBenefit" );
            this.ClaimsAddressVerified = new YesNoFlag();
            this.ForceUnChangedStatusFor( "ClaimsAddressVerified" );
            this.CoordinationOfbenefits = new YesNoFlag();
            this.ForceUnChangedStatusFor( "CoordinationOfbenefits" );
            this.TypeOfProduct = new TypeOfProduct();
            this.ForceUnChangedStatusFor( "TypeOfProduct" );
            this.PPOPricingOrBroker = string.Empty;
            this.ForceUnChangedStatusFor( "PPOPricingOrBroker" );
            this.FacilityContractedProvider = new YesNoFlag();
            this.ForceUnChangedStatusFor( "FacilityContractedProvider" );
            this.AutoInsuranceClaimNumber = string.Empty;
            this.ForceUnChangedStatusFor( "AutoInsuranceClaimNumber" );
            this.AutoMedPayCoverage = new YesNoFlag();
            this.ForceUnChangedStatusFor( "AutoMedPayCoverage" );
            this.TypeOfVerificationRule = new TypeOfVerificationRule();
            this.ForceUnChangedStatusFor( "TypeOfVerificationRule" );

        }
        protected override bool HaveVerificationFieldsChanged
        {
            get
            {

                ContactPoint attorneyInfo = 
                    this.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );
                ContactPoint agentInfo =
                    this.InsuranceAgent.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                return base.HaveVerificationFieldsChanged ||
                       this.HasChangedFor( PROPERTY_SERVICEFORPREEXISTINGCONDITION ) ||
                       this.HasChangedFor( PROPERTY_SERVICEISCOVEREDBENEFIT ) ||
                       this.HasChangedFor( PROPERTY_CLAIMSADDRESSVERIFIED ) ||
                       this.HasChangedFor( PROPERTY_COORDINATIONOFBENEFITS ) ||
                       this.HasChangedFor( PROPERTY_TYPEOFPRODUCT ) ||
                       this.HasChangedFor( PROPERTY_PPOPRICINGORBROKER ) ||
                       this.HasChangedFor( PROPERTY_FACILITYCONTRACTEDPROVIDER ) ||
                       this.HasChangedFor( PROPERTY_AUTOINSURANCECLAIMNUMBER ) ||
                       this.HasChangedFor( PROPERTY_AUTOMEDPAYCOVERAGE ) ||
                       this.HasChangedFor( PROPERTY_TYPEOFVERIFICATIONRULE ) ||
                       this.Constraints.HasChangedFor( "EligibilityPhone" ) ||
                       this.Constraints.HasChangedFor( "EffectiveDateForInsured" ) ||
                       this.Constraints.HasChangedFor( "InsuranceCompanyRepName" ) ||
                       this.Constraints.HasChangedFor( "TerminationDateForInsured" ) ||
                       this.Constraints.HasChangedFor( "TypeOfProduct" ) ||
                       this.Attorney.HasChangedFor( "AttorneyName" ) ||
                       attorneyInfo.PhoneNumber.HasChangedFor( "AreaCode" ) ||
                       attorneyInfo.PhoneNumber.HasChangedFor( "Number" ) ||
                       attorneyInfo.HasChangedFor( "Address" ) ||
                       this.InsuranceAgent.HasChangedFor( "AgentName" ) ||
                       agentInfo.PhoneNumber.HasChangedFor( "AreaCode" ) ||
                       agentInfo.PhoneNumber.HasChangedFor( "Number" ) ||
                       agentInfo.HasChangedFor( "Address" );


                       
            }
        }

        public override Account InsertFusNotesInto( Account account, Coverage originalCoverage )
        {
            FusNoteFactory fusNoteFactory = new FusNoteFactory();
            

            if( !this.AddElectronicVerificationFusNoteTo( account, originalCoverage ) )
            {
                this.AddManualVerificationFusNoteTo( account, originalCoverage );    
            }

            if( this.WriteBenefitsVerifiedFUSNote )
            {
                // Activity Code for Benefits Verification notes
                fusNoteFactory.AddRBVCANoteTo( account, this, originalCoverage );
            }

            if( this.HaveAuthorizationFieldsChanged )
            {
                // Activity Code for Authorization Required notes
                fusNoteFactory.AddRARRANoteTo( account, this, originalCoverage );
            }
           

            return account;
        }

        #endregion

        #region Properties
        public override string AssociatedNumber   // overriding property
        {
            get 
            {
                return i_AssociatedNumber;
            }
        }

        public YesNoFlag ServiceForPreExistingCondition
        {
            get
            {
                return i_ServiceForPreExistingCondition;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_ServiceForPreExistingCondition, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag ServiceIsCoveredBenefit
        {
            get
            {
                return i_ServiceIsCoveredBenefit;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_ServiceIsCoveredBenefit, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag ClaimsAddressVerified
        {
            get
            {
                return i_ClaimsAddressVerified;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_ClaimsAddressVerified, value, MethodBase.GetCurrentMethod() );
            }
        }
        
        public YesNoFlag CoordinationOfbenefits
        {
            get
            {
                return i_CoordinationOfbenefits;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_CoordinationOfbenefits, value, MethodBase.GetCurrentMethod() );
            }
        }

        public TypeOfProduct TypeOfProduct
        {
            get
            {
                return i_TypeOfProduct;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<TypeOfProduct>( ref this.i_TypeOfProduct, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string PPOPricingOrBroker
        {
            get
            {
                return i_PPOPricingOrBroker;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_PPOPricingOrBroker, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag FacilityContractedProvider
        {
            get
            {
                return i_FacilityContractedProvider;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_FacilityContractedProvider, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string AutoInsuranceClaimNumber
        {
            get
            {
                return i_AutoInsuranceClaimNumber;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_AutoInsuranceClaimNumber, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag AutoMedPayCoverage
        {
            get
            {
                return i_AutoMedPayCoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_AutoMedPayCoverage, value, MethodBase.GetCurrentMethod() );
            }
        }

        public TypeOfVerificationRule TypeOfVerificationRule
        {
            get
            {
                return i_TypeOfVerificationRule;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<TypeOfVerificationRule>( ref this.i_TypeOfVerificationRule, value, MethodBase.GetCurrentMethod() );
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
        private void SetBenefitsDetailsCollection( )
        {
            if( Constraints != null
                && Constraints.BenefitsCategoryDetails != null
                && Constraints.BenefitsCategoryDetails.Count > 0 )
            {
                foreach( BenefitsCategoryDetails bcd in Constraints.BenefitsCategoryDetails )
                {
                    this.AddBenefitsCategory( bcd.BenefitCategory, bcd );
                }
            }
        }
        #endregion

        #region Private Properties
        private CommercialConstraints Constraints
        {
            get
            {
                return (CommercialConstraints)i_CoverageConstraints;
            }
        }
        #endregion

        #region Construction and Finalization
        public CommercialCoverage(): base()
        {
            i_CoverageConstraints = new CommercialConstraints( );
        }
        #endregion

        #region Data Elements
        private YesNoFlag i_ServiceForPreExistingCondition = new YesNoFlag();
        private YesNoFlag i_ServiceIsCoveredBenefit = new YesNoFlag();
        private YesNoFlag i_ClaimsAddressVerified = new YesNoFlag();
        private YesNoFlag i_CoordinationOfbenefits = new YesNoFlag();
        private TypeOfProduct i_TypeOfProduct = new TypeOfProduct();
        private string i_PPOPricingOrBroker = string.Empty;
        private YesNoFlag i_FacilityContractedProvider = new YesNoFlag();
        private string i_AutoInsuranceClaimNumber = string.Empty;
        private YesNoFlag i_AutoMedPayCoverage = new YesNoFlag();
        private TypeOfVerificationRule i_TypeOfVerificationRule = new TypeOfVerificationRule() ;
        #endregion

        #region Constants
        #endregion
    }
}

