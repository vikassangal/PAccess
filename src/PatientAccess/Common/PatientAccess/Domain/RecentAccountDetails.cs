using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class RecentAccountDetails
    {
        public long AccountNumber { get; private set; }
        public DateTime AccountCreationDate { get; private set; }

        public RecentAccountDetails( long accountNumber, DateTime accountCreationDate )
        {
            AccountNumber = accountNumber;
            AccountCreationDate = accountCreationDate;
        }
    }
}