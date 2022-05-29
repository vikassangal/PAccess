using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for LocationPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LocationPreferred : LeafRule
    {
        #region Events

        public event EventHandler LocationPreferredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            LocationPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            LocationPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            LocationPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            
            var anAccount = (Account)context;

            
            if ( anAccount.Location                                  == null 
                || anAccount.Location.Room                          == null
                || anAccount.Location.Room.Code                     == null
                || anAccount.Location.Bed                           == null
                || anAccount.Location.Bed.Code                      == null
                || anAccount.Location.NursingStation                == null
                || anAccount.Location.NursingStation.Code           == null
                || anAccount.Location.NursingStation.Code.Trim()    == string.Empty
                || anAccount.Location.Room.Code.Trim()              == string.Empty
                || anAccount.Location.Bed.Code.Trim()               == string.Empty )
            {
                if ( FireEvents && LocationPreferredEvent != null )
                {
                    LocationPreferredEvent( this, null );
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
            return false;
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
