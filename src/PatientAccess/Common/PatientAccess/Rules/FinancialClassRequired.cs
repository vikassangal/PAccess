using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for FinancialClassRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class FinancialClassRequired : LeafRule
    {
        #region Events

        public event EventHandler FinancialClassRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.FinancialClassRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.FinancialClassRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.FinancialClassRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            Account anAccount = (Account)context;

            if( anAccount.Activity.GetType() ==  typeof( PreRegistrationActivity ) 
                &&  !HasPrimarySelected( anAccount ) ) 
            {
                return true;
            }

            if( anAccount.FinancialClass == null 
                || anAccount.FinancialClass.Code.Trim() == string.Empty ) 
            {
                if( this.FireEvents && FinancialClassRequiredEvent != null )
                {
                    this.FinancialClassRequiredEvent(this, null);
                }
            
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private bool HasPrimarySelected( Account anAccount )
        {
            if( anAccount.Insurance.Coverages.Count == 0 )
            {
                return false;
            }

            bool hasPrimary = false;
            foreach( Coverage coverage in anAccount.Insurance.Coverages )
            {
                if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    hasPrimary = true;
                    break;
                }
            }    
        
            return hasPrimary;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public FinancialClassRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
