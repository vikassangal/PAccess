using System;
using System.Collections;
using System.Data;
using System.Text;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FinancialClassesPBARBroker.
    /// </summary>
  
    [Serializable]
    public class FinancialClassesPBARBroker : PBARCodesBroker, IFinancialClassesBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName  = SP_SELECT_ALL_FINANCIALCLASSES;
            this.WithStoredProcName = SP_SELECT_FINANCIALCLASS_WITH;
        }
        
        /// <summary>
        /// Basic method to return all financial Classes for all facilities
        /// </summary>
        /// <returns></returns>
        
        public ICollection AllTypesOfFinancialClasses(long facilityID)
        {
            ICollection financialClasses = null;
            var key = CacheKeys.CACHE_KEY_FOR_FINANCIALCLASSES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    financialClasses = LoadDataToArrayList<FinancialClass>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("FinancialClassesPBARBroker failed to initialize", e, c_log);
                }
                return financialClasses;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_FINANCIALCLASSES;
                financialClasses = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("FinancialClassesPBARBroker failed to initialize", e, c_log);
            }
            return financialClasses;
        }
        /// <summary>
        /// FinancialClassesFor financialClassType
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="financialClassTypeID"></param>
        /// <returns></returns>
        
        public Hashtable FinancialClassesFor(long facilityID, long financialClassTypeID)
        {
            Hashtable financialClasses = new Hashtable();
            if (financialClassTypeID == STANDARD || financialClassTypeID == UNINSURED)
            {
                var key = CacheKeys.CACHE_KEY_FOR_FINANCIALCLASSES;
                this.InitFacility(facilityID);
                Hashtable oList = new Hashtable();
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
                        sb.Append("CALL " + SP_SELECT_FINANCIALCLASSES_FOR_TYPE + "(");
                        if (facilityID != 0)
                        {
                            sb.Append(PARAM_FACILITYID);
                            sb.Append("," + PARAM_FINANCIAL_CLASSE_TYPE);
                        }

                        sb.Append(")");

                        cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                        if (facilityID != 0)
                            cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                        cmd.Parameters[PARAM_FINANCIAL_CLASSE_TYPE].Value = financialClassTypeID;

                        read = cmd.ExecuteReader();
                        while (read.Read())
                        {
                            FinancialClass financialClass = new FinancialClass();
                            financialClass.Code = read.GetString(1).TrimEnd();
                            financialClass.Description = read.GetString(2).TrimEnd();

                            oList.Add(financialClass.Code,financialClass.Description);
                        }

                        financialClasses = oList;
                    }
                    catch (Exception e)
                    {
                        throw BrokerExceptionFactory.BrokerExceptionFrom("FinancialClassesBroker failed to initialize",
                            e, c_log);
                    }

                    return financialClasses;
                };
                try
                {
                    CacheManager cacheManager = new CacheManager();
                    this.AllStoredProcName = SP_SELECT_FINANCIALCLASSES_FOR_TYPE;
                    financialClasses =(Hashtable) cacheManager.GetCollectionBy(key,
                        financialClassTypeID,
                        loadData);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("FinancialClassesBroker failed to initialize", e,
                        c_log);
                }
            }

            return financialClasses;
        }

        /// <summary>
        /// Return a reference value for the financial class with a specific ID
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public FinancialClass FinancialClassWith( long oid )
        {
          throw new BrokerException( "This method not implemeted in PBAR Version" );
        }

        /// <summary>
        /// Get the financial class with the proper code 
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public FinancialClass FinancialClassWith( long facilityID, string code )
        {
            FinancialClass financialClass = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim();
            this.InitFacility( facilityID );
            try
            {
                ICollection allFinancialClasses = this.AllTypesOfFinancialClasses( facilityID );

                foreach (FinancialClass finClass in allFinancialClasses)
                {
                    if (finClass.Code.Equals(code))
                    {
                        financialClass = finClass;
                        break;
                    }
                }
                if (financialClass == null)
                {
                    financialClass = CodeWith<FinancialClass>(facilityID, code);
                }
             }
           catch (Exception e)
            {
                string msg = "FinancialClassesPBARBroker failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return financialClass;
        }
    
        /// <summary>
        /// Checks if the given financialclass is Unisured.
        /// </summary>
        public bool IsUninsured( long facilityID, FinancialClass aFinancialClass )
        {
            FinancialClassType uninsType = FinancialClassType.NewUninsuredFCType();
            Hashtable ht = this.FinancialClassesFor( facilityID,uninsType.Oid );

            return ht.Contains( aFinancialClass.Code );
        }

        /// <summary>
        /// Checks if the given financialclass has valid financial class.
        /// </summary>
        public bool IsValidFinancialClass( FinancialClass aFinancialClass )
        {            
            if( aFinancialClass != null  )
            {
                Hashtable validFinancialClasses = this.allFinancialClassesHash();
                return validFinancialClasses.Contains( aFinancialClass.Code );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given financial class contained in list of financial classes which 
        /// indicates that the patient is insured. UC162 - Pre-Condtion.
        /// </summary>
        /// <param name="aFinancialClass"></param>
        /// <returns></returns>
        public bool IsPatientInsured( FinancialClass aFinancialClass )
        {
            ArrayList validFinancialClasses = new ArrayList();
            validFinancialClasses.Add( "02" );
            validFinancialClasses.Add( "04" );
            validFinancialClasses.Add( "05" );
            validFinancialClasses.Add( "08" );
            validFinancialClasses.Add( "13" );
            validFinancialClasses.Add( "14" );
            validFinancialClasses.Add( "16" );
            validFinancialClasses.Add( "17" );
            validFinancialClasses.Add( "18" );
            validFinancialClasses.Add( "20" );
            validFinancialClasses.Add( "22" );
            validFinancialClasses.Add( "23" );
            validFinancialClasses.Add( "25" );
            validFinancialClasses.Add( "26" );
            validFinancialClasses.Add( "29" );
            validFinancialClasses.Add( "40" );
            validFinancialClasses.Add( "44" );
            validFinancialClasses.Add( "45" );
            validFinancialClasses.Add( "48" );
            validFinancialClasses.Add( "50" );
            validFinancialClasses.Add( "54" );
            validFinancialClasses.Add( "55" );
            validFinancialClasses.Add( "66" );
            validFinancialClasses.Add( "80" );
            validFinancialClasses.Add( "81" );
            validFinancialClasses.Add( "82" );
            validFinancialClasses.Add( "84" );
            validFinancialClasses.Add( "85" );
            validFinancialClasses.Add( "87" );
            validFinancialClasses.Add( "70" );
            validFinancialClasses.Add( "72" );
            validFinancialClasses.Add( "73" );
            validFinancialClasses.Add( "96" );

            return validFinancialClasses.Contains( aFinancialClass.Code );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private Hashtable allFinancialClassesHash()
        {
            Hashtable financialClasses = new Hashtable();

            try
            {          
                ICollection financialClassList = LoadDataToArrayList<FinancialClass>();
                foreach (FinancialClass fc in financialClassList)
	            {
                     financialClasses.Add( fc.Code, fc );
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "FinancialClassesPBARBroker failed to initialize.", e, c_log );
            }
            return financialClasses;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialClassesPBARBroker()
            : base()
        {
        }
        public FinancialClassesPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public FinancialClassesPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( FinancialClassesPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_FINANCIALCLASSES
                = "SELECTALLFINANCIALCLASSES",
            SP_SELECT_FINANCIALCLASS_WITH
                = "SELECTFINANCIALCLASSWITH",
            SP_SELECT_FINANCIALCLASSES_FOR_TYPE
                = "SELECTFINANCIALCLASSESFOR",
            PARAM_FINANCIAL_CLASSE_TYPE="@P_FINANCIAL_CLASSE_TYPE";

        private const long STANDARD = 1,
            UNINSURED = 2;

        #endregion
    }
}
