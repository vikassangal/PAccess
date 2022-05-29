using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for ModeOfArrivalPreferred.
	/// </summary>
	
	//TODO: Create XML summary comment for ModeOfArrivalPreferred
    [Serializable]
    [UsedImplicitly]
    public class ModeOfArrivalPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler ModeOfArrivalPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.ModeOfArrivalPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.ModeOfArrivalPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ModeOfArrivalPreferredEvent = null;   
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.ModeOfArrival == null ||
                anAccount.ModeOfArrival.Code.Equals(string.Empty) )
            {
                if( this.FireEvents && ModeOfArrivalPreferredEvent != null )
                {
                    ModeOfArrivalPreferredEvent( this, null );
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
        public ModeOfArrivalPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
