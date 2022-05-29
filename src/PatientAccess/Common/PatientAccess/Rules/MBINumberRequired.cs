using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MBINumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MBINumberRequired : LeafRule
    {
        #region Event Handlers

        public event EventHandler MedicareMBINumberRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            MedicareMBINumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            MedicareMBINumberRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            MedicareMBINumberRequiredEvent = null;
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
            if (context.GetType() == typeof (GovernmentMedicareCoverage))
            {
                GovernmentMedicareCoverage aCoverage = context as GovernmentMedicareCoverage;
                if (aCoverage == null)
                {
                    return true;
                }
                if (aCoverage.MBINumber == String.Empty)
                {
                    result = false;
                }
            }
            else
            {
                if (context.GetType() == typeof (CommercialCoverage))
                {
                    CommercialCoverage aCoverage = context as CommercialCoverage;
                    if (aCoverage == null || aCoverage.Account == null || aCoverage.Account.FinancialClass == null)
                    {
                        return true;
                    }
                 
                    if (aCoverage.Account.FinancialClass.IsSignedOverMedicare()  &&
                        aCoverage.MBINumber == String.Empty)
                    {

                        result = false;
                    }
                }
            }
            if (!result && FireEvents && MedicareMBINumberRequiredEvent != null)
            {
                MedicareMBINumberRequiredEvent(this, null);
            }
            return result;
        }

        #endregion

    }
}
