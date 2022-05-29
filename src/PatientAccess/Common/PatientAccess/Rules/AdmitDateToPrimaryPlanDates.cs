using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateToPrimaryPlanDates.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateToPrimaryPlanDates : LeafRule
    {
        #region Events
        public event EventHandler AdmitDateToPrimaryPlanDatesEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateToPrimaryPlanDatesEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateToPrimaryPlanDatesEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateToPrimaryPlanDatesEvent = null;  
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( !(context is Account) 
                || this.AssociatedControl == null )
            {
                return true;
            }

            Account anAccount = context as Account;

            if( anAccount == null)
            {
                return false;
            }

            string strName = this.AssociatedControl as string;

            if( strName != string.Empty
                && ( strName == DEMOGRAPHICS_VIEW || strName == STREAMLINED_ACTIVATION || strName == QUICKACCOUNTCREATIONVIEW || strName == WALKINACCOUNTCREATIONVIEW ) 
                && anAccount.AdmitDate != DateTime.MinValue )
            {
                if( !this.planIsValid( anAccount ) )
                {
                    if( this.FireEvents && AdmitDateToPrimaryPlanDatesEvent != null )
                    {
                        AdmitDateToPrimaryPlanDatesEvent( this, null );
                    }
                    return false;

                }
            }

            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// Determine if there is a valid plan contract for the specified plan
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        private bool planIsValid(Account anAccount)
        {
            bool rc = true;

            Coverage existingCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);

            if( existingCoverage != null )
            {
                rc = existingCoverage.InsurancePlan.IsValidFor( anAccount.AdmitDate );
            }

            return rc;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public AdmitDateToPrimaryPlanDates()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string DEMOGRAPHICS_VIEW = "demographicsView";
        private const string STREAMLINED_ACTIVATION = "ActivatePreRegistrationView";
        private const string QUICKACCOUNTCREATIONVIEW = "quickAccountCreationView";
        private const string  WALKINACCOUNTCREATIONVIEW = "paiWalkinAccountCreationView";


        #endregion
    }


}
