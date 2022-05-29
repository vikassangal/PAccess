using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface for retrieving <see cref="ResearchStudy"/> objects
    /// </summary>
    public interface IResearchStudyBroker
    {
        IEnumerable<ResearchStudy> AllResearchStudies( long facilityID );
        ResearchStudy ResearchStudyWith( long facilityID, string code );
    }
}
