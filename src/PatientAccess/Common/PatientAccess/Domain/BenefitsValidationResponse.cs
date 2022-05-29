using System;
using System.Collections;

namespace PatientAccess.Domain
{
    /// <summary>
    /// BenefitsValidationResponse - holds the updated DataValidationTicket and/or the reponse from DV service
    /// </summary>
    [Serializable]
    public class BenefitsValidationResponse
    {
        #region Event Handlers
        #endregion

        #region Methods

        public CoverageConstraints GetInNetworkConstraint()
        {
            foreach( CoverageConstraints cc in this.i_CoverageConstraintsCollection )
            {
                if( cc.GetType() == typeof( CommercialConstraints ) )
                {
                    if( ( cc as CommercialConstraints ).KindOfConstraint.IsInNetwork )
                    {
                        return cc;
                    }
                }
            }

            return null;
        }

        public CoverageConstraints GetOutOfNetworkConstraint()
        {
            foreach( CoverageConstraints cc in this.i_CoverageConstraintsCollection )
            {
                if( !( cc as CommercialConstraints ).KindOfConstraint.IsInNetwork )
                {
                    return cc;
                }
            }

            return null;
        }

        public Type GetParseCoverageType( )
        {
            Type result = new object( ).GetType( );

            if( this.ReturnedDataValidationTicket != null &&
                    this.ReturnedDataValidationTicket.BenefitsResponse != null )
            {
                result = Type.GetType( this.ReturnedDataValidationTicket.BenefitsResponse.BenefitsResponseParseStrategy );
            }
            return result;
        }

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

        public string Eligible
        {
            get
            {
                return this.i_Eligible;
            }
            private set
            {
                this.i_Eligible = value;
            }
        }

        public string MessageUUID
        {
            get
            {
                return this.i_MessageUUID;
            }
            private set
            {
                this.i_MessageUUID = value;
            }
        }

        public string PayorMessage
        {
            get
            {
                return this.i_PayorMessage;
            }
            private set
            {
                this.i_PayorMessage = value;
            }
        }

        public string PayorXmlMessage
        {
            get
            {
                return this.i_PayorXmlMessage;
            }
            set
            {
                this.i_PayorXmlMessage = value;
            }
        }

        public bool IsSuccess
        {

            get
            {
                return this.i_IsSuccess;
            }
            private set
            {
                this.i_IsSuccess = value;
            }

        }


        public ArrayList CoverageConstraintsCollection
        {
            get
            {
                return i_CoverageConstraintsCollection;
            }
            set
            {
                i_CoverageConstraintsCollection = value;
            }
        }

        public string FacilityCode
        {
            get 
            { 
                return i_FacilityCode;
            }
            set
            {
                i_FacilityCode = value;
            }
        }
	

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public BenefitsValidationResponse()
        {
        }

        public BenefitsValidationResponse( string facilityCode,
                                           DataValidationTicket dvt,
                                           string eligible,
                                           string messageUUID,
                                           string payorMessage,
                                           string payorXmlMessage,
                                           bool isSuccess )
        {
            this.FacilityCode = facilityCode;
            this.i_ReturnedDataValidationTicket = dvt;
            this.Eligible = eligible;
            this.MessageUUID = messageUUID;
            this.PayorMessage = payorMessage;
            this.PayorXmlMessage = payorXmlMessage;
            this.IsSuccess = isSuccess;
        }

        #endregion

        #region Data Elements

        private DataValidationTicket                    i_ReturnedDataValidationTicket = new DataValidationTicket();
        private ArrayList                               i_CoverageConstraintsCollection = new ArrayList();
        
        private string i_Eligible = String.Empty;

        private string i_MessageUUID = String.Empty;

        private string i_PayorMessage = String.Empty;

        private string i_PayorXmlMessage = String.Empty;

        private bool i_IsSuccess = false;
        private string i_FacilityCode = String.Empty;

        #endregion

        #region Constants
        public const string RESPONSE_TEXT_ON_MISMATCH = "The Insurance Plan on the account has changed since the last time benefits validation was performed. Please re-initiate Benefits Validation again.";
        #endregion
    }
}
