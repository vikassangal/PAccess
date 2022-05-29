using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonSSNPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonSSNPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonSSNPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonSSNPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonSSNPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonSSNPreferredEvent = null;   
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
        /// <param SSN="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person )
                && context.GetType().BaseType.BaseType != typeof(Person) )
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( aPerson != null 
                && aPerson.SocialSecurityNumber != null 
                && aPerson.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() != string.Empty )
            {
                return true;
            }
            else
            {
                if( context.GetType() == typeof(Guarantor)
                    || context.GetType() == typeof(Insured) )
                {                    
                    if( aPerson.Relationships.Count == 0 )
                    {
                        return true;
                    }

                    foreach(Relationship r in aPerson.Relationships)
                    {
						if( r != null
							&& r.Type != null
							&&
							(r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPLOYEE
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_UNKNOWN
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_BLANK
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPTY
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_ORGANDONOR      
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_CADAVAR 
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_INJUREDPLAINTIFF)
							)
                        {
                            return true;
                        }
                    }
                }

                if( this.FireEvents && PersonSSNPreferredEvent != null )
                {
                    PersonSSNPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonSSNPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
