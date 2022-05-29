using System; 

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for PriorVisitInformationRequest.
    /// </summary>
    [Serializable]
    public class PriorVisitInformationResponse
    {
        #region Properties

        public string PriorHospitalCode { get; set; }
      
        public string PriorAccountNumber { get; set; }

        public string PriorAdmitDate { get; set; }

        public string PriorDischargeDate { get; set; }
 
        #endregion
    }
}
