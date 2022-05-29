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
    /// Summary description for FacilityFlagPBARBroker.
    /// </summary>
    //TODO: Create XML summary comment for FacilityFlagPBARBroker
    [Serializable]
    public class FacilityFlagPBARBroker : PBARCodesBroker, IFacilityFlagBroker
    {
        
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Get FacilityFlagsFor
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public IList FacilityFlagsFor( long facilityid )
        {
            ICollection facilityFacilityFlags = new ArrayList();

            try
            {
                this.AllStoredProcName = SP_SELECT_ALL_FACILITY_FLAGS_FOR;
                facilityFacilityFlags = LoadDataToArrayList<FacilityDeterminedFlag>( facilityid,
                    this.FacilityFlagFrom );
            }
            catch( Exception e )
            {
                string msg = "FacilityFlagBroker failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
            }
            return (IList)facilityFacilityFlags;
        }

         /// <summary>
        /// Get one HospitalClinic object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public FacilityDeterminedFlag FacilityFlagWith(
            long facilityNumber, string code )
        {
            FacilityDeterminedFlag selectedFacilityFlag = null;
            ICollection facilityFacilityFlags = new ArrayList();
            if( code != null )
            {
                code = code.Trim();
            }
            try{
                facilityFacilityFlags = this.FacilityFlagsFor( facilityNumber );
                foreach( FacilityDeterminedFlag fFlag in facilityFacilityFlags )
                {
                    if( fFlag.Code.Equals( code ) )
                    {
                        selectedFacilityFlag = fFlag;
                        break;
                    }
                }
                if( selectedFacilityFlag == null )
                {
                    this.WithStoredProcName = SP_SELECT_FACILITY_FLAG_WITH;
                    selectedFacilityFlag = this.CodeWith<FacilityDeterminedFlag>( facilityNumber, code, this.FacilityFlagFrom );
                }
            }
            catch( Exception e )
            {
                string msg = "FacilityFlagPBARBroker failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
            }
            return selectedFacilityFlag;
        }

        /// <summary>
        /// Get one HospitalClinic object based on the facility and oid.
        /// <param name="facilityNumber"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        /// </summary>
        public FacilityDeterminedFlag FacilityFlagWith(
            long facilityNumber, long oid )
        {

            throw new BrokerException( "This method not implemeted in PBAR Version" );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Read facility flag details from reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private FacilityDeterminedFlag FacilityFlagFrom( SafeReader reader )
        {
            long FacilityFlagID = reader.GetInt64( COL_FACILITYFLAGID );
            string FacilityFlagCode = reader.GetString( COL_FACILITYFLAGCODE ).TrimEnd();
            string description = reader.GetString( COL_DESCRIPTION ).TrimEnd();
            long facilityID = reader.GetInt64( COL_FACILITYID );

            FacilityDeterminedFlag facilityFlag =
                new FacilityDeterminedFlag( FacilityFlagID,
                ReferenceValue.NEW_VERSION,
                description,
                FacilityFlagCode );

            return facilityFlag;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FacilityFlagPBARBroker()
            : base()
        {
        }

        public FacilityFlagPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public FacilityFlagPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( FacilityFlagPBARBroker ) );
        #endregion
        #region Constants
        private const string
            SP_SELECT_ALL_FACILITY_FLAGS_FOR = "SelectAllFacilityFlagsFor",
            SP_SELECT_FACILITY_FLAG_WITH = "SelectFacilityFlagWith";
        private const string
            COL_FACILITYID = "FacilityID",
            COL_FACILITYFLAGCODE = "FacilityFlagCode",
            COL_DESCRIPTION = "Description",
            COL_FACILITYFLAGID = "FacilityFlagID";

        #endregion
    }

}
