using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AreaCodeRequiresPhoneNumber.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AreaCodeRequiresPhoneNumber : LeafRule
    {
        #region Events
        public event EventHandler AreaCodeRequiresPhoneNumberEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.AreaCodeRequiresPhoneNumberEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.AreaCodeRequiresPhoneNumberEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AreaCodeRequiresPhoneNumberEvent = null;  
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

            if( !( context is PhoneNumber ) )
            {
                return true;
            }
            else if( context == null )
            {
                return false;
            }

            PhoneNumber phoneNumber = context as PhoneNumber;

            if( phoneNumber.AreaCode.Trim().Length == 3
                && phoneNumber.Number.Trim().Length == 0 )
            {
                if( this.FireEvents && AreaCodeRequiresPhoneNumberEvent != null )
                {
                    AreaCodeRequiresPhoneNumberEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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

        public AreaCodeRequiresPhoneNumber()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
