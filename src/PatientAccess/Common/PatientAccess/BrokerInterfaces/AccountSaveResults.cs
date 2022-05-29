using System;
using PatientAccess.Domain;
using PatientAccess.Domain.InterFacilityTransfer;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for AccountSaveResults.
	/// </summary>
	//TODO: Create XML summary comment for AccountSaveResults
    [Serializable]
    public class AccountSaveResults : object
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public InterFacilityTransferAccount interFacilityTransferAccount
        {
            get;set;
        }

        public long AccountNumber
        {
            private get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }
        public long MedicalRecordNumber
        {
            private get
            {
                return i_MedicalRecordNumber;
            }
            set
            {
                i_MedicalRecordNumber = value;
            }
        }
        public bool ClearClinics
        {
            private get
            {
                return i_ClearClinics;
            }
            set
            {
                i_ClearClinics = value;
            }
        }
       
        public long PrimaryInsuranceEmployerCode
        {
            private get
            {
                return i_PrimaryInsuranceEmployerCode;
            }
            set
            {
                i_PrimaryInsuranceEmployerCode = value;
            }
        }
        public long SecondaryInsuranceEmployerCode
        {
            private get
            {
                return i_SecondaryInsuranceEmployerCode;
            }
            set
            {
                i_SecondaryInsuranceEmployerCode = value;
            }
        }

        public Coverage PreMSECoverage
        {
            private get
            {
                return i_PreMSECoverage;
            }
            set
            {
                i_PreMSECoverage = value;
            }
        }

        public Insurance PreMSEInsurance
        {
            private get
            {
                return i_PreMSEInsurance;
            }
            set
            {
                i_PreMSEInsurance = value;
            }
        }

        public bool DeleteSecondaryCoverage
        {
            private get
            {
                return i_DeleteSecondaryCoverage;
            }
            set
            {
                i_DeleteSecondaryCoverage = value;
            }
        }
        #endregion

        #region Private Methods
        public void SetResultsTo(Account anAccount)
        {
            if( anAccount.AccountNumber == 0 )
            {
                anAccount.AccountNumber = this.AccountNumber;
                anAccount.IsNew = false;
            }

            if( anAccount.Patient != null &&
                anAccount.Patient.MedicalRecordNumber == 0)
            {
                anAccount.Patient.MedicalRecordNumber = this.MedicalRecordNumber;
            }

            if( anAccount.Guarantor != null &&
                anAccount.Guarantor.DataValidationTicket != null)
            {
                anAccount.Guarantor.DataValidationTicket.AccountNumber = this.AccountNumber;
                anAccount.Guarantor.DataValidationTicket.Facility = anAccount.Facility;
                anAccount.Guarantor.DataValidationTicket.MedicalRecordNumber = this.MedicalRecordNumber;
            }

            if (anAccount.Guarantor != null &&
                anAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).CellPhoneConsent !=
                null)
            {
                anAccount.OldGuarantorCellPhoneConsent =
                    anAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType())
                        .CellPhoneConsent;
            }

            anAccount.Patient.InterFacilityTransferAccount = this.interFacilityTransferAccount;

            if( this.ClearClinics )
            {
                anAccount.Clinics.Clear();
            }

            if( this.DeleteSecondaryCoverage )
            {
                anAccount.DeletedSecondaryCoverage = null;
            }

            // if this was a preMSE reg then there was no Insurance sent. The server/broker will create
            // a default one. Make sure we recieve it. 
            if (anAccount.Activity is PreMSERegisterActivity || anAccount.Activity is UCCPreMSERegistrationActivity)
            {
                if( anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) == null )
                {
                    anAccount.Insurance.AddCoverage( this.PreMSECoverage );
                }
                else
                {
                    anAccount.Insurance = this.PreMSEInsurance;
                }
            }

            if( anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID) != null )
            {
                Coverage aCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if( aCoverage.Insured.Employment != null
                    && aCoverage.Insured.Employment.Employer != null
                    && aCoverage.Insured.Employment.Employer.Name != string.Empty
                    && aCoverage.Insured.Employment.Employer.EmployerCode == 0 
                    && this.PrimaryInsuranceEmployerCode != INVALID_EMP_CODE)
                {
                    aCoverage.Insured.Employment.Employer.EmployerCode = this.PrimaryInsuranceEmployerCode;
                }
                if( aCoverage.DataValidationTicket != null
                    && aCoverage.DataValidationTicket.TicketId != null
                    && aCoverage.DataValidationTicket.TicketId.Trim() != string.Empty 
                    && aCoverage.DataValidationTicket.AccountNumber == 0 )
                {
                    aCoverage.DataValidationTicket.AccountNumber          = this.AccountNumber;
                    aCoverage.DataValidationTicket.Facility               = anAccount.Facility;
                    aCoverage.DataValidationTicket.MedicalRecordNumber    = this.MedicalRecordNumber;
                }
            }

            if( anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID) != null )
            {
                Coverage aCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
                if( aCoverage.Insured.Employment != null
                    && aCoverage.Insured.Employment.Employer != null
                    && aCoverage.Insured.Employment.Employer.Name != string.Empty
                    && aCoverage.Insured.Employment.Employer.EmployerCode == 0
                    && this.SecondaryInsuranceEmployerCode != INVALID_EMP_CODE)
                {
                    aCoverage.Insured.Employment.Employer.EmployerCode = this.SecondaryInsuranceEmployerCode;
                }
                if( aCoverage.DataValidationTicket != null
                    && aCoverage.DataValidationTicket.TicketId != null
                    && aCoverage.DataValidationTicket.TicketId.Trim() != string.Empty 
                    && aCoverage.DataValidationTicket.AccountNumber == 0 )
                {
                    aCoverage.DataValidationTicket.AccountNumber          = this.AccountNumber;
                    aCoverage.DataValidationTicket.Facility               = anAccount.Facility;
                    aCoverage.DataValidationTicket.MedicalRecordNumber    = this.MedicalRecordNumber;
                }
            }

        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AccountSaveResults()
        {
        }
        #endregion

        #region Data Elements
        
        private long i_AccountNumber;
        private long i_MedicalRecordNumber;
        private bool i_ClearClinics = false;
        private bool i_DeleteSecondaryCoverage = false;
        private long i_PrimaryInsuranceEmployerCode = INVALID_EMP_CODE;
        private long i_SecondaryInsuranceEmployerCode = INVALID_EMP_CODE;
        private Coverage i_PreMSECoverage = null;
        private Insurance i_PreMSEInsurance = new Insurance();

        #endregion

        #region Constants
        private const long INVALID_EMP_CODE = -1;
        #endregion
    }
}
