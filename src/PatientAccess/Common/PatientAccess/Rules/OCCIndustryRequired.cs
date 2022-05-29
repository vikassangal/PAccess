using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for OCCIndustryRequired.
    /// </summary>
    //TODO: Create XML summary comment for OCCIndustryRequired
    [Serializable]
    [UsedImplicitly]
    public class OCCIndustryRequired : PostRegRule
    {
        #region Event Handlers
        
        public event EventHandler OCCIndustryRequiredEvent;
        
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            OCCIndustryRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            OCCIndustryRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.OCCIndustryRequiredEvent = null;   
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

            if( anAccount.Patient != null &&
                anAccount.Patient.Employment != null &&
                anAccount.Patient.Employment.Status != null &&
                
                ( anAccount.Patient.Employment.Status.Code != string.Empty
                && anAccount.Patient.Employment.Status.Oid.ToString() != EmploymentStatus.NOT_EMPLOYED_CODE ) )
            {
                if( anAccount.Patient.Employment.Occupation == string.Empty )      
                {
                    if( this.FireEvents && OCCIndustryRequiredEvent != null )
                    {
                        OCCIndustryRequiredEvent( this, null );
                    }
                    return false;
                }
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
        public OCCIndustryRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

