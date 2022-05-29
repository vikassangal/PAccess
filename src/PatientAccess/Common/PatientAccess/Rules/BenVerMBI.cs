using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerMBI.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerMBI : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerMBIEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerMBIEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerMBIEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerMBIEvent     = null;  
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
            if( context == null || 
                context.GetType() != typeof( GovernmentMedicareCoverage ) )
            {                
                return true;
            } 	                        

            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE )
            {
                return true;
            }

            GovernmentMedicareCoverage aCoverage = context as GovernmentMedicareCoverage;

            if( aCoverage != null && aCoverage.MBINumber != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && BenVerMBIEvent != null )
                {
                    BenVerMBIEvent( this, null );
                }
                return false;
            }
        }
        #endregion

        
    }
}
