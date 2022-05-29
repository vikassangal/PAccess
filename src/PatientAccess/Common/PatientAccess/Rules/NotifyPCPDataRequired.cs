using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for NotifyPCPDataRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class NotifyPCPDataRequired : LeafRule
    {

        public event EventHandler NotifyPCPDataRequiredEvent;

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.NotifyPCPDataRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.NotifyPCPDataRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.NotifyPCPDataRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {

            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account Model = ((Account) context);

            if (Model.KindOfVisit != null &&
                Model.HospitalService != null &&
                Model.HospitalService.Code != null &&
                Model.KindOfVisit.Code != null)
            {
                if (Model.KindOfVisit.IsEmergencyPatient
                    || Model.KindOfVisit.IsInpatient
                    || IsNotifyPCPEnabledforHSV(Model))
                {
                    if (String.IsNullOrEmpty(Model.ShareDataWithPCPFlag.Code.Trim()))
                    {
                        if (this.FireEvents && NotifyPCPDataRequiredEvent != null)
                        {
                            this.NotifyPCPDataRequiredEvent(this, null);
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsNotifyPCPEnabledforHSV(Account account)
        {
            if (account != null && account.HospitalService != null)
            {
                return ((account.HospitalService.Code == "58"
                    || account.HospitalService.Code == "59")
                    && account.KindOfVisit.IsOutpatient);
            }

            return false;
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        

	}
}
