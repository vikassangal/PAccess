using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonRelationshipRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonRelationshipRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonRelationshipRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonRelationshipRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonRelationshipRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonRelationshipRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to PersonInfo).
        /// Refer to PersonInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( aPerson.Relationships.Count > 0 )
            {
                foreach( Relationship r in aPerson.Relationships  )
                {
                    if( r.Type != null
                        && r.Type.Description.Trim().Length > 0 )
                    {
                        return true;
                    }
                    else
                    {
                        if( this.FireEvents && PersonRelationshipRequiredEvent != null )
                        {
                            PersonRelationshipRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                        }

                        return false;
                    }
                }
            
                return true;
            }
            else
            {
                if( this.FireEvents && PersonRelationshipRequiredEvent != null )
                {
                    PersonRelationshipRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonRelationshipRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
