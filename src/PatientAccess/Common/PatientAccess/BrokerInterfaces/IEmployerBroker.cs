using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IEmployerBroker.
    /// </summary>
    public interface IEmployerBroker
    {
        int AddNewEmployer(Employer emp, FollowupUnit followUpUnit, string facilityCode, string userName);

        int AddNewEmployer(Employer emp, FollowupUnit followUpUnit, string facilityCode);

        Employer SelectEmployerByName(string facCode, string empName);

        SortedList AllEmployersWith(long facilityOid, string searchName);

        Employer EmployerFor(EmployerProxy employerProxy);

        Patient EmployerFor(Patient patient, long facilityOid);

        Guarantor EmployerFor(Guarantor guarantor, long accountNumber, long facilityOid);

        int AddEmployerForApproval(Employer emp, string facilityCode, string usrID);

        void DeleteEmployerForApproval(Employer employer, string facilityHspCode);

        void AddContactPointForEmployer(Employer employer, ContactPoint contactPoint, string facilityCode);

        SortedList<string, NewEmployerEntry> GetAllEmployersForApproval(string facilityHspCode);

        SortedList<string, Employer> GetAllEmployersWith(long facilityOid, string searchName);

        void DeleteAllEmployersForApproval(string facilityCode);

        void SaveEmployersForApproval(IList<NewEmployerEntry> newEmployerEntries, string facilityCode);
    }
}
