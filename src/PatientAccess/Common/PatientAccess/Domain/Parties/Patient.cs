using PatientAccess.Domain.InterFacilityTransfer;
using System;
using System.Collections;

namespace PatientAccess.Domain.Parties 
{
    /// <summary>
    /// Summary description for Patient.
    /// </summary>
    [Serializable]
    public class Patient : AbstractPatient
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override Patient AsPatient()
        {
            return this;
        }

        public void AddAccount( IAccount account )
        {
            i_Accounts.Add( account );
        }

        public void AddAccounts( IList accounts )
        {
            foreach( IAccount account in accounts )
            {
                AddAccount( account );
            }
        }

        private void AddAccountWithPaymentPlan( IAccount account )
        {
            i_AccountsWithPaymentPlan.Add( account );
        }

        public void AddAccountsWithPaymentPlan( IList accounts )
        {
            i_AccountsWithPaymentPlan.Clear();
            foreach( IAccount account in accounts )
            {
                AddAccountWithPaymentPlan( account );
            }
        }

        private void AddAccountWithNoPaymentPlan( IAccount account )
        {
            i_AccountsWithNoPaymentPlan.Add( account );
        }

        public void AddAccountsWithNoPaymentPlan( IList accounts )
        {
            i_AccountsWithNoPaymentPlan.Clear();
            foreach( IAccount account in accounts )
            {
                AddAccountWithNoPaymentPlan( account );
            }
        }

		public void AddAccountWithNoPaymentPlanWithPayments( IAccount account )
		{
			i_AccountsWithNoPaymentPlanWithPayments.Add( account );
		}

        public void ClearPriorSystemGeneratedOccurrenceSpans()
        {
            if ( SelectedAccount.OccurrenceSpans != null)
            {
                if ( SelectedAccount.OccurrenceSpans.Count > 1 &&
                     SelectedAccount.OccurrenceSpans[1] != null &&
                     ( IsNonPatient() ||
                     ( OccurrenceSpanIs70Or71(( OccurrenceSpan )SelectedAccount.OccurrenceSpans[1] ) &&
                       ( IsOutPatient() || IsActivatePreRegistrationActivity() ||
                         OccurrenceSpanIsSystemGeneratedForNonOutPatient(
                           ( OccurrenceSpan )SelectedAccount.OccurrenceSpans[1]) ) ) ) 
                   )
                {
                    SelectedAccount.OccurrenceSpans[1] = null;
                }

                if ( SelectedAccount.OccurrenceSpans.Count > 1 &&
                     SelectedAccount.OccurrenceSpans[0] != null &&
                     ( IsNonPatient() ||
                     ( OccurrenceSpanIs70Or71( ( OccurrenceSpan )SelectedAccount.OccurrenceSpans[0] ) &&
                       ( IsOutPatient() || IsActivatePreRegistrationActivity() ||
                         OccurrenceSpanIsSystemGeneratedForNonOutPatient(
                           ( OccurrenceSpan )SelectedAccount.OccurrenceSpans[0] ) ) ) )
                   )
                {
                    SelectedAccount.OccurrenceSpans[0] = null;
                }

                if ( SelectedAccount.OccurrenceSpans.Count > 0)
                {
                    SysGenSpan1 = (OccurrenceSpan)SelectedAccount.OccurrenceSpans[0];
                    if ( SelectedAccount.OccurrenceSpans.Count > 1)
                    {
                        SysGenSpan2 = (OccurrenceSpan)SelectedAccount.OccurrenceSpans[1];
                    }
                }
            }
        }

        private bool IsActivatePreRegistrationActivity()
        {
            return ( SelectedAccount.Activity != null &&
                     SelectedAccount.Activity.GetType().Equals( typeof( RegistrationActivity ) ) &&
                     SelectedAccount.Activity.AssociatedActivityType != null &&
                     SelectedAccount.Activity.AssociatedActivityType.Equals( typeof( ActivatePreRegistrationActivity ) ) );
        }

        private bool IsOutPatient()
        {
            return ( SelectedAccount.KindOfVisit != null && 
                     SelectedAccount.KindOfVisit.Code == VisitType.OUTPATIENT );
        }

        private bool IsNonPatient()
        {
            return ( SelectedAccount.KindOfVisit != null && 
                     SelectedAccount.KindOfVisit.Code == VisitType.NON_PATIENT );
        }

        private bool OccurrenceSpanIsSystemGeneratedForNonOutPatient( OccurrenceSpan occurrenceSpan )
        {
            return ( SelectedAccount.KindOfVisit != null && 
                     SelectedAccount.KindOfVisit.Code != VisitType.OUTPATIENT &&
                     occurrenceSpan.IsSystemGenerated );
        }

        private static bool OccurrenceSpanIs70Or71( OccurrenceSpan occurrenceSpan )
        {
            return  occurrenceSpan.SpanCode.IsQualifyingStayDate ||
                    occurrenceSpan.SpanCode.IsPriorStayDates;
        }

        public void AddAutoGeneratedSpanCodes70And71With(SpanCode spanCode70, SpanCode spanCode71)
        {
            ClearPriorSystemGeneratedOccurrenceSpans();

            if ( SelectedAccount != null &&
                 SelectedAccount.KindOfVisit != null &&
                 (( SelectedAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT && !SelectedAccount.Activity.IsActivatePreRegisterActivity()) ||
                    SelectedAccount.KindOfVisit.Code == VisitType.INPATIENT))
            {
                ArrayList sortedAccountHistory = new ArrayList( Accounts );
                SortAccountsByDischarge sortAccounts = new SortAccountsByDischarge();

                sortedAccountHistory.Sort(sortAccounts);

                sortedAccountHistory.Reverse();

                if (sortedAccountHistory.Count > 0)
                {
                    for (int i = 0; i < sortedAccountHistory.Count; i++)
                    {
                        IAccount proxy = ( IAccount )sortedAccountHistory[i];
                        if (proxy.AccountNumber == SelectedAccount.AccountNumber ||
                            proxy.KindOfVisit.Code != VisitType.INPATIENT ||
                            Passed60Days(proxy.DischargeDate))
                        {
                            sortedAccountHistory.Remove(proxy);
                            i -= 1;
                        }
                    }
                }

                if (sortedAccountHistory.Count > 0)
                {
                    FindSysGenSpan1(sortedAccountHistory, spanCode70, spanCode71);

                    if (sortedAccountHistory.Count > 0)
                    {
                        FindSysGenSpan2(sortedAccountHistory, spanCode71);
                    }
                }
            }

            UpdateOccurrenceSpansInModel();
        }

        public int AgeInYearsFor()
        {
            return AgeInYearsFor( Facility.GetCurrentDateTime() );
        }
        public void SetPatientContextHeaderData()
        {
            var aka = string.Empty;
            if (this.HasAliases())
            {
                aka = ((Name)this.Aliases[0]).AsFormattedName();
            }
            this.PatientContextHeaderData = new PatientContextHeaderData
            {
                PatientName = Name,
                DOB = DateOfBirth,
                MRN = MedicalRecordNumber,
                Sex = Sex,
                SSN = SocialSecurityNumber,
                AKA = aka
            }; 
        }

        public void SetAdditionalRacesToBlank()
        {
            SetRace3ToBlank();
            SetRace4ToBlank();
            SetRace5ToBlank();
        }

        #endregion

        #region Properties
        public decimal PreviousBalanceDue
        {
            get
            {
                decimal previousBalanceDue = 0L;
                foreach( IAccount   accountProxy in i_AccountsWithNoPaymentPlan)
                {
                    previousBalanceDue = previousBalanceDue + accountProxy.BalanceDue;
                }
                return previousBalanceDue;
            }
        }
        
        public IList Accounts 
        {
            get
            {
                return (IList)i_Accounts.Clone();
            }
        }

        public IList AccountsWithPaymentPlan 
        {
            get
            {
                return (IList)i_AccountsWithPaymentPlan.Clone();
            }
        }

        public IList AccountsWithNoPaymentPlan 
        {
            get
            {
                return (IList)i_AccountsWithNoPaymentPlan.Clone();
            }
        }

		public IList AccountsWithNoPaymentPlanWithPayments
		{
			get
			{
				return (IList)i_AccountsWithNoPaymentPlanWithPayments.Clone();
			}
		}
        /// <summary>
        /// MaritalStatus of the Patient. 
        /// </summary>
        public MaritalStatus MaritalStatus
        {
            get
            {
                return i_MaritalStatus;
            }
            set
            {
                i_MaritalStatus = value;
            }
        }

        public override Gender Sex 
        {
            get
            {
                return base.Sex;
            }
            set
            {
                if( value != null && i_SelectedAccount != null )
                {
                    ProcGenderPregnantVal( value );
                }

                base.Sex = value;
            }
        }
        public  Gender BirthSex { get; set; }

        public PatientContextHeaderData PatientContextHeaderData { get; set; }
        public string PrintRaceString
        {
            get
            {
                if (Nationality != null && !String.IsNullOrEmpty(Nationality.Code))
                {
                    return Nationality.Description;
                }
                if (Race != null && !String.IsNullOrEmpty(Race.Code))
                {
                    return Race.Description;
                }
                return string.Empty;
            }
        }

        public string PrintEthnicityString
        {
            get
            {
                if (Descent != null && !String.IsNullOrEmpty(Descent.Code))
                {
                    return Descent.Description;
                }

                if (Ethnicity != null && !String.IsNullOrEmpty(Ethnicity.Code))
                {
                    return Ethnicity.Description;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Race of the Patient 
        /// </summary>
        public Race Race
        {
            get
            {
                return i_Race;
            }
            set
            {
                i_Race = value;
            }
        }
        /// <summary>
        ///Second  Race of the Patient 
        /// </summary>
        public Race Race2
        {
            get
            {
                return i_Race2;
            }
            set
            {
                i_Race2 = value;
            }
        }
        /// <summary>
        ///Third  Race of the Patient 
        /// </summary>
        public Race Race3
        {
            get
            {
                return i_Race3;
            }
            set
            {
                i_Race3 = value;
            }
        }
        /// <summary>
        ///Forth  Race of the Patient 
        /// </summary>
        public Race Race4
        {
            get
            {
                return i_Race4;
            }
            set
            {
                i_Race4 = value;
            }
        }
        /// <summary>
        ///Fifth  Race of the Patient 
        /// </summary>
        public Race Race5
        {
            get
            {
                return i_Race5;
            }
            set
            {
                i_Race5 = value;
            }
        }
        public Race Nationality
        {
            get { return i_Nationality; }
            set { i_Nationality = value; }
        }
        public Race Nationality2
        {
            get { return i_Nationality2; }
            set { i_Nationality2 = value; }
        }
        public bool IsRace2Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Race2 != null) &&
                    (account.Patient.Race2.IsValid));
        }
        public bool IsRace3Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Race3 != null) &&
                    (account.Patient.Race3.IsValid));
        }
        public bool IsRace4Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Race4 != null) &&
                    (account.Patient.Race4.IsValid));
        }
        public bool IsRace5Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Race5 != null) &&
                    (account.Patient.Race5.IsValid));
        }
        public bool IsRaceValid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Race != null) &&
                    (account.Patient.Race.IsValid));
        }
        public bool IsNationalityValid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Nationality != null) &&
                    (account.Patient.Nationality.IsValid));
        }
        public bool IsNationality2Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Nationality2 != null) &&
                    (account.Patient.Nationality2.IsValid));
        }

        public bool IsEthnicityValid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Ethnicity != null) &&
                    (account.Patient.Ethnicity.IsValid));
        }
        public bool IsEthnicity2Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Ethnicity2 != null) &&
                    (account.Patient.Ethnicity2.IsValid));
        }
        public bool IsDescentValid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Descent != null) &&
                    (account.Patient.Descent.IsValid));
        }
        public bool IsDescent2Valid(Account account)
        {
            return ((account.Patient != null) &&
                    (account.Patient.Descent2 != null) &&
                    (account.Patient.Descent2.IsValid));
        }
        /// <summary>
        /// Ethnicity of the Patient 
        /// </summary>
        public Ethnicity Ethnicity
        {
            get
            {
                return i_Ethnicity;
            }
            set
            {
                i_Ethnicity = value;
            }
        }
        /// <summary>
        /// Ethnicity of the Patient 
        /// </summary>
        public Ethnicity Ethnicity2
        {
            get
            {
                return i_Ethnicity2;
            }
            set
            {
                i_Ethnicity2 = value;
            }
        }
        public Ethnicity Descent
        {
            get
            {
                return i_Descent;
            }
            set
            {
                i_Descent = value;
            }
        }
        public Ethnicity Descent2
        {
            get
            {
                return i_Descent2;
            }
            set
            {
                i_Descent2 = value;
            }
        }

        /// <summary>
        /// PlaceOfBirth of the Patient 
        /// </summary>
        public string  PlaceOfBirth
        {
            get
            {
                return i_PlaceOfBirth;
            }
            set
            {
                i_PlaceOfBirth = value;
            }
        }
        /// <summary>
        /// Language prefered by this Patient 
        /// </summary>
        public Language  Language
        {
            get
            {
                return i_Language;
            }
            set
            {
                i_Language = value;
            }
        }
        /// <summary>
        /// religion of the Patient 
        /// </summary>
        public Religion  Religion
        {
            get
            {
                return i_Religion;
            }
            set
            {
                i_Religion = value;
            }
        }
        /// <summary>
        /// PlaceOfWorship of this Patient 
        /// </summary>
        public PlaceOfWorship  PlaceOfWorship
        {
            get
            {
                return i_PlaceOfWorship;
            }
            set
            {
                i_PlaceOfWorship = value;
            }
        }
        
        public string  BloodlessPatient
        {
            get
            {
                return i_bloodlessPatient;
            }
            set
            {
                i_bloodlessPatient = value;
            }
        }
                       
        public string MaidenName
        {
            get
            {
                return i_MaidenName;
            }
            set
            {
                i_MaidenName = value;
            }
        }

        public string FathersName
        {
            get
            {
                return i_FathersName;
            }
            set
            {
                i_FathersName = value;
            }
        }

        // Do Not Resuscitate
        public YesNoFlag DoNotResuscitate
        {
            get
            {
                return i_DoNotResuscitate;
            }
            set
            {
                i_DoNotResuscitate = value;
            }
        }

        public int PreviousMRN
        {
            get
            {
                return i_PreviousMRN;
            }
            set
            {
                i_PreviousMRN = value;
            }
        }

        public DateTime LastUpdated
        {
            get
            {
                return i_LastUpdated;
            }
            set
            {
                i_LastUpdated = value;
            }
        }

        public Account SelectedAccount
        {
            get
            {
                return i_SelectedAccount;
            }
            set
            {
                i_SelectedAccount = value;
            }
        }

        public Account MothersAccount
        {
            get
            {
                return i_MothersAccount;
            }
            set
            {
                i_MothersAccount = value;
            }
        }

        public string MothersName
        {
            get
            {
                return i_MothersName;
            }
            set
            {
                i_MothersName = value;
            }
        }

        public DateTime MothersDateOfBirth
        {
            get
            {
                return i_MothersDateOfBirth;
            }
            set
            {
                i_MothersDateOfBirth = value;
            }
        }

        public DateTime FathersDateOfBirth
        {
            get
            {
                return i_FathersDateOfBirth;
            }
            set
            {
                i_FathersDateOfBirth = value;
            }
        }

        public NoticeOfPrivacyPracticeDocument NoticeOfPrivacyPracticeDocument
        {
            get
            {
                return i_NPPDocument;
            }
            set
            {
                i_NPPDocument = value;
            }
        }
        public YesNoFlag HospitalCommunicationOptIn
        {
            get { return i_HospitalCommunicationOptIn; }
            set { i_HospitalCommunicationOptIn = value; }
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

        private OccurrenceSpan SysGenSpan1
        {
            get
            {
                return i_SysGenSpan1;
            }
            set
            {
                i_SysGenSpan1 = value;
            }
        }

        private OccurrenceSpan SysGenSpan2
        {
            get
            {
                return i_SysGenSpan2;
            }
            set
            {
                i_SysGenSpan2 = value;
            }
        }

        public DateTime MostRecentAccountCreationDate { get; set; }

        public long MostRecentAccountNumber { get; set; }

        public bool SequesteredPatientAlertShown
        {
            get { return i_SequesteredPatientAlertShown; }
            set { i_SequesteredPatientAlertShown = value; }
        }

        public InterFacilityTransferAccount InterFacilityTransferAccount
        {
            get { return i_InterfacilityTransferAccount; }
            set { i_InterfacilityTransferAccount = value; }
        }

        #endregion

        #region Private Methods
        private void ProcGenderPregnantVal( Gender value )
        {
            if( value.Code == Gender.MALE_CODE ||
                value.Code == Gender.UNKNOWN_CODE )                     
            {
                if( SelectedAccount.Pregnant == null )
                {
                    SelectedAccount.Pregnant = new YesNoFlag();                        
                }
                
                if( Sex.Code == Gender.FEMALE_CODE )                        
                {
                    i_PreviousPregnantVal = (YesNoFlag)SelectedAccount.Pregnant.Clone();  
                }                    
 
                SelectedAccount.Pregnant.SetBlank( string.Empty );   
            }
            else
            {
                if( i_PreviousPregnantVal == null )
                {
                    i_PreviousPregnantVal = new YesNoFlag();
                    i_PreviousPregnantVal.SetBlank( string.Empty );
                }

                if( Sex.Code == Gender.MALE_CODE ||
                    Sex.Code == Gender.UNKNOWN_CODE )
                {
                    SelectedAccount.Pregnant = (YesNoFlag)i_PreviousPregnantVal.Clone();                    
                }
            }
        }

        private void FindSysGenSpan1(ArrayList sortedAccounts, SpanCode spanCode70, SpanCode spanCode71)
        {
            // When there is no prior System generated Qualifying Stay Date (70) 
            // and when SysGenSpan1 and SysGenSpan2 are not already assigned a 
            // Prior Stay Date (71) value, then generate Span 71 for SysGenSpan1, 
            // because there cannot be two 71 span codes for the same account.

            if ( !FindSpan70ForSysGenSpan1( sortedAccounts, spanCode70 ) &&
                 ( SysGenSpan1 == null ||  
                 ( SysGenSpan1 != null && !SysGenSpan1.SpanCode.IsPriorStayDates )) &&
                 ( SysGenSpan2 == null ||  
                 ( SysGenSpan2 != null && !SysGenSpan2.SpanCode.IsPriorStayDates )))
            {
                SysGenSpan1 = FindSpan71ForSysGenSpans(sortedAccounts, spanCode71);
            }
        }

        private void FindSysGenSpan2(ArrayList sortedAccounts, SpanCode spanCode71)
        {
            SysGenSpan2 = FindSpan71ForSysGenSpans(sortedAccounts, spanCode71);
        }

        private bool FindSpan70ForSysGenSpan1(ArrayList sortedAccounts, SpanCode spanCode70)
        {
            if (IsCurrentAccountValidForSpan70())
            {
                //using foreach causes error due to the collection change.
                for (int i = 0; i < sortedAccounts.Count; i++)
                {
                    IAccount proxy = ( IAccount )sortedAccounts[i];
                    if (IsPrevAccountValidForSpan70(proxy))
                    {
                        SysGenSpan1 = new OccurrenceSpan
                                          {
                                              SpanCode = spanCode70,
                                              FromDate = proxy.AdmitDate,
                                              ToDate = proxy.DischargeDate,
                                              Facility = proxy.Facility.Code,
                                              IsSystemGenerated = true
                                          };

                        sortedAccounts.Remove(proxy);
                        i -= 1;
                    }
                }

                return true;
            }
            
            return false;
        }

        private static OccurrenceSpan FindSpan71ForSysGenSpans(ArrayList sortedAccounts, SpanCode spanCode71)
        {
            IAccount proxy = ( IAccount )sortedAccounts[0];

            OccurrenceSpan sysGenSpan = new OccurrenceSpan
                                            {
                                                SpanCode = spanCode71,
                                                FromDate = proxy.AdmitDate,
                                                ToDate = proxy.DischargeDate,
                                                Facility = proxy.Facility.Code,
                                                IsSystemGenerated = true
                                            };

            sortedAccounts.Remove(proxy);

            return sysGenSpan;
        }

        private bool IsCurrentAccountValidForSpan70()
        {
            string primaryPlanID = string.Empty;

            if (SelectedAccount.Insurance.Coverages.Count > 0)
            {
                foreach (Coverage coverage in SelectedAccount.Insurance.Coverages)
                {
                    if (coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                    {
                        primaryPlanID = coverage.InsurancePlan.PlanID + " " + coverage.InsurancePlan.Payor.Name;
                        break;
                    }
                }
            }

            return IsPlanIDHSVValidForSpan70(primaryPlanID);
        }

        private bool IsPlanIDHSVValidForSpan70(string primaryPlanID)
        {
            if (IsPlanIdValidForSpan70(primaryPlanID) &&
                (SelectedAccount.HospitalService.Code == "95" ||
                SelectedAccount.HospitalService.Code == "96" ||
                SelectedAccount.HospitalService.Code == "97"))
            {
                return true;
            }
            
            return false;
        }


        private bool IsPrevAccountValidForSpan70(IAccount proxy)
        {
            string insurancePlanid = proxy.AsAccount().Insurance.CoverageFor(
                                     CoverageOrder.PRIMARY_OID).InsurancePlan.PlanID;
            if (IsPlanIDHSVValidForSpan70(insurancePlanid) &&
                proxy.DischargeDate.Date == SelectedAccount.AdmitDate.Date &&
                proxy.DischargeDate >= proxy.AdmitDate.AddDays(3))
            {
                return true;
            }
            
            return false;
        }

        private static bool IsPlanIdValidForSpan70(string primaryPlanID)
        {
            bool valid = false;

            if (primaryPlanID.Length >= 2)
            {
                if (primaryPlanID.Substring(0, 3).ToUpper().Equals(InsurancePlan.MSP_PAYORCODE))
                {
                    valid = true;
                }
                else if (primaryPlanID.Substring(0, 2) == "53")
                {
                    if (primaryPlanID.Length >= 3 && primaryPlanID.Substring(2, 1) == "5")
                    {
                        valid = true;
                    }
                }
            }

            return valid;
        }

        private static bool Passed60Days(DateTime dischargeDate)
        {
            if (DateTime.Now > dischargeDate.AddDays(60))
            {
                return true;
            }
            
            return false;
        }


        private void UpdateOccurrenceSpansInModel()
        {
            // Update System-generated Occurrence spans in Model only if current occurrence spans 
            // in the account are 70 or 71. If it is user-selected occurrence spans, like 74, do 
            // not remove it, but retain the same in the Model
            if (SelectedAccount.OccurrenceSpans.Count > 0)
            {
                OccurrenceSpan occurrenceSpan1 = (OccurrenceSpan)SelectedAccount.OccurrenceSpans[0];
                if ( occurrenceSpan1 == null ||
                     ( ( occurrenceSpan1.SpanCode.Code == String.Empty ||
                         occurrenceSpan1.SpanCode.IsPriorStayDates ||
                         occurrenceSpan1.SpanCode.IsQualifyingStayDate )))
                {
                    SelectedAccount.OccurrenceSpans.RemoveAt(0);
                    SelectedAccount.OccurrenceSpans.Insert(0, SysGenSpan1);
                }
            }
            else   // if there are no existing occurrence spans
            {
                SelectedAccount.OccurrenceSpans.Insert(0, SysGenSpan1);
            }

            if (SelectedAccount.OccurrenceSpans.Count > 1)
            {
                OccurrenceSpan occurrenceSpan2 = (OccurrenceSpan)SelectedAccount.OccurrenceSpans[1];
                if ( occurrenceSpan2 == null ||
                    ( ( occurrenceSpan2.SpanCode.Code == String.Empty ||
                        occurrenceSpan2.SpanCode.IsPriorStayDates ||
                        occurrenceSpan2.SpanCode.IsQualifyingStayDate )))
                {
                    SelectedAccount.OccurrenceSpans.RemoveAt(1);
                    SelectedAccount.OccurrenceSpans.Add(SysGenSpan2);
                }
            }
            else   // if there are no existing occurrence spans
            {
                SelectedAccount.OccurrenceSpans.Add(SysGenSpan2);
            }
        }

        private void SetRace3ToBlank()
        {
            Race3 = new Race();
        }
        private void SetRace4ToBlank()
        {
            Race4 = new Race();
        }

        private void SetRace5ToBlank()
        {
            Race5 = new Race();
        }

        #endregion

        #region Private Properties
        protected ArrayList PrimAccounts
        {
            get
            {
                return i_Accounts;
            }
            set
            {
                i_Accounts = value;
            }
        }
        protected ArrayList PrimAccountsWithPaymentPlan
        {
            get
            {
                return i_AccountsWithPaymentPlan;
            }
            set
            {
                i_AccountsWithPaymentPlan = value;
            }
        }
        protected ArrayList PrimAccountsWithNoPaymentPlan
        {
            get
            {
                return i_AccountsWithNoPaymentPlan;
            }
            set
            {
                i_AccountsWithNoPaymentPlan = value;
            }
        }
        public string OtherLanguage
        {
            get;
            set;
        }
        #endregion

        #region Construction and Finalization
        public Patient( long oid, DateTime version, string firstName, string lastName, string middleInitial,string  suffix , long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : base( oid, version, firstName, lastName, middleInitial,suffix , medicalRecordNumber, dateOfBirth, ssn, sex, facility )
        {
            BirthSex = new Gender();
            Sex = sex;
        }
        public Patient( long oid, DateTime version, string firstName, string lastName, string middleInitial, long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : base( oid, version, firstName, lastName, middleInitial, medicalRecordNumber, dateOfBirth, ssn, sex, facility )
        {
            BirthSex = new Gender();
            Sex = sex;
        }

        public Patient( long oid, DateTime version, Name patientsName, long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : this( oid, version, patientsName.FirstName, patientsName.LastName, patientsName.MiddleInitial,patientsName.Suffix, medicalRecordNumber, dateOfBirth, ssn, sex, facility )
        {
            Sex = sex;
        }

        public Patient( AbstractPatient aPatient )
            : this( aPatient.Oid, aPatient.Timestamp, aPatient.Name, aPatient.MedicalRecordNumber, aPatient.DateOfBirth, aPatient.SocialSecurityNumber, aPatient.Sex, aPatient.Facility )
        {
            foreach( Name name in aPatient.Aliases )
            {
                AddAlias( name );
            }
            Sex = aPatient.Sex;
        }
        
        public Patient( long oid, DateTime version, string firstName, string lastName, string middleInitial, long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility, string bloodlessPatient )
            : base( oid, version, firstName, lastName, middleInitial, medicalRecordNumber, dateOfBirth, ssn, sex, facility )
        {
            BirthSex = new Gender();
            i_bloodlessPatient              = bloodlessPatient;
            Sex                      = sex;
        }


        public Patient( long oid, DateTime version, string firstName, string lastName, string middleInitial, DateTime dateOfBirth, Gender sex, Religion religion, PlaceOfWorship placeOfWorship )
        {
            BirthSex = new Gender();
            Oid                        = oid;
            Timestamp                  = version;
            base.FirstName                  = firstName;
            base.LastName                   = lastName;
            MiddleInitial              = middleInitial;
            DateOfBirth                = dateOfBirth;
            i_Religion                 = religion;        	
            i_PlaceOfWorship           = placeOfWorship;        	
        }

        public Patient()
        {
            BirthSex = new Gender();
        }
        #endregion

        #region Data Elements

        private ArrayList           i_Accounts								= new ArrayList();
        private ArrayList           i_AccountsWithPaymentPlan				=  new ArrayList();
        private ArrayList           i_AccountsWithNoPaymentPlan				=  new ArrayList();
        private readonly ArrayList  i_AccountsWithNoPaymentPlanWithPayments =  new ArrayList();

        private MaritalStatus       i_MaritalStatus							= new MaritalStatus();
        private Race                i_Race									= new Race();
        private Race                i_Race2                                 = new Race();
        private Race                i_Race3                                 = new Race();
        private Race                i_Race4                                 = new Race();
        private Race                i_Race5                                 = new Race();
        private Race                i_Nationality                           = new Race();
        private Race                i_Nationality2                          = new Race();
        private Ethnicity           i_Ethnicity								= new Ethnicity();
        private Ethnicity           i_Ethnicity2                            = new Ethnicity();
        private Ethnicity           i_Descent                               = new Ethnicity();
        private Ethnicity           i_Descent2                              = new Ethnicity();

        private Language            i_Language								= new Language();
        private Religion            i_Religion								= new Religion();
        private PlaceOfWorship      i_PlaceOfWorship						= new PlaceOfWorship();
        private YesNoFlag           i_DoNotResuscitate						= new YesNoFlag();
        private YesNoFlag           i_PreviousPregnantVal;
        private Account             i_SelectedAccount;
        private Account             i_MothersAccount;

        private int                 i_PreviousMRN;
        private string              i_PlaceOfBirth							= string.Empty;
        private string              i_MaidenName							= String.Empty;
        private string              i_MothersName							= String.Empty;
        private string              i_FathersName							= String.Empty;       
        private DateTime            i_LastUpdated;
        private string 	            i_bloodlessPatient						= string.Empty;
        private bool                i_IsNew;
        private DateTime            i_MothersDateOfBirth;
        private DateTime            i_FathersDateOfBirth;

        private OccurrenceSpan      i_SysGenSpan1;
        private OccurrenceSpan      i_SysGenSpan2;
        private NoticeOfPrivacyPracticeDocument i_NPPDocument               = new NoticeOfPrivacyPracticeDocument();
        private YesNoFlag i_HospitalCommunicationOptIn = new YesNoFlag();
        private bool i_SequesteredPatientAlertShown = false;
        private InterFacilityTransferAccount i_InterfacilityTransferAccount=new InterFacilityTransferAccount();

        #endregion

        #region Constants

        #endregion
    }

    class SortAccountsByDischarge : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            IAccount a = ( IAccount )obj1;
            IAccount b = ( IAccount )obj2;

            return a.DischargeDate.CompareTo(b.DischargeDate);
        }
    }
}
