using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorAddressPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorAddressPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorAddressPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GuarantorAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GuarantorAddressPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GuarantorAddressPreferredEvent = null;   
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
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Guarantor ) )
            {
                return true;
            }

            Guarantor aGuarantor = context as Guarantor;
            
            ContactPoint cp = aGuarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
           
            if( cp != null && cp.Address != null  
                && cp.Address.Address1 != null
                && cp.Address.Address1.Trim().Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && GuarantorAddressPreferredEvent != null )
                {
                    GuarantorAddressPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public GuarantorAddressPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
