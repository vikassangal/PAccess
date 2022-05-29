using System;
using System.Collections;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
	/// <summary>
	/// AccountDetailsRequest object stores specific information pertaining to the 
	/// Coverage, Patient and Insured of an Account, in order to perform Benefits 
	/// Validation and Account Compliance Check. This avoids passing on the whole
	/// Account object from the client to the server and helps reduce network burden.
	/// </summary>
    [Serializable]
    public class AccountDetailsRequest
	{
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public long MedicalRecordNumber 
        {
            get
            {
                return medicalRecordNumber;
            }
            set
            {
                medicalRecordNumber = value;
            }
        }

        public long AccountNumber 
        {
            get
            {
                return accountNumber;
            }
            set
            {
                accountNumber = value;
            }
        }

        public long FacilityOid 
        {
            get
            {
                return facilityOid;
            }
            set
            {
                facilityOid = value;
            }
        }

        public string Upn 
        {
            get
            {
                return upn;
            }
            set
            {
                upn = value;
            }
        }

        public bool IsAccountNull 
        {
            get
            {
                return isAccountNull;
            }
            set
            {
                isAccountNull = value;
            }
        }

        public bool IsUserNull 
        {
            get
            {
                return isUserNull;
            }
            set
            {
                isUserNull = value;
            }
        }

        public bool IsCoverageNull 
        {
            get
            {
                return isCoverageNull;
            }
            set
            {
                isCoverageNull = value;
            }
        }

        public string PatientFirstName 
        {
            get
            {
                return patientFirstName;
            }
            set
            {
                patientFirstName = value;
            }
        }

        public string PatientLastName 
        {
            get
            {
                return patientLastName;
            }
            set
            {
                patientLastName = value;
            }
        }

        public string PatientMidInitial 
        {
            get
            {
                return patientMidInitial;
            }
            set
            {
                patientMidInitial = value;
            }
        }

        public string PatientSex 
        {
            get
            {
                return patientSex;
            }
            set
            {
                patientSex = value;
            }
        }

        public SocialSecurityNumber PatientSSN 
        {
            get
            {
                return patientSSN;
            }
            set
            {
                patientSSN = value;
            }
        }

        public DateTime PatientDOB 
        {
            get
            {
                return patientDOB;
            }
            set
            {
                patientDOB = value;
            }
        }

        public DateTime AdmitDate 
        {
            get
            {
                return admitDate;
            }
            set
            {
                admitDate = value;
            }
        }

        public RelationshipType PatientInsuredRelationship 
        {
            get
            {
                return patientInsuredRelationship;
            }
            set
            {
                patientInsuredRelationship = value;
            }
        }

        public string ReferringPhysicianNumber 
        {
            get
            {
                return referringPhysicianNumber;
            }
            set
            {
                referringPhysicianNumber = value;
            }
        }

        // Used for ComplianceChecker processing only

        public ICollection Coverages
        {
            get
            {
                return (ICollection)coverages;
            }
            set
            {
                coverages = (ICollection)value;
            }
        }

        public ContactPoint PatientMailingCP 
        {
            get
            {
                return patientMailingCP;
            }
            set
            {
                patientMailingCP = value;
            }
        }

        // Insured Details
        public string InsuredFirstName 
        {
            get
            {
                return insuredFirstName;
            }
            set
            {
                insuredFirstName = value;
            }
        }

        public string InsuredMiddleInitial
        {
            get
            {
                return insuredMiddleInitial;
            }
            set
            {
                insuredMiddleInitial = value;
            }
        }

        public string InsuredLastName 
        {
            get
            {
                return insuredLastName;
            }
            set
            {
                insuredLastName = value;
            }
        }

        public string InsuredGroupNumber 
        {
            get
            {
                return insuredGroupNumber;
            }
            set
            {
                insuredGroupNumber = value;
            }
        }

        public string InsuredSex 
        {
            get
            {
                return insuredSex;
            }
            set
            {
                insuredSex = value;
            }
        }

        public DateTime InsuredDOB 
        {
            get
            {
                return insuredDOB;
            }
            set
            {
                insuredDOB = value;
            }
        }

        public ContactPoint InsuredPhysicalCP 
        {
            get
            {
                return insuredPhysicalCP;
            }
            set
            {
                insuredPhysicalCP = value;
            }
        }

        // Coverage Details
        public string CoverageMemberId 
        {
            get
            {
                return coverageMemberId;
            }
            set
            {
                coverageMemberId = value;
            }
        }

        public string CoverageInsurancePlanId 
        {
            get
            {
                return coverageInsurancePlanId;
            }
            set
            {
                coverageInsurancePlanId = value;
            }
        }

        public DateTime CoverageIssueDate 
        {
            get
            {
                return coverageIssueDate;
            }
            set
            {
                coverageIssueDate = value;
            }
        }

        public long CoverageOrderOid 
        {
            get
            {
                return coverageOrderOid;
            }
            set
            {
                coverageOrderOid = value;
            }
        }

        // this property is used to persist the 'match' value for payor (that will be compared to the payor name on the response

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

        // this is a string representation of the actual coverage type (e.g. aCommCoverage.GetType().ToString())
        public string TypeOfCoverage
        {
            get
            {
                return typeOfCoverage;
            }
            set
            {
                this.typeOfCoverage = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AccountDetailsRequest()
        {
        }
        #endregion

        #region Data Elements
        //Account
        private long                                        facilityOid;
        private long                                        accountNumber;
        private DateTime                                    admitDate;
        private bool                                        isAccountNull = false;
        private string                                      referringPhysicianNumber;
        private ICollection                                 coverages = new ArrayList();

        //User
        private string                                      upn;
        private bool                                        isUserNull = false;

        //Patient
        private string                                      patientSex;
        private DateTime                                    patientDOB;
        private string                                      patientLastName;
        private string                                      patientFirstName;
        private string                                      patientMidInitial;
        private long                                        medicalRecordNumber;
        private ContactPoint                                patientMailingCP;
        private SocialSecurityNumber                        patientSSN;
        private RelationshipType                            patientInsuredRelationship;

        //Insured
        private string                                      insuredSex;
        private DateTime                                    insuredDOB;
        private string                                      insuredFirstName;
        private string                                      insuredMiddleInitial;
        private string                                      insuredLastName;        
        private string                                      insuredGroupNumber;
        private ContactPoint                                insuredPhysicalCP;

        //Coverage
        private long                                        coverageOrderOid;
        private string                                      coverageMemberId;
        private DateTime                                    coverageIssueDate;
        private bool                                        isCoverageNull = false;
        private string                                      coverageInsurancePlanId;
        private string                                      i_RequestPayorName = string.Empty;
        private string                                      typeOfCoverage = string.Empty;

        #endregion

        #region Constants
        #endregion
	}
}
