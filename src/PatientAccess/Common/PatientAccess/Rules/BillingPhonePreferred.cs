using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BillingPhonePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BillingPhonePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler BillingPhonePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BillingPhonePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BillingPhonePreferredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BillingPhonePreferredEvent = null;  
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
                aCoverage.BillingInformation.PhoneNumber == null ||
                aCoverage.BillingInformation.PhoneNumber.ToString() == string.Empty )
            {
                if( this.FireEvents && BillingPhonePreferredEvent != null )
                {
                    BillingPhonePreferredEvent( this, null );
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
        public BillingPhonePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
