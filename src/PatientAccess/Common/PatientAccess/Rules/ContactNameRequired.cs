using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactNameRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactNameRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler ContactNameRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ContactNameRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ContactNameRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ContactNameRequiredEvent = null;   
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
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            EmergencyContact anEC = context as EmergencyContact;
           
            if( anEC.Name != null
                && anEC.Name.Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && ContactNameRequiredEvent != null )
                {
                    ContactNameRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public ContactNameRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
