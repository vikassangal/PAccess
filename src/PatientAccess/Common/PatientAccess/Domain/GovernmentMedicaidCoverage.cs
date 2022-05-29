using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class GovernmentMedicaidCoverage : CoverageGroup
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoverageVerificationData()
        {
            base.RemoveCoverageVerificationData();

            this.TrackingNumber = string.Empty;         
            this.EVCNumber = string.Empty;
            this.ForceUnChangedStatusFor( "EVCNumber" );
            this.MedicaidCopay = -1;
            this.ForceUnChangedStatusFor( "MedicaidCopay" );
            this.EligibilityDate = DateTime.MinValue;
            this.ForceUnChangedStatusFor( "EligibilityDate" );
            this.PatienthasOtherInsuranceCoverage = new YesNoFlag();
            this.ForceUnChangedStatusFor( "PatienthasOtherInsuranceCoverage" );
            this.PatienthasMedicare = new YesNoFlag();
            this.ForceUnChangedStatusFor( "PatienthasMedicare" );
        }

        public override void RemoveCoveragePayorData()
        {
            this.IssueDate = DateTime.MinValue;
            this.EVCNumber = string.Empty;
            this.EligibilityDate = DateTime.MinValue;
            this.MedicaidCopay = -1;
            this.PatienthasMedicare = new YesNoFlag();
            this.PatienthasOtherInsuranceCoverage = new YesNoFlag();
            this.RemoveAuthorization();
        }

        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays                          = 0;
            this.Authorization.AuthorizationNumber      = string.Empty;
            this.Authorization.NumberOfDaysAuthorized   = 0;
            this.TrackingNumber = string.Empty;      
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
                if( this.HaveVerificationFieldsChanged )
                {
                    // Activity Code for Medicaid Coverage category only
                    fac.AddIMEVCNoteTo( account, this, originalCoverage );
                }
            } 
            
            if( this.WriteBenefitsVerifiedFUSNote )
            {
                // Activity Code for Benefits Verification notes
                fac.AddRBVCANoteTo( account, this, originalCoverage );
            }

            if (this.HaveAuthorizationFieldsChanged || HasChangedFor(PROPERTY_TRACKINGNUMBER))
            {
                // Activity Code for Authorization Required notes
                fac.AddRARRANoteTo( account, this, originalCoverage );
            }

            return account;
        }

        public bool ConstraintChangedFor( string propertyName )
        {

            return this.Constraints.HasChangedFor( propertyName );

        }

        #endregion

        #region Properties
        protected override bool HaveVerificationFieldsChanged
        {
            get
            {

                return base.HaveVerificationFieldsChanged || this.Constraints.HasChangedFor( PROPERTY_ELIGIBILITYDATE ) ||
                       this.HasChangedFor( PROPERTY_INFORMATIONRECEIVEDSOURCE ) || this.HasChangedFor( PROPERTY_PATIENTHASMEDICARE ) ||
                       this.HasChangedFor( PROPERTY_REMARKS ) || this.HasChangedFor( PROPERTY_PATIENTHASOTHERINSURANCECOVERAGE ) ||
                       this.Constraints.HasChangedFor( PROPERTY_MEDICAIDCOPAY ) || this.Constraints.HasChangedFor( PROPERTY_EVCNUMBER ) ;

            }
        }
        public override  string AssociatedNumber   // overriding property
        {
            get 
            {
                return i_AssociatedNumber;
            }
        }

        public DateTime IssueDate
        {
            get
            {
                return i_IssueDate;
            }
            set
            {
                i_IssueDate = value;
            }
        }

        public string EVCNumber
        {
            get
            {
                return Constraints.EVCNumber;
            }
            set
            {
                this.Constraints.EVCNumber = value;
            }
        }

        public string PolicyCINNumber
        {
            get
            {
                return i_PolicyCINNumber;
            }
            set
            {
                i_PolicyCINNumber = value;
            }
        }

        public DateTime EligibilityDate
        {
            get
            {
                return Constraints.EligibilityDate;
            }
            set
            {                
                this.Constraints.EligibilityDate = value;
            }
        }

        public YesNoFlag PatienthasMedicare
        {
            get
            {
                return i_PatienthasMedicare;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PatienthasMedicare, value, MethodBase.GetCurrentMethod() );
            }
        }
        
        public YesNoFlag PatienthasOtherInsuranceCoverage
        {
            get
            {
                return i_PatienthasOtherInsuranceCoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PatienthasOtherInsuranceCoverage, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float  MedicaidCopay
        {
            get
            {
                return Constraints.MedicaidCopay;
            }
            set
            {
                this.Constraints.MedicaidCopay =  value;
            }
        }
        public string TrackingNumber
        {
            get
            {
                return i_TrackingNumber;
            }
            set
            {
                Debug.Assert(value != null);
                if (!string.IsNullOrEmpty(value)) value = value.Trim();
                this.SetAndTrack<string>(ref this.i_TrackingNumber, value, MethodBase.GetCurrentMethod());
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private MedicaidConstraints Constraints
        {
            get
            {
                return (MedicaidConstraints)i_CoverageConstraints;
            }
        }
        #endregion

        #region Construction and Finalization
        public GovernmentMedicaidCoverage(): base()
        {
            i_CoverageConstraints = new MedicaidConstraints( );
        }
        #endregion

        #region Data Elements
        private DateTime i_IssueDate;
        private string i_PolicyCINNumber                        = String.Empty;
        private YesNoFlag i_PatienthasMedicare                  = new YesNoFlag();
        private YesNoFlag i_PatienthasOtherInsuranceCoverage    = new YesNoFlag();
        private string i_TrackingNumber                         = String.Empty;

        #endregion

        #region Constants

        private const string PROPERTY_ELIGIBILITYDATE = "EligibilityDate";
        private const string PROPERTY_INFORMATIONRECEIVEDSOURCE = "InformationReceivedSource";
        private const string PROPERTY_PATIENTHASMEDICARE = "PatienthasMedicare";
        private const string PROPERTY_REMARKS = "Remarks";
        private const string PROPERTY_PATIENTHASOTHERINSURANCECOVERAGE = "PatienthasOtherInsuranceCoverage";
        private const string PROPERTY_MEDICAIDCOPAY = "MedicaidCopay";
        private const string PROPERTY_EVCNUMBER = "EVCNumber";
        private const string PROPERTY_TRACKINGNUMBER = "TrackingNumber";
        #endregion
    }
}


