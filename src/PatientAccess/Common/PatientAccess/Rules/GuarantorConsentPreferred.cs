using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
	[Serializable]
	[UsedImplicitly]
    public class GuarantorConsentPreferred : LeafRule
    {
        # region Event Handlers
        public event EventHandler GuarantorConsentPreferredEvent;
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            GuarantorConsentPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            GuarantorConsentPreferredEvent -= eventHandler;
            return true;
        }
               
        public override void UnregisterHandlers()
        {
            GuarantorConsentPreferredEvent = null;            
        }

	    public override bool CanBeAppliedTo(object context)
	    {
            if (context == null || context.GetType() != typeof(Guarantor)
                           && context.GetType().BaseType != typeof(Guarantor))
            {
                return true;
            }

            var guarantor = context as Guarantor;

            if (guarantor != null && guarantor.ContactPoints != null)
	        {
                var contactPoint = guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());

	            if (contactPoint.PhoneNumber == null ||
                    (contactPoint.PhoneNumber.AreaCode == string.Empty ||
                     contactPoint.PhoneNumber.Number == string.Empty))
	            {
	                return true;
	            }

                if (contactPoint.CellPhoneConsent != null &&
                   contactPoint.CellPhoneConsent.Code != string.Empty &&
                   contactPoint.CellPhoneConsent.Code.Trim() != string.Empty)
                {
                    return true;
                }

                if (FireEvents && GuarantorConsentPreferredEvent != null)
                {
                    GuarantorConsentPreferredEvent(this, null);
                }

                return false;
	        }

	        return true;
	    }

	    public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
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
