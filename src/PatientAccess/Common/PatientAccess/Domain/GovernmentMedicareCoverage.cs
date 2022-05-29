using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class GovernmentMedicareCoverage : Coverage
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool ConstraintHasChangedFor( string propertyName )
        {
            return this.Constraints.HasChangedFor( propertyName );
        }

        public override void RemoveCoverageVerificationData()
        {
            base.RemoveCoverageVerificationData();

            this.VerifiedBeneficiaryName = new YesNoFlag();
            this.ForceUnChangedStatusFor( "VerifiedBeneficiaryName" );
            this.MedicareIsSecondary = new YesNoFlag();
            this.ForceUnChangedStatusFor( "MedicareIsSecondary" );

        }

        public override void RemoveCoveragePayorData()
        {
            this.DateOfLastBillingActivity          = DateTime.MinValue;
            this.MedicareIsSecondary                = new YesNoFlag();
            this.PartACoverage                      = new YesNoFlag();
            this.PartACoverageEffectiveDate         = DateTime.MinValue;
            this.PartBCoverage                      = new YesNoFlag();
            this.PartBCoverageEffectiveDate         = DateTime.MinValue;
            this.PatientHasMedicareHMOCoverage      = new YesNoFlag();
            this.PatientIsPartOfHospiceProgram      = new YesNoFlag();
            this.RemainingBenefitPeriod             = -1;
            this.RemainingCoInsurance               = -1;
            this.RemainingLifeTimeReserve           = -1;
            this.RemainingSNF                       = -1;
            this.RemainingSNFCoInsurance            = -1;
            this.VerificationCoPay                  = -1;
            this.RemainingPartADeductible           = -1;
            this.RemainingPartBDeductible           = -1;
            this.RemoveAuthorization();
        }
        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays = 0;
            this.Authorization.AuthorizationNumber = string.Empty;
            this.Authorization.NumberOfDaysAuthorized = 0;
          
            this.ForceUnChangedStatusFor("Authorization");
            this.TrackingNumber = String.Empty;
            this.Authorization = new Authorization();
            this.ForceUnChangedStatusFor("TrackingNumber");
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
                    // Activity Code for Medicare Coverage category only
                    fac.AddMCWFINoteTo( account, this, originalCoverage );
                }
            }
            if ( IsMedicareCoverageValidForAuthorization && this.HaveAuthorizationFieldsChanged )
            {
                // Activity Code for Authorization Required notes
                fac.AddRARRANoteTo(account, this, originalCoverage);
            }
            if( this.WriteBenefitsVerifiedFUSNote )
            {
                // Activity Code for Benefits Verification notes
                fac.AddRBVCANoteTo( account, this, originalCoverage );
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
                    this.Constraints.HasChangedFor( PROPERTY_PARTACOVERAGE ) || 
                    this.HasChangedFor( PROPERTY_VERIFIEDBENEFICIARYNAME ) ||
                    this.Constraints.HasChangedFor( PROPERTY_PARTACOVERAGEEFFECTIVEDATE ) || 
                    this.Constraints.HasChangedFor( PROPERTY_PARTBCOVERAGE ) ||
                    this.Constraints.HasChangedFor( PROPERTY_PATIENTHASMEDICAREHMOCOOVERAGE ) || 
                    this.Constraints.HasChangedFor( PROPERTY_PARTBCOVERAGEEFFECTIVEDATE ) ||
                    this.HasChangedFor( PROPERTY_MEDICAREISSECONDARY ) || 
                    this.Constraints.HasChangedFor( PROPERTY_DATEOFLASTBILLINGACTIVITY ) ||
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGBENEFITPERIOD ) || 
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGLIFETIMERESERVE ) ||
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGCOINSURANCE ) || 
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGSNF ) ||
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGSNFCOINSURANCE ) ||
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGPARTADEDUCTIBLE ) ||
                    this.Constraints.HasChangedFor( PROPERTY_REMAININGPARTBDEDUCTIBLE ) ||
                    this.HasChangedFor(PROPERTY_VERIFICATIONCOPAY);
            }
        }

        public override string AssociatedNumber   // overriding property
        {
            get 
            {
                return i_AssociatedNumber;
            }
        }

        public string HICNumber
        {
            get
            {
                return i_HICNumber;
            }
            set
            {
                i_HICNumber = value;
            }
        }
        public string MBINumber
        {
            get
            {
                return i_MBINumber;
            }
            set
            {
                i_MBINumber = value;
            }
        }

        public string FormattedMBINumber
        {
            get
            {
                return i_MBINumber.Length == 11
                    ? String.Format("{0}-{1}-{2}", i_MBINumber.Substring(0, 4), i_MBINumber.Substring(4, 3),
                        i_MBINumber.Substring(7, 4))
                    : String.Empty;

            }
        }

        public YesNoFlag PartACoverage
        {
            get
            {
                return Constraints.PartACoverage;
            }
            set
            {
                this.Constraints.PartACoverage = value;
            }
        }

        public YesNoFlag PartBCoverage
        {
            get
            {
                return Constraints.PartBCoverage;
            }
            set
            {
                this.Constraints.PartBCoverage = value;
            }
        }

        public DateTime PartACoverageEffectiveDate
        {
            get
            {
                return Constraints.PartACoverageEffectiveDate;
            }
            set
            {
                this.Constraints.PartACoverageEffectiveDate = value;
            }
        }

        public DateTime PartBCoverageEffectiveDate
        {
            get
            {
                return Constraints.PartBCoverageEffectiveDate;
            }
            set
            {
                this.Constraints.PartBCoverageEffectiveDate = value;
            }
        }

        public YesNoFlag PatientHasMedicareHMOCoverage
        {
            get
            {
                return Constraints.PatientHasMedicareHMOCoverage;
            }
            set
            {
                this.Constraints.PatientHasMedicareHMOCoverage = value;
            }
        }
        public YesNoFlag MedicareIsSecondary
        {
            get
            {
                return i_MedicareIsSecondary;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack( ref this.i_MedicareIsSecondary, value, MethodBase.GetCurrentMethod() );
            }
        }
        public DateTime DateOfLastBillingActivity
        {
            get
            {
                return Constraints.DateOfLastBillingActivity;
            }
            set
            {
                this.Constraints.DateOfLastBillingActivity = value;
            }
        }

        public int RemainingBenefitPeriod
        {
            get
            {
                return Constraints.RemainingBenefitPeriod;
            }
            set
            {
                this.Constraints.RemainingBenefitPeriod = value;
            }
        }

        public int RemainingCoInsurance
        {
            get
            {
                return Constraints.RemainingCoInsurance;
            }
            set
            {
                this.Constraints.RemainingCoInsurance = value;
            }
        }

        public int RemainingLifeTimeReserve
        {
            get
            {
                return Constraints.RemainingLifeTimeReserve;
            }
            set
            {
                this.Constraints.RemainingLifeTimeReserve = value;
            }
        }

        public int RemainingSNF
        {
            get
            {
                return Constraints.RemainingSNF;
            }
            set
            {
                this.Constraints.RemainingSNF = value;
            }
        }

        public int RemainingSNFCoInsurance
        {
            get
            {
                return Constraints.RemainingSNFCoInsurance;
            }
            set
            {
                this.Constraints.RemainingSNFCoInsurance = value;
            }
        }

        public YesNoFlag PatientIsPartOfHospiceProgram
        {
            get
            {
                return Constraints.PatientIsPartOfHospiceProgram;
            }
            set
            {
                Constraints.PatientIsPartOfHospiceProgram = value;
            }
        }

        public YesNoFlag VerifiedBeneficiaryName
        {
            get
            {
                return i_VerifiedBeneficiaryName;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack(ref this.i_VerifiedBeneficiaryName, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float RemainingPartADeductible
        {
            get
            {
                return Constraints.RemainingPartADeductible;
            }
            set
            {
                Constraints.RemainingPartADeductible = value;
            }
        }

        public float RemainingPartBDeductible
        {
            get
            {
                return Constraints.RemainingPartBDeductible;
            }
            set
            {
                Constraints.RemainingPartBDeductible = value;
            }
        }
        public Authorization Authorization
        {

            get
            {

                return this.i_Authorization;

            }
            set
            {

                Debug.Assert(value != null);
                Authorization oldValue = this.i_Authorization;

                oldValue.ChangedListeners -= RaiseChangedEvent;

                i_Authorization = value ?? new Authorization();

                this.i_Authorization.ChangedListeners +=
                    new Changed(this.RaiseChainedChangedEvent);

                if (this.i_Authorization != oldValue)
                    this.RaiseChangedEvent("Authorization", oldValue, value);



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
        private MedicareConstraints Constraints
        {
            get
            {
                return (MedicareConstraints)i_CoverageConstraints;
            }
        }
        protected virtual bool HaveAuthorizationFieldsChanged
        {
            get
            {
                return this.Authorization.HasChangedFor(AUTHORIZATION_COMPANY) ||
                       this.Authorization.HasChangedFor(AUTHORIZATION_NUMBER) ||
                       this.Authorization.HasChangedFor(AUTHORIZATION_STATUS) ||
                       this.Authorization.HasChangedFor(EFFECTIVE_DATE) ||
                       this.Authorization.HasChangedFor(EXPIRATION_DATE) ||
                       this.Authorization.HasChangedFor(NAME_OF_COMPANY_REPRESENTATIVE) ||
                       this.Authorization.HasChangedFor(NUMBER_OF_DAYS_AUTHORIZED) ||
                       this.Authorization.HasChangedFor(REMARKS) ||
                       this.Authorization.HasChangedFor(SERVICESAUTHORIZED);
            }
        }
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GovernmentMedicareCoverage()
        {
            i_CoverageConstraints = new MedicareConstraints( );
        }
        #endregion

        #region Data Elements
        private string i_HICNumber = String.Empty;
        private string i_MBINumber = String.Empty;
        private YesNoFlag i_MedicareIsSecondary             = new YesNoFlag();
        private YesNoFlag i_VerifiedBeneficiaryName         = new YesNoFlag();
        private Authorization i_Authorization = new Authorization();
        private string i_TrackingNumber = String.Empty;

        #endregion

        #region Constants
        private const string PROPERTY_PARTACOVERAGE = "PartACoverage";
        private const string PROPERTY_PARTACOVERAGEEFFECTIVEDATE = "PartACoverageEffectiveDate";
        private const string PROPERTY_PARTBCOVERAGE = "PartBCoverage";
        private const string PROPERTY_PARTBCOVERAGEEFFECTIVEDATE = "PartBCoverageEffectiveDate";
        private const string PROPERTY_PATIENTHASMEDICAREHMOCOOVERAGE = "PatientHasMedicareHMOCoverage";
        private const string PROPERTY_MEDICAREISSECONDARY = "MedicareIsSecondary";
        private const string PROPERTY_DATEOFLASTBILLINGACTIVITY = "DateOfLastBillingActivity";
        private const string PROPERTY_REMAININGBENEFITPERIOD = "RemainingBenefitPeriod";
        private const string PROPERTY_REMAININGCOINSURANCE = "RemainingCoInsurance"  ;
        private const string PROPERTY_REMAININGLIFETIMERESERVE = "RemainingLifeTimeReserve";
        private const string PROPERTY_REMAININGSNF= "RemainingSNF";
        private const string PROPERTY_REMAININGSNFCOINSURANCE = "RemainingSNFCoInsurance";
        private const string PROPERTY_REMAININGPARTADEDUCTIBLE = "RemainingPartADeductible";
        private const string PROPERTY_REMAININGPARTBDEDUCTIBLE = "RemainingPartBDeductible";
        private const string PROPERTY_VERIFICATIONCOPAY = "VerificationCoPay";
        private const string PROPERTY_VERIFIEDBENEFICIARYNAME = "VerifiedBeneficiaryName";
        
        #endregion
    }
}


