using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmployerRequired.
    /// </summary>
    //TODO: Create XML summary comment for EmployerRequired
    [Serializable]
    [UsedImplicitly]
    public class EmployerRequired : LeafRule
    {
        #region Event Handlers

        public event EventHandler EmployerRequiredEvent;


        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmployerRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmployerRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmployerRequiredEvent = null;    
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
            bool rc = true;

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  

            Account anAccount = context as Account;
            Employer employer = null;

            if( anAccount.Patient == null
                || anAccount.Patient.Employment == null )
            {
                rc = true;
            }
            else
            {            
                if ( anAccount.Patient.Employment.Status != null
                    && (
                    anAccount.Patient.Employment.Status.Code == string.Empty
                    || anAccount.Patient.Employment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE ) )
                {
                    rc = true;
                }                
                else
                {
                    employer = (Employer)anAccount.Patient.Employment.Employer;

                    if( employer == null 
                        || employer.Name.Trim() == string.Empty )
                    {
                        rc = false;

                        if( this.FireEvents && this.EmployerRequiredEvent != null )
                        {
                            this.EmployerRequiredEvent(this, null);
                        }
                    }
                }
            }            

            return rc;

        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmployerRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

