using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MailingAddressPhoneRequired.
    /// </summary>
    //TODO: Create XML summary comment for MailingAddressPhoneRequired
    [Serializable]
    [UsedImplicitly]
    public class MailingAddressPhoneRequired : PostRegRule
    {
        #region Event Handlers
        public event EventHandler MailingAddressPhoneRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            MailingAddressPhoneRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            MailingAddressPhoneRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MailingAddressPhoneRequiredEvent = null;    
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
            bool result = true;

            if( context.GetType() != typeof( Account ) )
            {
               return true;
            }
            Account anAccount = context as Account;
            TypeOfContactPoint mailingType = new TypeOfContactPoint( TypeOfContactPoint.MAILING_OID, "Mailing" );
            ContactPoint mailingContactPoint = anAccount.Patient.ContactPointWith( mailingType );

            if( mailingContactPoint.PhoneNumber.Number.Equals( String.Empty ) )
            {
                if( this.FireEvents && MailingAddressPhoneRequiredEvent != null )
                {
                    MailingAddressPhoneRequiredEvent( this, null );
                }
                result = false;
            }
            return result;
        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MailingAddressPhoneRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

