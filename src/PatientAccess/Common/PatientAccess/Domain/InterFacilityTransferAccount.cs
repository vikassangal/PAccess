using System;

namespace PatientAccess.Domain.InterFacilityTransfer
{
	/// <summary>
    /// InterFacilityTransferAccount object stores specific information pertaining to the 
	/// Interfacility transfer account record. This avoids passing on the whole
	/// Account object from the client to the server and helps reduce network burden.
	/// </summary>
    [Serializable]
    public class InterFacilityTransferAccount
	{

        #region Properties

        public long ToMedicalRecordNumber { get; set; }

        public long ToAccountNumber { get; set; }
        public long ToFacilityOid { get; set; }
        public Facility ToFacility { get; set; }

        public long FromFacilityOid { get; set; }
        public Facility FromFacility { get; set; }
        public long FromMedicalRecordNumber { get; set; }

        public long FromAccountNumber { get; set; }

        public DateTime ToAdmitDate { get; set; }
        public long ToAdmitTime { get; set; } // long time as HH:MM (12:35)
        public DateTime FromDischargeDate { get; set; }
        public long FromDischargeTime { get; set; } // long time as HH:MM (12:35)
        public string PatientName { get; set; }
        public string DischargeNote { get; set; }
        public Activity Activity { get; set; }

        public string HSV { get; set; }
        public string ERPhysician { get; set; }
        public string PatientType { get; set; }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
