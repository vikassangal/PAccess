using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary: Evaluates admit date entered in UI against admit date on account to verify
    /// that UI date is not more than 5 days before account date.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateEnteredFiveDaysPast : LeafRule
    {
        #region Events
        public event EventHandler AdmitDateEnteredFiveDaysPastEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            if( AdmitDateEnteredFiveDaysPastEvent != null )
            {
                Delegate[] delegates = this.AdmitDateEnteredFiveDaysPastEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }

            AdmitDateEnteredFiveDaysPastEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AdmitDateEnteredFiveDaysPastEvent -= eventHandler;
            return true;
        }
    
        public override void UnregisterHandlers()
        {
            this.AdmitDateEnteredFiveDaysPastEvent = null;  
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
			if ( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }

            Acct = (Account) context;
			
            if( Acct.Activity.GetType() != typeof( MaintenanceActivity ) && 
                Acct.Activity.GetType() != typeof( ShortMaintenanceActivity ) && 
                 Acct.Activity.GetType() != typeof( QuickAccountMaintenanceActivity ) )
			{
				return true;
			}
            
            DateTime originalDate = Acct.AdmitDateUnaltered;
            if (originalDate == DateTime.MinValue)
            {
                return true;
            }
            
            if (Acct.AdmitDate == DateTime.MinValue)
            {
                return true;
            }
            
            if(Acct.AdmitDate.Date == DateTime.MinValue.Date)
            {
                return true;
            }
            
            try
            {
                // Date entered on UI must be no more than 5 days before the 
                // admit date on account and cannot be later than account date.

                if( Acct.AdmitDate.Date < originalDate.AddDays(-5).Date )
                {
                    if( FireEvents && AdmitDateEnteredFiveDaysPastEvent != null )
                    {
                        AdmitDateEnteredFiveDaysPastEvent( this, null );
                    }
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Properties
        private Account Acct
        {
            get
            {
                return i_Acct;
            }
            set
            {
                i_Acct = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Construction and Finalization
        public AdmitDateEnteredFiveDaysPast()
        {
        }
        #endregion

        #region Data Elements
        private Account     i_Acct;
        #endregion
    }
}
