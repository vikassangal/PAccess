 
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public class VIWEBFeatureManager
    {

        public bool IsHTML5VIWebEnabledForFacility(Account account)
        {
            if (account != null && account.Facility != null)
            {
                if (account.Facility.IsValidForHTML5VIWeb)
                {
                    return true;
                }

            }

            return false;
        }
    }
}
