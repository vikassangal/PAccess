using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    public class OccuranceCodePBARBroker : PBARCodesBroker, IOccuranceCodeBroker 
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }
        /// <summary>
        /// Get a list of all occurrence Codes
        /// </summary>
        /// <returns></returns>
        
        public ICollection AllOccurrenceCodes(long facilityID)
        {
            ICollection allOccurrenceCodes = null;
            var key = CacheKeys.CACHE_KEY_FOR_OCCURANCECODES;
            InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allOccurrenceCodes = LoadDataToArrayList<OccurrenceCode>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("OccuranceCodePBARBroker(all) failed to initialize", e, c_log);
                }
                return allOccurrenceCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SELECTALLOCCURRENCECODES;
                allOccurrenceCodes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("OccuranceCodePBARBroker(all) failed to initialize", e, c_log);
            }
            return allOccurrenceCodes;
        }

        /// <summary>
        /// Get a list of all manual selectable occurrence Codes
        /// </summary>
        /// <returns></returns>
        //public ICollection AllSelectableOccurrenceCodes(long facilityID)
        //{
        //    base.InitFacility(facilityID);

        //    ICollection selectableOccurrenceCodes = null;
        //    string key = CacheKeys.CACHE_KEY_FOR_SELECTABLEOCCURANCECODES;
        //    InitFacility(facilityID);
        //    try
        //    {
        //        CacheManager cacheManager = new CacheManager();
        //        AllStoredProcName = GETSELECTABLEOCCURRENCECODES;
        //        selectableOccurrenceCodes = cacheManager.GetCollectionBy<OccurrenceCode>(key, this.HubName,
        //            LoadDataToArrayList<OccurrenceCode>);
        //    }
        //    catch (Exception e)
        //    {
        //        throw BrokerExceptionFactory.BrokerExceptionFrom("OccurrenceCode(Selectable) failed to initialize", e, c_log);
        //    }
        //    return selectableOccurrenceCodes;
        //}
        public ICollection AllSelectableOccurrenceCodes(long facilityID)
        {
            ICollection selectableOccurrenceCodes = null;
            var key = CacheKeys.CACHE_KEY_FOR_SELECTABLEOCCURANCECODES;
            InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    selectableOccurrenceCodes = LoadDataToArrayList<OccurrenceCode>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("OccurrenceCode(Selectable) failed to initialize", e, c_log);
                }
                return selectableOccurrenceCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = GETSELECTABLEOCCURRENCECODES;
                selectableOccurrenceCodes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("OccurrenceCode(Selectable) failed to initialize", e, c_log);
            }
            return selectableOccurrenceCodes;
        }
        /// <summary>
        /// Get one occurrence Code based on the OID
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="occurrenceCodeID"></param>
        /// <returns></returns>
        public OccurrenceCode OccurrenceCodeWith( long facilityID, long occurrenceCodeID )
        {
            throw new BrokerException("This method not Implemented in PBAR version");
        }

        /// <summary>
        /// Get One OccurrenceCode based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="occurrenceCode"></param>
        /// <returns></returns>
      
        public OccurrenceCode OccurrenceCodeWith(long facilityID, string occurrenceCode)
        {
            OccurrenceCode selectedOccurrenceCode = null;
            if (occurrenceCode == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            occurrenceCode = occurrenceCode.Trim().ToUpper();
            this.InitFacility(facilityID);
            try
            {
                ICollection allOccurrenceCodes = this.AllOccurrenceCodes(facilityID);

                foreach (OccurrenceCode code in allOccurrenceCodes)
                {
                    if (code.Code.Equals(occurrenceCode))
                    {
                        selectedOccurrenceCode = code;
                        break;
                    }
                }

                if (selectedOccurrenceCode == null)
                {
                    this.WithStoredProcName = SELECTOCCURRENCECODEWITH;
                    selectedOccurrenceCode = CodeWith<OccurrenceCode>(facilityID, occurrenceCode);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception", ex, c_log);
            }
            return selectedOccurrenceCode;
        }       
        /// <summary>
        /// Get a list of Occurrence Codes that relate to accidents. This is
        /// a subset of the complete list.
        /// </summary>
        /// <returns></returns>
        public ICollection GetAccidentTypes( long facilityID )
        {
            base.InitFacility( facilityID );

            ArrayList types = new ArrayList();
            ArrayList list = (ArrayList)this.AllOccurrenceCodes( facilityID );
            foreach( OccurrenceCode code in list )
            {
                TypeOfAccident accident = null;
                switch( code.Code )
                {
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO:
                        accident = TypeOfAccident.NewAuto();
                        break;
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT:
                        accident = TypeOfAccident.NewAutoNoFaultInsurance();
                        break;
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL:
                        accident = TypeOfAccident.NewEmploymentRelated();
                        break;
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY:
                        accident = TypeOfAccident.NewTortLiability();
                        break;
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER:
                        accident = TypeOfAccident.NewOther();
                        break;
                    default:
                        break;
                } // end switch
                if( accident != null )
                {
                    accident.OccurrenceCode = code;
                    types.Add(accident);
                }
            }
            return types;
        }

        /// <summary>
        /// Get the TypeOfAccident that relates to the occurrence code
        /// passed as a parameter
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="occurrenceCode"></param>
        /// <returns></returns>
        public TypeOfAccident AccidentTypeFor( long facilityID, OccurrenceCode occurrenceCode )
        {
            base.InitFacility( facilityID );

            ArrayList accidentTypes = (ArrayList)this.GetAccidentTypes( facilityID );
            TypeOfAccident accidentType = null;
            foreach( TypeOfAccident type in accidentTypes )
            {
                if( type.OccurrenceCode.Code == occurrenceCode.Code )
                {
                    accidentType = type;
                }
            }
            return accidentType;
        }

        public OccurrenceCode CreateOccurrenceCode(long facilityId, string occurrenceCode, long occurrenceDate)
        {
            var occurrenceCodeWithoutDate = OccurrenceCodeWith(facilityId, occurrenceCode);
            
            var occurredDate = DateTimeUtilities.DateTimeFromPacked(occurrenceDate);
            
            var newOccurrenceCodeWithDate = new OccurrenceCode(occurrenceCodeWithoutDate.Oid, occurrenceCodeWithoutDate.Timestamp, occurrenceCodeWithoutDate.Description, occurrenceCodeWithoutDate.Code, occurredDate);

            return newOccurrenceCodeWithDate;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OccuranceCodePBARBroker()
            : base()
    {
    }
        public OccuranceCodePBARBroker( string cxnString )
            : base( cxnString )
    {
    }

        public OccuranceCodePBARBroker(IDbTransaction txn)
            : base( txn )
    {
    }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(OccuranceCodePBARBroker));

        #endregion

        #region Constants
        private const string
            SELECTALLOCCURRENCECODES = "SELECTALLOCCURANCECODES",
            GETSELECTABLEOCCURRENCECODES = "GETSELECTABLEOCCURANCECODES",
            SELECTOCCURRENCECODEWITH = "SELECTOCCURANCECODEWITH";
        #endregion
    }
}
