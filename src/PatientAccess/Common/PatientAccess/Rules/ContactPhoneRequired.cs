using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactPhoneRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactPhoneRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler ContactPhoneRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ContactPhoneRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ContactPhoneRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ContactPhoneRequiredEvent = null;   
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
        /// <param Phone="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            EmergencyContact anEC = context as EmergencyContact;           
            ContactPoint cp         = new ContactPoint();

            cp = anEC.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());

            if( cp != null
                && cp.PhoneNumber != null
                && cp.PhoneNumber.ToString().Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && ContactPhoneRequiredEvent != null )
                {
                    ContactPhoneRequiredEvent( this, null );
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
        public ContactPhoneRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
