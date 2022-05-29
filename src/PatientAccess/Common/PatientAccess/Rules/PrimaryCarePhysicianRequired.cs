using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PrimaryCarePhysicianRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PrimaryCarePhysicianRequired : LeafRule
    {
    
        #region Events

        public event EventHandler PrimaryCarePhysicianRequiredEvent;

        #endregion        

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            PrimaryCarePhysicianRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            PrimaryCarePhysicianRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            PrimaryCarePhysicianRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof (Account))
            {
                return true;
            }

            Account anAccount = (Account) context;

            if (anAccount.Activity == null)
            {
                return true;
            }

            if (anAccount.KindOfVisit == null)
            {
                return true;
            }


            var accountDoesNotHaveAnPrimaryCarePhysician = AccountDoesNotHaveAnPrimaryCarePhysician(anAccount);

            primaryCarePhysicianRequiredFeatureManager = new PrimaryCarePhysicianRequiredFeatureManager();
            var makePrimarycarephysicianrequired =
                primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(anAccount);

            if (accountDoesNotHaveAnPrimaryCarePhysician &&
                makePrimarycarephysicianrequired)
            {
                if (FireEvents && PrimaryCarePhysicianRequiredEvent != null)
                {
                    PrimaryCarePhysicianRequiredEvent(this, null);
                }

                return false;
            }
            return true;
        }

        private static bool AccountDoesNotHaveAnPrimaryCarePhysician(IAccount anAccount)
        {
            return ( anAccount.PrimaryCarePhysician == null || anAccount.PrimaryCarePhysician.FirstName.Trim() + anAccount.PrimaryCarePhysician.LastName.Trim() == string.Empty );
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }
     
        #endregion

        #region Private Properties
        private IPrimaryCarePhysicianRequiredFeatureManager primaryCarePhysicianRequiredFeatureManager { get; set; }

        #endregion
      
    }


}
