using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using Extensions.PersistenceCommon;
using Extensions.UI.Builder;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for AccountProxy.
    /// </summary>
    [Serializable]
    public class AccountProxy : IAccount
    {
        #region Event Handlers

        #endregion

        #region Methods

        public Account AsAccount()
        {
            // OTD# 37233 fix - Commenting out the following 2 lines of code since clearing the other 
            // AccountProxies on the Proxy's patient was causing undesired results in the BillingView 
            // Occurrence Spans. Not sure why these accounts were removed from the Patient in the first place.

            // Clear the other AccountProxies loaded on this Proxy's Patient.
            Account result = null;
            var broker = BrokerFactory.BrokerOfType<IAccountBroker>();
            result = broker.AccountFor(this);
            result.ActionsLoader = new ActionLoader(result);

            // set the account in the occurrence code manager
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;
            ocm.OccurrenceCodesLoader = new OccurrenceCodesLoader();
            ocm.Account = result;
            return result;
        }

        public Account AsAccount(Activity activity)
        {
            // OTD# 37233 fix - Commenting out the following 2 lines of code since clearing the other 
            // AccountProxies on the Proxy's patient was causing undesired results in the BillingView 
            // Occurrence Spans. Not sure why these accounts were removed from the Patient in the first place.

            // Clear the other AccountProxies loaded on this Proxy's Patient.
            Account result = null;
            var broker = BrokerFactory.BrokerOfType<IAccountBroker>();
            result = broker.AccountFor( this, activity );
            result.ActionsLoader = new ActionLoader( result );

            // set the account in the occurrence code manager
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;
            ocm.OccurrenceCodesLoader = new OccurrenceCodesLoader();
            ocm.Account = result;
            return result;
        }
        /// <summary>
        /// Returns all occurences of a physician relationship matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A collection of all physician relationships matching the physician role</returns>
        public ICollection PhysicianRelationshipsWith(PhysicianRole aRole)
        {
            var physicianRelationships = new ArrayList();

            foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
            {
                if (aRelationship.PhysicianRole.Role().Equals(aRole))
                {
                    physicianRelationships.Add(aRelationship);
                }
            }
            return physicianRelationships;
        }

        /// <summary>
        /// Returns all occurences of physicians matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A collection of all physicians  matching the physician role</returns>
        public ICollection PhysiciansWith(PhysicianRole aRole)
        {
            var physicians = new ArrayList();

            foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
            {
                if (aRelationship.PhysicianRole.Role().Equals(aRole))
                {
                    physicians.Add(aRelationship.Physician);
                }
            }

            return physicians;
        }

        /// <summary>
        /// Adds a physician relationship to the collection of physician relationships on the proxy.
        /// If a physician relationship exists in the collection with the identical physician role 
        /// being added, then that physician relationship will be replaced by the physician relationship
        /// being added.  Consulting physician relationships will allow a maximum of 5 relationships before 
        /// recycling a relationship.
        /// </summary>
        /// <param name="aPhysicianRelationship"></param>
        public void AddPhysicianRelationship(PhysicianRelationship aPhysicianRelationship)
        {
            //Question:  When adding a new physician do we replace any existing physician
            //with that same role?
            //Answer 08/09/2005 from Vic and Drew:  Yes, with no warning message!
            if (IsConsultingPhysician(aPhysicianRelationship))
            {
                AddConsultingPhysicianRelationship(aPhysicianRelationship);
            }
            else
            {
                AddNonConsultingPhysicianRelationship(aPhysicianRelationship);
            }
        }

        /// <summary>
        /// Encapsulates a physician role and a physician into a physician relationship and then adds
        /// this relationship to the collection of all physician relationships.
        /// </summary>
        /// <param name="aRole"></param>
        /// <param name="aPhysician"></param>
        public void AddPhysicianWithRole(PhysicianRole aRole, Physician aPhysician)
        {
            var aPhysicianRelationship = new PhysicianRelationship(aRole, aPhysician);
            AddPhysicianRelationship(aPhysicianRelationship);
        }

        // Determines if the Patient type is disabled or enabled for the Activity and Visit Type.
        public bool IsPatientTypeChangeable()
        {
            bool isPatientTypeChangeableForThisActivity = Activity.CanPatientTypeChange();
            //Filtering out maintenace activity here as the  dependency between 
            //Activity and KindOfVisit is only for maintenance activity

            if (!(Activity is MaintenanceActivity))
            {
                return isPatientTypeChangeableForThisActivity;
            }
            else
            {
                if (KindOfVisit != null)
                {
                    return KindOfVisit.IsPatientTypeChangeableFor(Location) && isPatientTypeChangeableForThisActivity;
                }
                return true;
            }
        }

        // An account is routed to the 8-tab view on Edit/Maintain if the account:
        // a) was Short-Registered or Short-PreRegistered with the Short-Reg flag set to 'Y' in PBAR (and)
        // b) is of PatientType - Outpatient (or) Recurring Patient (or) Prereg patient (and)
        // c) its hospital service code is not that of a bedded Outpatient.
        public bool IsShortRegisteredNonDayCareAccount()
        {
            return ( IsShortRegistered && KindOfVisit != null &&
                     ( KindOfVisit.Code == VisitType.OUTPATIENT ||
                       KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                       KindOfVisit.Code == VisitType.PREREG_PATIENT ) &&
                     HospitalService != null && !HospitalService.IsDayCare()
                   );
        }
        // An account is routed to the 8-tab view on Edit/Maintain if the account:
        // a) was Short-Registered or Short-PreRegistered with the Short-Reg flag set to 'Y' in PBAR (and)
        // b) is of PatientType - Outpatient (or) Recurring Patient (or) Prereg patient (and)
        // c) its hospital service code is not that of a bedded Outpatient.
        public bool IsQuickPreRegAccount()
        {
            return ( IsQuickRegistered && KindOfVisit != null &&
                     (  KindOfVisit.Code == VisitType.PREREG_PATIENT )  
                   );
        }

        // An account is routed to the PAI walkin screen on Edit/Maintain if the account:
        // a) was PAI walkin registred with the PAI_walkin flag set to 'Y' in PBAR
        public bool IsPAIWalkinRegisteredAccount()
        {
            return (IsPAIWalkinRegistered && KindOfVisit != null &&
                     (KindOfVisit.Code == VisitType.PREREG_PATIENT)
                   );
        }

        public bool IsCanceledPreRegistration
        {
            get { return DerivedVisitType == Account.PRE_CAN; }
        }

        public string CalculatedAmountDue()
        {
            string displayText = string.Empty;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            if (insuredFinancialCodeCheck.IndexOf(FinancialClass.Code) > 0)

            {
                if ((AmountDue == 0) && (PrimaryNoLiability != true))
                {
                    displayText = "Due:     " + "Not determined";
                }
                else if ((AmountDue > 0) && (PrimaryNoLiability != true))
                {
                    AmountDues = AmountDue;
                    displayText = "Due:     " + AmountDues.ToString("C");
                }
                else if (PrimaryNoLiability)
                {
                    displayText = "Due:     " + "No liability";
                }
            }
            else
            {
                displayText = "Due:     " + "Not applicable";
            }

            return displayText;
        }

        public void AddHospitalClinic(HospitalClinic hospitalClinics)
        {
            i_HospitalClinics.Add(hospitalClinics);
        }

        /// <summary>
        /// Get a count of all remainingActions
        /// </summary>
        /// <returns></returns>
        public long GetCountOfAllRemainingActions()
        {
            long count = 0;
            foreach (DictionaryEntry entry in RemainingActions)
            {
                foreach (IAction action in entry.Value as ActionsList)
                {
                    if (action is CompositeAction)
                    {
                        var ca = action as CompositeAction;
                        count += ca.NumberOfAllLeafActions();
                    }
                    else // it is a leaf action
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Get a list of All RemainingAction from all worklists
        /// into a single list
        /// </summary>
        /// <returns></returns>
        public ICollection GetAllRemainingActions()
        {
            var result = new ArrayList();
            foreach (DictionaryEntry entry in RemainingActions)
            {
                foreach (IAction action in entry.Value as ActionsList)
                {
                    if (! result.Contains(action))
                    {
                        result.Add(action);
                    }
                }
            }

            return result as ICollection;
        }

        public object GetRemainingActionsFor(Worklist worklist)
        {
            long wlid = worklist.Oid;
            object rActions = ((Hashtable) RemainingActions)[wlid];
            return rActions;
        }

        public string AddOnlyLegends()
        {
            string legend = String.Empty;
            if (OptOutOnName)
            {
                legend = "nhlr";
            }
            else
            {
                if (OptOutOnHealthInformation)
                {
                    legend = legend + "h";
                }
                if (OptOutOnLocation)
                {
                    legend = legend + "l";
                }
                if (OptOutOnReligion)
                {
                    legend = legend + "r";
                }
            }

            if (!legend.Equals(String.Empty))
            {
                legend = "[" + legend + "]";
            }

            if (!Confidential.Trim().Equals(String.Empty))
            {
                legend = "*" + legend;
            }

            return legend;
        }

        /// <summary>
        /// Returns the first occurence of a physician relationship matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A physician relationship matching the physician role</returns>
        public PhysicianRelationship PhysicianRelationshipWithRole(PhysicianRole aRole)
        {
            PhysicianRelationship aPhysicianRelationship = null;

            foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
            {
                if (aRelationship.PhysicianRole.Role().Equals(aRole))
                {
                    aPhysicianRelationship = aRelationship;
                    break;
                }
            }

            return aPhysicianRelationship;
        }

        private void RemovePhysicianRelationship(PhysicianRelationship aPhysicianRelationship)
        {
            try
            {
                int index = 0;
                foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
                {
                    if (aRelationship.PhysicianRole.Role().Equals(aPhysicianRelationship.PhysicianRole.Role()))
                    {
                        index = ((ArrayList) AllPhysicianRelationships).IndexOf(aRelationship);
                        ((ArrayList) AllPhysicianRelationships).RemoveAt(index);
                        break;
                    }
                }
            }
            catch (NotSupportedException nse)
            {
                Console.WriteLine(nse.ToString());
                throw;
            }
        }

        public bool IsUrgentCarePreMse
        {
            get
            {
                return KindOfVisit != null &&
                       KindOfVisit.Code == VisitType.OUTPATIENT &&
                       FinancialClass != null &&
                       FinancialClass.IsMedScreenExam();
            }
        }

        public void SetUCCVisitType()
        {
            if (KindOfVisit != null && KindOfVisit.Code == VisitType.OUTPATIENT &&
                FinancialClass != null && FinancialClass.IsMedScreenExam())
            {
                KindOfVisit = VisitType.UCCOutpatient;
            }
        }

        #endregion

        #region Properties

        public IValueLoader ActionsLoader
        {
            set { i_ActionHolder = new ValueHolder(value); }
        }

        public string ConfidentialFlag
        {
            get { return i_ConfidentialFlag; }
            set { i_ConfidentialFlag = value; }
        }

        public string MultiSiteFlag
        {
            get { return i_MultiSiteFlag; }
            set { i_MultiSiteFlag = value; }
        }

        public string SiteCode
        {
            get { return i_SiteCode; }
            set { i_SiteCode = value; }
        }

        public string PendingDischarge
        {
            get { return i_PendingDischarge; }
            set { i_PendingDischarge = value; }
        }

        public DischargeStatus DischargeStatus
        {
            get { return i_DischargeStatus; }
            set { i_DischargeStatus = value; }
        }

        public DischargeDisposition DischargeDisposition
        {
            get { return i_DischargeDisposition; }
            set { i_DischargeDisposition = value; }
        }

        public bool AbstractExists
        {
            get { return i_AbstractExists; }
            set { i_AbstractExists = value; }
        }

        public Location Location
        {
            get { return i_Location; }
            set { i_Location = value; }
        }

        public string IsolationCode
        {
            get { return i_IsolationCode; }
            set { i_IsolationCode = value; }
        }

        public AdmitSource AdmitSource
        {
            get { return i_AdmitSource; }
            set { i_AdmitSource = value; }
        }

        public DateTime LastUpdated
        {
            get { return i_LastUpdated; }
            set { i_LastUpdated = value; }
        }

        public HospitalClinic HospitalClinic
        {
            get { return i_HospitalClinic; }
            set { i_HospitalClinic = value; }
        }

        public ArrayList HospitalClinics
        {
            get { return i_HospitalClinics; }
        }

        public MedicalGroupIPA MedicalGroupIPA
        {
            get { return i_MedicalGroupIPA; }
            set { i_MedicalGroupIPA = value; }
        }

        public string SeenInED
        {
            get { return i_SeenInED; }
            set
            {
                i_SeenInED = value;
                if (i_SeenInED.Trim().Length > 0)
                {
                    i_SeenInED = "N";
                }
            }
        }

        public NursingStation NursingStation
        {
            get { return i_NursingStation; }
            set { i_NursingStation = value; }
        }

        public RoomCondition RoomCondition
        {
            get { return i_RoomCondition; }
            set { i_RoomCondition = value; }
        }

        public string Overflow
        {
            get { return i_Overflow; }
            set { i_Overflow = value; }
        }

        public long ActionCount
        {
            get { return i_ActionCount; }
            set { i_ActionCount = value; }
        }


        public string PhysicianRelationship
        {
            get { return i_Relationship; }
            set { i_Relationship = value; }
        }

        public Diagnosis Diagnosis
        {
            get { return i_Diagnosis; }
            set { i_Diagnosis = value; }
        }

        public string PendingAdmission
        {
            get { return i_PendingAdmission; }
            set { i_PendingAdmission = value; }
        }

        public string Confidential
        {
            get { return i_Confidential; }
            set { i_Confidential = value; }
        }

        public string OptOutInformation
        {
            set
            {
                if (!value.Trim().Equals(String.Empty))
                {
                    SetOptOutInformation(value);
                }
            }
        }

        public bool OptOutOnName
        {
            get { return i_OptOutOnName; }
        }

        public bool OptOutOnLocation
        {
            get { return i_OptOutOnLocation; }
        }

        private bool OptOutOnHealthInformation
        {
            get { return i_OptOutOnHealthInformation; }
        }

        public bool OptOutOnReligion
        {
            get { return i_OptOutOnReligion; }
        }

        public Location LocationFrom
        {
            get { return i_LocationFrom; }
            set { i_LocationFrom = value; }
        }

        public Location LocationTo
        {
            get { return i_LocationTo; }
            set { i_LocationTo = value; }
        }

        public string TransactionTime
        {
            get { return i_TransactionTime; }
            set { i_TransactionTime = value; }
        }

        public string TransactionType
        {
            get { return i_TransactionType; }
            set { i_TransactionType = value; }
        }

        public long LengthOfStay
        {
            get { return i_LengthOfStay; }
            set { i_LengthOfStay = value; }
        }

        public decimal AmountDue
        {
            get { return i_AmountDue; }
            set { i_AmountDue = value; }
        }

        public decimal Payments
        {
            get { return i_Payments; }
            set { i_Payments = value; }
        }

        public string PayorName
        {
            get { return i_PayorName; }
            set { i_PayorName = value; }
        }

        public string SecondaryPlanName
        {
            get { return i_SecondaryPlanName; }
            set { i_SecondaryPlanName = value; }
        }

        public string SecondaryPlan
        {
            get { return i_SecondaryPlan; }
            set { i_SecondaryPlan = value; }
        }

        public string PrimaryInsurancePlan
        {
            get { return i_PrimaryInsurancePlan; }
            set { i_PrimaryInsurancePlan = value; }
        }

        public bool PrimaryNoLiability
        {
            private get { return i_PrimaryNoLiability; }
            set { i_PrimaryNoLiability = value; }
        }

        public string PrimaryPlanName
        {
            get { return i_PrimaryPlanName; }
            set { i_PrimaryPlanName = value; }
        }

        public YesNoFlag Bloodless
        {
            get { return i_Bloodless; }
            set { i_Bloodless = value; }
        }

        public YesNoFlag ClergyVisit
        {
            get { return i_ClergyVisit; }
            set { i_ClergyVisit = value; }
        }

        public long LastMaintenanceDate
        {
            get { return i_LastMaintenanceDate; }
            set { i_LastMaintenanceDate = value; }
        }

        public long LastMaintenanceLogNumber
        {
            get { return i_LastMaintenanceLogNumber; }
            set { i_LastMaintenanceLogNumber = value; }
        }

        public long UpdateLogNumber
        {
            get { return i_UpdateLogNumber; }
            set { i_UpdateLogNumber = value; }
        }

        public YesNoFlag IsPurged
        {
            get { return i_IsPurged; }
            set { i_IsPurged = value; }
        }

        public bool HasPaymentPlan
        {
            get { return i_HasPaymentPlan; }
            set { i_HasPaymentPlan = value; }
        }

        public string PreDischargeFlag
        {
            get { return i_PreDischargeFlag; }
            set { i_PreDischargeFlag = value; }
        }

        public DateTime AccountCreatedDate
        {
            get { return i_AccountCreatedDate; }
            set { i_AccountCreatedDate = value; }
        }

        public long AccountNumber
        {
            get { return i_AccountNumber; }
            set { i_AccountNumber = value; }
        }

        public DateTime AdmitDate
        {
            get { return i_AdmitDate; }
            set { i_AdmitDate = value; }
        }

        public DateTime DischargeDate
        {
            get { return i_DischargeDate; }
            set { i_DischargeDate = value; }
        }

        public YesNoFlag ValuablesAreTaken
        {
            get { return i_ValuablesAreTaken; }
            set { i_ValuablesAreTaken = value; }
        }

        public Facility Facility
        {
            get { return i_Facility; }
            set { i_Facility = value; }
        }

        public FinancialClass FinancialClass
        {
            get { return i_FinancialClass; }
            set { i_FinancialClass = value; }
        }

        public HospitalService HospitalService
        {
            get { return i_HospitalService; }
            set { i_HospitalService = value; }
        }

        public VisitType KindOfVisit
        {
            get { return i_KindOfVisit; }
            set { i_KindOfVisit = value; }
        }

        public string DerivedVisitType
        {
            get { return i_DerivedVisitType; }
            set { i_DerivedVisitType = value; }
        }

        public YesNoFlag IsPatientInClinicalResearchStudy { get; set; }

        public bool IsLocked
        {
            get { return AccountLock.IsLocked; }
            set { i_IsLocked = value; }
        }

        public AccountLock AccountLock
        {
            get { return i_AccountLock; }
            set { i_AccountLock = value; }
        }

        public Patient Patient
        {
            get { return i_Patient; }
            set { i_Patient = value; }
        }

        public decimal BalanceDue
        {
            get { return i_BalanceDue; }
            set { i_BalanceDue = value; }
        }

        public Clinic Clinic
        {
            get { return i_Clinic; }
            set { i_Clinic = value; }
        }

        public string PrimaryPayor
        {
            get { return i_PrimaryPayor; }
            set { i_PrimaryPayor = value; }
        }

        public ArrayList ConsultingPhysicians
        {
            get
            {
                var result = new ArrayList();

                foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
                {
                    if (aRelationship.PhysicianRole.Role().Equals(PhysicianRole.Consulting().Role()))
                    {
                        result.Add(aRelationship.Physician);
                    }
                }
                return result;
            }
        }

        public Physician AdmittingPhysician
        {
            get { return PhysicianWithRole(PhysicianRole.Admitting().Role()); }
        }

        public Physician AttendingPhysician
        {
            get { return PhysicianWithRole(PhysicianRole.Attending().Role()); }
        }

        public Physician ReferringPhysician
        {
            get { return PhysicianWithRole(PhysicianRole.Referring().Role()); }
        }

        public Physician OperatingPhysician
        {
            get { return PhysicianWithRole(PhysicianRole.Operating().Role()); }
        }

        public Physician PrimaryCarePhysician
        {
            get { return PhysicianWithRole(PhysicianRole.PrimaryCare().Role()); }
        }

        public ICollection AllPhysicianRelationships
        {
            get { return i_AllPhysicianRelationships; }
            set { i_AllPhysicianRelationships = value; }
        }

        public bool IsCanceled
        {
            get { return i_IsCanceled; }
            set { i_IsCanceled = value; }
        }

        public bool IsNew
        {
            get { return i_IsNew; }
            set { i_IsNew = value; }
        }

        public bool IsShortRegistered
        {
            get { return i_IsShortRegistered; }
            set { i_IsShortRegistered = value; }
        }
        public bool IsQuickRegistered
        {
            get { return i_IsQuickRegistered; }
            set { i_IsQuickRegistered = value; }
        }

        public bool IsPAIWalkinRegistered
        {
            get { return i_IsPAIWalkinRegistered; }
            set { i_IsPAIWalkinRegistered = value; }
        }

        public bool IsNewBorn { get; set; }
        public bool BirthTimeIsEntered { get; set; }
        public string AdmittingCategory { get; set; }

        public Activity Activity
        {
            get { return i_Activity; }
            set { i_Activity = value; }
        }

        public IList ConditionCodes
        {
            get { return (IList) i_ConditionCodes; }
        }

        #endregion

        #region Private Methods

        private ICollection RemainingActions
        {
            get
            {
                if (i_RemainingActions == null)
                {
                    i_RemainingActions = (Hashtable) i_ActionHolder.GetValue();
                }
                return i_RemainingActions;
            }
        }

        public YesNoFlag PatientPortalOptIn { get; set; }
        public YesNoFlag HospitalCommunicationoptin { get; set; }

        private void SetOptOutInformation(string optOut)
        {
            i_OptOutOnName = false;
            i_OptOutOnLocation = false;
            i_OptOutOnHealthInformation = false;
            i_OptOutOnReligion = false;
            if (optOut.Substring(0, 1).Equals("Y"))
            {
                if (optOut.Length == 5)
                {
                    if (optOut.Substring(1, 1).Equals("N"))
                    {
                        i_OptOutOnName = true;
                        i_OptOutOnLocation = true;
                        i_OptOutOnHealthInformation = true;
                        i_OptOutOnReligion = true;
                    }
                    else
                    {
                        if (optOut.Substring(2, 1).Equals("N"))
                        {
                            i_OptOutOnLocation = true;
                        }
                        if (optOut.Substring(3, 1).Equals("N"))
                        {
                            i_OptOutOnHealthInformation = true;
                        }
                        if (optOut.Substring(4, 1).Equals("N"))
                        {
                            i_OptOutOnReligion = true;
                        }
                    }
                }
            }
        }

        private Physician PhysicianWithRole(PhysicianRole aRole)
        {
            Physician result = null;

            foreach (PhysicianRelationship aRelationship in AllPhysicianRelationships)
            {
                if (aRelationship.PhysicianRole.Role().Equals(aRole))
                {
                    result = aRelationship.Physician;
                    break;
                }
            }

            return result;
        }

        private void AddConsultingPhysicianRelationship(PhysicianRelationship aRelationship)
        {
            if (IsAtMaxConsultingPhysicians())
            {
                RemovePhysicianRelationship(aRelationship);
            }

            ((ArrayList) AllPhysicianRelationships).Add(aRelationship);
        }

        private void AddNonConsultingPhysicianRelationship(PhysicianRelationship aRelationship)
        {
            if (PhysicianWithRole(aRelationship.PhysicianRole.Role()) != null)
            {
                RemovePhysicianRelationship(aRelationship);
            }

            ((ArrayList) AllPhysicianRelationships).Add(aRelationship);
        }

        private bool IsAtMaxConsultingPhysicians()
        {
            return ConsultingPhysicians.Count >= MAX_NUMBER_OF_CONSULTING_PHYSICIANS;
        }

        private bool IsConsultingPhysician(PhysicianRelationship aPhysicianRelationship)
        {
            return aPhysicianRelationship.PhysicianRole.Role().Equals(PhysicianRole.Consulting().Role());
        }

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public AccountProxy()
        {
        }

        public AccountProxy(long accountNumber, Patient thePatient, DateTime admitDate, DateTime dischargeDate,
                            VisitType kindOfVisit, Facility facility, FinancialClass financialClass,
                            HospitalService hospitalService, string derivedVisitType, bool isLocked)
        {
            i_Patient = thePatient;
            i_AccountNumber = accountNumber;
            i_AdmitDate = admitDate;
            i_DischargeDate = dischargeDate;
            i_KindOfVisit = kindOfVisit;
            i_Facility = facility;
            i_FinancialClass = financialClass;
            i_HospitalService = hospitalService;
            i_DerivedVisitType = derivedVisitType;
            i_IsLocked = isLocked;
        }

        public AccountProxy(long accountNumber, Patient thePatient, DateTime admitDate, DateTime dischargeDate,
                            VisitType kindOfVisit, Facility facility, FinancialClass financialClass,
                            HospitalService hospitalService, string derivedVisitType, bool isLocked,
                            Clinic aClinic, HospitalClinic hospitalClinic)
        {
            i_Patient = thePatient;
            i_AccountNumber = accountNumber;
            i_AdmitDate = admitDate;
            i_DischargeDate = dischargeDate;
            i_KindOfVisit = kindOfVisit;
            i_Facility = facility;
            i_FinancialClass = financialClass;
            i_HospitalService = hospitalService;
            i_DerivedVisitType = derivedVisitType;
            i_IsLocked = isLocked;
            i_Clinic = aClinic;
            i_HospitalClinic = hospitalClinic;
        }

        #endregion

        #region Data Elements

        private decimal AmountDues;
        private bool i_AbstractExists;
        private DateTime i_AccountCreatedDate = new DateTime();
        private AccountLock i_AccountLock = new AccountLock();
        private long i_AccountNumber;
        private long i_ActionCount;
        private ValueHolder i_ActionHolder;
        private Activity i_Activity = null;
        private DateTime i_AdmitDate;
        private AdmitSource i_AdmitSource;
        private ICollection i_AllPhysicianRelationships = new ArrayList();
        private decimal i_AmountDue;
        private decimal i_BalanceDue;
        private YesNoFlag i_Bloodless;
        private YesNoFlag i_ClergyVisit = new YesNoFlag();
        private Clinic i_Clinic;
        private ArrayList i_ConditionCodes = new ArrayList();
        private string i_Confidential;
        private string i_ConfidentialFlag;
        private string i_DerivedVisitType;
        private Diagnosis i_Diagnosis;
        private DateTime i_DischargeDate;
        private DischargeDisposition i_DischargeDisposition;
        private DischargeStatus i_DischargeStatus;
        private Facility i_Facility;
        private FinancialClass i_FinancialClass;
        private bool i_HasPaymentPlan = false;
        private HospitalClinic i_HospitalClinic = new HospitalClinic();
        private ArrayList i_HospitalClinics = new ArrayList();
        private HospitalService i_HospitalService;
        private bool i_IsCanceled = false;
        private bool i_IsLocked;
        private bool i_IsNew = false;
        private string i_IsolationCode;
        private YesNoFlag i_IsPurged;
        private VisitType i_KindOfVisit;
        private long i_LastMaintenanceDate;
        private long i_LastMaintenanceLogNumber;
        private DateTime i_LastUpdated;
        private long i_LengthOfStay;
        private Location i_Location;
        private Location i_LocationFrom;
        private Location i_LocationTo;
        private MedicalGroupIPA i_MedicalGroupIPA;
        private string i_MultiSiteFlag;
        private NursingStation i_NursingStation;
        private bool i_OptOutOnHealthInformation = false;
        private bool i_OptOutOnLocation = false;
        private bool i_OptOutOnName = false;
        private bool i_OptOutOnReligion = false;
        private string i_Overflow;
        private Patient i_Patient = new Patient();
        private decimal i_Payments;
        private string i_PayorName;
        private string i_PendingAdmission;
        private string i_PendingDischarge;
        private string i_PreDischargeFlag = string.Empty;
        private string i_PrimaryInsurancePlan;
        private bool i_PrimaryNoLiability;
        private string i_PrimaryPayor;
        private string i_PrimaryPlanName;
        private string i_Relationship;
        private Hashtable i_RemainingActions;
        private RoomCondition i_RoomCondition;
        private string i_SecondaryPlan;
        private string i_SecondaryPlanName;
        private string i_SeenInED;
        private string i_SiteCode;
        private string i_TransactionTime;
        private string i_TransactionType;
        private long i_UpdateLogNumber;
        private YesNoFlag i_ValuablesAreTaken = new YesNoFlag();
        private bool i_IsShortRegistered;
        private bool i_IsQuickRegistered;
        private bool i_IsPAIWalkinRegistered;

        #endregion

        #region Constants

        private const string insuredFinancialCodeCheck
            = "02,04,05,08,13,16,17,18,20,22,23,25,26,29,40, " +
              "44,48,50,54,55,70,72,73,80,81,84,85,87,96";

        private const int
            MAX_NUMBER_OF_CONSULTING_PHYSICIANS = 5,
            PHYSICIAN_AT_ZERO = 0;

        #endregion
    }
}