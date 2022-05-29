using PatientAccess.Domain;
using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
    public interface IServiceCategoryBroker
    {
        ArrayList GetServiceCategoryForClinicCode(long FacilityId, string ClinicCode);
        ClinicServiceCategory GetServiceCategoryForClinicCodeWith(long FacilityId, string ClinicCode, string serviceCategoryCode);
    }
}
