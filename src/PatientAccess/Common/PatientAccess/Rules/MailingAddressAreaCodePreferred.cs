using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MailingAddressAreaCodeRequired.
    /// </summary>
    //TODO: Create XML summary comment for MailingAddressAreaCodeRequired
    [Serializable]
    [UsedImplicitly]
    public class MailingAddressAreaCodePreferred : PreRegRule
    {
        #region Event Handlers
        public event EventHandler MailingAddressAreaCodePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MailingAddressAreaCodePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            MailingAddressAreaCodePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MailingAddressAreaCodePreferredEvent = null;   
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  

            Account anAccount = context as Account;
            TypeOfContactPoint mailingType = new TypeOfContactPoint(TypeOfContactPoint.MAILING_OID,"Mailing");
            ContactPoint mailingContactPoint = anAccount.Patient.ContactPointWith(mailingType);

            if( mailingContactPoint.PhoneNumber == null ||
                mailingContactPoint.PhoneNumber.AreaCode == null || 
                mailingContactPoint.PhoneNumber.AreaCode == string.Empty )
            {
                if( this.FireEvents && MailingAddressAreaCodePreferredEvent != null )
                {
                    MailingAddressAreaCodePreferredEvent( this, null );
                }
                return false ;
            }
            else
            {
                return true ;
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
        public MailingAddressAreaCodePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

