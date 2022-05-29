using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for SocialSecurityNumberStatusRequired
    [Serializable]
    [UsedImplicitly]
    public class SocialSecurityNumberStatusRequired : PostRegRule
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            Account anAccount = context as Account;
            SocialSecurityNumber ssnNumber  =  anAccount.Patient.SocialSecurityNumber;
            ContactPoint contactPoint  = anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

            if( contactPoint == null || contactPoint.Address == null || contactPoint.Address.State == null )
            {
                return true;
            }
            else if( contactPoint.Address.State.Code == "FL" )
            {
                if( ssnNumber.ToString() == "7777777777" )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if( ssnNumber.ToString() == "9999999999" )
                {
                    return false;
                }
                else
                {
                    return true;
                }
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
        public SocialSecurityNumberStatusRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
