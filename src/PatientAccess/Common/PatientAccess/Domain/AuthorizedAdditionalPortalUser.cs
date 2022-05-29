using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class AuthorizedAdditionalPortalUser : Person
    {
        #region Public methods

        public int SequenceNumber
        {
            get { return i_SequenceNumber; }

            set { i_SequenceNumber = value; }
        }

        public YesNoFlag RemoveUserFlag
        {
            get { return i_RemoveUserFlag; }

            set { i_RemoveUserFlag = value; }
        }

        #endregion
        #region Data Elements
        private YesNoFlag i_RemoveUserFlag  = new YesNoFlag();
        private int i_SequenceNumber = 0 ;

        #endregion
    }
}