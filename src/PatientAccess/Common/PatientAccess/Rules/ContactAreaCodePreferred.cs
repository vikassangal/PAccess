using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactAreaCodePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactAreaCodePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler ContactAreaCodePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ContactAreaCodePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ContactAreaCodePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.ContactAreaCodePreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to ContactInfo).
        /// Refer to ContactInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param AreaCode="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            EmergencyContact anEC = context as EmergencyContact;
            ContactPoint cp = new ContactPoint();

            cp = anEC.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

            if( cp != null
                && cp.PhoneNumber != null
                && cp.PhoneNumber.AreaCode.Trim().Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && ContactAreaCodePreferredEvent != null )
                {
                    ContactAreaCodePreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
            }

        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ContactAreaCodePreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
