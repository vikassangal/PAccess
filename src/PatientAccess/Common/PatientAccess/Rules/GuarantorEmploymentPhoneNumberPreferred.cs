using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorEmploymentPhoneNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorEmploymentPhoneNumberPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorEmploymentPhoneNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GuarantorEmploymentPhoneNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GuarantorEmploymentPhoneNumberPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            GuarantorEmploymentPhoneNumberPreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to EmploymentInfo).
        /// Refer to EmploymentInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param PhoneNumber="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || context.GetType() != typeof( Account )
                                   && context.GetType().BaseType != typeof( Account ) )
            {
                return true;
            }

            Account anAccount = context as Account;
            if ( anAccount == null || anAccount.Guarantor == null )
            {
                return true;
            }

            Guarantor aGuarantor = anAccount.Guarantor;
            if ( aGuarantor.Employment == null )
            {
                return true;
            }

            Employment anEmployment = anAccount.Guarantor.Employment;
            if ( anEmployment != null && anEmployment.Status != null &&
                 ( anEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE ||
                   anEmployment.Status.Code == EmploymentStatus.RETIRED_CODE ||
                   anEmployment.Status.Code == string.Empty ) )
            {
                return true;
            }

            if ( anEmployment != null
                && anEmployment.Employer != null
                && anEmployment.Employer.PartyContactPoint != null
                && anEmployment.Employer.PartyContactPoint.PhoneNumber != null
                && anEmployment.Employer.PartyContactPoint.PhoneNumber.Number != null
                && anEmployment.Employer.PartyContactPoint.PhoneNumber.Number.Trim().Length > 0 )
            {
                return true;
            }

            if ( FireEvents && GuarantorEmploymentPhoneNumberPreferredEvent != null )
            {
                GuarantorEmploymentPhoneNumberPreferredEvent( this, new PropertyChangedArgs( AssociatedControl ) );
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
