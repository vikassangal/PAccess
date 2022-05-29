using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Rules;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Facility : CodedReferenceValue, IFacility
    {

        #region Event Handlers
        #endregion

        #region Methods

        public void AddContactPoint( ContactPoint contactPoint )
        {
            i_ContactPoints.Add( contactPoint );
        }

        /// <summary>
        /// Similar to CodedReferenceValue's ToCodedString, but inserts a hyphen between
        /// the code and the description.
        /// </summary>
        /// <returns></returns>
        public override string ToCodedString()
        {
            return String.Format( "{0} - {1}", base.Code, base.Description );
        }

        public ContactPoint GetContactPoint( TypeOfContactPoint aTypeOfContactPoint )
        {
            ContactPoint contactPoint = null;
            foreach ( ContactPoint contact in ContactPoints )
            {
                if ( contact != null
                    && contact.TypeOfContactPoint.Oid == aTypeOfContactPoint.Oid )
                {
                    contactPoint = contact;
                    break;
                }
            }
            return contactPoint;
        }

        public DateTime GetCurrentDateTime()
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();

            return timeBroker.TimeAt( GMTOffset, DSTOffset );
        }

        public bool IsFacilityInState( string stateCode )
        {
            bool facilityIsInState = false;

            if ( !String.IsNullOrEmpty( stateCode ) &&
                 !String.IsNullOrEmpty( FacilityStateCode ) )
            {
                facilityIsInState = ( FacilityStateCode == stateCode );
            }

            return facilityIsInState;
        }

            public void SetFacilityStateCode()
        {
            if ( ContactPoints != null && ContactPoints.Count > 0 )
            {
                foreach ( ContactPoint cp in ContactPoints )
                {
                    if ( cp != null
                        && cp.TypeOfContactPoint.Equals( TypeOfContactPoint.NewPhysicalContactPointType() )
                        && cp.Address != null )
                    {
                        FacilityState = cp.Address.State;
                    }
                }
            }
        }

        public bool IsValidForClinicalResearchFields()
        {
           //This extended property was added so that divested hopitals could show clinical trials related fields.
           //Later on a different clinical trials feature was added to the non-divested hospitals only. 
           //These two features are mutually exclusive, here the flag is being used to filter out the divested hospitals.
           
            return this["IsFacilityClinicalTrialEnabled"] == null;
        }

        public State GetPersonState ()
        {
            State state = new State(); // instance required in case no physical contact point found
            foreach ( ContactPoint cp in ContactPoints )
            {
                if (cp != null && cp.TypeOfContactPoint.Equals( TypeOfContactPoint.NewPhysicalContactPointType() ))
                {
                    state = cp.Address.State;
                    break;
                }
            }

            return state;
        }

        public bool IsValidForRCRPFields()
        {
            // This extended property was added so that participating 
            // hopitals could show RCRP related fields.
            return this["IsFacilityRCRPEnabled"] != null;
        }

        public bool IsValidForPatientPortalOptIn()
        {
            return this["IsPatientPortalOptInEnabled"] != null;
        }
        public bool IsValidForAuthorizePortalUser()
        {
            return this["IsAuthorizePortalUserEnabled"] != null;
        }

        public bool IsBaylorFacility()
        {
            return this["IsBaylorFacility"] != null;
        }
        public bool IsAcceptingPreRegistrationSubmissions()
        {
            // This extended property was added so that participating 
            // hopitals could accept PreRegistration Submissions
            return this["IsAcceptingPreRegistrationSubmissions"] != null;
        }

        // This extended property was added so that participating 
        // hopitals will search using EMPI
        public bool IsEMPIEnabled
        {
            get { return this["EMPIEnabled"] != null; }
        }

        public bool IsMonthlyDueDateEnabled
        {
            get { return this["MonthlyDueDateEnabled"] != null; }
        }
        public bool IsUCCRegistrationEnabled
        {
            get { return this["UCCRegistrationEnabled"] != null; }
        }

        public bool IsAutoCompleteNoLiabilityDueEnabled
        {
            get { return this["AutoCompleteNoLiabilityDueEnabled"] != null; }
        }

        public bool IsValidForSequesteredPatient()
        {
            if (this["IsSequesteredPatientEnabled"] == null)
            {
                return false;
            }
            else
            {
                this.SequesteredHelpLine = this["IsSequesteredPatientEnabled"].ToString();
                return true;
            }

        }

        public bool IsValidForHTML5VIWeb
        {
            get { return this["HTML5VIWebEnabled"] != null; }
        }

        public object DefaultForNotifyPCPOfVisit
        {
            get { return this["DefaultForNotifyPCPOfVisit"] ; }
        }

        public object DefaultForShareHIEData
        {
            get { return this["DefaultForShareHIEData"]; }
        }
        public bool IsDuplicateBedsAllowed
        {
            get { return this["IsDuplicateBedsAllowed"] != null; }
        }

        public bool IsOKTAEnabled
        {
            get { return this["IsOKTAEnabled"] != null; }
        }

        public bool IsInterFacilityTransferEnabled
        {
            get
            {
                var itfrFeatureManager = new InterFacilityTransferFeatureManager();

                return itfrFeatureManager.AllTransferFacilities.Contains(this.Code);
            }
        }

        public bool IsSATXEnabled
        {
            get
            {
                if (this["IsSATXEnabled"] == null)
                {
                    return false;
                }
                else
                {
                    var isEnabled = this["IsSATXEnabled"].ToString();
                    return bool.Parse(isEnabled);
                }
            }
        }

        public string DOFRFacilityMappingforTesting
        {
            get 
            {
                if (this["DOFRFacilityMappingforTesting"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this["DOFRFacilityMappingforTesting"].ToString();
                }
            }
        }

        public bool IsDOFREnabled
        {
            get
            {
                if (this["IsDOFREnabled"] == null)
                {
                    return false;
                }
                else
                {
                    var isEnabled = this["IsDOFREnabled"].ToString();
                    return bool.Parse(isEnabled);
                }
            }
        }


        public YesNoFlag DefaultShareHIEData()
        {
            var defaultForShareHIEData = this["DefaultForShareHIEData"] ;

            if (defaultForShareHIEData != null)
            {
                if (defaultForShareHIEData.ToString().ToUpper() == "YES")
                {
                    return YesNoFlag.Yes;
                }
                else if (defaultForShareHIEData.ToString().ToUpper() == "NO")
                {
                    return YesNoFlag.No;
                }
                else
                {
                    return YesNoFlag.Blank;
                }
            }
            else
            {
                return YesNoFlag.Yes;
            }
        }

        public YesNoFlag DefaultNotifyPCPOfVisit()
        {
            var defaultForNotifyPCPOfVisit = DefaultForNotifyPCPOfVisit;
            if(defaultForNotifyPCPOfVisit!= null)
            {
                if (defaultForNotifyPCPOfVisit.ToString().ToUpper() == "YES")
                {
                    return YesNoFlag.Yes;
                }
                else if (defaultForNotifyPCPOfVisit.ToString().ToUpper() == "NO")
                {
                    return YesNoFlag.No; ;
                }
                else
                {
                    return YesNoFlag.Blank;
                }
            }
            else
            {
                return YesNoFlag.Blank;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Answers the same as if sent the message ToCodedString().  This is implemented
        /// as a Property so that it can be "bound" to to controls such as a Combobox.
        /// </summary>
        public string CodeAndDescription
        {
            get
            {
                return ToCodedString();
            }
        }

        public ICollection ContactPoints
        {
            get
            {
                return (ICollection)i_ContactPoints.Clone();
            }
        }

        public FollowupUnit FollowupUnit
        {
            get
            {
                return i_FollowupUnit;
            }
            set
            {
                i_FollowupUnit = value;
            }
        }
        public string  SequesteredHelpLine { get; set; }

        public int GMTOffset
        {
            get
            {
                return i_GMTOffset;
            }
            set
            {
                i_GMTOffset = value;
            }
        }

        public int DSTOffset
        {
            get
            {
                return i_DSTOffset;
            }
            set
            {
                i_DSTOffset = value;
            }
        }


        public bool IsOrderCommunicationFacility
        {
            get
            {
                return i_IsOrderCommunicationFacility;
            }
            set
            {
                i_IsOrderCommunicationFacility = value;
            }
        }

        public long ModType
        {
            get
            {
                return i_ModType;
            }
            set
            {
                i_ModType = value;
            }
        }

        public YesNoFlag TenetCare
        {
            get
            {
                return i_TenetCare;
            }
            set
            {
                i_TenetCare = value;
            }
        }

        public YesNoFlag Reregister
        {
            get
            {
                return i_Reregister;
            }
            set
            {
                i_Reregister = value;
            }
        }

        public string FederalTaxID
        {
            get
            {
                return i_FederalTaxID;
            }
            set
            {
                i_FederalTaxID = value;
            }
        }

        public bool MedicaidIssueDateRequired
        {
            get
            {
                return i_MedicaidIssueDateRequired;
            }
            set
            {
                i_MedicaidIssueDateRequired = value;
            }
        }

        public State FacilityState
        {
            get
            {
                return i_FacilityState;
            }
            set
            {
                i_FacilityState = value;
            }
        }

        public string FacilityStateCode
        {
            get
            {
                return FacilityState.Code;
            }
        }

        public bool UsesUSCMRN
        {
            get { return i_UsesUSCMRN; }
            set { i_UsesUSCMRN = value; }
        }

        public ConnectionSpec ConnectionSpec
        {
            get
            {
                return i_ConnectionSpec;
            }
            set
            {
                i_ConnectionSpec = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Facility()
        {
            SetFacilityStateCode();
        }

        public Facility( long oid,
                            DateTime version,
                            string name,
                            string hospitalCode
            )
            : base( oid, version, name, hospitalCode )
        {
            SetFacilityStateCode();
        }

        // Overloaded the constructor with new parameter ModType, which is used
        // to generate the checkdigit for new account number.  
        public Facility( long oid,
                            DateTime version,
                            string name,
                            string hospitalCode,
                            long modType )
            : base( oid, version, name, hospitalCode )
        {
            ModType = modType;
            SetFacilityStateCode();
        }

        public Facility( long oid,
                            DateTime version,
                            string name,
                            string hospitalCode,
                            long modType,
                            int gmtOffset,
                            bool ocFacility )
            : base( oid, version, name, hospitalCode )
        {
            ModType = modType;
            GMTOffset = gmtOffset;
            IsOrderCommunicationFacility = ocFacility;
            SetFacilityStateCode();
        }
        #endregion

        #region Data Elements
        private int i_GMTOffset;

        private int i_DSTOffset;
        private bool i_IsOrderCommunicationFacility;
        private bool i_MedicaidIssueDateRequired = true;
        private readonly ArrayList i_ContactPoints = new ArrayList();
        private long i_ModType;
        private FollowupUnit i_FollowupUnit;
        private YesNoFlag i_TenetCare = new YesNoFlag();
        private YesNoFlag i_Reregister = new YesNoFlag();
        private string i_FederalTaxID;
        private State i_FacilityState = new State();
        private bool i_UsesUSCMRN;
        private ConnectionSpec i_ConnectionSpec;


        #endregion
    }

    public interface IFacility
    {
        void AddContactPoint( ContactPoint contactPoint );

        /// <summary>
        /// Similar to CodedReferenceValue's ToCodedString, but inserts a hyphen between
        /// the code and the description.
        /// </summary>
        /// <returns></returns>
        string ToCodedString();

        ContactPoint GetContactPoint( TypeOfContactPoint aTypeOfContactPoint );
        DateTime GetCurrentDateTime();
        bool IsFacilityInState( string stateCode );
        void SetFacilityStateCode();

        /// <summary>
        /// Answers the same as if sent the message ToCodedString().  This is implemented
        /// as a Property so that it can be "bound" to to controls such as a Combobox.
        /// </summary>
        string CodeAndDescription { get; }

        ICollection ContactPoints { get; }
        FollowupUnit FollowupUnit { get; set; }
        string SequesteredHelpLine { get; set; }
        int GMTOffset { get; set; }
        int DSTOffset { get; set; }
        bool IsOrderCommunicationFacility { get; set; }
        long ModType { get; set; }
        YesNoFlag TenetCare { get; set; }
        YesNoFlag Reregister { get; set; }
        string FederalTaxID { get; set; }
        bool MedicaidIssueDateRequired { get; set; }
        State FacilityState { get; set; }
        string FacilityStateCode { get; }
        bool UsesUSCMRN { get; set; }
        ConnectionSpec ConnectionSpec { get; set; }
        bool IsValidForClinicalResearchFields();
        bool IsAcceptingPreRegistrationSubmissions();
    }
}
