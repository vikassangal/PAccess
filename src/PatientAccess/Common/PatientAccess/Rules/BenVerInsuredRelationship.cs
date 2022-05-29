using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerInsuredRelationship.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerInsuredRelationship : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerInsuredRelationshipEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredRelationshipEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredRelationshipEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerInsuredRelationshipEvent = null;  
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
            if( context == null || context.GetType() != typeof( Insured )
                && context.GetType().BaseType != typeof( Insured ))
            {
                return true;
            }

            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE 
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_SELF_PAY )
            {
                return true;
            }

            Insured anInsured = context as Insured;
           
            if( anInsured.Relationships.Count > 0 )
            {
                foreach( Relationship r in anInsured.Relationships  )
                {
                    if( r.Type != null
                        && r.Type.Description.Trim().Length > 0 )
                    {
                        return true;
                    }
                    else
                    {
                        if( this.FireEvents && BenVerInsuredRelationshipEvent != null )
                        {
                            BenVerInsuredRelationshipEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                        }

                        return false;
                    }
                }
            
                return true;
            }
            else
            {
                if( this.FireEvents && BenVerInsuredRelationshipEvent != null )
                {
                    BenVerInsuredRelationshipEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public BenVerInsuredRelationship()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
