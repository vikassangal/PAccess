using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class Coverage : PersistentModel
    {
        private const string PROPERTY_REMARKS = "Remarks";
        private const string PROPERTY_INFORMATIONRECEIVEDSOURCE = "InformationReceivedSource";

        #region Event Handlers
        #endregion

        #region Methods

        private void ClearConstraints()
        {
            if (i_CoverageConstraints != null)
            {
                if (i_CoverageConstraints.GetType() == typeof( CommercialConstraints ))
                {
                    i_CoverageConstraints = new CommercialConstraints();
                }
                else if (i_CoverageConstraints.GetType() == typeof( GovernmentOtherConstraints ))
                {
                    i_CoverageConstraints = new GovernmentOtherConstraints();
                }
                else if (i_CoverageConstraints.GetType() == typeof( MedicaidConstraints ))
                {
                    i_CoverageConstraints = new MedicaidConstraints();
                }
                else if (i_CoverageConstraints.GetType() == typeof( MedicareConstraints ))
                {
                    i_CoverageConstraints = new MedicareConstraints();
                }
            }
        }

        public virtual void SetCoverageConstraints( CoverageConstraints constraints )
        {
            if (constraints != null)
            {
                i_CoverageConstraints = constraints;
            }
        }

        public static Coverage CoverageFor( InsurancePlan plan, Insured insured )
        {
            Coverage coverage = Activator.CreateInstance( c_CoverageToPlanMap[plan.GetType()] as Type ) as Coverage;
            coverage.InsurancePlan = plan;
            coverage.Insured = insured;

            return coverage;
        }

        public new Coverage DeepCopy()
        {
            BinaryFormatter formatter;
            MemoryStream stream = null; Coverage result = null;
            try
            {
                stream = new MemoryStream();
                formatter = new BinaryFormatter();
                formatter.Serialize( stream, this );
                stream.Position = 0;
                result = formatter.Deserialize( stream ) as Coverage;
            }
            finally
            {
                if (null != stream)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return result;
        }


        public abstract string GenerateFusNotes();
        public abstract Account InsertFusNotesInto( Account account, Coverage originalCoverage );

        public abstract void RemoveCoveragePayorData();

        public virtual void RemoveCoverageVerificationData()
        {

            ClearConstraints();

            PermittedDays = -1;
            ForceUnChangedStatusFor( "PermittedDays" );
            Attorney = new Attorney();
            ForceUnChangedStatusFor( "Attorney" );
            DataValidationTicket = new DataValidationTicket();
            EligibilityVerified = new YesNotApplicableFlag();
            ForceUnChangedStatusFor( "EligibilityVerified" );
            InformationReceivedSource = new InformationReceivedSource();
            ForceUnChangedStatusFor( "InformationReceivedSource" );
            InsuranceAgent = new InsuranceAgent();
            ForceUnChangedStatusFor( "InsuranceAgent" );
            VerificationCoPay = -1;
            ForceUnChangedStatusFor( "VerificationCoPay" );
            VerificationDeductible = -1;
            ForceUnChangedStatusFor( "VerificationDeductible" );
            Remarks = String.Empty;
            ForceUnChangedStatusFor( "Remarks" );
            BenefitsVerified.SetBlank();
            ForceUnChangedStatusFor( "BenefitsVerified" );
            AuthorizingPerson = String.Empty;
            ForceUnChangedStatusFor( "AuthorizingPerson" );
            DateTimeOfVerification = DateTime.MinValue;
            ForceUnChangedStatusFor( "DateTimeOfVerification" );


        }

        public void RemoveCoverageLiabilityData()
        {
            CoPay = 0M;
            Deductible = 0M;
            NoLiability = false;
            VerificationCoPay = 0M;
            VerificationDeductible = 0M;
        }

        public virtual void RemoveAuthorization()
        {
        }

        public bool IsNotMedicareOrMedicaid()
        {
            bool coverageIsNotMedicareOrMedicaid = true;
            if( this is GovernmentMedicareCoverage ||
                this is GovernmentMedicaidCoverage )
            {
                coverageIsNotMedicareOrMedicaid = false;
            }

            if ( InsurancePlan != null && !InsurancePlan.IsNotMedicareOrMedicaid() )
            {
                coverageIsNotMedicareOrMedicaid = false;
            }

            return coverageIsNotMedicareOrMedicaid;
        }

        #endregion

        #region Properties
        public abstract string AssociatedNumber
        {
            get;
        }

        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
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

        public InsurancePlan InsurancePlan
        {
            get
            {
                return i_InsurancePlan;
            }
            set
            {
                i_InsurancePlan = value;
            }
        }

        public Insured Insured
        {
            get
            {
                return i_Insured;
            }
            set
            {
                i_Insured = value;
            }
        }

        public string KindOfAssociatedNumber
        {
            get
            {
                return i_KindOfAssociatedNumber;
            }
        }

        public BillingInformation BillingInformation
        {
            get
            {
                return i_BillingInformation;
            }
            set
            {
                i_BillingInformation = value;
            }
        }
        
        protected int PermittedDays
        {
            get
            {
                return i_PermittedDays;
            }
            set
            {
                i_PermittedDays = value;
            }
        }

        public string AuthorizingPerson
        {
            get
            {
                return i_AuthorizingPerson;
            }
            set
            {

                Debug.Assert( value != null );
                SetAndTrack( ref i_AuthorizingPerson, value, MethodBase.GetCurrentMethod() );

            }
        }

        public DateTime DateTimeOfVerification
        {
            get
            {
                return i_DateTimeOfVerification;
            }
            set
            {

                SetAndTrack( ref i_DateTimeOfVerification, value, MethodBase.GetCurrentMethod() );

            }
        }

        public decimal Deductible
        {
            get
            {
                return i_Deductible;
            }
            set
            {
                i_Deductible = value;
            }
        }

        public decimal CoPay
        {
            get
            {
                return i_CoPay;
            }
            set
            {
                i_CoPay = value;
            }
        }

        private decimal VerificationDeductible
        {
            set
            {
                i_VerificationDeductible = value;
            }
        }

        protected decimal VerificationCoPay
        {
            get
            {
                return i_VerificationCoPay;
            }
            set
            {
                SetAndTrack( ref i_VerificationCoPay, value, MethodBase.GetCurrentMethod() );
            }
        }
        public YesNotApplicableFlag EligibilityVerified
        {
            get
            {
                return i_EligibilityVerified;
            }
            set
            {
                i_EligibilityVerified = value; //Possible values are "YES","NO","N/A"
            }
        }

        public bool NoLiability
        {
            get
            {
                return i_NoLiability;
            }
            set
            {
                i_NoLiability = value;
            }
        }


        public YesNotApplicableFlag BenefitsVerified
        {
            get
            {
                return i_BenefitsVerified;
            }
            set
            {
                Debug.Assert( value != null );
                SetAndTrack( ref i_BenefitsVerified, value, MethodBase.GetCurrentMethod() );
            }
        }


        public string Remarks
        {
            get
            {
                return i_Remarks;
            }
            set
            {

                SetAndTrack( ref i_Remarks, value, MethodBase.GetCurrentMethod() );

            }
        }

        public bool WasElectronicallyVerifiedDuringCurrentActivity
        {
            private get
            {
                return i_WasElectronicallyVerifiedDuringCurrentActivity;
            }
            set
            {
                i_WasElectronicallyVerifiedDuringCurrentActivity = value;
            }
        }

        public InformationReceivedSource InformationReceivedSource
        {
            get
            {
                return i_InformationReceivedSource;
            }
            set
            {

                SetAndTrack( ref i_InformationReceivedSource, value, MethodBase.GetCurrentMethod() );

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

        public bool IsEmployerPlan
        {
            get
            {
                return i_IsEmployerPlan;
            }

            set
            {
                i_IsEmployerPlan = value;
            }
        }

        public bool IsNew
        {
            get
            {
                return i_IsNew;
            }
            set
            {
                i_IsNew = value;
            }
        }

        public bool WriteBenefitsVerifiedFUSNote
        {
            get
            {
                return i_WriteBenefitsVerifiedFUSNote;
            }
            set
            {
                i_WriteBenefitsVerifiedFUSNote = value;
            }
        }

        public bool WriteAuthRequiredFUSNote
        {
            get
            {
                return i_WriteAuthRequiredFUSNote;
            }
            set
            {
                i_WriteAuthRequiredFUSNote = value;
            }
        }

        public bool WriteVerificationEntryFUSNote
        {
            get
            {
                return i_WriteVerificationEntryFUSNote;
            }
            set
            {
                i_WriteVerificationEntryFUSNote = value;
            }
        }

        public Attorney Attorney
        {
            get
            {
                return i_Attorney;
            }
            set
            {
                i_Attorney = value;
            }
        }

        public InsuranceAgent InsuranceAgent
        {
            get
            {
                return i_InsuranceAgent;
            }
            set
            {

                SetAndTrack( ref i_InsuranceAgent, value, MethodBase.GetCurrentMethod() );

            }
        }
        public int Priority
        {
            get
            {
                return i_Priority;
            }
            set
            {
                i_Priority = value;
            }
        }

        #endregion

        #region Private Methods

        private static void PopulateCoverageToPlanMap()
        {
            if (c_CoverageToPlanMap == null)
            {
                lock (c_Sync)
                {
                    if (c_CoverageToPlanMap == null)
                    {
                        c_CoverageToPlanMap = new Hashtable();
                        c_CoverageToPlanMap.Add( typeof( CommercialInsurancePlan ), typeof( CommercialCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( GovernmentMedicaidInsurancePlan ), typeof( GovernmentMedicaidCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( GovernmentMedicareInsurancePlan ), typeof( GovernmentMedicareCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( GovernmentOtherInsurancePlan ), typeof( GovernmentOtherCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( OtherInsurancePlan ), typeof( OtherCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( SelfPayInsurancePlan ), typeof( SelfPayCoverage ) );
                        c_CoverageToPlanMap.Add( typeof( WorkersCompensationInsurancePlan ), typeof( WorkersCompensationCoverage ) );
                    }
                }
            }
        }


        protected virtual bool AddManualVerificationFusNoteTo( Account anAccount, Coverage originalCoverage )
        {

            bool didRecordNote = false;

            if (HaveVerificationFieldsChanged && !WasElectronicallyVerifiedDuringCurrentActivity)
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                if (anAccount.Insurance.PrimaryCoverage == this)
                {

                    fusNoteFactory.AddRVINBNoteTo( anAccount, this, originalCoverage );

                }//if
                else
                {

                    fusNoteFactory.AddRVINSNoteTo( anAccount, this, originalCoverage );

                }//else

                didRecordNote = true;

            }//if

            return didRecordNote;

        }


        protected virtual bool HaveVerificationFieldsChanged
        {
            get
            {
                return HasChangedFor( PROPERTY_INFORMATIONRECEIVEDSOURCE ) ||
                       HasChangedFor( PROPERTY_REMARKS );
            }
        }


        protected virtual bool AddElectronicVerificationFusNoteTo( Account anAccount, Coverage originalCoverage )
        {

            bool didRecordNote = false;

            if (WasElectronicallyVerifiedDuringCurrentActivity && HaveVerificationFieldsChanged)
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRDOTVNoteTo( anAccount, this, originalCoverage );
                didRecordNote = true;

            }//if

            return didRecordNote;

        }


        #endregion

        #region Private Properties

        public bool IsMedicareCoverageValidForAuthorization
        {
            get
            {
                if (this.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var cov = (GovernmentMedicareCoverage)this;
                    return cov.InsurancePlan != null &&
                           cov.InsurancePlan.PlanID == InsurancePlan.MEDICARE_PLAN_ID_53544;
                }
                return false;
            }
        }

        #endregion

        #region Construction and Finalization
        public Coverage()
        {
        }

        static Coverage()
        {
            PopulateCoverageToPlanMap();
        }
        #endregion

        #region Data Elements

        [NonSerialized]
        private Account i_Account = new Account();

        private InsurancePlan i_InsurancePlan;
        private CoverageOrder i_CoverageOrder = new CoverageOrder();
        private Insured i_Insured = new Insured();
        protected string i_AssociatedNumber = String.Empty;
        private string i_KindOfAssociatedNumber = String.Empty;
        private static Hashtable c_CoverageToPlanMap;
        private static object c_Sync = new Object();
        private BillingInformation i_BillingInformation = new BillingInformation();
        private int i_PermittedDays = 0;
        private string i_AuthorizingPerson = String.Empty;
        private DateTime i_DateTimeOfVerification;
        private decimal i_Deductible;
        private decimal i_CoPay;
        private bool i_NoLiability;
        private bool i_IsEmployerPlan;
        private bool i_IsNew;
        private YesNotApplicableFlag i_EligibilityVerified = new YesNotApplicableFlag();  //Possible values are "YES","NO","N/A"
        private YesNotApplicableFlag i_BenefitsVerified = new YesNotApplicableFlag();
        private string i_Remarks = String.Empty;
        private InformationReceivedSource i_InformationReceivedSource = new InformationReceivedSource();
        private DataValidationTicket i_DataValidationTicket;

        private bool i_WriteBenefitsVerifiedFUSNote = false;
        private bool i_WriteAuthRequiredFUSNote = false;
        private bool i_WriteVerificationEntryFUSNote = false;

        private Attorney i_Attorney = new Attorney();
        private InsuranceAgent i_InsuranceAgent = new InsuranceAgent();
        private decimal i_VerificationDeductible = 0M;
        private decimal i_VerificationCoPay;
        private int i_Priority;
        protected CoverageConstraints i_CoverageConstraints;

        private bool i_WasElectronicallyVerifiedDuringCurrentActivity = false;

        #endregion
        public static readonly Collection<String> TestMBINumbers = new Collection<String>
        {
            "1EG4TE5MK73" ,
            "1EG4TE5MK23"
        };
        #region Constants

        protected const string AUTHORIZATION_COMPANY = "AuthorizationCompany";
        protected const string AUTHORIZATION_NUMBER = "AuthorizationNumber";
        protected const string AUTHORIZATION_STATUS = "AuthorizationStatus";
        protected const string EFFECTIVE_DATE = "EffectiveDate";
        protected const string EXPIRATION_DATE = "ExpirationDate";
        protected const string NAME_OF_COMPANY_REPRESENTATIVE = "NameOfCompanyRepresentative";
        protected const string NUMBER_OF_DAYS_AUTHORIZED = "NumberOfDaysAuthorized";
        protected const string REMARKS = "Remarks";
        protected const string SERVICESAUTHORIZED = "ServicesAuthorized";
        
        #endregion
    }
}

