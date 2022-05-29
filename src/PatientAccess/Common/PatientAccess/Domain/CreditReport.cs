using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CreditReport : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods

        public string GetCreditAdvice( decimal totalCurrentAmountDue ) 
        {
            string result = string.Empty;
			
            if( IsInsured )
            {
                if( this.CreditScore < 600 )
                {
                    result = String.Format( INSURED_SCORE_LESS_THAN_600, totalCurrentAmountDue );
                }
                else if( this.CreditScore >= 600 && this.CreditScore <= 699 )
                {
                    result = String.Format( INSURED_SCORE_IS_600_TO_699, totalCurrentAmountDue );
                }
                else if( this.CreditScore >= 700 && this.CreditScore <= 850 )
                {
                    result = String.Format( INSURED_SCORE_IS_700_TO_850, totalCurrentAmountDue );
                }
            }
            else
            {
                if( this.CreditScore < 600 )
                {
                    result = String.Format( UNINSURED_SCORE_LESS_THAN_600, totalCurrentAmountDue );
                }
                else if( this.CreditScore >= 600 && this.CreditScore <= 699 )
                {
                    result = String.Format( UNINSURED_SCORE_IS_600_TO_699, totalCurrentAmountDue );
                }
                else if( this.CreditScore >= 700 && this.CreditScore <= 850 )
                {
                    result = String.Format( UNINSURED_SCORE_IS_700_TO_850, totalCurrentAmountDue );
                }
            }

            return result;
        }


        #endregion

        #region Properties

        public string Report
        {
            get
            {
                return i_Report;
            }
            set
            {
                i_Report = value;
            }        
        }

        public long CreditScore
        {
            get
            {

                return i_CreditScore;
            }
            set
            {
                i_CreditScore = value;
            }
        }

        public string CreditAdvice
        {
            get
            {
                return GetCreditAdvice( this.CreditScore );
            }

        }

        public bool IsInsured
        {
            get
            {
                return i_IsInsured;
            }
            set
            {
                i_IsInsured = value;
            }
        }

        public string ServiceLastName
        {
            get
            {
                return i_ServiceLastName;
            }
            set
            {
                i_ServiceLastName = value;
            }
        }

        public string ServiceFirstName
        {
            get
            {
                return i_ServiceFirstName;
            }
            set
            {
                i_ServiceFirstName = value;
            }
        }

        public string ServiceMiddleName
        {
            get
            {
                return i_ServiceMiddleName;
            }
            set
            {
                i_ServiceMiddleName = value;
            }
        }

        public string ServiceSSN
        {
            get
            {
                return i_ServiceSSN;
            }
            set
            {
                i_ServiceSSN = value;
            }
        }

        public PhoneNumber ServicePhoneNumber
        {
            get
            {
                return i_ServicePhoneNumber;
            }
            set
            {
                i_ServicePhoneNumber = value;
            }
        }

        public ArrayList ServiceAddresses
        {
            get
            {
                return i_ServiceAddresses;
            }
            set
            {
                i_ServiceAddresses = value;
            }
        }

        public ArrayList ServiceHawkAlerts
        {
            get
            {
                return i_ServiceHawkAlerts;
            }
            set
            {
                i_ServiceHawkAlerts = value;
            }
        }

		public string FormatedHawkAlert
		{
			get
			{
				return i_FormatedHawkAlert;
			}
			set
			{
				i_FormatedHawkAlert = value;
			}
		}
        
        #endregion

        #region Private Methods

		

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CreditReport()
        {
        
		}
        
   
        #endregion

        #region Data Elements

        private string          i_Report                = string.Empty;
        private long            i_CreditScore ;
        private bool            i_IsInsured             = false;
        private string          i_CreditAdvice          = string.Empty;
        private string          i_ServiceLastName       = string.Empty;
        private string          i_ServiceFirstName      = string.Empty;
        private string          i_ServiceMiddleName     = string.Empty;
        private string          i_ServiceSSN            = string.Empty;
		private string			i_FormatedHawkAlert     = string.Empty;
        private PhoneNumber     i_ServicePhoneNumber    = new PhoneNumber();
        private ArrayList       i_ServiceAddresses      = new ArrayList();
        private ArrayList       i_ServiceHawkAlerts     = new ArrayList();
		
        #endregion

        #region Constants

        internal string INSURED_SCORE_LESS_THAN_600 = "Your insurance does not cover the entire cost of today's hospital services.  " +
            "Your estimated balance will be ${0} and is due in full. How will you be paying today?  We accept cash, check and major " +
            "credit cards." + Environment.NewLine + Environment.NewLine + "(If the patient cannot pay the full amount)  I may be able to " +
            "approve you for a down payment and payment plan (offer plan).  " + "We accept cash, check and major credit cards." +
            Environment.NewLine + Environment.NewLine + "(If the patient cannot make a down payment).  Please contact our Financial " +
            "Counselor to make financial arrangements for your account.";

        internal string UNINSURED_SCORE_LESS_THAN_600 = "We offer special pricing to our uninsured patients. Your estimated balance " +
            "will be ${0}.  We are unable to determine the exact charges because all of the charges may not be posted to your account yet.  " +
            "Should you owe more than ${0}, we will send you a bill for the balance. How will you be paying today?   We accept cash, check " +
            "and major credit cards." + Environment.NewLine + Environment.NewLine + "(If the patient cannot pay the full amount)  I may be able " +
            "to approve you for a down payment and payment plan (offer plan).  We accept cash, check and major credit cards.  The bills that " +
            "we will send you will reflect the special pricing with instructions on where you may send your payment." + Environment.NewLine + 
            Environment.NewLine + "If you are unable to pay any portion of your charges, you might qualify for other programs. This financial " +
            "pamphlet will give you additional information and a telephone number where you can obtain more information regarding your options.  " +
            "(Give the patient the ER leaflet.)";

        internal string INSURED_SCORE_IS_600_TO_699 = "Your insurance does not cover the entire cost of today's hospital services.  " +
            "Your estimated balance will be ${0} and is due in full. How will you be paying today?  We accept cash, check, and major credit cards." +
            Environment.NewLine + Environment.NewLine + "(If the patient cannot pay the full amount)  I may be able to approve you for a down " +
            "payment and payment plan (offer plan).  We accept cash, check and major credit cards.";

        internal string UNINSURED_SCORE_IS_600_TO_699 = "We offer special pricing to our uninsured patients. Your estimated balance will " +
            "be ${0}.  We are unable to determine the exact charges because all of the charges may not be posted to your account yet.  " +
            "Should you owe more than ${0}, we will send you a bill for the balance. How will you be paying today?   We accept cash, check, and " +
            "major credit cards." + Environment.NewLine + Environment.NewLine + "(If the patient cannot pay the full amount)  I may be able to " +
            "approve you for a down payment and payment plan (offer plan).  We accept cash, check and major credit cards.  The bills that we " +
            "will send you will reflect the special pricing with instructions on where you may send your payment.";

        internal const string INSURED_SCORE_IS_700_TO_850 = "Your insurance does not cover the entire cost of today's hospital services.  " +
            "Your estimated balance will be ${0} and is due in full. How will you be paying today?  We accept cash, check and major credit cards.";

        internal const string UNINSURED_SCORE_IS_700_TO_850 = "We offer special pricing to our uninsured patients. Your estimated balance will be  " +
            "${0}.  We are unable to determine the exact charges because all of the charges may not be posted to your account yet.  Should you " +
            "owe more than ${0}, we will send you a bill for the balance. How will you be paying today?   We accept cash, check and major credit cards.";

        #endregion
    }
}
