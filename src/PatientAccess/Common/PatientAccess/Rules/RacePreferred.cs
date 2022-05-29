using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for RaceRequired.
    /// </summary>
    //TODO: Create XML summary comment for RaceRequired
    [Serializable]
    [UsedImplicitly]
    public class RacePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler RacePreferredPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            RacePreferredPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            RacePreferredPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.RacePreferredPreferredEvent = null;    
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
            if( context.GetType() != typeof(Account) )
            {
                throw new ArgumentException( "Context in the rule is not an Account" );
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.Patient != null && 
                ( anAccount.Patient.Race == null
                  || anAccount.Patient.Race.Code.Trim() == String.Empty
                  || anAccount.Patient.Race.Code.Trim() == Race.ZERO_CODE )
                )
            {
                if( this.FireEvents && RacePreferredPreferredEvent != null )
                {
                    RacePreferredPreferredEvent( this, null );
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
        public RacePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        #endregion
    }
}

