using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompensationCoverage : CoverageGroup
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoveragePayorData()
        {
            this.ClaimNumberForIncident                 = string.Empty;
            this.ClaimsAddressVerified                  = new YesNoFlag();
            this.EmployerhasPaidPremiumsToDate          = new YesNoFlag();
            this.InsurancePhone                         = string.Empty;
            this.PPOPricingOrBroker                     = string.Empty;
            this.RemoveAuthorization();
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

        public override Account InsertFusNotesInto( Account account, Coverage originalCoverage )
        {
            FusNoteFactory fac = new FusNoteFactory();

            if( !this.AddElectronicVerificationFusNoteTo( account, originalCoverage ) )
            {
                this.AddManualVerificationFusNoteTo( account, originalCoverage );
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
                ContactPoint attorneyInfo = 
                    this.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                return base.HaveVerificationFieldsChanged ||
                       this.HasChangedFor( "PPOPricingOrBroker" ) ||
                       this.HasChangedFor( "ClaimNumberForIncident" ) ||
                       this.HasChangedFor( "ClaimsAddressVerified" ) ||
                       this.HasChangedFor( "InsurancePhone" ) ||
                       this.HasChangedFor( "EmployerhasPaidPremiumsToDate" ) ||
                       this.Attorney.HasChangedFor( "AttorneyName" ) ||
                       attorneyInfo.PhoneNumber.HasChangedFor( "AreaCode" ) ||
                       attorneyInfo.PhoneNumber.HasChangedFor( "Number" ) ||
                       attorneyInfo.HasChangedFor( "Address" );
            }
        }

        public override void RemoveCoverageVerificationData()
        {

            base.RemoveCoverageVerificationData();

            this.PPOPricingOrBroker = String.Empty;
            this.ClaimNumberForIncident = String.Empty;
            this.ClaimsAddressVerified = new YesNoFlag();
            this.InsurancePhone = String.Empty;
            this.EmployerhasPaidPremiumsToDate = new YesNoFlag();
            this.ForceUnChangedStatusFor( "PPOPricingOrBroker" );
            this.ForceUnChangedStatusFor( "ClaimNumberForIncident" );
            this.ForceUnChangedStatusFor( "ClaimsAddressVerified" );
            this.ForceUnChangedStatusFor( "InsurancePhone" );
            this.ForceUnChangedStatusFor( "EmployerhasPaidPremiumsToDate" );

        }

        public override string AssociatedNumber   // overriding property
        {
            get 
            {
                return i_AssociatedNumber;
            }
        }

        public string PolicyNumber
        {
            get
            {
                return i_PolicyNumber;
            }
            set
            {
                i_PolicyNumber = value;
            }
        }
        public string InsuranceAdjuster
        {
            get
            {
                return i_InsuranceAdjuster;
            }
            set
            {
                i_InsuranceAdjuster = value;
            }
        }
        public string PatientsSupervisor
        {
            get
            {
                return i_PatientsSupervisor;
            }
            set
            {
                i_PatientsSupervisor = value;
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
                this.SetAndTrack<string>(ref this.i_PPOPricingOrBroker, value, MethodBase.GetCurrentMethod() );
            }
        }
        public string ClaimNumberForIncident
        {
            get
            {
                return i_ClaimNumberForIncident;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>(ref this.i_ClaimNumberForIncident,value, MethodBase.GetCurrentMethod() );
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
                this.SetAndTrack<YesNoFlag>(ref this.i_ClaimsAddressVerified, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string InsurancePhone
        {
            get
            {
                return i_InsurancePhone;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>(ref this.i_InsurancePhone, value, MethodBase.GetCurrentMethod() );
            }
        }
        
        public YesNoFlag EmployerhasPaidPremiumsToDate
        {
            get
            {
                return i_EmployerhasPaidPremiumsToDate;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>(ref this.i_EmployerhasPaidPremiumsToDate, value, MethodBase.GetCurrentMethod() );
            }
        }


        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public WorkersCompensationCoverage(): base()
        {
        }
        #endregion

        #region Data Elements
        private string   i_PolicyNumber = String.Empty;
        private string   i_InsuranceAdjuster = String.Empty;
        private string   i_PatientsSupervisor = String.Empty;
        private string   i_PPOPricingOrBroker = String.Empty;
        private string   i_ClaimNumberForIncident = String.Empty;
        private YesNoFlag   i_ClaimsAddressVerified = new YesNoFlag();
        private string   i_InsurancePhone = String.Empty;
        private YesNoFlag   i_EmployerhasPaidPremiumsToDate = new YesNoFlag();

        #endregion

        #region Constants
        #endregion
    }
}


