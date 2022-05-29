using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BillingAddressPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BillingAddressPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler BillingAddressPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BillingAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BillingAddressPreferredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BillingAddressPreferredEvent = null;  
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
                context.GetType() != typeof( ContactPoint )  &&
                context.GetType().BaseType != typeof ( ContactPoint ) )
            {                
                return true;
            } 

            ContactPoint cp = null;

            try
            {
                cp = (ContactPoint)context;                     
            }       
            catch
            {
                return true;
            }

            if( cp.Address == null || cp.Address.AsMailingLabel() == string.Empty )
            {
                if( this.FireEvents && BillingAddressPreferredEvent != null )
                {
                    BillingAddressPreferredEvent( this, null );
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
        public BillingAddressPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
