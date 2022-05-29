using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Utilities
{
    public static class Pbar
    {
        public static bool IsAvailable()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            return facilityBroker.IsDatabaseAvailableFor(User.GetCurrent().Facility.ConnectionSpec.ServerIP);
        }
    }
}
