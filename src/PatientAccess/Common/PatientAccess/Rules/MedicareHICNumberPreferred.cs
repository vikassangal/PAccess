using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareHICNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicareHICNumberPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareHICNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareHICNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareHICNumberPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareHICNumberPreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            bool result = true;
            if (context == null)
            {
                return true;
            }
            if (context.GetType() == typeof(GovernmentMedicareCoverage))
            {
                GovernmentMedicareCoverage aCoverage = context as GovernmentMedicareCoverage;
                if (aCoverage == null)
                {
                    return true;
                }
                if (aCoverage.MBINumber != String.Empty) // MBI is populated
                {
                    return true;
                }
                if (aCoverage.HICNumber == String.Empty) // HIC is not populated
                {
                    result = false;
                }
            }
            else
            {
                if (context.GetType() == typeof(CommercialCoverage))
                {
                    CommercialCoverage aCoverage = context as CommercialCoverage;
                    if (aCoverage == null || aCoverage.Account == null || aCoverage.Account.FinancialClass == null)
                    {
                        return true;
                    }
                    if (aCoverage.Account.FinancialClass.IsSignedOverMedicare())
                    {
                        if (aCoverage.MBINumber != String.Empty) // MBI is populated
                        {
                            return true;
                        }
                        if (aCoverage.SignedOverMedicareHICNumber == String.Empty)// HIC is not populated
                        {
                            result = false;
                        }
                    }
                }
            }
            if (!result && FireEvents && MedicareHICNumberPreferredEvent != null)
            {
                MedicareHICNumberPreferredEvent(this, null);
            }
            return result;
        }
        #endregion
        
        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
