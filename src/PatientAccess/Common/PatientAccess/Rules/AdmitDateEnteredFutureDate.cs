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
    /// that UI date is not later than account date.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateEnteredFutureDate : LeafRule
	{
        #region Events
        public event EventHandler AdmitDateEnteredFutureDateEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            if( AdmitDateEnteredFutureDateEvent != null )
            {
                Delegate[] delegates = this.AdmitDateEnteredFutureDateEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }

            AdmitDateEnteredFutureDateEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AdmitDateEnteredFutureDateEvent -= eventHandler;
            return true;
        }
                        
        public override void UnregisterHandlers()
        {
            this.AdmitDateEnteredFutureDateEvent = null;   
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
            if( context == null || context.GetType() != typeof( Account ) )
			{
                return true;
            }
			
            Acct = (Account) context;
			if( Acct.Activity.GetType() != typeof( MaintenanceActivity)  &&
                Acct.Activity.GetType() != typeof( ShortMaintenanceActivity ) &&
                  Acct.Activity.GetType() != typeof( QuickAccountMaintenanceActivity ) )
			{
				return true;
			}

            DateTime originalDate = Acct.AdmitDateUnaltered;

			if( originalDate == DateTime.MinValue
				|| Acct.AdmitDate == DateTime.MinValue )
			{
				return true;
			}

            try
            {
				// Date entered on UI must not be later than account date.
                if( originalDate.Date < Acct.AdmitDate.Date )
                {
                    if( FireEvents && AdmitDateEnteredFutureDateEvent != null )
                    {
                        AdmitDateEnteredFutureDateEvent( this, null );
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
        public AdmitDateEnteredFutureDate()
		{
		}
        #endregion

        #region Data Elements
        private Account     i_Acct;
        #endregion
    }
}
