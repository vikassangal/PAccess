using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateRequired : LeafRule
    {
        #region Events
        public event EventHandler AdmitDateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateRequiredEvent = null;  
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
		
            if(context != null && context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.AdmitDate.Date.Equals( DateTime.MinValue ) )
            {
                if( this.FireEvents && AdmitDateRequiredEvent != null )
                {
                    AdmitDateRequiredEvent( this, null );
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

        public AdmitDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
