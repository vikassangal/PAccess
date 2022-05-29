using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Http;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class ReferenceObjectsLoader : object
    {
        #region Delegates
        
		// List delegates here for loading particular caches async.
        // Follow the pattern of Load[All]<Whatever it is you're loading>
        // Keep this list in alpha order.

        // AddressBrokerProxy

        private delegate IList LoadAllCountries( long facilityID );
        private delegate IList LoadAllStates(long facilityID);

		// AdmitSourceBrokerProxy

		private delegate ICollection		LoadAllAdmitSources( long facilityID);

		// DemographicsBrokerProxy

        private delegate ICollection      LoadAllTypesOfGenders( long facilityId );							// not implemented
        protected delegate ICollection      LoadAllMaritalStatuses( long facilityId );							// not implemented
        protected delegate ICollection      LoadAllLanguages( long facilityId );									// not implemented

		// FacilityFlagsBrokerProxy

		private delegate IList			LoadFacilityFlagsFor( long facilityId );

		// FinancialClassesBrokerProxy

        private delegate ICollection LoadAllFinancialClasses( long facilityId );
		private delegate Hashtable		LoadFinancialClassesForAFinancialClassType( long facilityId,long financialClassTypeId );

		// HSVCodeBrokerProxy

		private delegate IList			LoadAllHsvCodes( long facilityId );

		// ModeOfArrivalBrokerProxy

		private delegate ArrayList		LoadModesOfArrivalFor( long facilityID );			// not implemented

		// OccurrenceCodeBrokerProxy

        private delegate ICollection LoadAllAccidentTypes( long facilityID );
        private delegate ICollection LoadAllOccurrenceCodes( long facilityID );							// not implemented
        private delegate ICollection LoadAllSelectableOccurrenceCodes( long facilityID );					// not implemented
		
		// PatientBrokerProxy

		private delegate ICollection		LoadAllPatientTypes(long facilityID);	

		// ReAdmitCodeBrokerProxy

        private delegate ICollection LoadReAdmitCodesFor( long facilityNumber );			// not implemeneted

		// ReferralSourceBrokerProxy

		private delegate ICollection			LoadAllReferralSources( long facilityNumber );

		// ReferralTypeBrokerProxy

        private delegate ICollection      LoadReferralTypesFor( long facilityNumber );		// not implemented

		// RuleBrokerProxy		
		
		private delegate void				LoadActivityRules();

		// ScheduleCodeBrokerProxy

        private delegate ICollection      LoadAllScheduleCodes( long facilityID );								// not implemented

		// SSNBrokerProxy ???

        private delegate ArrayList LoadSSNStatuses( long facilityNumber, string stateCode );				// not implemented
        private delegate ArrayList LoadSSNStatusesForGuarantor(long facilityNumber, string stateCode );	// not implemented

        // ResearchStudyBrokerProxy

        private delegate IEnumerable<ResearchStudy> LoadAllResearchStudies( long facilityID );

		// Misc

		private delegate void				LoadReferenceObjectsAsync();
        

        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Asynchronously loads Reference Data such as FacilityFlags, Rules, Genders, etc.
        /// </summary>
        public void LoadAllAsync()
        {
            new LoadReferenceObjectsAsync( this.DoLoadAsync ).BeginInvoke( null, null );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void DoLoadAsync()
        {
            // Put this thread to sleep for two seconds so that the 
            // sender, the PatientAccessView ususally, can finish loading
            // before we take up all thread bandwidth.

            Thread.Sleep( 5000 );

            this.LoadAccidentTypes();
            this.LoadAdmitSources();
            this.LoadCountriesAndStates();
            this.LoadFacilityFlags();
            this.LoadHSVCodes();
			this.LoadLanguages();
			this.LoadMaritalStatuses();
			this.LoadOccurrenceCodes();
			this.LoadTheModesOfArrivalFor();
			this.LoadPatientTypes();
			this.LoadTheReAdmitCodesFor();
            this.LoadReferralSources();
			this.LoadReferralTypes();
            this.LoadRules();
			this.LoadScheduleCodes();
			this.LoadTheSSNStatuses();
			this.LoadTypesOfGenders();
            LoadResearchStudies();
        }

        private void LoadAccidentTypes()
        {
            OccuranceCodeBrokerProxy occurrenceCodeBrokerProxy = new OccuranceCodeBrokerProxy();
            new LoadAllAccidentTypes(occurrenceCodeBrokerProxy.GetAccidentTypes).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );                        
        }

        private void LoadAdmitSources()
        {
            AdmitSourceBrokerProxy admitSourceBrokerProxy = new AdmitSourceBrokerProxy();
            new LoadAllAdmitSources( admitSourceBrokerProxy.AllTypesOfAdmitSources ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null);                        
        }

        private void LoadHSVCodes()
        {
            HSVBrokerProxy hsvBrokerProxy = new HSVBrokerProxy();
            new LoadAllHsvCodes( hsvBrokerProxy.SelectHospitalServicesFor ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null);
        }

        private void LoadCountriesAndStates()
        {
            AddressBrokerProxy addressBroker = new AddressBrokerProxy();
            new LoadAllCountries( addressBroker.AllCountries ).BeginInvoke(User.GetCurrent().Facility.Oid, null, null );
            new LoadAllStates(addressBroker.AllStates).BeginInvoke(User.GetCurrent().Facility.Oid, null, null);
        }

        private void LoadFacilityFlags()
        {
            FacilityFlagBrokerProxy flagsBroker = new FacilityFlagBrokerProxy();
            new LoadFacilityFlagsFor( flagsBroker.FacilityFlagsFor ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
        }

        private void LoadFinancialClasses()
        {
            FinancialClassesBrokerProxy financialClassesBroker = new FinancialClassesBrokerProxy();
            new LoadAllFinancialClasses( financialClassesBroker.AllTypesOfFinancialClasses ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
            new LoadFinancialClassesForAFinancialClassType( financialClassesBroker.FinancialClassesFor )
                .BeginInvoke(User.GetCurrent().Facility.Oid,STANDARD_FINANCIAL_CLASS_TYPE, null, null );
            new LoadFinancialClassesForAFinancialClassType( financialClassesBroker.FinancialClassesFor )
                .BeginInvoke(User.GetCurrent().Facility.Oid, UNINSURED_FINANCIAL_CLASS_TYPE, null, null );
        }

        private void LoadLanguages()
		{
			DemographicsBrokerProxy demographicsBrokerProxy = new DemographicsBrokerProxy();
            new LoadAllTypesOfGenders( demographicsBrokerProxy.AllLanguages ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadMaritalStatuses()
		{
			DemographicsBrokerProxy demographicsBrokerProxy = new DemographicsBrokerProxy();
            new LoadAllTypesOfGenders( demographicsBrokerProxy.AllMaritalStatuses ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadTheModesOfArrivalFor()
		{
			ModeOfArrivalBrokerProxy modeOfArrivalBrokerProxy = new ModeOfArrivalBrokerProxy();
			new LoadModesOfArrivalFor( modeOfArrivalBrokerProxy.ModesOfArrivalFor ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadOccurrenceCodes()
		{
			OccuranceCodeBrokerProxy occurrenceCodeBrokerProxy = new OccuranceCodeBrokerProxy();
            new LoadAllOccurrenceCodes(occurrenceCodeBrokerProxy.AllOccurrenceCodes).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
            new LoadAllSelectableOccurrenceCodes(occurrenceCodeBrokerProxy.AllSelectableOccurrenceCodes).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadPatientTypes()
		{
			PatientBrokerProxy patientBrokerProxy = new PatientBrokerProxy();
            new LoadAllPatientTypes( patientBrokerProxy.AllPatientTypes ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadTheReAdmitCodesFor()
		{
			ReAdmitCodeBrokerProxy reAdmitCodeBrokerProxy = new ReAdmitCodeBrokerProxy();
			new LoadReAdmitCodesFor( reAdmitCodeBrokerProxy.ReAdmitCodesFor ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

        private void LoadReferralSources()
        {
            ReferralSourceBrokerProxy referralSourceBrokerProxy = new ReferralSourceBrokerProxy();
            new LoadAllReferralSources( referralSourceBrokerProxy.AllReferralSources ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );                        
        }

		private void LoadReferralTypes()
		{
			ReferralTypeBrokerProxy referralTypeBrokerProxy = new ReferralTypeBrokerProxy();
			new LoadReferralTypesFor( referralTypeBrokerProxy.ReferralTypesFor ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );                        
		}

        private void LoadRules()
        {
            RuleBrokerProxy ruleBrokerProxy = new RuleBrokerProxy();
            new LoadActivityRules( ruleBrokerProxy.PreCacheRules ).BeginInvoke(null, null);         
        }

		private void LoadScheduleCodes()
		{
			ScheduleCodeBrokerProxy scheduleCodeBrokerProxy = new ScheduleCodeBrokerProxy();
            new LoadAllScheduleCodes( scheduleCodeBrokerProxy.AllScheduleCodes ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadTheSSNStatuses()
		{
			SSNBrokerProxy ssnBrokerProxy = new SSNBrokerProxy();

			string facilityStateCode = string.Empty;
			ArrayList listOfContactPoints = (ArrayList)User.GetCurrent().Facility.ContactPoints;

			foreach( ContactPoint cp in listOfContactPoints )
			{
				if (cp != null && cp.TypeOfContactPoint.Oid == TypeOfContactPoint.MAILING_OID)
				{
					facilityStateCode = cp.Address.State.Code;
					break;
				}
			}

			if( facilityStateCode != string.Empty )
			{
                new LoadSSNStatuses(ssnBrokerProxy.SSNStatuses).BeginInvoke( User.GetCurrent().Facility.Oid, facilityStateCode,null, null );
                new LoadSSNStatusesForGuarantor(ssnBrokerProxy.SSNStatusesForGuarantor).BeginInvoke(User.GetCurrent().Facility.Oid, facilityStateCode, null, null);
                //new LoadSSNStatuses(ssnBrokerProxy.SSNStatuses).BeginInvoke(facilityStateCode, null, null);
                //new LoadSSNStatusesForGuarantor(ssnBrokerProxy.SSNStatusesForGuarantor).BeginInvoke(facilityStateCode, null, null);
            }			
		}

		private void LoadTypesOfGenders()
		{
			DemographicsBrokerProxy demographicsBrokerProxy = new DemographicsBrokerProxy();
            new LoadAllTypesOfGenders( demographicsBrokerProxy.AllTypesOfGenders ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
		}

		private void LoadResearchStudies()
		{
            ResearchStudyBrokerProxy researchStudyBrokerProxy = new ResearchStudyBrokerProxy( BrokerFactory.BrokerOfType<IResearchStudyBroker>(), new HttpRuntimeCache() );
            new LoadAllResearchStudies( researchStudyBrokerProxy.AllResearchStudies ).BeginInvoke( User.GetCurrent().Facility.Oid, null, null );
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferenceObjectsLoader()
        {
        }
        public ReferenceObjectsLoader(long facilityID)
        {
            i_facilityID = facilityID;
        }
        #endregion

        #region Data Elements

        private long i_facilityID;
        #endregion

        #region Constants
        private const long
            STANDARD_FINANCIAL_CLASS_TYPE    = 1L,
            UNINSURED_FINANCIAL_CLASS_TYPE   = 2L;
        #endregion
    }
}