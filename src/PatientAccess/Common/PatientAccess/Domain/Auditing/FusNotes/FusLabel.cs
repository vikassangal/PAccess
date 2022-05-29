namespace PatientAccess.Domain.Auditing.FusNotes
{
	/// <summary>
	/// Summary description for FusLabel.
	/// </summary>
	public class FusLabel
	{
        #region Constants

        public const string
           LABEL_SPACE = " ",
            LABEL_COLON                 = ":",
            LABEL_COLON_SPACE           = ": ",
            LABEL_SEMI_COLON_SPACE      = "; ",


            // Benefits Verified
            IS_VERIFIED                 = "VERIFIED?",
            INITIALED_BY                = "INITIALED",
            BENEFITS_VERIFIED_DATE      = "DATE",

            // Authorization Required
            IS_AUTH_REQUIRED            = "AUTH REQ'D",
            AUTH_COMPANY_NAME           = "AUTH CO",
            PHONE_NUMBER                = "PH#",
            PHONE_EXTENSION             = "EXT",
            AUTHORIZATION_NUMBER        = "AUTHORIZATION NUMBER",
            AUTHORIZATION_STATUS        = "AUTHORIZATION STATUS",
            AUTHORIZATION_EFFECTIVE_DATE    = "AUTHORIZATION EFFECTIVE DATE",
            AUTHORIZATION_EXPIRATION_DATE   = "AUTHORIZATION EXPIRATION DATE",
            AUTHORIZATION_REMARKS       = "AUTHORIZATION REMARKS",
            SERVICES_AUTHORIZED         = "SERVICES AUTHORIZED",
            AUTHORIZATION_NUMBER_OF_DAYS_AUTHORIZED = "NUMBER OF DAYS AUTHORIZED",
            AUTH_CO_REP_NAME = "AUTH CO REP NAME",
            TRACKING_NUMBER             = "TRACKING NUMBER",
            NOT_APPLICABLE = "n/a",

            // Commercial Coverage Category
            PAYOR_NAME                  = "PAYOR",
            INFO_RECEIVED               = "INFO REC'D",
            ELIGIBILITY_PHONE           = "ELIG PH#",
            INS_COMPANY_REP_NAME        = "INS CO REP",
            INSURED_EFF_DATE            = "EFF DATE",
            INSURED_TERM_DATE           = "TERM DATE",
            PRIMARY_PAYOR_VERIFICATION  = "PRIMARY PAYOR VERIFICATION INFORMATION; ",
            SECONDARY_PAYOR_VERIFICATION = "SECONDARY PAYOR VERIFICATION INFORMATION; ",

            // Benefit Categories
            BENEFITS_CATEGORY           = "BENEFITS CATEGORY",
            CATEGORY_INPATIENT          = "IP",
            CATEGORY_OUTPATIENT         = "OP",
            CATEGORY_NEWBORN            = "NB",
            CATEGORY_NICU               = "NICU",
            CATEGORY_OB                 = "OB",
            CATEGORY_PSYCH_IP           = "PSYCH IP",
            CATEGORY_PSYCH_OP           = "PSYCH OP",
            CATEGORY_CHEM_DP            = "CHEM DEP",
            CATEGORY_SNF_SUBACUTE       = "SNF/SUBACUTE",
            CATEGORY_REHAB_IP           = "REHAB IP",
            CATEGORY_REHAB_OP           = "REHAB OP",
            CATEGORY_GENERAL            = "GENERAL",

            // Fields within each Benefit Category
            DEDUCTIBLE                  = "DED",
            TIME_PERIOD                 = "TIME PD (YEAR/VISIT)",
            IS_MET                      = "MET?",
            DOLLAR_MET                  = "$MET",
            CO_INSURANCE                = "CO-INS",
            OUT_OF_POCKET               = "OUT OF POCK",
            PERCENTAGE_OUT_OF_POCKET    = "% OUT OF POCK",
            CO_PAY_AMOUNT               = "CO-PAY",
            WAIVE_COPAY                 = "WAIVE CO-PAY IF ADMITTED?",
            NUMBER_VISITS_PER_YEAR      = "# OF VISITS/YR",
            LIFETIME_MAX_BENEFIT        = "LIFETIME MAX",
            REMAINING_LIFETIME_VALUE    = "REM LIFETIME",
            REMAINING_BENEFIT_PER_VISITS = "REM BEN/VISIT",
            MAX_BENEFIT_PER_VISIT       = "MAX BEN/VISIT",
            IS_PRE_EXISTING             = "PRE-EXISTING?",
            IS_COVERED_BENEFIT          = "COV'D BENEFIT?",
            CLAIMS_ADDR_VERIFIED        = "CLMS ADDR VERIFIED?",
            COORDINATION_OF_BENEFITS    = "COORD OF BENEFITS",
            COB_RULE                    = "COB RULE",
            TYPE_OF_PRODUCT             = "TYPE OF PRODUCT",
            PPO_NETWORK_NAME            = "BROKER/NETWORK",
            IS_CONTRACTED_PROVIDER      = "CONTRACTED?",
            AUTO_CLAIM_NUMBER           = "AUTO CLM #",
            AUTO_MEDPAY_COVERAGE        = "MEDPAY COV'G",
            LABEL_REMARKS               = "REMARKS",

            //Medicare Coverage Category
            PART_A_COVERAGE             = "PART A COV'G",
            PART_A_EFFECTIVE_DATE       = "PART A EFF",
            PART_B_COVERAGE             = "PART B COV'G",
            PART_B_EFFECTIVE_DATE       = "PART B EFF",
            HAS_MEDICARE_HMO            = "HMO?",
            IS_MEDICARE_SECONDARY       = "MSP?",
            LAST_BILLING_DATE           = "LAST BLG ACT?",
            REMAINING_HOSP_DAYS         = "REMAINING HOSP DAYS",
            REMAINING_CO_INS_DAYS       = "REMAINING COINS DAYS",
            REMAINING_LIFITIME_RESV     = "REMAINING LIFETIME RES DAYS",
            REMAINING_SNF_DAYS          = "REMAINING SNF DAYS",
            REMAINING_SNF_CO_INS_DAYS   = "REMAINING SNF CO-INS DAYS",
            REMAINING_PART_A_DEDUCTIBLE = "REMAINING PART-A DED",
            REMAINING_PART_B_DEDUCTIBLE = "REMAINING PART-B DED",
            IS_PATIENT_IN_HOSPICE       = "HOSPICE",
            VERIFIED_BENEFICIARY_NAME   = "VERIFIED BENEFICIARY?",
            INFO_RECEIVED_FROM          = "INFO REC'D FROM",

            // Medicaid Coverage Category
            ELIGIBILITY_DATE            = "ELIG DATE",
            PATIENT_HAS_MEDICARE        = "DOES PT HAVE MCARE?",
            THIRD_PARTY_INSURANCE       = "3RD PARTY INS?",
            EVC_NUMBER                  = "EVC #",

            // Workers Compensation
            PPO_NETWORK_BROKER          = "PPO NETWORK/BROKER",
            INCIDENT_CLAIM_NUMBER       = "CLAIM #",
            IS_ADDRESS_VERIFIED            = "ADDR VERIFIED?",
            INSURANCE_PHONE_NUMBER      = "INS PH #",
            HAS_EMP_PAID_PREMIUMS       = "HAS EMP PD PREMIUMS?",

            // Government-Other
            TYPE_OF_COVERAGE            = "TYPE OF COV'G",

            // Self Pay
            PATIENT_HAS_MEDICAID        = "DOES PT HAVE MCAID?",
            INS_INFO_UNAVAILABLE        = "INS UNAVAILABLE?",

            // Patient Liability - 
            // RPFCR Financial Activity
            TRANSACTION_DATE            = "TRANS DATE",
            ACCOUNT_NUMBER              = "ACCOUNT#",
            PATIENT_NAME                = "PNAME",
            CASH_RECEIPT_NUMBER         = "CRJ#",
            CREDIT_CARD_PAYMENT         = "CC1 PYMT",
            CREDIT_CARD_TYPE            = "CC1 TYPE",
            CREDIT_CARD_PAYMENT1         = "CC2 PYMT",
            CREDIT_CARD_TYPE1            = "CC2 TYPE",
            CREDIT_CARD_PAYMENT2         = "CC3 PYMT",
            CREDIT_CARD_TYPE2            = "CC3 TYPE",
            CHECK_PAYMENT               = "CK PYMT",
            CHECK_NUMBER                = "CK#",
            CASH_PAYMENT                = "CASH PYMT",
            MONEY_ORDER_PAYMENT         = "MO PYMT",
            SUM_OF_TODAYS_PAYMENT       = "TODAYS PYMT",
            ZERO_PAYMENT_REASON         = "REASON FOR 0$?",
            NUMBER_OF_PAYMENTS          = "#PYMTS",
            MONTHLY_PAYMENT_AMOUNT      = "MO PYMT AMT",
            MONTHLY_DUE_DATE            = "Monthly DUE DATE",
            LOGGED_IN_USER_NAME         = "USERNAME"  ,              
            ATTORNEYNAME                = "ATTORNEY NAME",
            ATTORNEYPHONE               = "ATTORNEY PHONE" ,
            ATTORNEYADDRESS             = "ATTORNEY ADDRESS",
            INS_COMPANY_REP_PHONE       = "INS COMPANY REP PHONE" ,
            INS_COMPANY_REP_ADDRESS     = " INS COMPANY REP ADDRESS",

            TOTAL_CURRENT_AMOUNT_DUE    = "TOTAL CURRENT AMOUNT DUE",
            
            // RFCMO Financial Activity
            DOLLAR_AMOUNT1              = "Dollar Amount 1",
            TOTAL_MONTHLY_DUE_CHANGED   = "Total Monthly Due Changed From ${0} to ${1}",
            
            // RFCAC Financial Activity
            TOTAL_AMT_COLLECTED_CHANGED = "Total Amount Collected Changed From ${0} to ${1}",
            
            // RCALC Financial Activity
            TOTAL_AMOUNT_DUE_CHANGED    = "Total Amount Due Changed From ${0} to ${1}",
            
            // RFCNL Financial Activity
             PATIENT_LIABILITY_CHANGED = "Patient Liability has Changed From ${0} to $0",
            OPT_OUT_OPTIONS_SELECTED = "OPT OUT OPTION(S) SELECTED ",
            NPPSIGNED = "NPP SIGNED",
            COSSIGNED = "COS SIGNED",
            COSNOTSIGNED = "COS not signed",
            VALUABLES_COLLECTED = "VALUABLES COLLECTED",
            NAME_CHANGED_FROM = "PATIENTS NAME CHANGED FROM {0} to {1}"  ,
	        DIALYSISCENTERPATIENT = "DIALYSIS CENTER PATIENT",
	        IS_SELECTED = "selected",
	        RIGHT_TO_RESTRICT = "Patient Requested Right To Restrict",

           //Guarantor Consent and Cell Phone Number
            CHANGE_TO_GUARANTOR_CONSENT_FLAG = "Guar Cell Consent Flg Fr: {0} To: {1}",

            //DOFR FUS Note
            PLANID = "Plan ID",
            DOFR_API_SUCCESS_MSG="was recommended as having the highest probability of being accepted by the payor for this patient encounter.",
            DOFR_API_FAILURE_MSG = "No plan id was recommended.",
            FACILITY = "Facility",
            PAYOR = "Payor",
            PT = "PT",
            HSV = "HSV",
            FC = "FC",
            CLINIC_CODE = "Clinic Code",
            SERVICE_CATEGORY= "Service Category",
            AID_CODE = "Plan Type",
            IPA_CODE_AND_CLINIC = "IPA Code and Clinic: {0} {1}";

            #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FusLabel()
        {
        }
        #endregion

        #region Data Elements
        #endregion
	}
}
