using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for PriorVisitInformationRequest.
    /// </summary>
    [Serializable]
    public class PriorVisitInformationRequest
    {

        #region Properties

        public Facility Facility { get; set; }

        public string AccountNumber { get; set; }

        public string MedicareHic { get; set; }

        public string MBINumber { get; set; }
        public string Certificate { get; set; }

        public string First4OfLastName { get; set; }

        public string First1OfFirstName { get; set; }

        public string Gender { get; set; }

        public string DateOfBirth { get; set; }

        #endregion
    }
}
