using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MailingAddressRequired.
    /// </summary>
    //TODO: Create XML summary comment for MailingAddressRequired
    [Serializable]
    [UsedImplicitly]
    public class MailingAddressPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MailingAddressPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MailingAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MailingAddressPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MailingAddressPreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if(context.GetType() != typeof(Account))
            {
               return true;
            }
            Account anAccount = context as Account;
            TypeOfContactPoint mailingType = new TypeOfContactPoint(TypeOfContactPoint.MAILING_OID,"Mailing");
            ContactPoint mailingContactPoint = anAccount.Patient.ContactPointWith(mailingType);
            
            if(mailingContactPoint != null
                && mailingContactPoint.Address != null
                && mailingContactPoint.Address.Address1 != null
                && mailingContactPoint.Address.Address1.Trim().Length > 0)
            {
                return true ;
            }
            else
            {
                if( this.FireEvents && this.MailingAddressPreferredEvent != null )
                {
                    this.MailingAddressPreferredEvent(this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
            }
        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MailingAddressPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

