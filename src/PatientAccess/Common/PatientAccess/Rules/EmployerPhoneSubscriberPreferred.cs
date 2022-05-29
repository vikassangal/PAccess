using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmployerPhoneSubscriberRequired.
    /// </summary>
    //TODO: Create XML summary comment for EmployerPhoneSubscriberRequired
    [Serializable]
    [UsedImplicitly]
    public class EmployerPhoneSubscriberPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler EmployerPhoneSubscriberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            EmployerPhoneSubscriberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            EmployerPhoneSubscriberPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmployerPhoneSubscriberPreferredEvent = null;   
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

            Account anAccount = context as Account;

            if( anAccount == null )
            {
                result = false;
            }

            if ( anAccount.Patient != null
                && anAccount.Patient.Employment != null 
                && anAccount.Patient.Employment.Status != null
                && ( anAccount.Patient.Employment.Status.Code == EmploymentStatus.RETIRED_CODE
                    || anAccount.Patient.Employment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE ) )
            {
                return true;
            }            

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            else
            {               
                if( anAccount.Patient == null
                    || anAccount.Patient.Employment == null
                    || anAccount.Patient.Employment.Employer == null )                 
                {
                    result = false;
                }
                else
                {            
                    Employer employer = (Employer)anAccount.Patient.Employment.Employer;

                    ContactPoint employerContactPoint = employer.PartyContactPoint;

                    if( employerContactPoint == null )
                    {
                        result = false;
                    }
                    else
                    {
                          if( employerContactPoint == null
                              || employerContactPoint.PhoneNumber == null
                              || string.IsNullOrEmpty( employerContactPoint.PhoneNumber.Number ) 
                            ) 
                        {                    
                            result = false;
                        }
                    }
                }
                
            }

            if( !result )
            {
                if( this.FireEvents && EmployerPhoneSubscriberPreferredEvent != null )
                {
                    EmployerPhoneSubscriberPreferredEvent( this, null );
                }
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
        public EmployerPhoneSubscriberPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

