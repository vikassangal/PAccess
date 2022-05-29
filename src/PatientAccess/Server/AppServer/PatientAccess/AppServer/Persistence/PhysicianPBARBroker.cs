using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;
using log4net;
using System.Text;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for PhysicianPBARBroker.
	/// </summary>
    [Serializable]
    public class PhysicianPBARBroker : PBARCodesBroker, IPhysicianBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            AllStoredProcName = string.Empty;
            WithStoredProcName = string.Empty;
        }

        /// <summary>
        /// Getting a PhysicianSpeciality Name and Number based on Name
        /// </summary>
        /// <param name="physicianSearchCriteria"></param>
        /// <returns></returns>
        public ICollection PhysiciansMatching( PhysicianSearchCriteria physicianSearchCriteria )
        {
            iDB2Command cmd       = null;
            SafeReader reader     = null;
            ArrayList physicians    = new ArrayList();

            try
            {
                cmd = CommandFor( "CALL " + SP_PHYSICIANS_MATCHING +
                    "(" + PARAM_FACILITYID + 
                    "," + PARAM_LASTNAME + 
                    "," + PARAM_FIRSTNAME + 
                    "," + PARAM_PHYSICIANNUMBER + ")",
                    CommandType.Text,
                    physicianSearchCriteria.Facility);
                cmd.Parameters[PARAM_FACILITYID].Value = physicianSearchCriteria.Facility.Oid;
                
                if( physicianSearchCriteria.LastName != null && 
                    physicianSearchCriteria.LastName.Trim().Length > 0 )
                {
                    cmd.Parameters[PARAM_LASTNAME].Value = StringFilter.mangleName( physicianSearchCriteria.LastName );
                }
                else
                {
                    cmd.Parameters[PARAM_LASTNAME].Value = DBNull.Value;
                }

                if( physicianSearchCriteria.FirstName != null && 
                    physicianSearchCriteria.FirstName.Length > 0 )
                {
                    cmd.Parameters[PARAM_FIRSTNAME].Value = StringFilter.mangleName( physicianSearchCriteria.FirstName );
                }
                else
                {
                    cmd.Parameters[PARAM_FIRSTNAME].Value = DBNull.Value;
                }

                if( physicianSearchCriteria.PhysicianNumber != 0 ) 
                {
                    cmd.Parameters[PARAM_PHYSICIANNUMBER].Value = 
                        physicianSearchCriteria.PhysicianNumber;
                }
                else
                {
                    cmd.Parameters[PARAM_PHYSICIANNUMBER].Value = DBNull.Value;
                }

                reader = ExecuteReader( cmd );

                while( reader.Read() )
                {
                    Physician physician = PhysicianFrom(reader);

                    physicians.Add( physician );
                }
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return physicians;
        }

        public Physician PhysicianStatisticsFor( long facilityOid, long physicianNumber )
        {
            iDB2Command cmd      = null;
            SafeReader reader       = null;
            Physician physician;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityOid );
                
                cmd = CommandFor( "CALL " + SP_PHYSICIAN_STATISTICS +
                            "(" + PARAM_FACILITYID + 
                            "," + PARAM_PHYSICIANNUMBER + 
                            "," + OUTPUT_PARAM_O_TOTAL + 
                            "," + OUTPUT_PARAM_O_ATTENDING + 
                            "," + OUTPUT_PARAM_O_ADMITTING + 
                            "," + OUTPUT_PARAM_O_REFERRING  + 
                            "," + OUTPUT_PARAM_O_OPERATING + 
                            "," + OUTPUT_PARAM_O_CONSULTING + ")",
                            CommandType.Text,
                            facility);

                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_PHYSICIANNUMBER].Value = physicianNumber;

                cmd.Parameters[OUTPUT_PARAM_O_TOTAL].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_O_ATTENDING].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_O_ADMITTING].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_O_REFERRING].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_O_OPERATING].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_O_CONSULTING].Direction = ParameterDirection.Output;

                reader = ExecuteReader( cmd );

                int totalPatients;
                int attending;
                int admitting;
                int referring;
                int consulting;
                int operating;

                totalPatients = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_TOTAL].Value );
                attending = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_ATTENDING].Value );
                admitting = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_ADMITTING].Value );
                referring = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_REFERRING].Value );
                operating = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_OPERATING].Value );
                consulting = Convert.ToInt32( cmd.Parameters[OUTPUT_PARAM_O_CONSULTING].Value );
                physician = new Physician( physicianNumber, 
                    ReferenceValue.NEW_VERSION, 
                    totalPatients, 
                    attending, 
                    admitting, 
                    referring, 
                    consulting, 
                    operating ); 
            }
            
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return physician;
        }
        /// <summary>
        /// Getting list of PhysicianSpeciality depending on Physician Number
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="speciality"></param>
        /// <returns></returns>
        /// TODO: Need to convert the response to use physicianFrom()
        public ICollection PhysiciansSpecialtyMatching( long facilityOid, 
            Speciality speciality )
        {
            iDB2Command cmd         = null;
            SafeReader reader       = null;
            ArrayList physicians    = new ArrayList();
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityOid );

                cmd = CommandFor( "CALL " + SP_PHYSICIANS_SPECIALITY_MATCHING +
                    "(" + PARAM_FACILITYID + 
                    "," + PARAM_PHYSICIANSPECIALITY + ")",
                    CommandType.Text,
                    facility);
                
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                if( speciality != null )
                {
                    cmd.Parameters[PARAM_PHYSICIANSPECIALITY].Value = speciality.Description;
                }
                else
                {
                    cmd.Parameters[PARAM_PHYSICIANSPECIALITY].Value = null;
                }
            
                reader = ExecuteReader( cmd );

                while( reader.Read() )
                {
                    Physician physician = PhysicianFrom(reader);

                    physicians.Add( physician );
                }
            }
            
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            
            return physicians;
        }

        public Physician VerifyPhysicianWith( long facilityNumber, long physicianNumber )
        {
            Physician selectedPhysician;

            try
            {
                selectedPhysician = PhysicianWith( facilityNumber, physicianNumber );
            }
            catch( Exception )
            {
                selectedPhysician = new Physician();
                selectedPhysician.PhysicianNumber = 0;
            }

            return selectedPhysician;

        }
        /// <summary>
        /// Getting all the Physician details.
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="physicianNumber"></param>
        /// <returns></returns>
        public Physician PhysicianDetails( long facilityOid, long physicianNumber )
        {
            iDB2Command cmd          = null;
            SafeReader reader        = null;
            Physician physician      = null;
            Name name                = null;
            State state              = null;
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            string title                    = String.Empty;
            string areaCode                 = String.Empty;
            string stateCode                = String.Empty;
            string city                     = String.Empty;
            string fullName                 = String.Empty;
            string firstName                = String.Empty;
            string lastName                 = String.Empty;
            string middleInitial            = String.Empty;
            string physicianAddress         = String.Empty;
            string phoneNumber              = String.Empty;
            string activeInactive           = String.Empty;
            string federalLicenseNumber     = String.Empty;
            string stateLicenseNumber       = String.Empty;
            string admPriveleges            = String.Empty;
            string mdGrpNo                  = String.Empty;
            string cellPhoneNo              = String.Empty; 
            string cellPhoneAreaCode        = String.Empty;
            string pager                    = String.Empty;
            string pagerAreaCode            = String.Empty; 
            string upin                     = String.Empty;
            string dateActivated            = String.Empty;
            string dateInactivated          = String.Empty;         
            string dateExcluded             = String.Empty;
            string status                   = String.Empty;
            string statusDesc               = string.Empty;
            string specialityCode           = String.Empty;
            string zipCode                  = String.Empty;
            string nationalProviderID       = String.Empty;
            long physicianNum;
            long pin;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityOid );
                
                cmd = CommandFor( "CALL " + SP_PHYSICIANS_DETAILS +
                    "(" + PARAM_FACILITYID + 
                    "," + PARAM_PHYSICIANUMBER + ")",
                    CommandType.Text,
                    facility);
                
                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;

                cmd.Parameters[PARAM_PHYSICIANUMBER].Value = physicianNumber;

                reader = ExecuteReader( cmd );

                if( reader.Read() )
                {
                    physicianNum         = reader.GetInt64( COL_DOCTOR_NO );
                    firstName            = reader.GetString( COL_FIRSTNAME );
                    lastName             = reader.GetString( COL_LASTNAME );
                    middleInitial        = reader.GetString( COL_MIDDLEINITIAL );
                    fullName             = reader.GetString( COL_NAME ).Trim();
                    physicianAddress     = reader.GetString( COL_ADDRESS ).Trim();
                    phoneNumber          = reader.GetInt64( COL_PHONE_NO ).ToString();
                    specialityCode       = reader.GetString( COL_SPEC_CODE );
                    activeInactive       = reader.GetString( COL_ACTIVE_INACTIVE ).Trim();
                    status               = reader.GetString( COL_STATUS ).Trim();
                    statusDesc           = reader.GetString( COL_STATUS_DESC ).Trim();
                    federalLicenseNumber = reader.GetString( COL_FEDERAL_LICENSE_NO ).Trim();
                    stateLicenseNumber   = reader.GetString( COL_STATELICENSENUMBER ).Trim();
                    admPriveleges        = reader.GetString( COL_ADM_PRV ).Trim();
                    mdGrpNo              = reader.GetString( COL_MD_GRP_NO ).Trim();
                    title                = reader.GetString( COL_TITLE );
                    areaCode             = reader.GetInt64( COL_AREA_CODE ).ToString();
                    cellPhoneNo          = reader.GetInt64( COL_CELL_PHONE_NO ).ToString();
                    cellPhoneAreaCode    = reader.GetInt64( COL_CELL_PHONE_AREA_NO ).ToString();
                    pager                = reader.GetString( COL_PAGER );
                    nationalProviderID   = reader.GetString( COL_NATIONAL_PROVIDERID );
                    if( pager.Trim().Length == 0 )
                    {
                        pager = "0";
                    }
                    pagerAreaCode        = reader.GetInt64( COL_PAGER_AREA_CODE ).ToString();
                    pin                  = reader.GetInt64( COL_PIN );
                    upin                 = reader.GetString( COL_UPIN );
                    dateActivated        = reader.GetString( COL_DATE_ACTIVATED );
                    dateInactivated      = reader.GetString( COL_DATE_INACTIVATED );
                    dateExcluded         = reader.GetString( COL_DATE_EXCLUDED ) ;
                    stateCode            = reader.GetString( COL_STATE );
                    city                 = reader.GetString( COL_CITY ); 
                    zipCode              = reader.GetString( COL_ZIP ); 

                    name = new Name( firstName, lastName, middleInitial );
                    physician   = new Physician( 
                        physicianNum, ReferenceValue.NEW_VERSION, 
                        fullName, name );
                    physician.Status = string.Empty;
                    state = addressBroker.StateWith(facility.Oid, stateCode);
                    physician.Address = new Address( physicianAddress, string.Empty , city, new ZipCode( zipCode ), state, null ); 
                    physician.PhoneNumber = new PhoneNumber
                        ( areaCode.PadLeft( 3, '0'), phoneNumber.PadLeft( 7, '0' ) );
                    physician.CellPhoneNumber = new PhoneNumber
                        ( cellPhoneAreaCode.PadLeft( 3, '0' ), cellPhoneNo.PadLeft( 7, '0' ) );
                    physician.PagerNumber = new PhoneNumber
                        ( pagerAreaCode.PadLeft( 3, '0' ), pager.PadLeft( 7, '0' ) );
                    physician.Specialization = SpecialityWith( facility.Oid, specialityCode );
					physician.StateLicense = stateLicenseNumber;
                    physician.FederalLicense = new DriversLicense( federalLicenseNumber, null );
                    physician.AdmittingPrivileges = admPriveleges;
                    physician.ActiveInactiveFlag = activeInactive;
                    physician.DateActivated = DateTimeUtilities.DateTimeFromString
                        ( DateTimeFrom( dateActivated ) );
                    physician.DateInactivated = DateTimeUtilities.DateTimeFromString
                        ( DateTimeFrom(  dateInactivated ) );
                    if( dateExcluded.Length == 8 )
                    {
                        string flipYear = dateExcluded.Substring(4,4) + dateExcluded.Substring(0,2) + dateExcluded.Substring(2,2);
                        dateExcluded = flipYear;
                    }
                    physician.DateExcluded = DateTimeUtilities.DateTimeFromString
                        ( DateTimeFrom( dateExcluded ) );
                    physician.Title = title;
                    physician.PIN = pin;
                    physician.UPIN = upin;
                    physician.NPI = nationalProviderID;
                    physician.MedicalGroupNumber = mdGrpNo;

                    if( !physician.DateExcluded.Equals( DateTime.MinValue ) )
                    {
                        physician.ExcludedStatus = "Y";
                    }
                    else
                    {
                        physician.ExcludedStatus = "N";
                    }

                    physician.Status = statusDesc;
                }
            }

            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            
            return physician;
        }

        public ICollection SpecialtiesFor(long facilityID)
        {
            ICollection specialties = null;
            var key = CacheKeys.CACHE_KEY_FOR_PHYSICIANSPECIALTIES;
            this.InitFacility(facilityID);
            ArrayList oList = new ArrayList();
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();
            //SafeReader reader = null;
            iDB2DataReader read = null;

            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    Facility facility = facilityBroker.FacilityWith(facilityID);
                    sb.Append("CALL " + SP_SELECTSPECIALTIESFOR + "(");
                    if (facilityID != 0)
                    {
                        sb.Append(PARAM_FACILITYID);
                        sb.Append("," + PARAM_HSP);
                    }
                    sb.Append(")");

                    cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                    if (facilityID != 0)
                        cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                    cmd.Parameters[PARAM_HSP].Value = facilityID;

                    read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Speciality speciality = new Speciality();
                        speciality.Code = read.GetString(1).TrimEnd();
                        speciality.Description = read.GetString(2).TrimEnd();

                        oList.Add(speciality);
                    }
                    specialties = oList;
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("PhysicianPBARBroker(Specialties) failed to initialize", e, c_log);
                }

                return specialties;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECTSPECIALTIESFOR;
                specialties = cacheManager.GetCollectionBy(key, facilityID, loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("PhysicianPBARBroker(Specialties) failed to initialize", e, c_log);
            }

            finally
            {
                base.Close(cmd);
                base.Close(read);
            }

            return specialties;
        }
 
       /// <summary>
       /// Getting SpecialityDescription 
       /// </summary>
       /// <param name="facilityNumber"></param>
       /// <param name="code"></param>
       /// <returns></returns>
        public Speciality SpecialityWith( long facilityNumber, string code )
        {
            Speciality selectedSpeciality = null;

            ArrayList specialities = ( ArrayList )SpecialtiesFor( facilityNumber );
            foreach( Speciality speciality in specialities )
            {
                if (speciality.Code.Trim() == code.Trim())
                {
                    selectedSpeciality = speciality;
                    break;
                }
            }
            if( selectedSpeciality == null )
            {
                selectedSpeciality = new Speciality( ReferenceValue.NEW_OID, DateTime.Now, code, code );
                selectedSpeciality.IsValid = false;
            }

            return selectedSpeciality;
        }

        public Physician PhysicianWith(long facilityNumber, long physicianNumber)
        {
            Physician selectedPhysician = new Physician();
            SafeReader reader = null;
            iDB2Command cmd = null;

            InitFacility(facilityNumber);

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

                if (physicianNumber != 0)
                {
                    cmd = CommandFor("CALL " + SP_PHYSICIAN_WITH +
                        "(" + PARAM_FACILITYID +
                         "," + PARAM_HSP +
                        "," + PARAM_PHYSICIANUMBER + ")",
                        CommandType.Text,
                        facilityBroker.FacilityWith(facilityNumber));

                    cmd.Parameters[PARAM_FACILITYID].Value = facilityNumber;
                    cmd.Parameters[PARAM_HSP].Value = facilityNumber;
                    cmd.Parameters[PARAM_PHYSICIANUMBER].Value = physicianNumber;

                    reader = ExecuteReader(cmd);

                    if (reader.Read())
                    {
                        selectedPhysician = PhysicianFrom(reader);
                    }
                }
            }
            catch (Exception e)
            {
                string msg = "PhysicianBroker(Physician) failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
            return selectedPhysician;
        }

        public long GetNonStaffPhysicianNumber()
        {
            return Physician.NON_STAFF_PHYSICIAN_NUMBER;
        }

        		public PhysicianRelationship BuildAdmittingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			AdmittingPhysician admittingRole = new AdmittingPhysician();
			Physician admittingPhysician = PhysicianWith( facilityNumber, physicianNumber );
			return new PhysicianRelationship( admittingRole, admittingPhysician );
		}

		public PhysicianRelationship BuildReferringPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			PhysicianRole referringRole = new ReferringPhysician();
			Physician referringPhysician = PhysicianWith( facilityNumber, physicianNumber );
			return new PhysicianRelationship( referringRole, referringPhysician );
		}

		public PhysicianRelationship BuildAttendingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			PhysicianRole attendingRole = new AttendingPhysician();
			Physician attendingPhysician = PhysicianWith( facilityNumber, physicianNumber );
			return new PhysicianRelationship( attendingRole, attendingPhysician );
		}

		public PhysicianRelationship BuildOperatingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			PhysicianRole operatingRole = new OperatingPhysician();
			Physician operatingPhysician = PhysicianWith( facilityNumber, physicianNumber );
			return new PhysicianRelationship( operatingRole, operatingPhysician );
		}

		public PhysicianRelationship BuildPrimaryCarePhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			PhysicianRole PCPRole = new PrimaryCarePhysician();
            Physician primaryCarePhysician = PhysicianWith(facilityNumber, physicianNumber);
			return new PhysicianRelationship( PCPRole, primaryCarePhysician );
		}

		public PhysicianRelationship BuildConsultingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber )
		{
			PhysicianRole consultingRole = new ConsultingPhysician();
			Physician consultingPhysician = PhysicianWith( facilityNumber, physicianNumber );
			return new PhysicianRelationship( consultingRole, consultingPhysician );
		}

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Physician PhysicianFrom(SafeReader reader)
        {
            string fullName           = reader.GetString( COL_NAME );
            string firstName          = reader.GetString( COL_FIRSTNAME );
            string lastName           = reader.GetString( COL_LASTNAME );
            string middleInitial      = reader.GetString( COL_MIDDLEINITIAL );
            long physicianNumber    = reader.GetInt64( COL_NUMBER );
            string specialtyCode     = reader.GetString( COL_SPEC_CODE );
            string description       = reader.GetString( COL_SPEC_DESC );
            string statusDescription = reader.GetString( COL_STATUSDESCRIPTION );
            string activeFlag        = reader.GetString( COL_ACTIVEFLAG );
            string admitPrivilege    = reader.GetString( COL_ADMITPRIVILEGE );
            string excludeDateStr     = reader.GetString( COL_EXCLUDEDATE );
            long admitPrivilegeSuspendDateL = reader.GetInt64( COL_ADMIT_PRIV_SUSPEND_DATE );
            string upin                 = reader.GetString( COL_UPIN );
            string stateLicenseNumber   = reader.GetString( COL_STATELICENSENUMBER );

            DateTime excludeDate = DateTimeUtilities.DateTimeFromString(excludeDateStr);
            DateTime admitPrivilegeSuspendDate = DateTimeUtilities.DateTimeForYYYYMMDDFormat(admitPrivilegeSuspendDateL);

            Physician physician = new Physician( physicianNumber,
                ReferenceValue.NEW_VERSION, 
                fullName,
                firstName, lastName, middleInitial  );

            physician.Specialization = new Speciality( 
                0L,
                ReferenceValue.NEW_VERSION,
                description,
                specialtyCode );
            physician.Status = statusDescription;
            physician.ActiveInactiveFlag = activeFlag == "A" ? "Active" : "Inactive";
            physician.AdmittingPrivileges = admitPrivilege;
            physician.ExcludedStatus = excludeDate == DateTime.MinValue? "N" : "Y";
            physician.AdmitPrivilegeSuspendDate = admitPrivilegeSuspendDate;
            physician.UPIN = upin;
            physician.StateLicense = stateLicenseNumber;

            return physician;
        }

        private string DateTimeFrom( string dateFromPBAR )
        {
            string formatedDate = String.Empty;
            if( dateFromPBAR == "0" || dateFromPBAR == string.Empty || dateFromPBAR == "00000000" )
                return formatedDate = "0";
            if( dateFromPBAR.Length == 8 )
            {
                string year =  dateFromPBAR.Substring( 0, 4 ) ;
                string month = dateFromPBAR.Substring( 4, 2 ) ;
                string day = dateFromPBAR.Substring( 6, 2 ) ;
                formatedDate = string.Concat( month,day,year );
                 
            }
           
            if( dateFromPBAR.Length == 5 )
            {
                string year =  dateFromPBAR.Substring( 0, 2 ) ;
                string month = dateFromPBAR.Substring( 2, 1 ) ;
                string day = dateFromPBAR.Substring( 3, 2 ) ;
                formatedDate = string.Concat( month,day,year );
                
            }
               return formatedDate;    
        }

        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public PhysicianPBARBroker()
        {
        }

        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( PhysicianPBARBroker ) );
        #endregion

        #region Constants
        private const string 
            SP_PHYSICIANS_MATCHING              = "PHYSICIANSMATCHING",
            SP_PHYSICIAN_STATISTICS             = "PhysicianSummaryFor",
            SP_PHYSICIANS_DETAILS               = "PHYSICIANSDETAILS",
            SP_PHYSICIANS_SPECIALITY_MATCHING   = "PHYSICIANSSPECIALTYMATCHING",
            SP_SELECTSPECIALTIESFOR             = "SELECTPHYSICIANSPECIALITESFOR",
            SP_PHYSICIAN_WITH                   = "SELECTPHYSICIANWITH",

            PARAM_HSP                   = "@P_HSP",
            PARAM_PHYSICIANUMBER        = "@P_PhysicianNumber",
            PARAM_LASTNAME              = "@P_LastName",
            PARAM_FIRSTNAME             = "@P_FirstName",
            PARAM_PHYSICIANNUMBER       = "@P_PhysicianNumber",
            PARAM_PHYSICIANSPECIALITY   = "@P_PhysicianSpeciality",
            OUTPUT_PARAM_O_TOTAL            = "@O_Total_Patients",
            OUTPUT_PARAM_O_ATTENDING        = "@O_Total_Attending",
            OUTPUT_PARAM_O_ADMITTING        = "@O_Total_Admitting",
            OUTPUT_PARAM_O_REFERRING        = "@O_Total_Refering",
            OUTPUT_PARAM_O_CONSULTING       = "@O_Total_Consulting",
            OUTPUT_PARAM_O_OPERATING        = "@O_Total_Operating";
         
        private const string
            COL_NAME                = "PHYSICIANNAME",
            COL_NUMBER              = "PHYSICIANNUMBER",
            COL_FIRSTNAME           = "PHYSICIANFIRSTNAME",
            COL_LASTNAME            = "PHYSICIANLASTNAME",
            COL_MIDDLEINITIAL       = "PHYSICIANMIDDLEINITIAL",
            COL_SPEC_CODE            = "PHYSICIANSPECIALITYCODE",
            COL_SPEC_DESC            = "PHYSICIANSPECIALITYDESC",
            COL_DOCTOR_NO            = "PhysicianNumber",
            COL_ADDRESS              = "PHYSICIANADDRESS",
            COL_PHONE_NO             = "PhysicianPhoneNumber",
            COL_ACTIVE_INACTIVE      = "PhysicianInactivaActive",
            COL_STATUS               = "ActiveOrInactive",
            COL_STATUS_DESC          = "StatusDesc",
            COL_FEDERAL_LICENSE_NO   = "FederalLicenseNumber",
            COL_STATELICENSENUMBER    = "StateLicenseNumber",
            COL_ADM_PRV              = "AdmPriveleges",
            COL_MD_GRP_NO            = "MDGrpNo",
            COL_TITLE                = "TITLE",
            COL_AREA_CODE            = "AreaCode",
            COL_CELL_PHONE_NO        = "CELLPHONENO",
            COL_CELL_PHONE_AREA_NO   = "CELLPHONEAREACODE",
            COL_PAGER                = "BEEPERNUMBER",
            COL_PAGER_AREA_CODE      = "BEEPERAREACODE",
            COL_PIN                  = "BEEPERPIN",
            COL_UPIN                 = "UPIN",
            COL_DATE_ACTIVATED       = "DATEACTIVATED",
            COL_DATE_INACTIVATED     = "DATEINACTIVATED",
            COL_DATE_EXCLUDED        = "DATEEXCLUDED",
            COL_STATE                = "STATE",
            COL_CITY                 = "CITY",
            COL_ZIP                  = "ZIP",
            COL_NATIONAL_PROVIDERID  = "NATIONALPROVIDERID";

        private const string
            
            COL_STATUSDESCRIPTION       = "STATUSDESCRIPTION",
            COL_ACTIVEFLAG              = "ACTIVEFLAG",
            COL_ADMITPRIVILEGE          = "ADMITPRIVILEGE",
            COL_EXCLUDEDATE             = "EXCLUDEDATE",
            COL_ADMIT_PRIV_SUSPEND_DATE = "ADMITPRIVSUSPENDDATE";

            #endregion
	}
}
