using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for SignatureOnCOSFormRequired
    [Serializable]
    [UsedImplicitly]
    public class SignatureOnCOSFormRequired : PostRegRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
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

            if ( context.GetType() != typeof( Account ) )
            {
                throw new ArgumentException( "Context in the rule is not an Account" );
            }
            Account anAccount = context as Account;
            if ( anAccount == null )
            {
                return false;
            }
            if ( ( anAccount.COSSigned != null ) &&
                ( anAccount.COSSigned.Code == "I" ) &&
                ( !anAccount.BillHasDropped )
                )
            {
                return false;
            } 

            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SignatureOnCOSFormRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
