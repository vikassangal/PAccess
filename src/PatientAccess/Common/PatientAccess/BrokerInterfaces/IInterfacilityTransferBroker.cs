using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;
using System.Data;
using PatientAccess.Domain.InterFacilityTransfer;
 
namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IInterfacilityTransferBroker.
    /// </summary>
    public interface IInterfacilityTransferBroker
    {
        InterFacilityTransferAccount Accountsfromtransferlogfordischarge(long ToMRN, Facility ToFacility, long ToAccount);
        InterFacilityTransferAccount AccountsfromtransferlogforRegistration(long FromMRN, Facility FromFacility, long FromAccount);
        DataTable GetAccountsForPatient(long PatientMRN, Facility FromFacility);
        ArrayList AllInterFacilityTransferHospitals(Facility facility);
        List<AccountProxy> GetAllAccountsForPatient(long PatientMRN, Facility FromFacility);
        DataTable GETIFXRFROMHOSPITALACCOUNTSFOR(long PatientMRN, Facility FromFacility);

    }
}
