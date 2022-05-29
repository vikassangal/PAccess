using System;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    [Serializable]
    public class OnlinePreRegistrationItem
    {
        #region Properties

        public string PatientName
        {
            get
            {
                return Name.AsFormattedName();
            }

        }

        public Name Name { get; private set; }

        public DateTime AdmitDate { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        public string Ssn { get; private set; }

        public string Address { get; private set; }

        public string Gender { get; private set; }

        public bool IsLocked { get; set; }

        public bool? VisitedBefore { get; private set; }

        [UsedImplicitly]
        public Guid Id { get; set; }

        #endregion

        #region Construction and Finalization

        public OnlinePreRegistrationItem( Guid id,
                                          Name name, 
                                          string gender, 
                                          DateTime dob, 
                                          bool? visitedBefore, 
                                          string ssn, 
                                          string address, 
                                          DateTime admitDate, 
                                          bool isLocked )
        {
            
            Guard.ThrowIfArgumentIsNull(id,"id");
            Guard.ThrowIfArguementIsEmpty(id, "id");
            Guard.ThrowIfArgumentIsNull(name, "name");
            
            //ssn is optional so it can be empty
            Guard.ThrowIfArgumentIsNull(ssn, "ssn");
            
            Guard.ThrowIfArgumentIsNullOrEmpty(address, "address");
           
            Id = id;
            
            Name = name;
            Gender      = gender;
            DateOfBirth = dob;
            AdmitDate   = admitDate;
            VisitedBefore = visitedBefore;
            Ssn         = ssn;
            Address     = address;
            IsLocked = isLocked;
        }

        #endregion

        #region Constants
        #endregion
    }
}
