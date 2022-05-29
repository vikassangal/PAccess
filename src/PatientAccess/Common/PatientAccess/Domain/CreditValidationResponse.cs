using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// CreditValidationResponse - holds the updated DataValidationTicket and/or the reponse from DV service
    /// </summary>
    [Serializable]
    public class CreditValidationResponse
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public DataValidationTicket ReturnedDataValidationTicket
        {
            get
            {
                return i_ReturnedDataValidationTicket;
            }  
            set
            {
                i_ReturnedDataValidationTicket = value;
            }
        }

        public CreditReport ResponseCreditReport
        {
            get
            {
                return i_ResponseCreditReport;
            }
            set
            {
                i_ResponseCreditReport = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public CreditValidationResponse()
        {
        }

        public CreditValidationResponse( DataValidationTicket dvt, CreditReport cr )
        {
            this.ReturnedDataValidationTicket     = dvt;
            this.ResponseCreditReport             = cr;            
        }

        #endregion

        #region Data Elements

        DataValidationTicket                i_ReturnedDataValidationTicket = null;
        CreditReport                        i_ResponseCreditReport = null;

        #endregion

        #region Constants
        #endregion
    }
}
