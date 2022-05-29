using System;
using Extensions.UI.Builder;
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
    public class OCCIndustryPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler OCCIndustryPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            OCCIndustryPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            OCCIndustryPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.OCCIndustryPreferredEvent = null;   
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
            if( context.GetType() != typeof( Account ) )
            {
                throw new ArgumentException( "Context in the rule is not an Account" );
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.Patient != null &&
                anAccount.Patient.Employment != null &&
                anAccount.Patient.Employment.Status != null &&

                ( anAccount.Patient.Employment.Status.Code != string.Empty
                && anAccount.Patient.Employment.Status.Code != EmploymentStatus.RETIRED_CODE
                && anAccount.Patient.Employment.Status.Code != EmploymentStatus.NOT_EMPLOYED_CODE 
                ) )
            {
                if( anAccount.Patient.Employment.Occupation == string.Empty )      
                {
                    if( this.FireEvents && OCCIndustryPreferredEvent != null )
                    {
                        OCCIndustryPreferredEvent( this, null );
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
        public OCCIndustryPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

