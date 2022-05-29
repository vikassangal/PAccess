using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using Quartz;
using Quartz.Impl;
using log4net.Config;
using PatientAccess.Utilities;

namespace PatientAccess.AppServer
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : HttpApplication
    {
		#region Constants 

        private const string LOAD_CACHE_SERIAL = "LOADCACHESERIAL";
        private const string MAX_SERVICE_POINT_IDLE_TIME = "CCMaxServicePointIdleTime";
        private const string PAS_SERVER_ENVIRONMENT = "PASServerEnvironment";

		#endregion Constants 

		#region Fields 

        private IScheduler _scheduler = 
            new StdSchedulerFactory().GetScheduler();


        private static readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger( typeof( Global ) );
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        private bool _shouldLoadSerial = false;
        private static ServerEnvironment _PASServerEnvironment = ServerEnvironment.UNKNOWN;
		#endregion Fields 

		#region Constructors 

        public Global()
        {
            InitializeComponent();
        }

		#endregion Constructors 

		#region Properties 

        private bool LoadSerial 
        {
            get
            {
                return this._shouldLoadSerial;
            }
            set
            {
                this._shouldLoadSerial = value;
            }
        }

        private IScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
            set
            {
                this._scheduler = value;
            }
        }

        public static ServerEnvironment PASServerEnvironment
        {
            get { return _PASServerEnvironment; }
           private  set { _PASServerEnvironment = value; }
        }

        #endregion Properties 

		#region Delegates and Events 

        private delegate ICollection LoadAllAdmitSources( long facilityNumber );
        private delegate ICollection LoadAllAuthorizationStatuses();
        private delegate ICollection LoadAllBenefitCategories( long facilityNumber );
        private delegate IList LoadAllConditionCodes( long facilityNumber );
        private delegate ICollection LoadAllConditionsOfService();
        private delegate ICollection<EmailReason> LoadAllEmailReasons();
        private delegate ICollection<CellPhoneConsent> LoadAllCellPhoneConsents();
        private delegate IList LoadAllConfidentialCodes( long facilityNumber );
        private delegate IList LoadAllCountries( long facilityNumber );
        private delegate IList LoadAllCreditCardTypes();
        private delegate ICollection LoadAllDischargeDispositions( long facilityNumber );
        private delegate ICollection LoadAllEmploymentStatuses( long facilityNumber );
        private delegate ICollection LoadAllEthnicities( long facilityNumber );
        private delegate ICollection LoadAllFinancialClasses( long facilityNumber );
        private delegate ICollection LoadAllGenders( long facilityNumber );
        private delegate ICollection LoadAllHospitalClinics( long facilityNumber );
        private delegate HospitalClinic LoadAllHospitalPretestClinics( long facilityNumber );
        private delegate ICollection LoadAllHSVs( long facilityNumber );
        private delegate ICollection LoadAllLanguages( long facilityNumber );
        private delegate ICollection LoadAllMaritalStatuses( long facilityNumber );
        private delegate ICollection LoadAllModesOfArrival( long facilityNumber );
        private delegate ICollection LoadAllNPPs( long facilityNumber );
        private delegate ICollection LoadAllOccurrenceCodes( long facilityNumber );
        private delegate ICollection LoadAllPhysicianRoles();
        private delegate ICollection LoadAllPhysicianSpecialties( long facilityNumber );
        private delegate ICollection LoadAllRaces( long facilityNumber );
        private delegate ICollection LoadAllReAdmitCodes( long facilityNumber );
        private delegate ICollection LoadAllReferralFacilities( long facilityNumber );
        private delegate ICollection LoadAllReferralSources( long facilityNumber );
        private delegate ICollection LoadAllReferralTypes( long facilityNumber );
        private delegate ICollection LoadAllReligions( long facilityNumber );
        private delegate IList LoadAllReligiousPlacesOfWorship( long facilityNumber );
        private delegate ICollection LoadAllRoles();
        private delegate ICollection LoadAllScheduleCodes( long facilityNumber );
        private delegate ICollection LoadAllSelectableOccurrenceCodes( long facilityNumber );
        private delegate ICollection LoadAllSpanCodes( long facilityNumber );
        private delegate IList LoadAllStates(long facilityNumber);
        private delegate ICollection LoadAllTypesOfRelationShips( long facilityNumber );
        private delegate ICollection LoadAllVisitTypes( long facilityNumber );
        private delegate IEnumerable<ResearchStudy> LoadAllResearchStudies(long facilityNumber);
        private delegate ICollection<string> LoadAllAlternateCareFacilities(long facilityNumber);
        private delegate ICollection<string> LoadAllSuffixCodes(long facilityNumber);
        private delegate ICollection<LeftOrStayed> LoadAllLeftOrStayed();
        private delegate ICollection<string> LoadAllDialysisCenterNames(long facilityNumber);

		#endregion Delegates and Events 

		#region Methods 
        private void ReadPASServerEnvironment()
        {
            var serverEnvironment = ConfigurationManager.AppSettings[PAS_SERVER_ENVIRONMENT];
            
            switch (serverEnvironment.ToUpper())
            {
                case "LOCAL":
                    PASServerEnvironment = ServerEnvironment.LOCAL;
                    break;
                case "DEVELOPMENT":
                    PASServerEnvironment = ServerEnvironment.DEVELOPMENT;
                    break;
                case "TEST":
                    PASServerEnvironment = ServerEnvironment.TEST;
                    break;
                case "MODEL":
                    PASServerEnvironment = ServerEnvironment.MODEL;
                    break;
                case "PRODUCTION":
                    PASServerEnvironment = ServerEnvironment.PRODUCTION;
                    break;
                default:
                    _logger.ErrorFormat("UNKNOWN PAS environment : {0}", serverEnvironment ); 
                    PASServerEnvironment = ServerEnvironment.UNKNOWN;
                    break;
            }
        }
        protected void Application_AuthenticateRequest( Object sender, EventArgs e )
        {
        }


        protected void Application_BeginRequest( Object sender, EventArgs e )
        {
            
        }


        protected void Application_End( Object sender, EventArgs e )
        {

            if( _logger.IsDebugEnabled )
            {
                _logger.DebugFormat( "{0}() - {1}",
                MethodBase.GetCurrentMethod().Name,
                "Entered" );
            }//if

            _logger.InfoFormat( "{0}() - {1}",
                              MethodBase.GetCurrentMethod().Name,
                              "Purging pooled connections for this process" );

            iDB2ProviderSettings.CleanupPooledConnections();
            
            this.Scheduler.Shutdown( false );

            if( _logger.IsDebugEnabled )
            {
                _logger.DebugFormat( "{0}() - {1}",
                MethodBase.GetCurrentMethod().Name,
                "Exited" );
            }//if
        
        }


        protected void Application_EndRequest( Object sender, EventArgs e )
        {
        }


        protected void Application_Error( Object sender, EventArgs e )
        {
        }


        protected void Application_Start( Object sender, EventArgs e )
        {
            var log4NetConfigFilePath = Path.Combine( HostingEnvironment.ApplicationPhysicalPath, "Log4Net.config" );
            XmlConfigurator.Configure( new FileInfo( log4NetConfigFilePath ) ); 

            this.Scheduler = 
                new StdSchedulerFactory().GetScheduler();

            this.Scheduler.Start();

            this.LoadSerial = 
                bool.Parse(ConfigurationManager.AppSettings[LOAD_CACHE_SERIAL]);

            // Set the time limit in milliseconds that a service point object 
            // can remain idle before it is marked eligible for garbage collection.  
            // This does not apply to connections that are created with Keep-alives disabled.
            System.Net.ServicePointManager.MaxServicePointIdleTime =
                int.Parse( ConfigurationManager.AppSettings[MAX_SERVICE_POINT_IDLE_TIME] );

            _logger.InfoFormat( "{0}() - {1}{2}",
                              MethodBase.GetCurrentMethod().Name,
                              "LoadSerial set to ",
                              this.LoadSerial );

            PreLoadCaches();
            ReadPASServerEnvironment();

        }


        private void LoadAdmitSources( long facilityNumber )
        {
            IAdmitSourceBroker admitSourceBroker = BrokerFactory.BrokerOfType<IAdmitSourceBroker>();
            if (this.LoadSerial)
            {
                admitSourceBroker.AllTypesOfAdmitSources(facilityNumber);
            }
            else
            {
                new LoadAllAdmitSources(admitSourceBroker.AllTypesOfAdmitSources).BeginInvoke(facilityNumber, null, null);
            }
        }


        private void LoadAuthorizationStatuses()
        {
            IAuthorizationStatusBroker authorizationStatusBroker = BrokerFactory.BrokerOfType<IAuthorizationStatusBroker>();
            if (this.LoadSerial)
            {
                authorizationStatusBroker.AllAuthorizationStatuses();
            }
            else
            {
                new LoadAllAuthorizationStatuses(authorizationStatusBroker.AllAuthorizationStatuses).BeginInvoke(null, null);
            }
        }


        private void LoadBenefitCategories( long facilityNumber )
        {
            IBenefitsCategoryBroker benefitsCategoryBroker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            if (this.LoadSerial)
            {
                benefitsCategoryBroker.AllBenefitsCategories(facilityNumber);
            }
            else
            {
                new LoadAllBenefitCategories(benefitsCategoryBroker.AllBenefitsCategories).BeginInvoke(facilityNumber, null, null);
            }
        }


        private void LoadConditionCodes( long facilityNumber )
        {

            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            if (this.LoadSerial)
            {
                conditionCodeBroker.AllSelectableConditionCodes(facilityNumber);
            }
            else
            {
                new LoadAllConditionCodes( conditionCodeBroker.AllSelectableConditionCodes ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadConditionsOfService()
        {
            IConditionOfServiceBroker conditionOfServiceBroker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
            if (this.LoadSerial)
            {
                conditionOfServiceBroker.AllConditionsOfService();
            }
            else
            {
                new LoadAllConditionsOfService( conditionOfServiceBroker.AllConditionsOfService ).BeginInvoke( null, null );
            }
        }

        private void LoadEmailReasons()
        {
           var emailReasonBroker = BrokerFactory.BrokerOfType<IEmailReasonBroker>();
            if (LoadSerial)
            {
                emailReasonBroker.AllEmailReasons();
            }
            else
            {
                new LoadAllEmailReasons(emailReasonBroker.AllEmailReasons).BeginInvoke(null, null);
            }

        }

        private void LoadCellPhoneConsents()
        {
            var cellPhoneConsentBroker = BrokerFactory.BrokerOfType<ICellPhoneConsentBroker>();

            if (LoadSerial)
            {
                cellPhoneConsentBroker.AllCellPhoneConsents();
            }

            else
            {
                new LoadAllCellPhoneConsents(cellPhoneConsentBroker.AllCellPhoneConsents).BeginInvoke(null, null);
            }
        }

        private void LoadConfidentialCodes( long facilityNumber )
        {
            IConfidentialCodeBroker confidentialCodeBroker = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
            if (this.LoadSerial)
            {
                confidentialCodeBroker.ConfidentialCodesFor(facilityNumber);
            }
            else
            {
                new LoadAllConfidentialCodes(confidentialCodeBroker.ConfidentialCodesFor).BeginInvoke(facilityNumber, null, null);
            }
        }


        private void LoadCountries( long facilityNumber )
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            if (this.LoadSerial)
            {
                addressBroker.AllCountries( facilityNumber );
            }
            else
            {
                new LoadAllCountries( addressBroker.AllCountries ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadCreditCardTypes()
        {
            ICreditCardTypeBroker creditCardTypeBroker = BrokerFactory.BrokerOfType<ICreditCardTypeBroker>();
            if (this.LoadSerial)
            {
                creditCardTypeBroker.AllCreditCardTypes();
            }
            else
            {
                new LoadAllCreditCardTypes( creditCardTypeBroker.AllCreditCardTypes ).BeginInvoke( null, null );
            }
        }


        private void LoadDischargeDispositions(long facilityNumber)
        {
            IDischargeBroker dischargeBroker = BrokerFactory.BrokerOfType<IDischargeBroker>();
            if (this.LoadSerial)
            {
                dischargeBroker.AllDischargeDispositions(facilityNumber);
            }
            else
            {
                new LoadAllDischargeDispositions( dischargeBroker.AllDischargeDispositions ).BeginInvoke(facilityNumber, null, null );
            }
        }


        private void LoadEmploymentStatuses( long facilityNumber )
        {
            IEmploymentStatusBroker employmentStatusBroker = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
            if (this.LoadSerial)
            {
                employmentStatusBroker.AllTypesOfEmploymentStatuses(facilityNumber);
            }
            else
            {
                new LoadAllEmploymentStatuses( employmentStatusBroker.AllTypesOfEmploymentStatuses ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadEthnicities( long facilityNumber )
        {
            IOriginBroker originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
            if (this.LoadSerial)
            {
                originBroker.AllEthnicities(facilityNumber);
            }
            else
            {
                new LoadAllEthnicities( originBroker.AllEthnicities ).BeginInvoke(facilityNumber, null, null );
            }
        }


        private void LoadFinancialClasses( long facilityNumber )
        {
            IFinancialClassesBroker financialClassesBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
            if (this.LoadSerial)
            {
                financialClassesBroker.AllTypesOfFinancialClasses(facilityNumber);
            }
            else
            {
                new LoadAllFinancialClasses( financialClassesBroker.AllTypesOfFinancialClasses ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadGenders( long facilityNumber )
        {
            IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            if (this.LoadSerial)
            {
                demographicsBroker.AllTypesOfGenders(facilityNumber);
            }
            else
            {
                new LoadAllGenders( demographicsBroker.AllTypesOfGenders ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadHospitalClinics( long facilityNumber )
        {
            IHospitalClinicsBroker hospitalClinicsBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();
            if (this.LoadSerial)
            {
                hospitalClinicsBroker.HospitalClinicsFor(facilityNumber);
            }
            else
            {
                new LoadAllHospitalClinics( hospitalClinicsBroker.HospitalClinicsFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadHospitalOPretestClinics( long facilityNumber )
        {
            IHospitalClinicsBroker hospitalClinicsBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();
            if (this.LoadSerial)
            {
                hospitalClinicsBroker.PreTestHospitalClinicFor(facilityNumber);
            }
            else
            {
                new LoadAllHospitalPretestClinics( hospitalClinicsBroker.PreTestHospitalClinicFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        // this list takes 2 parameters. Facility and patientType. 
        // need to determine how to loop through 2 lists to make this work.
        //protected void LoadHSVs(long facilityNumber)
        //{
        //    HSVBroker hsvBroker = new HSVBroker();
        //    new LoadAllHSVs( hsvBroker.HospitalServicesFor ).BeginInvoke( facilityNumber, null, null );                        
        //}
        private void LoadLanguages( long facilityNumber )
        {
            IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            if (this.LoadSerial)
            {
                demographicsBroker.AllLanguages(facilityNumber);
            }
            else
            {
                new LoadAllLanguages( demographicsBroker.AllLanguages ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadMaritalStatuses( long facilityNumber )
        {
            IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            if (this.LoadSerial)
            {
                demographicsBroker.AllMaritalStatuses(facilityNumber);
            }
            else
            {
                new LoadAllMaritalStatuses( demographicsBroker.AllMaritalStatuses ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadModesOfArrival( long facilityNumber )
        {
            IModeOfArrivalBroker modeOfArrivalBroker = BrokerFactory.BrokerOfType<IModeOfArrivalBroker>();
            if (this.LoadSerial)
            {
                modeOfArrivalBroker.ModesOfArrivalFor(facilityNumber);
            }
            else
            {
                new LoadAllModesOfArrival( modeOfArrivalBroker.ModesOfArrivalFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadNPPs( long facilityNumber )
        {
            INPPVersionBroker nppVersionBroker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
            if (this.LoadSerial)
            {
                nppVersionBroker.NPPVersionsFor(facilityNumber);
            }
            else
            {
                new LoadAllNPPs( nppVersionBroker.NPPVersionsFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadOccurrenceCodes(long facilityNumber)
        {
            IOccuranceCodeBroker occurrenceCodeBroker = BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
            if (this.LoadSerial)
            {
                occurrenceCodeBroker.AllOccurrenceCodes(facilityNumber);
            }
            else
            {
                new LoadAllOccurrenceCodes(occurrenceCodeBroker.AllOccurrenceCodes).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadPatientTypes( long facilityNumber )
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            if (this.LoadSerial)
            {
                patientBroker.AllPatientTypes(facilityNumber);
            }
            else
            {
                new LoadAllVisitTypes( patientBroker.AllPatientTypes ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadPhysicialSpecialties( long facilityNumber )
        {
            IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
            if (this.LoadSerial)
            {
                physicianBroker.SpecialtiesFor(facilityNumber);
            }
            else
            {
                new LoadAllPhysicianSpecialties( physicianBroker.SpecialtiesFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadRaces( long facilityNumber )
        {
            IOriginBroker originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
            if (this.LoadSerial)
            {
                originBroker.AllRaces(facilityNumber);
            }
            else
            {
                new LoadAllRaces( originBroker.AllRaces ).BeginInvoke( facilityNumber,null, null );
            }
        }


        private void LoadReAdmitCodes( long facilityNumber )
        {
            IReAdmitCodeBroker reAdmitCodeBroker = BrokerFactory.BrokerOfType<IReAdmitCodeBroker>();
            if (this.LoadSerial)
            {
                reAdmitCodeBroker.ReAdmitCodesFor(facilityNumber);
            }
            else
            {
                new LoadAllReAdmitCodes( reAdmitCodeBroker.ReAdmitCodesFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadReferralFacilities( long facilityNumber )
        {
            IReferralFacilityBroker referralFacilityBroker = BrokerFactory.BrokerOfType<IReferralFacilityBroker>();
            if (this.LoadSerial)
            {
                referralFacilityBroker.ReferralFacilitiesFor(facilityNumber);
            }
            else
            {
                new LoadAllReferralFacilities( referralFacilityBroker.ReferralFacilitiesFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadReferralSources( long facilityNumber )
        {
            IReferralSourceBroker referralSourceBroker = BrokerFactory.BrokerOfType<IReferralSourceBroker>();
            if (this.LoadSerial)
            {
                referralSourceBroker.AllReferralSources(facilityNumber);
            }
            else
            {
                new LoadAllReferralSources( referralSourceBroker.AllReferralSources ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadReferralTypes( long facilityNumber )
        {
            IReferralTypeBroker referralTypeBroker = BrokerFactory.BrokerOfType<IReferralTypeBroker>();
            if (this.LoadSerial)
            {
                referralTypeBroker.ReferralTypesFor(facilityNumber);
            }
            else
            {
                new LoadAllReferralTypes( referralTypeBroker.ReferralTypesFor ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadReligions( long facilityNumber )
        {
            IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
            if (this.LoadSerial)
            {
                religionBroker.AllReligions(facilityNumber);
            }
            else
            {
                new LoadAllReligions( religionBroker.AllReligions ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadReligiousPlacesOfWorship( long facilityNumber )
        {
            IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
            if (this.LoadSerial)
            {
                religionBroker.AllPlacesOfWorshipFor(facilityNumber);
            }
            else
            {
                new LoadAllReligiousPlacesOfWorship( religionBroker.AllPlacesOfWorshipFor ).BeginInvoke( facilityNumber, null, null );
            }
        }

        private void LoadRoles()
        {
            IRoleBroker roleBroker = BrokerFactory.BrokerOfType<IRoleBroker>();
            if (this.LoadSerial)
            {
                roleBroker.AllRoles();
            }
            else
            {
                new LoadAllRoles( roleBroker.AllRoles ).BeginInvoke( null, null );
            }
        }


        private void LoadScheduleCodes( long facilityNumber )
        {
            IScheduleCodeBroker scheduleCodeBroker = BrokerFactory.BrokerOfType<IScheduleCodeBroker>();
            if (this.LoadSerial)
            {
                scheduleCodeBroker.AllScheduleCodes(facilityNumber);
            }
            else
            {
                new LoadAllScheduleCodes( scheduleCodeBroker.AllScheduleCodes ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadSelectableOccurrenceCodes( long facilityNumber )
        {
            IOccuranceCodeBroker occurrenceCodeBroker = BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
            if (this.LoadSerial)
            {
                occurrenceCodeBroker.AllSelectableOccurrenceCodes(facilityNumber);
            }
            else
            {
                new LoadAllSelectableOccurrenceCodes( occurrenceCodeBroker.AllSelectableOccurrenceCodes ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadSpanCodes( long facilityNumber )
        {
            ISpanCodeBroker spanCodeBroker = BrokerFactory.BrokerOfType<ISpanCodeBroker>();
            if (this.LoadSerial)
            {
                spanCodeBroker.AllSpans(facilityNumber);
            }
            else
            {
                new LoadAllSpanCodes( spanCodeBroker.AllSpans ).BeginInvoke( facilityNumber, null, null );
            }
        }


        private void LoadStates(long facilityNumber)
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            if (this.LoadSerial)
            {
                addressBroker.AllStates(facilityNumber);
            }
            else
            {
                new LoadAllStates(addressBroker.AllStates).BeginInvoke(facilityNumber,null, null);
            }
        }


        private void LoadTypesOfRelationships( long facilityNumber )
        {
            IRelationshipTypeBroker relationshipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
            if (this.LoadSerial)
            {
                relationshipTypeBroker.AllTypesOfRelationships(facilityNumber);
            }
            else
            {
                new LoadAllTypesOfRelationShips( relationshipTypeBroker.AllTypesOfRelationships ).BeginInvoke(facilityNumber, null, null );
            }
        }

        private void LoadResearchStudy( long facilityNumber )
        {
            IResearchStudyBroker researchStudyBroker = BrokerFactory.BrokerOfType<IResearchStudyBroker>();
            if ( LoadSerial )
            {
                researchStudyBroker.AllResearchStudies( facilityNumber );
            }
            else
            {
                new LoadAllResearchStudies( researchStudyBroker.AllResearchStudies ).BeginInvoke( facilityNumber, null, null );
            }
        }

        private void LoadAlternateCareFacility(long facilityNumber)
        {
            IAlternateCareFacilityBroker alternateCareFacilityBroker = BrokerFactory.BrokerOfType<IAlternateCareFacilityBroker>();
            if (this.LoadSerial)
            {
                alternateCareFacilityBroker.AllAlternateCareFacilities(facilityNumber);
            }
            else
            {
                new LoadAllAlternateCareFacilities(alternateCareFacilityBroker.AllAlternateCareFacilities).BeginInvoke(facilityNumber, null, null);
            }
        }

        private void LoadSuffixCodes(long facilityNumber)
        {
            ISuffixBroker suffixBroker =
                BrokerFactory.BrokerOfType<ISuffixBroker>();
            if (this.LoadSerial)
            {
                suffixBroker.AllSuffixCodes(facilityNumber);
            }
            else
            {
                new LoadAllSuffixCodes(suffixBroker.AllSuffixCodes).BeginInvoke(
                    facilityNumber, null, null);
            }
        }

        private void LoadLeftOrStayed()
        {
            ILeftOrStayedBroker leftOrStayedBroker = BrokerFactory.BrokerOfType<ILeftOrStayedBroker>();
            if (this.LoadSerial)
            {
                leftOrStayedBroker.AllLeftOrStayed();
            }
            else
            {
                new LoadAllLeftOrStayed(leftOrStayedBroker.AllLeftOrStayed).BeginInvoke(null, null);
            }
        }

        private void LoadDialysisCenterNames(long facilityNumber)
        {
            IDialysisCenterBroker dialysisCenterBroker = BrokerFactory.BrokerOfType<IDialysisCenterBroker>();
            if (this.LoadSerial)
            {
                dialysisCenterBroker.AllDialysisCenterNames(facilityNumber);
            }
            else
            {
                new LoadAllDialysisCenterNames(dialysisCenterBroker.AllDialysisCenterNames).BeginInvoke(facilityNumber, null, null);
            }
        }
        protected void Session_End( Object sender, EventArgs e )
        {
        }


        protected void Session_Start( Object sender, EventArgs e )
        {
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
        }


        private void PreLoadCaches()
        {

            if( _logger.IsDebugEnabled )
            {
                _logger.DebugFormat( "{0}() - {1}",
                MethodBase.GetCurrentMethod().Name,
                "Entered" );
            }//if
        
            IFacilityBroker facilityBroker = 
                BrokerFactory.BrokerOfType< IFacilityBroker >();
            ICollection facilities = 
                facilityBroker.AllFacilities();
            
            this.LoadConditionsOfService();
            this.LoadCellPhoneConsents();
            this.LoadEmailReasons();
            //this.LoadCountries();
            this.LoadCreditCardTypes();
            this.LoadPatientTypes( ((facilities as ArrayList)[0] as Facility).Oid );
            this.LoadRoles();
            //this.LoadStates();
            this.LoadAuthorizationStatuses();
            this.LoadLeftOrStayed();

            foreach( Facility entry in facilities )
            {
                this.LoadCountries( entry.Oid);
                this.LoadHospitalClinics( entry.Oid );
                this.LoadConfidentialCodes( entry.Oid );
                this.LoadHospitalOPretestClinics( entry.Oid );
                this.LoadModesOfArrival( entry.Oid );
                this.LoadNPPs( entry.Oid );
                this.LoadPhysicialSpecialties( entry.Oid );
                this.LoadReAdmitCodes( entry.Oid );
                this.LoadReferralFacilities( entry.Oid );
                this.LoadReferralTypes( entry.Oid );
                this.LoadReligiousPlacesOfWorship( entry.Oid );
                this.LoadConditionCodes( entry.Oid );
                this.LoadResearchStudy(entry.Oid);
                this.LoadReferralSources( entry.Oid );
                this.LoadReligions( entry.Oid );
                this.LoadAdmitSources( entry.Oid );
                this.LoadLanguages( entry.Oid );
                this.LoadMaritalStatuses( entry.Oid );
                this.LoadGenders( entry.Oid );
                this.LoadScheduleCodes( entry.Oid );
                this.LoadDischargeDispositions( entry.Oid );
                this.LoadOccurrenceCodes( entry.Oid );
                this.LoadSelectableOccurrenceCodes( entry.Oid );
                this.LoadFinancialClasses( entry.Oid );
                this.LoadEthnicities( entry.Oid );
                this.LoadRaces( entry.Oid );
                this.LoadEmploymentStatuses( entry.Oid );
                this.LoadBenefitCategories( entry.Oid ); 
                this.LoadSpanCodes( entry.Oid );
                this.LoadAlternateCareFacility(entry.Oid);
                this.LoadSuffixCodes(entry.Oid);
                this.LoadDialysisCenterNames(entry.Oid);
                this.LoadStates(entry.Oid);
            }

            if( _logger.IsDebugEnabled )
            {
                _logger.DebugFormat( "{0}() - {1}",
                MethodBase.GetCurrentMethod().Name,
                "Exited" );
            }//if
        
        }

		#endregion Methods 
    }
}
