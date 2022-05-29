using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for FinancialClassPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class FinancialClassPreferred : LeafRule
    {
        #region Events

        public event EventHandler FinancialClassPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.FinancialClassPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.FinancialClassPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.FinancialClassPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            Account anAccount = (Account)context;

            if( anAccount.Activity.GetType().Equals( typeof( PreRegistrationActivity ) ) &&
                !HasPrimarySelected( anAccount ) &&
                anAccount.FinancialClass == null )                
            {
                if( this.FireEvents && FinancialClassPreferredEvent != null )
                {
                    this.FinancialClassPreferredEvent(this, null);
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

        public FinancialClassPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
