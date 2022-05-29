using System;
using System.Collections.Generic;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for FinancialClass.
    /// </summary>
    [Serializable]
    public class FinancialClass : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool WarrantsValidation()
        {
            if( this.IsSelfPay()
                || this.IsNonGovernment()
                )
            {
                return true;
            }
            else
            {            
                return false;
            }
        }

        public override string ToString()
        {   // Display 2-digit code (with zero-placeholder if only 1 digit) and description
            return String.Format("{0:00} {1}", Code, Description);
        }

        private bool IsSelfPay()
        {
            if( this.Code == "03"
                || this.Code == "04"
                || this.Code == "05"
                || this.Code == "52"
                || this.Code == "66"
                || this.Code == "70"
                || this.Code == "72"
                || this.Code == "73" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsNonGovernment()
        {
            if( this.Code == "08"
                || this.Code == "10"
                || this.Code == "14"
                || this.Code == "16"
                || this.Code == "19"
                || this.Code == "20"
                || this.Code == "21"
                || this.Code == "22"
                || this.Code == "23"
                || this.Code == "25"
                || this.Code == "26"
                || this.Code == "29"
                || this.Code == "80"
                || this.Code == "81"
                || this.Code == "84"
                || this.Code == "85"
                || this.Code == "86"
                || this.Code == "87" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsMedicare()
        {
            if( this.Code == "04"
                || this.Code == "23"
                || this.Code == "40"
                || this.Code == "42"
                || this.Code == "43"
                || this.Code == "44"
                || this.Code == "45"
                || this.Code == "46"
                || this.Code == "47"
                || this.Code == "48"
                || this.Code == "49"
                || this.Code == "56"
                || this.Code == "59"
                || this.Code == "75"
                || this.Code == "79"
                || this.Code == "85"
                || this.Code == "88"
                || this.Code == "89" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsSpecialtyMedicare()
        {
            if( this.IsMedicare()
                || this.Code == "10"
                || this.Code == "13"
                || this.Code == "26"
                || this.Code == "41"
                || this.Code == "58"
                || this.Code == "85"
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsSignedOverMedicare()
        {
            if( this.Code != null
                && ( this.Code == "84"
                    || this.Code == "87" )
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsMedScreenExam()
        {
            if (this.Code != null && (this.Code == MED_SCREEN_EXM_CODE) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static FinancialClass MedScreenFinancialClass
        {
            get
            {
                return new FinancialClass(NEW_OID, NEW_VERSION, MED_SCREEN_EXM_CODE, MED_SCREEN_EXM_CODE);
            }
        }
        #endregion

        public bool IsValidForCOBReceived
        {
            get { return (Code != null && ValidFinancialClassesForCOBReceived.Contains(Code)); }
        }
       
        public bool IsValidForIMFMReceived
        {
            get { return (Code != null && ValidFinancialClassesForIMFMReceived.Contains(Code)); }
        }

        private IList<string> ValidFinancialClassesForCOBReceived 
        {
            get
            {
                IList<string> cobReceivedFinancialClassItems = new List<string>();
                cobReceivedFinancialClassItems.Add("02");
                cobReceivedFinancialClassItems.Add("14");
                cobReceivedFinancialClassItems.Add("20");
                cobReceivedFinancialClassItems.Add("80");
                cobReceivedFinancialClassItems.Add("81");
                cobReceivedFinancialClassItems.Add("21");
                return cobReceivedFinancialClassItems;
            }
        }

        private IList<string> ValidFinancialClassesForIMFMReceived
        {
            get
            {
                IList<string> imfmReceivedFinancialClassItems = new List<string>();
                imfmReceivedFinancialClassItems.Add("04");
                imfmReceivedFinancialClassItems.Add("10");
                imfmReceivedFinancialClassItems.Add("13");
                imfmReceivedFinancialClassItems.Add("23");
                imfmReceivedFinancialClassItems.Add("26");
                imfmReceivedFinancialClassItems.Add("40");
                imfmReceivedFinancialClassItems.Add("42");
                imfmReceivedFinancialClassItems.Add("44");
                imfmReceivedFinancialClassItems.Add("45");
                imfmReceivedFinancialClassItems.Add("46");
                imfmReceivedFinancialClassItems.Add("47");
                imfmReceivedFinancialClassItems.Add("49");
                imfmReceivedFinancialClassItems.Add("58");
                imfmReceivedFinancialClassItems.Add("84");
                imfmReceivedFinancialClassItems.Add("85");
                return imfmReceivedFinancialClassItems;
            }
        }

        public bool IsMedicaidManagedFC17
        {
            get { return Code == MCAID_MGD_CONTR_CODE; }
        }

        #region Properties

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialClass()
        {
        }
        public FinancialClass( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public FinancialClass( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        

        #endregion

        #region Constants

        public const string MED_SCREEN_EXM_CODE = "37";
        public const string SIGNED_OVER_MEDICARE_FINANCIAL_CLASS_CODE = "84";
        public const string UNINSURED_CODE = "70";
        public const string MCARE_MCD_CROSSOVER_CODE = "45";
        public const string MCAID_MGD_CONTR_CODE = "17";

        #endregion
    }
}
