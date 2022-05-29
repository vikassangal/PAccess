using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    //TODO: Create XML summary comment for PatientSearchCriteria
    [Serializable]
    public class PatientCensusSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string LastName 
        {
            get
            {
                return i_LastName;
            }
            set
            {
                i_LastName = value;
            }
        }

        public string FirstName 
        {
            get
            {
                return i_FirstName;
            }
            set
            {
                i_FirstName = value;
            }
        }

        public long AccountNumber 
        {
            get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }

        public Facility Facility
        {
            get
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }

        #endregion

        #region Private Methods

        public override ValidationResult Validate()
        {
            return null;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PatientCensusSearchCriteria( string lastName, 
                                            string firstName, 
                                            long accountNumber, 
                                            Facility facility )
            : base(facility.Code)
        {
            i_FirstName              = firstName;
            i_LastName               = lastName;
            i_AccountNumber          = accountNumber;
            i_Facility               = facility;
        }

        #endregion

        #region Data Elements
        private string                  i_FirstName;
        private string                  i_LastName;
        private long                    i_AccountNumber;
        private Facility                i_Facility;
        #endregion

        #region Constants
        #endregion
    }
}