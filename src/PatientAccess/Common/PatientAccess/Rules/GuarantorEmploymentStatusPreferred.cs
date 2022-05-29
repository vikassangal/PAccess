using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorEmploymentStatusPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorEmploymentStatusPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorEmploymentStatusPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GuarantorEmploymentStatusPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GuarantorEmploymentStatusPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GuarantorEmploymentStatusPreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to GuarantorInfo).
        /// Refer to GuarantorInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param EmploymentStatus="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Guarantor )
                && context.GetType().BaseType != typeof( Guarantor ))
            {
                return true;
            }

            Guarantor aGuarantor = context as Guarantor;
                      
            if( aGuarantor != null 
                && aGuarantor.Employment != null
                && aGuarantor.Employment.Status != null
                && aGuarantor.Employment.Status.Description != null
                && aGuarantor.Employment.Status.Description.Trim().Length > 0 )   
            {
                return true;
            }
            else
            {
                if( aGuarantor.Relationships.Count == 0 )
                {
                    return true;
                }

                foreach(Relationship r in aGuarantor.Relationships)
                {
                    if( r != null
						&& r.Type != null
						&&
						( r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPLOYEE
                        || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_UNKNOWN
                        || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_BLANK
                        || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPTY )
						)
                    {
                        return true;
                    }
                }

                if( this.FireEvents && GuarantorEmploymentStatusPreferredEvent != null )
                {
                    GuarantorEmploymentStatusPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public GuarantorEmploymentStatusPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
