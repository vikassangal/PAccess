using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorEmployerAddressRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorEmployerAddressRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorEmployerAddressRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GuarantorEmployerAddressRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GuarantorEmployerAddressRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GuarantorEmployerAddressRequiredEvent = null;   
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

            bool rc = true;

            Guarantor aGuarantor = context as Guarantor;
            
            if( aGuarantor.Employment != null
                && aGuarantor.Employment.Status != null
                && ( aGuarantor.Employment.Status.Description.Trim().Length > 0 
                && !aGuarantor.Employment.Status.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE ) ) )
            {
                if( aGuarantor.Employment.Employer != null )
                {
                    ContactPoint cp = aGuarantor.Employment.Employer.PartyContactPoint;

                    // we have employment status, check if we have an address

                    if( cp != null 
                        && cp.Address != null
                        && cp.Address.Address1.Trim().Length > 0 )
                    {
                        rc = true;
                    }
                    else
                    {                        
                        rc = false;
                    }
                }
                else
                {
                    rc = false;
                }
            }

            if( !rc && this.FireEvents && GuarantorEmployerAddressRequiredEvent != null )
            {
                GuarantorEmployerAddressRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
            }

            return rc;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GuarantorEmployerAddressRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
