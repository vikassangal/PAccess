using System;
using Extensions.UI.Builder;
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
    public class MailingAddressPhonePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MailingAddressPhonePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MailingAddressPhonePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MailingAddressPhonePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MailingAddressPhonePreferredEvent = null;   
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
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            TypeOfContactPoint mailingType = new TypeOfContactPoint( TypeOfContactPoint.MAILING_OID, "Mailing" );
            ContactPoint mailingContactPoint = anAccount.Patient.ContactPointWith( mailingType );

            if (mailingContactPoint.PhoneNumber == null ||
                mailingContactPoint.PhoneNumber.Number == null ||
                (mailingContactPoint.PhoneNumber.AreaCode.Equals(String.Empty) &&
                 mailingContactPoint.PhoneNumber.Number.Equals(String.Empty)
                )
            )
            {
                if (this.FireEvents && MailingAddressPhonePreferredEvent != null)
                {
                    MailingAddressPhonePreferredEvent(this, null);
                }

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
        public MailingAddressPhonePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

