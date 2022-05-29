using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BillingCONamePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BillingCONamePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler BillingCONamePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BillingCONamePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BillingCONamePreferredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BillingCONamePreferredEvent = null;  
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || 
                context.GetType() != typeof( Coverage ) &&
                context.GetType().BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof ( Coverage )
                )
            {                
                return true;
            } 

            Coverage aCoverage = null;

            try
            {
                aCoverage = (Coverage)context;                     
            }       
            catch
            {
                return true;
            }

//            Coverage aCoverage = context as Coverage;
           
            if( aCoverage.BillingInformation == null ||
                aCoverage.BillingInformation.BillingCOName == null ||
                aCoverage.BillingInformation.BillingCOName == string.Empty )
            {
                if( this.FireEvents && BillingCONamePreferredEvent != null )
                {
                    BillingCONamePreferredEvent( this, null );
                }
                return false;
            }
            else
            {
                return true;
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
        public BillingCONamePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
