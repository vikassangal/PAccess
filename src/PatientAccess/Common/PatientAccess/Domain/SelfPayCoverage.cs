using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class SelfPayCoverage : Coverage
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoveragePayorData()
        {
        }

        public override void RemoveCoverageVerificationData()
        {

            base.RemoveCoverageVerificationData();
            
            this.PatientHasMedicaid = new YesNoFlag();
            this.ForceUnChangedStatusFor( "PatientHasMedicaid" );
            this.InsuranceInfoUnavailable = new YesNoFlag();
            this.ForceUnChangedStatusFor( "InsuranceInfoUnavailable" );

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

            return account;
        }

        protected override bool HaveVerificationFieldsChanged
        {
            get
            {

                return base.HaveVerificationFieldsChanged || 
                       this.HasChangedFor( "PatientHasMedicaid" ) || 
                       this.HasChangedFor( "InsuranceInfoUnavailable" );
                       
            }
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

        public YesNoFlag PatientHasMedicaid
        {
            get
            {
                return i_patientHasMedicaid;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_patientHasMedicaid, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag InsuranceInfoUnavailable
        {
            get
            {
                return i_insuranceInfoUnavailable;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_insuranceInfoUnavailable, value, MethodBase.GetCurrentMethod() );
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SelfPayCoverage(): base()
        {
        }
        #endregion

        #region Data Elements
        private YesNoFlag i_patientHasMedicaid = new YesNoFlag();
        private YesNoFlag i_insuranceInfoUnavailable = new YesNoFlag();
        #endregion

        #region Constants
        #endregion
    }
}


