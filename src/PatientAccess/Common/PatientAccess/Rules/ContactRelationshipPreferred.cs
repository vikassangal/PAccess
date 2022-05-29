using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactRelationshipPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactRelationshipPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler ContactRelationshipPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ContactRelationshipPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ContactRelationshipPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ContactRelationshipPreferredEvent = null;   
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
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            EmergencyContact anEC = context as EmergencyContact;
           
            if( anEC.Relationships.Count > 0 && anEC.RelationshipType.Code != String.Empty )
			{
                return true;
            }
            
            if( this.FireEvents && ContactRelationshipPreferredEvent != null )
            {
                ContactRelationshipPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
            }
            
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
        public ContactRelationshipPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
