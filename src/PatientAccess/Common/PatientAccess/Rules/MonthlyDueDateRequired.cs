using System;
using Extensions.UI.Builder; 
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MonthlyDueDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MonthlyDueDateRequired : LeafRule
    {
        [NonSerialized]
        private IMonthlyDueDateFeatureManager _monthlyDueDateFeatureManager;

        private IMonthlyDueDateFeatureManager monthlyDueDateFeatureManager
        {
            get { return _monthlyDueDateFeatureManager; }
            set { _monthlyDueDateFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler MonthlyDueDateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MonthlyDueDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MonthlyDueDateRequiredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            MonthlyDueDateRequiredEvent = null;   
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
            bool result = true;

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  

            var anAccount = context as Account;
            if (anAccount == null)
            {
                return true;
            }

            monthlyDueDateFeatureManager = new MonthlyDueDateFeatureManager();

            if (monthlyDueDateFeatureManager.IsMonthlyDueDateEnabled(anAccount.Facility) &&
                anAccount.HasOutstandingBalance && anAccount.NumberOfMonthlyPayments > 0 && 
                (anAccount.MonthlyPayment > 0 )&&
                String.IsNullOrEmpty(anAccount.DayOfMonthPaymentDue.Trim()))

            {
                if (FireEvents && MonthlyDueDateRequiredEvent != null)
                {
                    MonthlyDueDateRequiredEvent(this, null);
                }
                result = false;
            }
            return result;
        }
        #endregion
 
    }
}
