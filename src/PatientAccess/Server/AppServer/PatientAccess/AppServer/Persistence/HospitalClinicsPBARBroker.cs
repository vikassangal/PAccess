using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for HospitalClinicsPBARBroker.
	/// </summary>
	public class HospitalClinicsPBARBroker : PBARCodesBroker, IHospitalClinicsBroker
	{
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_HOSPITAL_CLINICS_FOR;
            this.WithStoredProcName = SP_SELECT_HOSPITAL_CLINIC_WITH;
        }
       

        /// <summary>
        /// Get a list of all HospitalClinic objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
        public ICollection HospitalClinicsFor(long facilityID)
        {
            ArrayList allHospitalClinics = null;
            string key = CacheKeys.CACHE_KEY_FOR_HOSPITALCLINICS;
            this.InitFacility( facilityID );
            try
            {
                CacheManager cacheManager = new CacheManager();
                AllStoredProcName = SP_SELECT_ALL_HOSPITAL_CLINICS_FOR;
                allHospitalClinics = (ArrayList)cacheManager.GetCollectionBy<HospitalClinic>(key, facilityID, 
                    LoadDataToArrayList<HospitalClinic>,
                    this.HospitalClinicFrom);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("HospitalClinicsPBARBroker failed to initialize", e, c_log);
            }

            return allHospitalClinics;
        }

      
        /// <summary>
        /// PreTestHospitalClinic For  facility.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <returns></returns>
     public HospitalClinic PreTestHospitalClinicFor(long facilityNumber)
     {
         HospitalClinic selectedHospitalClinic = null;
        this.InitFacility( facilityNumber );
        try
        {
            CacheManager cacheManager = new CacheManager();
            this.AllStoredProcName = SP_SELECT_PREADMIT_HOSPITAL_CLINIC_FOR;
            ICollection preTestHospitalClinics = 
               cacheManager.GetCollectionBy<HospitalClinic>(
                    CacheKeys.CACHE_KEY_FOR_PRETESTHOSPITALCLINICS, 
                    facilityNumber, 
                    LoadDataToArrayList<HospitalClinic>,
                    this.HospitalClinicFrom );
            // this should never happen but if the database does not define
            // one of the hospital Clinics to be the PreTest clinic we have to
            // create an empty one here.
            if (preTestHospitalClinics.Count > 0)
            {
                // This is an indication that ICollection returned by
                // the CacheManager class is probably too general. Until that
                // class is replaced or redesigned, the following idiom should
                // suffice. It will select the first object in the collection.
                foreach(object clinic in preTestHospitalClinics)
                {
                    selectedHospitalClinic = clinic as HospitalClinic;
                    break;
                }
    
            }
            else
            {
    
               selectedHospitalClinic = 
                    new HospitalClinic(ReferenceValue.NEW_OID, DateTime.Now, string.Empty, string.Empty);
                selectedHospitalClinic.IsValid = false;
   
           }
        }
        catch (Exception e)
        {   
            throw BrokerExceptionFactory.BrokerExceptionFrom("HospitalClinicsPBARBroker failed to initialize", e, c_log);
        }
  
      return selectedHospitalClinic;
   
    }



        /// <summary>
        /// Get one HospitalClinic object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public HospitalClinic HospitalClinicWith( long facilityNumber, string code )
        {
            HospitalClinic selectedHospitalClinic = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            this.InitFacility( facilityNumber );
            try
            {
                ICollection allHospitalClinics = this.HospitalClinicsFor( facilityNumber );
                foreach (HospitalClinic hospitalClinic in allHospitalClinics)
                {
                    if (hospitalClinic.Code.Equals(code))
                    {
                        selectedHospitalClinic = hospitalClinic;
                        break;
                    }
                }
                if (selectedHospitalClinic == null)
                {
                    WithStoredProcName = SP_SELECT_HOSPITAL_CLINIC_WITH;
                    selectedHospitalClinic = CodeWith<HospitalClinic>(facilityNumber,code,this.HospitalClinicFrom);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("HospitalClinicsPBARBroker failed to initialize.", e, c_log);
            }
            return selectedHospitalClinic;
        }

        public HospitalClinic HospitalClinicWith( long facilityNumber, long oid )
        {
            throw new BrokerException( "This method not implemeted in PBAR Version" );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods   
        /// <summary>
        /// read HospitalClinic details From reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private HospitalClinic HospitalClinicFrom(SafeReader reader)
        {
            HospitalClinic hospitalClinic = null;

            long hospitalClinicID = reader.GetInt64( COL_HOSPITALCLINICCODEID );
            string hospitalClinicCode = reader.GetString( COL_HOSPITALCLINICCODE ).TrimEnd();
            string description = reader.GetString( COL_DESCRIPTION ).TrimEnd();
            long facilityID = reader.GetInt64( COL_FACILITYID );
            string preAdmitTest = reader.GetString( COL_PREADMITTEST );
            string siteCode = reader.GetString( COL_SITECODE );

            hospitalClinic = new HospitalClinic(hospitalClinicID,
                ReferenceValue.NEW_VERSION,
                description,
                hospitalClinicCode,
                preAdmitTest,
                siteCode);

            return hospitalClinic;
        }

        #endregion

        #region Private Properties
        

        #endregion

        #region Construction and Finalization
        public HospitalClinicsPBARBroker()
            : base()
        {
        }
        public HospitalClinicsPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public HospitalClinicsPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( HospitalClinicsPBARBroker ) );
        #endregion

        #region Constants
        
        private const string 
            SP_SELECT_ALL_HOSPITAL_CLINICS_FOR = "SELECTALLAHOSPITALCLINICFOR",
            SP_SELECT_HOSPITAL_CLINIC_WITH  = "SELECTHOSPITALCLINICWITH",
            SP_SELECT_PREADMIT_HOSPITAL_CLINIC_FOR = "SELECTPREADMITCLINICFOR";

        private const string
            COL_HOSPITALCLINICCODEID = "ClinicCodeID",
            COL_HOSPITALCLINICCODE   = "ClinicCODE",
            COL_DESCRIPTION  = "DESCRIPTION",
            COL_FACILITYID   = "FACILITYID",
            COL_PREADMITTEST = "PREADMITTEST",
            COL_SITECODE     = "SITECODE";     

        #endregion
    }
}
