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
    public class EmployerPhoneAreaCodeRequired : PostRegRule
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
            if( anAccount == null )
            {
                return false;
            }
            Employer employer = (Employer)anAccount.Patient.Employment.Employer;
            TypeOfContactPoint employerType = new TypeOfContactPoint(TypeOfContactPoint.EMPLOYER_OID,"Employer");
            ContactPoint employerContactPoint = employer.ContactPointWith(employerType);
            if(base.CanBeAppliedTo(context) == true && 
                employerContactPoint.PhoneNumber.AreaCode != string.Empty)
            {
                return false;
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
        public EmployerPhoneAreaCodeRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

