using System;
using PatientAccess.Annotations;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class OtherCoverage : CommercialCoverage
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void RemoveCoveragePayorData( OtherCoverage coverage )
        {
            this.TrackingNumber                         = string.Empty;           
            this.RemoveAuthorization();
        }

        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays                          = 0;
            this.Authorization.AuthorizationNumber      = string.Empty;
            this.Authorization.NumberOfDaysAuthorized   = 0;
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
        #endregion

        #region Private Methods
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

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OtherCoverage(): base()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}


