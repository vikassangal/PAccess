using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerInsuredLastName.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerInsuredLastName : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerInsuredLastNameEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredLastNameEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredLastNameEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerInsuredLastNameEvent     = null;  
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
        /// <param LastName="context"></param>
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
           
            if( anInsured != null 
                && anInsured.Name != null
                && anInsured.Name.LastName != null
                && anInsured.Name.LastName.Trim().Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && BenVerInsuredLastNameEvent != null )
                {
                    BenVerInsuredLastNameEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public BenVerInsuredLastName()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
