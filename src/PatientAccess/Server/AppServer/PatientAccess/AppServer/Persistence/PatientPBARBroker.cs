using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Extensions;
using Extensions.DB2Persistence;
using Extensions.Exceptions;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Manages the retrieval of Patient related information from the PBAR
    /// DB2 database.
    /// </summary>
    [Serializable]
    public class PatientPBARBroker : PBARCodesBroker, IPatientBroker
    {

        #region Constants

        private const string COL_DFIRSTNAME = "DisplayFirstName";
        private const string COL_DLASTNAME = "DisplayLastName";
        private const string COL_DMIDDLEINITIAL = "DisplayMiddleInitial";
        private const string COL_DOB = "DOB";
        private const string COL_FIRSTNAME = "FirstName";
        private const string COL_LASTNAME = "LastName";
        private const string COL_MAILINGCITY = "MAILINGCITY";
        private const string COL_MAILINGSTATE = "MAILINGSTATE";
        private const string COL_MAILINGSTREET = "MAILINGSTREET";
        private const string COL_MAILINGSTREET1EXTENDED = "MAILINGSTREET1EXTENDED";
        private const string COL_MAILINGSTREET2EXTENDED = "MAILINGSTREET2EXTENDED";
        private const string COL_MAILINGZIP = "MAILINGZIP";
        private const string COL_MIDDLEINITIAL = "MiddleInitial";
        private const string COL_TITLE = "TITLE";
        private const string COL_MRN = "MRN";
        private const string COL_PHYSICALCITY = "PHYSICALCITY";
        private const string COL_PHYSICALSTATE = "PHYSICALSTATE";
        private const string COL_PHYSICALSTREET = "PHYSICALSTREET";
        private const string COL_PHYSICALSTREET1EXTENDED = "PHYSICALSTREET1EXTENDED";
        private const string COL_PHYSICALSTREET2EXTENDED = "PHYSICALSTREET2EXTENDED";
        private const string COL_PHYSICALZIP = "PHYSICALZIP";
        private const string COL_SEXCODE = "SexCode";
        private const string COL_SSN = "SSN";
        private const string COL_TYPEOFNAMEID = "TypeOfNameId";
        private const string COL_PATIENTIPACODE = "PATIENTIPACODE";
        private const string COL_PATIENTIPACLINICCODE = "PATIENTIPACLINICCODE";
        private const string FC_37 = "37";
        private const string PARAM_ACCOUNTNUMBER = "@P_ACCOUNTNUMBER";
        private const string PARAM_BIRTH_MONTH = "@P_MONTH";
        private const string PARAM_BIRTH_YEAR = "@P_YEAR";
        private const string PARAM_FIRST_NAME = "@P_FNAME";
        private const string PARAM_GENDER = "@P_GENDER";
        protected const string PARAM_HSPNUMBER = "@P_FACILITYID";
        private const string PARAM_LAST_NAME = "@P_LNAME";
        protected const string PARAM_MRN = "@P_MRN";
        private const string PARAM_SSN = "@P_SSN";
        private const string PARAM_HSPCODE = "@P_HSPCODE";
        private const string PARAM_MRC = "@P_MRC";
        private const string PARAM_PHONENUMBER = "@P_PHONENUMBER";
        private const string PARAM_DOB_YYYYMMDD = "@P_DOB";
        private const string SP_FIND_PATIENTS_BY_GUARANTOR_FOR =
            "CALL FINDPATIENTSBYGUARANTORFOR( @P_FACILITYID, @P_FNAME, @P_LNAME, @P_SSN, @P_GENDER )";
        private const string SP_FIND_PATIENTS_FOR =
            "CALL FINDPATIENTSFOR( @P_FACILITYID, @P_MRN, @P_ACCOUNTNUMBER, @P_SSN, @P_FNAME, @P_LNAME, @P_GENDER, @P_MONTH, @P_YEAR, @P_PHONENUMBER  )";
        private const string SP_MRN_FORACCOUNT =
            "MRNFORACCOUNT";
        private const string SP_SELECT_ALL_PATIENT_TYPES =
            "SELECTALLPATIENTTYPES";
        private const string SP_SELECT_PATIENT_TYPE_WITH =
            "SELECTPATIENTTYPEWITH";
        private const string SP_GET_ALIASES_FOR_PATIENT =
            "CALL GETALIASESFORPATIENT( @P_FACILITYID, @P_MRN )";
         
        private const string SP_SELECTSEQUESTEREDPATIENTS =
            "CALL SELECTSEQUESTEREDPATIENTS( @P_FACILITYID, @P_MRN, @P_LNAME,   @P_FNAME,@P_DOB,  @P_GENDER  )";

        private const string COL_NUMBEROFSEQUESTEREDPATIENTS = "NUMBEROFSEQUESTEREDPATIENTS";

        private const string SP_GETNEWMRN = "GETNEWMR#";
        private const string PARAM_HOSPITALCODE = "@IHOSPCODE";
        private const string OUT_PARAM_MRN = "@ONEWMR#";

        private const string SP_GETMOSTRECENTACCDETAILSFORAPATIENT = "GETMOSTRECENTACCDETAILSFORAPATIENT";
        private const string COL_ACC_NUMBER = "ACCOUNTNUMBER";
        private const string COL_ACCREATEDDATE = "ACENTD";
        private const string SP_GETIPAFORPATIENT = "GETIPAFORPATIENT";
        private const string ErrorIPARetreival = "Error retrieving IPA at the patient level";
        #endregion Constants

        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger( typeof( PatientPBARBroker ) );

        #endregion Fields

        #region Enums

        private enum PatientTypeIndex
        {
            BLANK = 0,
            PREREG = 1,
            INPATIENT = 2,
            OUTPATIENT = 3,
            ER = 4,
            RECURRING = 5,
            NONPAT = 6
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="PatientPBARBroker"/> class.
        /// </summary>
        static PatientPBARBroker()
        {
            Model.IsTrackingEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientPBARBroker"/> class.
        /// </summary>
        public PatientPBARBroker()
        {
            Model.IsTrackingEnabled = false;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// AllPatientTypes - (Cache and) return a list of all patient types
        /// </summary>
        /// <returns>patient type list</returns>
        /// <exception cref="Exception">PatientBroker failed to initialize</exception>
        
        public ICollection AllPatientTypes(long facilityID)
        {
            ICollection patientTypes = null;
            var key = CacheKeys.CACHE_KEY_FOR_PATIENTTYPES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    patientTypes = LoadDataToArrayList<VisitType>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("PatientBroker failed to initialize", e, Logger);
                }
                return patientTypes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_PATIENT_TYPES;
                patientTypes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("PatientBroker failed to initialize", e, Logger);
            }
            return patientTypes;
        }

        /// <summary>
        /// Gets the patient search response for.
        /// </summary>
        /// <param name="patientCriteria">The patient criteria.</param>
        /// <returns></returns>
        public PatientSearchResponse GetPatientSearchResponseFor( PatientSearchCriteria patientCriteria )
        {

            IFacilityBroker facilityBroker =
                BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility =
                facilityBroker.FacilityWith( patientCriteria.HSPCode );
            iDB2Command db2Command =
                CreateCommandFromSearchCriteriaFor( patientCriteria, facility );

            return GetPatientSearchResponseFor( db2Command, patientCriteria.HSPCode );

        }

        /// <summary>
        /// Gets the patient search response for.
        /// </summary>
        /// <param name="guarantorCriteria">The guarantor criteria.</param>
        /// <returns></returns>
        public PatientSearchResponse GetPatientSearchResponseFor( GuarantorSearchCriteria guarantorCriteria )
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            Facility facility = facilityBroker.FacilityWith( guarantorCriteria.HSPCode );

            iDB2Command db2Command = CreateCommandFromSearchCriteriaFor( guarantorCriteria, facility );

            return GetPatientSearchResponseFor( db2Command, guarantorCriteria.HSPCode );
        }

        public MedicalGroupIPA GetIpaForPatient( Patient patient )
        {
            var ipa = new MedicalGroupIPA();
            iDB2Command cmd = null;
            SafeReader ipaReader = null;
            try
            {
                var parameters = new[] { PARAM_HSPNUMBER, PARAM_MRC };
                var cmdTextForCallingGetIpaForPatient = new Db2StoredProcedureCallBuilder( parameters, SP_GETIPAFORPATIENT )
                                                        .Build();
                cmd = CommandFor( cmdTextForCallingGetIpaForPatient, CommandType.Text, patient.Facility );
                cmd.Parameters[PARAM_HSPNUMBER].Value = patient.Facility.Oid;
                cmd.Parameters[PARAM_MRC].Value = patient.MedicalRecordNumber;

                ipaReader = ExecuteReader( cmd );

                while ( ipaReader.Read() )
                {
                    var patientIpaCode = ipaReader.GetString( COL_PATIENTIPACODE );
                    var patientIpaClinicCode = ipaReader.GetString( COL_PATIENTIPACLINICCODE );
                    if ( patientIpaCode != null & patientIpaClinicCode != null )
                    {
                        ipa = insuranceBroker.IPAWith( patient.Facility.Oid, patientIpaCode, patientIpaClinicCode );
                    }
                }

                return ipa;
            }

            catch ( Exception generalException )
            {
                Log.Error( ErrorIPARetreival, generalException );
                throw;
            }

            finally
            {
                Close( ipaReader );
                Close( cmd );
            }
        }

        /// <summary>
        /// MRNForAccount - return the medical record number for the selected account
        /// </summary>
        /// <param name="accountNumber">a long, account number</param>
        /// <param name="facilityCode">a string, facility code</param>
        /// <returns>a long, medical record number</returns>
        /// <exception cref="Exception">PatientsMatching for the given PatientSearchCriteria method failed with an unknown error.</exception>
        public long MRNForAccount( long accountNumber, string facilityCode )
        {

            iDB2Command cmd = null;
            SafeReader reader = null;
            long medicalRecordNumber = -1;

            IFacilityBroker facilityBroker =
                BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility =
                facilityBroker.FacilityWith( facilityCode );

            try
            {
                cmd = CommandFor( "CALL " + SP_MRN_FORACCOUNT +
                    "(" + PARAM_ACCOUNTNUMBER +
                    "," + PARAM_HSPNUMBER + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                cmd.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    medicalRecordNumber = reader.GetInt64( COL_MRN );
                }

            }
            catch ( Exception ex )
            {
                const string message = "PatientsMatching for the given " +
                                   "PatientSearchCriteria method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Logger );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return medicalRecordNumber;
        }


        /// <summary>
        /// Gets the new MRN for the given facility.
        /// </summary>
        /// <param name="facility">The facility.</param>
        /// <returns></returns>
        /// <exception cref="Exception">when a new MRN could not be obtained.</exception>
        /// <exception cref="ArgumentNullException"><c>facility</c> is null.</exception>
        /// <exception cref="EnterpriseException">if an invalid mrn (zero or non integer) is returned by PBAR.</exception>
        public long GetNewMrnFor( Facility facility )
        {
            Guard.ThrowIfArgumentIsNull( facility, "facility" );

            IDbCommand newMrnCommand = null;
            try
            {
                newMrnCommand = GetCommandForNewMrnStoredProcedure( facility );

                object rawMrn = TryExecutingNewMrnNumberStoredProcedure( newMrnCommand );

                if ( IntegerHelper.IsPositiveInteger( rawMrn ) )
                {
                    return Convert.ToInt32( rawMrn );
                }

                else
                {
                    string message = string.Format( "Invalid MRN:{0} returned for facility: {1}", rawMrn, facility.Code );
                    throw new EnterpriseException( message );
                }
            }

            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Logger );
            }

            finally
            {
                Close( newMrnCommand );
            }
        }

        /// <summary>
        /// PatientTypeFor - return the patient age
        /// </summary>
        /// <param name="dateOfBirth">date of birth</param>
        /// <param name="facilityCode">facility code</param>
        /// <returns>a int, age</returns>
        public int PatientAgeFor( DateTime dateOfBirth, string facilityCode )
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            Facility facility = facilityBroker.FacilityWith( facilityCode );

            ITimeBroker timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();

            DateTime facilityTime = timeBroker.TimeAt( facility.GMTOffset, facility.DSTOffset );

            Patient patient = new Patient();

            patient.DateOfBirth = dateOfBirth;

            int ageAtFacility = patient.AgeInYearsFor( facilityTime );

            return ageAtFacility;
        }

        /// <summary>
        ///Creates a Patient instance based on <see cref="PatientSearchResult"/>
        /// </summary>
        /// <param name="patientResult">The patient result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>patientResult</c> is null.</exception>
        public virtual Patient PatientFrom( PatientSearchResult patientResult )
        {
            if ( patientResult == null )
            {
                throw new ArgumentNullException( "patientResult", "Parameter PatientSearchResult should not be null." );
            }

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith( patientResult.HspCode );

            Name patientName = patientResult.Name;

            if ( patientResult.Name.TypeOfName == TypeOfName.Alias )
            {
                // For an alias result, the real name is the only AKA entry
                patientName = patientResult.AkaNames[0];
            }

            Patient newPatient =
                new Patient( PersistentModel.NEW_OID,
                             PersistentModel.NEW_VERSION,
                             patientName,
                             patientResult.MedicalRecordNumber,
                             patientResult.DateOfBirth,
                             new SocialSecurityNumber( patientResult.SocialSecurityNumber ),
                             patientResult.Gender,
                             facility );

            AddPatientDetailsTo( newPatient );

            return newPatient;
        }

        /// <summary>
        /// Patients the types for.
        /// </summary>
        /// <param name="activityType">Type of the activity.</param>
        /// <param name="associatedActivityType">Type of Associated Activity</param>
        /// <param name="kindOfVisitCode">The kind of visit code.</param>
        /// <param name="financialClassCode">The financial class code.</param>
        /// <param name="locationBedCode">The location bed code.</param>
        /// <param name="facilityID">The facility ID.</param>
        /// <returns></returns>
        public ArrayList PatientTypesFor( string activityType, string associatedActivityType, string kindOfVisitCode, string financialClassCode, string locationBedCode, long facilityID )
        {
            InitFacility( facilityID );

            // TLG 2/8/2006 - modified per UC166

            ArrayList allPatientTypes = ( ArrayList )AllPatientTypes( facilityID );
            ArrayList patientTypesList = new ArrayList();
			if ( activityType == null || kindOfVisitCode == null )
            {
                return patientTypesList;
            }

            if ( ( typeof( MaintenanceActivity ) ).ToString().Equals( activityType ) )
            {
                switch ( kindOfVisitCode )
                {
                    case VisitType.PREREG_PATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.PREREG] );
                            break;
                        }
                    case VisitType.INPATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.INPATIENT] );
                            break;
                        }
                    case VisitType.RECURRING_PATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.RECURRING] );
                            break;
                        }
                    case VisitType.NON_PATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.NONPAT] );
                            break;
                        }
                    case VisitType.EMERGENCY_PATIENT:
                        {
                            // TLG 04-10-2006 per UC166

                            if ( financialClassCode != null && financialClassCode != FC_37 )
                            {
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.BLANK] );
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.OUTPATIENT] );
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.ER] );
                            }
                            else
                            {
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.ER] );
                            }
                            break;
                        }
                    case VisitType.OUTPATIENT:
                        {
                            // if bed assigned, only OP

                            if ( locationBedCode != null
                                && locationBedCode.Trim() != string.Empty )
                            {
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.OUTPATIENT] );
                            }
                            else
                            {
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.BLANK] );
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.OUTPATIENT] );
                                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.ER] );
                            }

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else if ( ( typeof( ShortMaintenanceActivity ) ).ToString().Equals( activityType ) )
            {
                switch ( kindOfVisitCode )
                {
                    case VisitType.PREREG_PATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.PREREG] );
                            break;
                        }
                    case VisitType.RECURRING_PATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.RECURRING] );
                            break;
                        }
                    case VisitType.OUTPATIENT:
                        {
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.BLANK] );
                            patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.OUTPATIENT] );

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            //RegistrationActivity
            else if ( ( typeof( RegistrationActivity ) ).ToString().Equals( activityType ) )
            {
                // TLG 2/8/2006

                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.BLANK] );
                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.INPATIENT] );
                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.OUTPATIENT] );
                if ( !associatedActivityType.Contains( "ActivatePreRegistrationActivity" ) )
                {
                    patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.ER] );
                }
                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.RECURRING] );
                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.NONPAT] );

            }
            else if ( ( typeof( PreRegistrationActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[( int )PatientTypeIndex.PREREG] );
            }
            else if ( ( typeof( QuickAccountCreationActivity ) ).ToString().
               Equals( activityType ) )
            {
                patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.PREREG]);
            }
            else if ( ( typeof( QuickAccountMaintenanceActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.PREREG]);
            }
			
            else if ((typeof ( PAIWalkinOutpatientCreationActivity )).ToString().
                Equals(activityType))
            {
                patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.PREREG]);
            }
                //ShortRegistrationActivity
            else if ( ( typeof( ShortRegistrationActivity ) ).ToString().Equals( activityType ) )
            {
                // TLG 2/8/2006

                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.BLANK] );
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.OUTPATIENT] );
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.RECURRING] );
            }
            else if ( ( typeof( ShortPreRegistrationActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.PREREG] );
            }
            else if ( ( typeof( AdmitNewbornActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.INPATIENT] );
            }
            else if ( ( typeof( PreAdmitNewbornActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.PREREG] );
            }
            else if ( ( typeof( PreMSERegisterActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.ER] );
            }
            else if ( ( typeof( PostMSERegistrationActivity ) ).ToString().
                Equals( activityType ) )
            {
                patientTypesList.Add( allPatientTypes[(int)PatientTypeIndex.ER] );
            }

            return patientTypesList;
        }
        /// <summary>
        /// Patients the types for walkin account
        /// </summary>
        /// <param name="facilityID">The facility ID.</param>
        /// <returns></returns>
        public ArrayList PatientTypesForWalkinAccount( long facilityID )
        {
            InitFacility(facilityID);

            ArrayList allPatientTypes = (ArrayList) AllPatientTypes(facilityID);
            ArrayList patientTypesList = new ArrayList();

            patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.BLANK]);
            patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.OUTPATIENT]);
            patientTypesList.Add(allPatientTypes[(int) PatientTypeIndex.RECURRING]);
            return patientTypesList;
        }
        public ArrayList PatientTypesForUCCAccount(long facilityID)
        {
            ArrayList patientTypesList = new ArrayList();
            patientTypesList.Add(VisitType.UCCOutpatient); 
            return patientTypesList;
        }

        /// <summary>
        /// PatientTypeWith - return the patient type for the specified code
        /// </summary>
        /// <param name="facilityID">a Facility ID</param>
        /// <param name="code">a PT code</param>
        /// <returns>a <see cref="VisitType"/> instance</returns>        
        /// <exception cref="ArgumentNullException"><c>code</c> is null.</exception>
        /// <exception cref="Exception"><c>AdmitSourceEmployerBroker</c> failed to retrieve object</exception>
        public VisitType PatientTypeWith( long facilityID, string code )
        {
            if ( code == null )
            {
                throw new ArgumentNullException( "code", "parameter cannot be null or empty." );
            }

            code = code.Trim();

            InitFacility( facilityID );

            VisitType selectedPatientType = null;

            try
            {
                ArrayList allPatientTypes = ( ArrayList )AllPatientTypes( facilityID );

                foreach ( VisitType visitType in allPatientTypes )
                {
                    if ( visitType.Code.Equals( code ) )
                    {
                        selectedPatientType = visitType;
                        break;
                    }
                }

                // didn't find it in the list see if it is in the database
                if ( selectedPatientType == null )
                {
                    selectedPatientType = CodeWith<VisitType>(facilityID, code);
                }
            }

            catch ( Exception exception )
            {
                Logger.Error( "Error getting PatientType", exception );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "AdmitSourceEmployerBroker failed to retrieve object", exception, Logger );

            }

            return selectedPatientType;
        }

        /// <summary>
        /// SparsePatientWith - return a streamlined patient
        /// </summary>
        /// <param name="mrn"></param>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <returns>patient object</returns>
        /// <exception cref="BrokerException"><c>BrokerException</c>.</exception>
        /// <exception cref="Exception">Unexpected Exception</exception>
        public virtual Patient SparsePatientWith( long mrn, string facilityCode )
        {
            Patient patient = null;

            try
            {
                PatientSearchCriteria patientSearchCriteria =
                    new PatientSearchCriteria( facilityCode,
                                               string.Empty,
                                               string.Empty,
                                               string.Empty,
                                               null,
                                               -1,
                                               -1,
                                               mrn.ToString(),
                                               string.Empty );

                PatientSearchResponse patientSearchResponse =
                    GetPatientSearchResponseFor( patientSearchCriteria );

                List<PatientSearchResult> realNameSearchResults =
                    patientSearchResponse.GetResultsOfType( TypeOfName.Normal );

                if ( realNameSearchResults.Count > 1 )
                {
                    throw new BrokerException( "Query for patient by MRN returned multiple patients: " +
                        mrn + " at: " + facilityCode );
                }

                if ( realNameSearchResults.Count == 1 )
                {
                    PatientSearchResult theSearchResult = realNameSearchResults[0];

                    IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    Facility theFacility = facilityBroker.FacilityWith( facilityCode );

                    patient =
                        new Patient( PersistentModel.NEW_OID,
                                     PersistentModel.NEW_VERSION,
                                     theSearchResult.Name,
                                     theSearchResult.MedicalRecordNumber,
                                     theSearchResult.DateOfBirth,
                                     new SocialSecurityNumber( theSearchResult.SocialSecurityNumber ),
                                     theSearchResult.Gender,
                                     theFacility );

                    IEmployerBroker employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();

                    patient = employerBroker.EmployerFor( patient, patient.Facility.Oid );

                    IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

                    ArrayList contactPoints = addressBroker.ContactPointsForPatient( patient.Facility.Code,
                                                               patient.MedicalRecordNumber );

                    foreach ( ContactPoint contactPoint in contactPoints )
                    {
                        patient.AddContactPoint( contactPoint );

                    }
                }

                return patient;
            }

            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, Logger );
            }
        }

        /// <summary>
        /// Normalizes the middle initial for PBAR names.
        /// </summary>
        /// <param name="aName">A name.</param>
        /// <returns></returns>
        internal static Name NormalizeMiddleInitialFor( Name aName )
        {
            string firstName = aName.FirstName;

            if ( HasSpaceAndACharacterAtTheEnd( firstName ) )
            {
                int lengthOfFirstName = firstName.Length;

                aName.MiddleInitial = firstName[lengthOfFirstName - 1].ToString();

                aName.FirstName = firstName.Remove( lengthOfFirstName - 2 );
            }

            return aName;
        }

        /// <summary>
        /// Inits the proc names.
        /// </summary>
        protected override void InitProcNames()
        {
            AllStoredProcName = SP_SELECT_ALL_PATIENT_TYPES;
            WithStoredProcName = SP_SELECT_PATIENT_TYPE_WITH;
        }

        /// <summary>
        /// Gets the aliases for patient.
        /// </summary>
        /// <param name="facility">The facility.</param>
        /// <param name="medicalRecordNumber">The medical record number.</param>
        /// <returns></returns>
        private List<Name> GetAliasesForPatient( Facility facility, long medicalRecordNumber )
        {
            List<Name> aliasList = new List<Name>();
            iDB2Command databaseCommand = null;
            SafeReader reader = null;

            try
            {
                databaseCommand =
                    CommandFor( SP_GET_ALIASES_FOR_PATIENT,
                                     CommandType.Text,
                                     facility );

                databaseCommand.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                databaseCommand.Parameters[PARAM_MRN].Value = medicalRecordNumber;

                reader = ExecuteReader( databaseCommand );

                while ( reader.Read() )
                {
                    string firstName = reader.GetString( "FIRSTNAME" );
                    firstName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    string lastName = reader.GetString( "LASTNAME" );
                    lastName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    Name newAlias = new Name( firstName, lastName, String.Empty, String.Empty, TypeOfName.Alias );

                    newAlias.EntryDate = reader.GetDateTime( "ENTRYDATE" );

                    aliasList.Add( NormalizeMiddleInitialFor( newAlias ) );
                }
            }

            finally
            {
                Close( reader );
                Close( databaseCommand );
            }

            return aliasList;
        }

        private IDbCommand GetCommandForNewMrnStoredProcedure( Facility facility )
        {
            string commandText = String.Format( "CALL {0}( {1},{2})", SP_GETNEWMRN, PARAM_HOSPITALCODE, OUT_PARAM_MRN );

            iDB2Command cmd = CommandFor( commandText, CommandType.Text, facility );

            cmd.Parameters[PARAM_HOSPITALCODE].Value = facility.Code;
            cmd.Parameters[OUT_PARAM_MRN].Direction = ParameterDirection.Output;
            return cmd;
        }

        /// <summary>
        /// Tries the executing new MRN number stored procedure.
        /// This method is made public and virtual to enable testing it is not meant to be used by the clients of this class
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The MRN returned by PBAR as an object</returns>
        public virtual object TryExecutingNewMrnNumberStoredProcedure( IDbCommand command )
        {
            command.ExecuteNonQuery();

            IDbDataParameter parameter = ( IDbDataParameter )command.Parameters[OUT_PARAM_MRN];

            return parameter.Value;
        }

        private static bool HasSpaceAndACharacterAtTheEnd( string firstName )
        {
            return Regex.IsMatch( firstName, @"[\s][A-Z]\b$" );
        }

        /// <summary>
        /// Creates a Patient instance based on the PatientSe
        /// <see cref="PatientSearchResult"/> </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unexpected Exception</exception>
        private void AddPatientDetailsTo( Patient patient )
        {
            try
            {
                patient.MedicalGroupIPA = GetIpaForPatient(patient);
                IAccountBroker acctBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                ArrayList accounts = acctBroker.AccountsFor( patient );

                patient.AddAccounts( accounts );

                IEmployerBroker employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
                patient = employerBroker.EmployerFor( patient, patient.Facility.Oid );

                IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

                ArrayList contactPoints = addressBroker.ContactPointsForPatient( patient.Facility.Code, patient.MedicalRecordNumber );
                foreach ( ContactPoint cp in contactPoints )
                {
                    patient.AddContactPoint( cp );
                }

                List<Name> patientAliases = GetAliasesForPatient( patient.Facility,
                                               patient.MedicalRecordNumber );

                foreach ( Name akaName in patientAliases )
                {
                    patient.AddAlias( akaName );
                }
            }

            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, Logger );
            }

            return;
        }

        /// <summary>
        /// Creates the command from search criteria for.
        /// </summary>
        /// <param name="patientCriteria">The patient criteria.</param>
        /// <param name="facility">The facility.</param>
        /// <returns></returns>
        private iDB2Command CreateCommandFromSearchCriteriaFor( PatientSearchCriteria patientCriteria, Facility facility )
        {
            iDB2Command db2Command = CommandFor( SP_FIND_PATIENTS_FOR, CommandType.Text, facility );

            db2Command.Parameters[PARAM_FACILITYID].Value = facility.Oid;


            if ( !string.IsNullOrEmpty( patientCriteria.MedicalRecordNumber ) )
            {
                db2Command.Parameters[PARAM_MRN].Value = patientCriteria.MedicalRecordNumber;
            }

            if ( !string.IsNullOrEmpty( patientCriteria.AccountNumber ) )
            {
                db2Command.Parameters[PARAM_ACCOUNTNUMBER].Value = patientCriteria.AccountNumber;
            }

            if ( !string.IsNullOrEmpty( patientCriteria.SocialSecurityNumber.UnformattedSocialSecurityNumber ) )
            {
                db2Command.Parameters[PARAM_SSN].Value =
                    patientCriteria.SocialSecurityNumber.UnformattedSocialSecurityNumber;
            }

            if ( !string.IsNullOrEmpty( patientCriteria.FirstName ) )
            {
                db2Command.Parameters[PARAM_FIRST_NAME].Value =
                    StringFilter.mangleName( patientCriteria.FirstName );
            }

            if ( !string.IsNullOrEmpty( patientCriteria.LastName ) )
            {
                db2Command.Parameters[PARAM_LAST_NAME].Value =
                    StringFilter.mangleName( patientCriteria.LastName );
            }

            if ( patientCriteria.Gender != null && !string.IsNullOrEmpty( patientCriteria.Gender.Code ) )
            {
                db2Command.Parameters[PARAM_GENDER].Value =
                    patientCriteria.Gender.Code;
            }

            if ( patientCriteria.MonthOfBirth >= 1 && patientCriteria.MonthOfBirth <= 12 )
            {
                db2Command.Parameters[PARAM_BIRTH_MONTH].Value = patientCriteria.MonthOfBirth;
            }

            if ( patientCriteria.YearOfBirth > 0 )
            {
                db2Command.Parameters[PARAM_BIRTH_YEAR].Value = patientCriteria.YearOfBirth;
            }
            if ( patientCriteria.PhoneNumber != null && !string.IsNullOrEmpty(patientCriteria.PhoneNumber.AreaCode) &&
                !string.IsNullOrEmpty(patientCriteria.PhoneNumber.Number ))
            {
                db2Command.Parameters[PARAM_PHONENUMBER].Value =
                    patientCriteria.PhoneNumber.AsUnformattedString();
            }
            return db2Command;
        }

        /// <summary>
        /// Creates the command from search criteria for.
        /// </summary>
        /// <param name="guarantorCriteria">The guarantor criteria.</param>
        /// <param name="facility">The facility.</param>
        /// <returns></returns>
        private iDB2Command CreateCommandFromSearchCriteriaFor( GuarantorSearchCriteria guarantorCriteria, Facility facility )
        {
            iDB2Command db2Command =
                CommandFor( SP_FIND_PATIENTS_BY_GUARANTOR_FOR,
                                 CommandType.Text,
                                 facility );

            db2Command.Parameters[PARAM_FACILITYID].Value = facility.Oid;

            if ( !string.IsNullOrEmpty( guarantorCriteria.FirstName ) )
            {
                db2Command.Parameters[PARAM_FIRST_NAME].Value =
                    StringFilter.mangleName( guarantorCriteria.FirstName );
            }//if

            if ( !string.IsNullOrEmpty( guarantorCriteria.LastName ) )
            {
                db2Command.Parameters[PARAM_LAST_NAME].Value =
                    StringFilter.mangleName( guarantorCriteria.LastName );
            }//if

            if ( !string.IsNullOrEmpty( guarantorCriteria.SocialSecurityNumber
                                           .UnformattedSocialSecurityNumber ) )
            {
                db2Command.Parameters[PARAM_SSN].Value =
                    guarantorCriteria.SocialSecurityNumber.UnformattedSocialSecurityNumber;
            }//if

            if ( guarantorCriteria.Gender != null &&
                !string.IsNullOrEmpty( guarantorCriteria.Gender.Code ) )
            {
                db2Command.Parameters[PARAM_GENDER].Value =
                    guarantorCriteria.Gender.Code;
            }//if

            return db2Command;
        }

        /// <summary>
        /// Gets the patient search response for.
        /// </summary>
        /// <param name="db2Command">The DB2 command.</param>
        /// <param name="facilityCode">The facility code.</param>
        /// <returns></returns>
        private PatientSearchResponse GetPatientSearchResponseFor( IDbCommand db2Command, string facilityCode )
        {
            PatientSearchResponse patientSearchResponse = new PatientSearchResponse();
            SafeReader safeReader = null;

            try
            {
                safeReader = ExecuteReader( db2Command );

                patientSearchResponse.PatientSearchResults =
                    BuildPatientsSearchResultsFrom( safeReader, facilityCode );

                patientSearchResponse.PatientSearchResultStatus =
                    PatientSearchResultStatus.Success;
            }

            catch ( Exception anyException )
            {
                Logger.Error( "Patient Search failed with an exception", anyException );

                patientSearchResponse.PatientSearchResultStatus = PatientSearchResultStatus.Exception;
            }

            finally
            {
                Close( safeReader );
                Close( db2Command );
            }

            return patientSearchResponse;
        }

        /// <summary>
        /// Builds the patients search results from.
        /// </summary>
        /// <param name="aReader">A reader.</param>
        /// <param name="hospitalCode">The hospital code.</param>
        /// <returns></returns>
        private static List<PatientSearchResult> BuildPatientsSearchResultsFrom( SafeReader aReader, string hospitalCode )
        {
            List<PatientSearchResult> patientSearchResults = new List<PatientSearchResult>();
            IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            PatientSearchResult newPatientSearchResult = null;
            Facility theFacility = facilityBroker.FacilityWith( hospitalCode );

            while ( aReader.Read() )
            {
                DateTime dateOfBirth;
                TypeOfName typeOfName = ( TypeOfName )aReader.GetInt64( COL_TYPEOFNAMEID );

                string socialSecurityNumber = aReader.GetString( COL_SSN );

                Gender gender =
                    demographicsBroker.GenderWith( theFacility.Oid, aReader.GetString( COL_SEXCODE ) );

                long medicalRecordNumber = aReader.GetInt64( COL_MRN );

                string displayFirstName = aReader.GetString( COL_DFIRSTNAME ).Trim();
                displayFirstName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( displayFirstName );

                string displayLastName = aReader.GetString( COL_DLASTNAME ).Trim();
                displayLastName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( displayLastName );

                string displayMiddleInitial = aReader.GetString( COL_DMIDDLEINITIAL ).Trim();
                displayMiddleInitial = StringFilter.RemoveFirstCharNonLetter( displayMiddleInitial );

                string firstName = aReader.GetString( COL_FIRSTNAME ).Trim();
                firstName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                string lastName = aReader.GetString( COL_LASTNAME ).Trim();
                lastName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                string middleInitial = aReader.GetString( COL_MIDDLEINITIAL ).Trim();
                middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                string title = aReader.GetString( COL_TITLE ).Trim();
                title = StringFilter.RemoveFirstCharNonLetterAndRestNonLetter( title );

                Address address = new Address();

                DateTime.TryParseExact( aReader.GetString( COL_DOB ),
                                        "MMddyyyy",
                                        CultureInfo.InvariantCulture.DateTimeFormat,
                                        DateTimeStyles.None,
                                        out dateOfBirth );


                string physicalStreet1 = aReader.GetString(COL_PHYSICALSTREET1EXTENDED).Trim();
                if ( string.IsNullOrEmpty(physicalStreet1) )
                {
                    physicalStreet1 = aReader.GetString( COL_PHYSICALSTREET ).Trim();
                }

                string physicalStreet2 = aReader.GetString( COL_PHYSICALSTREET2EXTENDED ).Trim();
                if ( !string.IsNullOrEmpty(physicalStreet2) )
                {
                    physicalStreet2 = aReader.GetString(COL_PHYSICALSTREET2EXTENDED).Trim();
                    physicalStreet2 = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash(physicalStreet2);
                    address.Address2 = physicalStreet2;
                }

                if ( !string.IsNullOrEmpty( physicalStreet1 ) )
                {
                    physicalStreet1 = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( physicalStreet1 );
                    address.Address1 = physicalStreet1;

                    string city = aReader.GetString( COL_PHYSICALCITY ).Trim();
                    city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );
                    address.City = city;

                    string state = aReader.GetString( COL_PHYSICALSTATE ).Trim();
                    address.State = new State( state );

                    string zip = aReader.GetString( COL_PHYSICALZIP ).Trim();
                    zip = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( zip );
                    address.ZipCode = new ZipCode( zip );
                }//if
                else
                {
                    string mailingStreet1 = aReader.GetString( COL_MAILINGSTREET1EXTENDED ).Trim();
                    if ( string.IsNullOrEmpty(mailingStreet1) )
                    {
                        mailingStreet1 = aReader.GetString(COL_MAILINGSTREET).Trim();
                    }
                    string mailingStreet2 = aReader.GetString(COL_MAILINGSTREET2EXTENDED).Trim();
                    if (!string.IsNullOrEmpty(mailingStreet2))
                    {
                        mailingStreet2 = StringFilter.
                            RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash(mailingStreet2);
                        address.Address2 = mailingStreet2;
                    }
                    if ( !string.IsNullOrEmpty( mailingStreet1 ) )
                    {
                        mailingStreet1 = StringFilter.
                            RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( mailingStreet1 );
                        address.Address1 = mailingStreet1;

                        string city = aReader.GetString( COL_MAILINGCITY ).Trim();
                        city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );
                        address.City = city;

                        string state = aReader.GetString( COL_MAILINGSTATE ).Trim();
                        address.State = new State( state );

                        string zip = aReader.GetString( COL_MAILINGZIP ).Trim();
                        zip = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( zip );
                        address.ZipCode = new ZipCode( zip );
                    }
                }

                Name displayName =
                            new Name( displayFirstName,
                                      displayLastName,
                                      displayMiddleInitial,
                                      title,
                                      typeOfName );

                switch ( typeOfName )
                {
                    case TypeOfName.Alias:

                        Name realName =
                            new Name( firstName,
                                      lastName,
                                      middleInitial,
                                      title,
                                      TypeOfName.Normal );

                        newPatientSearchResult =
                            new PatientSearchResult( NormalizeMiddleInitialFor( displayName ),
                                                     gender,
                                                     dateOfBirth,
                                                     socialSecurityNumber,
                                                     medicalRecordNumber,
                                                     address,
                                                     hospitalCode );

                        newPatientSearchResult.AkaNames.Add( NormalizeMiddleInitialFor( realName ) );

                        patientSearchResults.Add( newPatientSearchResult );

                        break;

                    case TypeOfName.Normal:

                        if ( newPatientSearchResult == null ||
                            newPatientSearchResult.Name.TypeOfName != TypeOfName.Normal ||
                            newPatientSearchResult.MedicalRecordNumber != medicalRecordNumber )
                        {
                            newPatientSearchResult =
                                new PatientSearchResult( NormalizeMiddleInitialFor( displayName ),
                                                         gender,
                                                         dateOfBirth,
                                                         socialSecurityNumber,
                                                         medicalRecordNumber,
                                                         address,
                                                         hospitalCode );

                            patientSearchResults.Add( newPatientSearchResult );
                        }

                        if ( !string.IsNullOrEmpty( firstName ) && !string.IsNullOrEmpty( lastName ) )
                        {
                            Name aliasName =
                                new Name( firstName,
                                          lastName,
                                          middleInitial,
                                          title,
                                          TypeOfName.Alias );

                            newPatientSearchResult.AkaNames.Add( NormalizeMiddleInitialFor( aliasName ) );
                        }

                        break;
                }
            }

            return patientSearchResults;
        }

        /// <summary>
        ///Set the most recently created account details for a patient.
        /// <see cref="PatientSearchResult"/> </summary>
        ///<param name="medicalRecordNumber"> </param>
        ///<param name="facility">Facility</param>
        ///<returns></returns>
        /// <exception cref="Exception">Unexpected Exception</exception>
        public RecentAccountDetails GetMostRecentAccountDetailsFor(long medicalRecordNumber, Facility facility)
        {
            Guard.ThrowIfArgumentIsNull(facility, "faclity");

            SafeReader reader = null;

            var parameters = new[] { PARAM_HSPNUMBER, PARAM_HSPCODE, PARAM_MRC };

            var cmdTextForCallingGetMostRecentAccountDetails = new Db2StoredProcedureCallBuilder(parameters, SP_GETMOSTRECENTACCDETAILSFORAPATIENT).Build();

            var command = CommandFor(cmdTextForCallingGetMostRecentAccountDetails, CommandType.Text, facility);
            
            command.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;
            command.Parameters[PARAM_HSPCODE].Value = facility.Code;
            command.Parameters[PARAM_MRC].Value = medicalRecordNumber;

            long mostRecentAccountNumber = 0;
            var mostRecentAccountCreationDate = DateTime.MinValue;
            
            try
            {
                reader = ExecuteReader(command);

                if ( reader.Read() )
                {
                    mostRecentAccountNumber = reader.GetInt32(COL_ACC_NUMBER);
                    mostRecentAccountCreationDate = DateTimeUtilities.DateTimeForYYMMDDFormat(reader.GetInt64(COL_ACCREATEDDATE));
                }

            }
                
            catch (Exception generalException)
            {
                Logger.Error("Error getting recent account detials for the patient", generalException);
                throw;
            }

            finally
            {
                Close(reader);
                Close(command);
            }
            
            var result = new RecentAccountDetails(mostRecentAccountNumber, mostRecentAccountCreationDate);

            return result;
        }

        public Boolean IsPatientSequestered(Account account)
        {

            long facilityId = account.Facility.Oid;
            long patientMRN = account.Patient.MedicalRecordNumber;
            SafeReader reader = null;
            iDB2Command db2Command = null;
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(facilityId);
                db2Command = CommandFor(SP_SELECTSEQUESTEREDPATIENTS, CommandType.Text, facility);

                db2Command.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                db2Command.Parameters[PARAM_MRN].Value = patientMRN;

                db2Command.Parameters[PARAM_LAST_NAME].Value = account.Patient.Name.LastName;
                db2Command.Parameters[PARAM_FIRST_NAME].Value = account.Patient.Name.FirstName;
                var patientDOB = account.Patient.DateOfBirth; 
                db2Command.Parameters[PARAM_DOB_YYYYMMDD].Value = (patientDOB != DateTime.MinValue) ? patientDOB.ToString("yyyy-MM-dd") : "1800-01-01";
                db2Command.Parameters[PARAM_GENDER].Value = account.Patient.Sex.Code;

                reader = ExecuteReader(db2Command);

                while (reader.Read())
                {
                    long sequesteredPatientsCount = reader.GetInt64(COL_NUMBEROFSEQUESTEREDPATIENTS);

                    if (sequesteredPatientsCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "IsPatientSequestered method failed for Facility:" + facilityId +
                             " MRN: " + patientMRN;
                Log.Error(msg, ex);
                throw;
            }
            finally
            {
                Close(reader);
                Close(db2Command);
            }

            return false;
        }

        #endregion Methods

        #region Data Elements
        private readonly IInsuranceBroker insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        private static readonly ILog Log = LogManager.GetLogger(typeof(PatientPBARBroker));
        

        #endregion 
    }
}