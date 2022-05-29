using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmployerPhoneAreaCodeRequired.
    /// </summary>
    //TODO: Create XML summary comment for EmployerPhoneAreaCodeRequired
    [Serializable]
    [UsedImplicitly]
    public class EmployerPhoneAreaCodePreferred : PreRegRule
    {
        #region Events

        public event EventHandler EmployerPhoneAreaCodePreferredEvent;
        
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmployerPhoneAreaCodePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmployerPhoneAreaCodePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmployerPhoneAreaCodePreferredEvent = null;   
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

            if( anAccount.Patient != null
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
                            || string.IsNullOrEmpty( employerContactPoint.PhoneNumber.AreaCode )
                          )
                        {
                            result = false;
                        }
                    }
                }

            }

            if( !result )
            {
                if( this.FireEvents && EmployerPhoneAreaCodePreferredEvent != null )
                {
                    EmployerPhoneAreaCodePreferredEvent( this, null );
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
        public EmployerPhoneAreaCodePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

