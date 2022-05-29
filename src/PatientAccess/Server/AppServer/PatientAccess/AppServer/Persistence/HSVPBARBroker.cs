using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for HSVPBARBroker.
    /// </summary>
    public class HSVPBARBroker : PBARCodesBroker , IHSVBroker
    {
        #region Events
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }
        //For( facilityNumber )
        /// <summary>
        /// Get a list of HospitalServices objects based on the facility.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception">HSVPBARBroker(HospitalServices) failed to initialize</exception>
        public IList SelectHospitalServicesFor( long facilityNumber )
        {
            ArrayList selectedHospitalServices = null;
            string key = CacheKeys.CACHE_KEY_FOR_HSVS;
            this.InitFacility( facilityNumber );
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_HOSPITAL_SERVICE_CODES_FOR;
                selectedHospitalServices = (ArrayList)cacheManager.GetCollectionBy<HospitalService>(
                    key, facilityNumber,
                    LoadDataToArrayList<HospitalService>, this.HospitalServicesFrom );
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "HSVPBARBroker(HospitalServices) failed to initialize", e, c_log );
            }

            return selectedHospitalServices;
        }

        /// <summary>
        /// Get one PatientType object based on the code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>code</c> is null.</exception>
        /// <exception cref="Exception">HSVPBARBroker(HospitalServices) failed to initialize.</exception>
        public HospitalService HospitalServiceWith( long facilityNumber, string code )
        {
            HospitalService selectedHospitalService = null;
            if( code == null )
            {
                throw new ArgumentNullException("code");
            }
            code = code.Trim().ToUpper();

            this.InitFacility( facilityNumber );

            try
            {
                ArrayList hsvList = (ArrayList)this.SelectHospitalServicesFor( facilityNumber );

                foreach( HospitalService hsv in hsvList )
                {
                    if( hsv.Code.Equals( code ) )
                    {
                        selectedHospitalService = hsv;
                        break;
                    }
                }

                if( selectedHospitalService == null )
                {
                    WithStoredProcName = SP_HOSPITAL_SERVICE_CODE_WITH;
                    selectedHospitalService = CodeWith<HospitalService>( facilityNumber, code, this.HospitalServicesFrom );
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "HSVPBARBroker(HospitalServices) failed to initialize.", e, c_log );
            }
            return selectedHospitalService;
        }

        /// <summary>
        /// Get a list HospitalServices given FacilityNumber, PatientTypeCode and Daycare flag
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="patientTypeCode"></param>
        /// <param name="dayCare"></param>
        /// <returns></returns>
        public ICollection HospitalServicesFor(
            long facilityNumber, string patientTypeCode, string dayCare )
        {
            ArrayList facilityHospitalServices =
                (ArrayList)this.SelectHospitalServicesFor( facilityNumber );

            HospitalService hospitalService = new HospitalService();
            return hospitalService.HospitalServicesFor(
                facilityHospitalServices, patientTypeCode, dayCare );
        }
        /// <summary>
        /// Get a Collection of Hospital Services for a given FacilityNumber and PatientTypeCode
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="patientTypeCode"></param>
        /// <returns></returns>
        public ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode )
        {
            ArrayList facilityHospitalServices =
                (ArrayList)this.SelectHospitalServicesFor( facilityNumber );

            HospitalService hospitalService = new HospitalService();
            return hospitalService.HospitalServicesFor(
                facilityHospitalServices, patientTypeCode );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Caches the complete list of Hospital Service objects
        /// </summary>
        /// <returns></returns>
        private HospitalService HospitalServicesFrom( SafeReader reader )
        {
            long serviceId = (long)reader.GetDecimal( COL_SERVICE_ID );
            string serviceCode = reader.GetString( COL_SERVICE_CODE ).Trim();
            string servicedescription = reader.GetString(
                COL_SERVICE_DESCRIPTION ).Trim();
            string ipFlag = reader.GetString( COL_IP_TRANSFER_RESTRICTION ).Trim();
            string opFlag = reader.GetString( COL_OP_FLAG ).Trim();
            string dayCareFlag = reader.GetString( COL_DAYCARE_FLAG ).Trim();
            long facilityID = reader.GetInt64( COL_FACILITYID );

            HospitalService hsv = new HospitalService( serviceId,
                PersistentModel.NEW_VERSION,
                servicedescription,
                serviceCode,
                ipFlag,
                opFlag,
                dayCareFlag,
                facilityID );

            return hsv;
        }


        /*----------------------------------------------------------------------------- */
        #endregion

        #region Construction an Finalization
        public HSVPBARBroker()
            : base()
        {
        }
        public HSVPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public HSVPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( HSVPBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_HOSPITAL_SERVICE_CODES_FOR = "SELECTALLHOSPITALSERVICESFOR",
            SP_HOSPITAL_SERVICE_CODE_WITH = "SELECTALLHOSPITALSERVICESWITH",

            COL_SERVICE_ID = "serviceId",
            COL_SERVICE_CODE = "serviceCode",
            COL_SERVICE_DESCRIPTION = "serviceDesc",
            COL_IP_TRANSFER_RESTRICTION = "IPFlag",
            COL_OP_FLAG = "OPFlag",
            COL_FACILITYID = "FacilityID",
            COL_DAYCARE_FLAG = "DayCareFlag";
          
        #endregion
    }
}
