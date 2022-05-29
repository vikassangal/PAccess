using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    public class NotifyPCPDataFeatureManager : INotifyPCPFeatureManager
    {
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

        public bool IsNotifyPCPEnabledforaccount(Account account)
        {
            if (account != null && account.Activity != null)
            {
                return ((account.KindOfVisit.IsEmergencyPatient
                    || account.KindOfVisit.IsInpatient)
                    || IsNotifyPCPEnabledforHSV(account));

            }
            return false;
        }

        public YesNoFlag DefaultNotifyPCPForFacility(Facility facility)
        {
            if (facility != null && facility.Code != null)
            {
                return facility.DefaultNotifyPCPOfVisit();
            }
            return YesNoFlag.Blank;
        }
    }
}
