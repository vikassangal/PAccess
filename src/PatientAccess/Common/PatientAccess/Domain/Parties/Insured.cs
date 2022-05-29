using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Insured : Person
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public string PreviousEmployerName
        {
            get
            {
                return i_PreviousEmployerName;
            }
            set
            {
                i_PreviousEmployerName = value;
            }
        }
        public string PreviousEmploymentStatusCode
        {
            get
            {
                return i_PreviousEmploymentStatusCode;
            }
            set
            {
                i_PreviousEmploymentStatusCode = value;
            }
        }
       
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        public string GroupNumber
        {
            get
            {
                return i_GroupNumber;
            }
            set
            {
                i_GroupNumber = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public Insured() : base()
        {
        }

        public Insured( long oid, DateTime version )
            : base( oid, version )
        {
        }

        public Insured( long oid, DateTime version, Name insuredsName )
            : base( oid, version, insuredsName )
        {
        }
        #endregion

        #region Data Elements
        
        private string i_GroupNumber  = String.Empty;
        private string i_PreviousEmployerName;
        private string i_PreviousEmploymentStatusCode;
       
        
        #endregion

        #region Constants
        #endregion
    }

}