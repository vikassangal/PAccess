using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// MailingAddressZipRequired - ensure that the mailing address zip code is present
    /// </summary>

    [Serializable]
    [UsedImplicitly]
    public class MailingAddressZipRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler MailingAddressZipRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MailingAddressZipRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MailingAddressZipRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.MailingAddressZipRequiredEvent = null;
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
            if (!( context is Account ))
            {
                return true;
            }
            Account anAccount = context as Account;
            TypeOfContactPoint mailingType = new TypeOfContactPoint( TypeOfContactPoint.MAILING_OID, "Mailing" );
            ContactPoint mailingContactPoint = anAccount.Patient.ContactPointWith( mailingType );

            if( mailingContactPoint != null
                && mailingContactPoint.Address != null               
                && !string.IsNullOrEmpty( mailingContactPoint.Address.Address1.Trim() ) )
            {
                Address addr = mailingContactPoint.Address;

                string zip = String.Empty;
                if ( addr.ZipCode != null )
                {
                    zip = addr.ZipCode.ZipCodePrimary;
                }

                if( zip != null )
                {
                    zip = zip.Replace( "-", string.Empty );
                    zip = zip.Trim();
                }

                if( ( string.IsNullOrEmpty( zip )
                    || zip == "0" )
                    &&
                    ( addr.Country != null
                        && addr.Country.Code != null
                        && addr.Country.Code == Country.USA_CODE ) )
                {
                    if( this.FireEvents && MailingAddressZipRequiredEvent != null )
                    {
                        this.MailingAddressZipRequiredEvent( this, null );
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
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
        public MailingAddressZipRequired()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}

