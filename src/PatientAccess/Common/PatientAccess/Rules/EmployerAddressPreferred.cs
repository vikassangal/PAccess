using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmployerAddressRequired.
    /// </summary>
    //TODO: Create XML summary comment for EmployerAddressRequired
    [Serializable]
    [UsedImplicitly]
    public class EmployerAddressPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler EmployerAddressPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            EmployerAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            EmployerAddressPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmployerAddressPreferredEvent = null;   
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

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  

            Account anAccount = context as Account;

            if( anAccount == null
                && anAccount.Patient == null
                && anAccount.Patient.Employment == null )
            {
                result = true;
            }
            else
            {
                Employer employer = anAccount.Patient.Employment.Employer as Employer;                 
               
                if( anAccount.Patient.Employment.Status != null
                    && anAccount.Patient.Employment.Status.Code != EmploymentStatus.NOT_EMPLOYED_CODE 
                    && anAccount.Patient.Employment.Status.Code != EmploymentStatus.RETIRED_CODE
                    && anAccount.Patient.Employment.Status.Code != EmploymentStatus.SELF_EMPLOYED_CODE
                    && anAccount.Patient.Employment.Status.Code != string.Empty
                    && ( employer != null
                        && employer.Name != string.Empty
                    &&
                    (   employer.PartyContactPoint == null
                    || employer.PartyContactPoint.Address == null
                    || employer.PartyContactPoint.Address.City.Trim() == string.Empty ) ) )
                {
                    result = false;
                }               
                else
                {                    
                    result = true;
                }
            }

            if( ! result
                && this.FireEvents && EmployerAddressPreferredEvent != null )
            {
                EmployerAddressPreferredEvent( this, null );
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
        public EmployerAddressPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

