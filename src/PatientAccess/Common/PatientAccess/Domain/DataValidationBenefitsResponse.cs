using System;

namespace PatientAccess.Domain
{

    [Serializable]
    public class DataValidationBenefitsResponse
    {
        
        #region Event Handlers

        #endregion

        #region Methods

        #endregion

        #region Properties

        public BenefitResponseStatus ResponseStatus
        {
            get
            {
                return i_ResponseStatus;
            }
            set
            {
                i_ResponseStatus = value;
            }
        }

        public string BenefitsResponseParseStrategy
        {
            get
            {
                return i_BenefitsResponseParseStrategy;
            }
            set
            {
                i_BenefitsResponseParseStrategy = value;
            }
        }

        public CoverageOrder CoverageOrder
        {
            get
            {
                return i_CoverageOrder;
            }
            set
            {
                i_CoverageOrder = value;
            }
        }

        public string PlanCode
        {
            get
            {
                return i_PlanCode;
            }
            set
            {
                i_PlanCode = value;
            }
        }

        public InsurancePlanCategory PlanCategory
        {
            get
            {
                return i_PlanCategory;
            }
            set
            {
                i_PlanCategory = value;
            }
        }

        public string ResponseGroupNumber
        {
            get
            {
                return i_ResponseGroupNumber;
            }
            set
            {
                i_ResponseGroupNumber = value;
            }
        }

        public string ResponseInsuredDOB
        {
            get
            {
                if( !string.IsNullOrEmpty(i_ResponseInsuredDOB) 
                    && i_ResponseInsuredDOB.Length == 8 )
                {
                    return i_ResponseInsuredDOB.Substring( 4, 2 ) + '/' + i_ResponseInsuredDOB.Substring( 6, 2 ) + "/" + i_ResponseInsuredDOB.Substring( 0, 4 );
                }
                else
                {
                    return i_ResponseInsuredDOB;
                }
                
            }
            set
            {
                i_ResponseInsuredDOB = value;
            }
        }

        public string ResponseInsuredName
        {
            get
            {
                if( i_ResponseInsuredMiddleInitial != null && i_ResponseInsuredMiddleInitial != string.Empty )
                {
                    return i_ResponseInsuredFirstName + " " + i_ResponseInsuredMiddleInitial + " " + i_ResponseInsuredLastName;
                }
                else
                {
                    return i_ResponseInsuredFirstName + " " + i_ResponseInsuredLastName;
                }
            }            
        }

        public string ResponsePayorName
        {
            get
            {
                return i_ResponsePayorName;
            }
            set
            {
                i_ResponsePayorName = value;
            }
        }

        public string ResponseSubscriberID
        {
            get
            {
                return i_ResponseSubscriberID;
            }
            set
            {
                i_ResponseSubscriberID = value;
            }
        }

        public string RequestInsuredDOB
        {
            get
            {
                if( !string.IsNullOrEmpty( i_RequestInsuredDOB)
                    && i_RequestInsuredDOB.Length == 8 )
                {
                    return i_RequestInsuredDOB.Substring( 4, 2 ) + '/' + i_RequestInsuredDOB.Substring( 6, 2 ) + "/" + i_RequestInsuredDOB.Substring( 0, 4 );
                }
                else
                {
                    return i_RequestInsuredDOB;
                }
            }
            set
            {
                i_RequestInsuredDOB = value;
            }
        }

        public string RequestInsuredName
        {
            get
            {
                if( i_RequestInsuredMiddleInitial != null && i_RequestInsuredMiddleInitial != string.Empty )
                {
                    return i_RequestInsuredFirstName + " " + i_RequestInsuredMiddleInitial + " " + i_RequestInsuredLastName;
                }
                else
                {
                    return i_RequestInsuredFirstName + " " + i_RequestInsuredLastName;
                }
            }            
        }

        public string RequestPayorName
        {
            get
            {
                return i_RequestPayorName;
            }
            set
            {
                i_RequestPayorName = value;
            }
        }

        public string RequestSubscriberID
        {
            get
            {
                return i_RequestSubscriberID;
            }
            set
            {
                i_RequestSubscriberID = value;
            }
        }

        public string RequestInsuredFirstName
        {
            get
            {
                return i_RequestInsuredFirstName;
            }
            set
            {
                i_RequestInsuredFirstName = value;
            }
        }

        public string RequestInsuredMiddleInitial
        {
            get
            {
                return i_RequestInsuredMiddleInitial;
            }
            set
            {
                i_RequestInsuredMiddleInitial = value;
            }
        }

        public string RequestInsuredLastName
        {
            get
            {
                return i_RequestInsuredLastName;
            }
            set
            {
                i_RequestInsuredLastName = value;
            }
        }

        public string ResponseInsuredFirstName
        {
            get
            {
                return i_ResponseInsuredFirstName;
            }
            set
            {
                i_ResponseInsuredFirstName = value;
            }
        }

        public string ResponseInsuredMiddleInitial
        {
            get
            {
                return i_ResponseInsuredMiddleInitial;
            }
            set
            {
                i_ResponseInsuredMiddleInitial = value;
            }
        }

        public string ResponseInsuredLastName
        {
            get
            {
                return i_ResponseInsuredLastName;
            }
            set
            {
                i_ResponseInsuredLastName = value;
            }
        }

        public string ResponseAuthCoPhone
        {
            get
            {
                return i_ResponseAuthCoPhone;
            }
            set
            {
                i_ResponseAuthCoPhone = value;
            }
        }

        public string ResponseAuthCo
        {
            get
            {
                return i_ResponseAuthCo;
            }
            set
            {
                i_ResponseAuthCo = value;
            }
        }

        public bool IsInParsableState
        {

            get
            {

                bool canBeParsed = true;

                if ( this.ResponseStatus.Oid == BenefitResponseStatus.ACCEPTED_OID ||
                     this.ResponseStatus.Oid == BenefitResponseStatus.AUTO_ACCEPTED_OID ||
                     this.ResponseStatus.Oid == BenefitResponseStatus.REJECTED_OID )
                {

                    canBeParsed = false;

                }//if

                return canBeParsed;

            }//get

        }//property

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization
        public DataValidationBenefitsResponse( )
        {

        }
        #endregion

        #region Field Elements

        private BenefitResponseStatus                       i_ResponseStatus = BenefitResponseStatus.NewUnknownStatus();
        private string                                      i_BenefitsResponseParseStrategy = string.Empty;
        private CoverageOrder                               i_CoverageOrder = new CoverageOrder();
        private string                                      i_PlanCode = string.Empty; // Concatenation of PlanSuffix and PayorCode
        private InsurancePlanCategory                       i_PlanCategory = new InsurancePlanCategory();
        private string                                      i_ResponseGroupNumber = string.Empty;
        private string                                      i_ResponseInsuredDOB = string.Empty;
        private string                                      i_ResponseInsuredName = string.Empty;
        private string                                      i_ResponseInsuredFirstName = string.Empty;
        private string                                      i_ResponseInsuredMiddleInitial = string.Empty;
        private string                                      i_ResponseInsuredLastName = string.Empty;
        private string                                      i_ResponsePayorName = string.Empty;
        private string                                      i_ResponseSubscriberID = string.Empty;
        private string                                      i_ResponseAuthCo = string.Empty;
        private string                                      i_ResponseAuthCoPhone = string.Empty;
        private string                                      i_RequestInsuredDOB = string.Empty;
        private string                                      i_RequestInsuredName = string.Empty;
        private string                                      i_RequestPayorName = string.Empty;
        private string                                      i_RequestSubscriberID = string.Empty;
        private string                                      i_RequestInsuredFirstName = string.Empty;
        private string                                      i_RequestInsuredMiddleInitial = string.Empty;
        private string                                      i_RequestInsuredLastName = string.Empty;

        
        #endregion

        #region Constants

        #endregion
    }
}
