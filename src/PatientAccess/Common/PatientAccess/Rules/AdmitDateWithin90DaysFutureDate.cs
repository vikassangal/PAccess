using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateWithin90DaysFutureDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateWithin90DaysFutureDate : LeafRule
    {
        #region Events
        public event EventHandler AdmitDateWithin90DaysFutureDateEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            AdmitDateWithin90DaysFutureDateEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AdmitDateWithin90DaysFutureDateEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            AdmitDateWithin90DaysFutureDateEvent = null;  
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
            var anAccount = context as Account;
            if( anAccount == null )
            {
                return true;
            }
            // the admit date must be within 90 days 
            if (anAccount.AdmitDate.Date > DateTime.Now.AddDays(90))
            {
                if ( FireEvents && AdmitDateWithin90DaysFutureDateEvent != null )
                {
                    AdmitDateWithin90DaysFutureDateEvent( this, null );
                }
                return false;
            }
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

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
