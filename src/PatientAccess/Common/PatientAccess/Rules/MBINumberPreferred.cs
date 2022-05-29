using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareMBINumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MBINumberPreferred : LeafRule
    {
        #region Event Handlers

        public event EventHandler MedicareMBINumberPreferredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            MedicareMBINumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            MedicareMBINumberPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            MedicareMBINumberPreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool CanBeAppliedTo(object context)
        {
            bool result = true;
            
            if (context == null)
            {
                return true;
            }
            if (!(context.GetType() == typeof (GovernmentMedicareCoverage) ||
                  (context.GetType() == typeof (CommercialCoverage))
                )
                )
            {
                return true;
            }

            if (context.GetType() == typeof(GovernmentMedicareCoverage))
            {
                var aCoverage = context as GovernmentMedicareCoverage;
                if (aCoverage == null || aCoverage.Account == null || aCoverage.Account.KindOfVisit == null)
                {
                    return true;
                }

                if (aCoverage.Account.KindOfVisit.IsPreRegistrationPatient &&
                    !aCoverage.Account.Activity.IsPreAdmitNewbornActivity())
                {
                    if (aCoverage.MBINumber.Trim() == String.Empty)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                if (context.GetType() == typeof(CommercialCoverage))
                {
                    var aCoverage = context as CommercialCoverage;
                    if (aCoverage == null || aCoverage.Account == null || aCoverage.Account.KindOfVisit == null ||
                        aCoverage.Account.FinancialClass == null)
                    {
                        return true;
                    }

                    if (aCoverage.Account.KindOfVisit.IsPreRegistrationPatient &&
                        !aCoverage.Account.Activity.IsPreAdmitNewbornActivity())
                    {

                        if (aCoverage.Account.FinancialClass.IsSignedOverMedicare() &&
                            aCoverage.MBINumber.Trim() == String.Empty)
                        {
                            result = false;
                        }
                    }
                }
            }
            if (!result && FireEvents && MedicareMBINumberPreferredEvent != null)
            {
                MedicareMBINumberPreferredEvent(this, null);
            }
            return result;
        }

        #endregion

    }
}
