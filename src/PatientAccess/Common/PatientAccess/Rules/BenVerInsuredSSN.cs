using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerInsuredSSN.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerInsuredSSN : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerInsuredSSNEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredSSNEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerInsuredSSNEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerInsuredSSNEvent = null;  
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to InsuredInfo).
        /// Refer to InsuredInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Insured ) )
            {
                return true;
            }

            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_SELF_PAY )
            {
                return true;
            }

            Insured anInsured = context as Insured;
            
            if( anInsured.SocialSecurityNumber != null
                && anInsured.SocialSecurityNumber.DisplayString.Trim().Length > 0)
            {
               return true;
            }
            else
            {
                if( this.FireEvents && BenVerInsuredSSNEvent != null )
                {
                    BenVerInsuredSSNEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public BenVerInsuredSSN()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
