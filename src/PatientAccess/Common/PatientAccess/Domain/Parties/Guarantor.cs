using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Guarantor : Person
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
          
        public CreditReport  CreditReport 
        {
            get
            {
                return this.i_CreditReport;
            }
            set
            {
                this.i_CreditReport = value;
            }
        }

        public DataValidationTicket DataValidationTicket
        {
            get
            {
                return i_DataValidationTicket;
            }
            set
            {
                i_DataValidationTicket = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Guarantor() : base()
        {
        }

        public Guarantor( long oid, DateTime version )
            : base( oid, version )
        {
        }

        public Guarantor( long oid, DateTime version, Name guarantorsName )
            : base( oid, version, guarantorsName )
        {
        }
        #endregion

        #region Data Elements
        
        private CreditReport            i_CreditReport = new CreditReport();
        private DataValidationTicket    i_DataValidationTicket = null;

        #endregion

        #region Constants
        #endregion
    }
}
