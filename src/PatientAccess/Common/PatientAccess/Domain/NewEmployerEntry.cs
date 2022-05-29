using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class NewEmployerEntry
    {
        private readonly Employer _employer;
        private readonly string _userID;
        public Employer Employer
        {
            get
            {
                return this._employer;
            }

        }
        public string UserID
        {
            get
            {
                return this._userID;
            }

        }

        public NewEmployerEntry(Employer employer, string userID)
        {
            this._employer = employer;
            this._userID = userID;
        }
    }
}
