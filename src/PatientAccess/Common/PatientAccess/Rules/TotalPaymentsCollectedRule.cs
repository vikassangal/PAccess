using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

//using Extensions.ClassLibrary;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for TotalPaymentsCollectedRule.
	/// </summary>
	//TODO: Create XML summary comment for TotalPaymentsCollectedRule
    [Serializable]
    [UsedImplicitly]
    public class TotalPaymentsCollectedRule : LeafRule
    {
        #region Events
        
        public event EventHandler TotalPaymentsCollectedEvent;

        #endregion
 
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TotalPaymentsCollectedEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TotalPaymentsCollectedEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TotalPaymentsCollectedEvent = null;    
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            
            Account anAccount = (Account)context;

            if( anAccount.TotalPaid < anAccount.Payment.TotalRecordedPayment )
            {
                if( this.FireEvents && TotalPaymentsCollectedEvent != null )
                {
                    this.TotalPaymentsCollectedEvent(this, null);
                }
            
                return false;
            }

            return true;
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
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
        public TotalPaymentsCollectedRule()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
