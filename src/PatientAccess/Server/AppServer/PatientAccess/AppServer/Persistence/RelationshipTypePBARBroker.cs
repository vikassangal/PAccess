using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;
using log4net;
using PatientAccess.Domain;
using IBM.Data.DB2.iSeries;
using System.Text;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for RelationshipTypeBroker.
    /// </summary>
    [Serializable]
    public class RelationshipTypePBARBroker : PBARCodesBroker, IRelationshipTypeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_RELATIONSHIPS;
            this.WithStoredProcName = SP_SELECT_RELATIONSHIP_WITH;
        }

        /// <summary>
        /// AllTypesOfRelationships WITH FACILITYID
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public ICollection AllTypesOfRelationships( long facilityID )
        {
            return GetTypesOfRelationships( facilityID,ALL_TYPES );
        }

        /// <summary>
        /// AllTypesOfRelationships with facilityID & typeofRole
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="typeOfRole"></param>
        /// <returns></returns>
        public ICollection AllTypesOfRelationships( long facilityID,TypeOfRole typeOfRole )
        {            
            return GetTypesOfRelationships(facilityID, (int)typeOfRole );
        }
        /// <summary>
        /// RelationshipTypeWith with Facility and Oid
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        /// <exception cref="BrokerException">This method not implemeted in PBAR Version</exception>
        public RelationshipType RelationshipTypeWith( long facilityID,long oid )
        {
            throw new BrokerException( "This method not implemeted in PBAR Version" );
        }
        /// <summary>
        /// RelationshipTypeWith with faiclity and code
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>code</c> is null.</exception>
        public RelationshipType RelationshipTypeWith( long facilityID,string code )
        {
            RelationshipType selectedRelationshipType = null;
             if( code == null )
              {
                  throw new ArgumentNullException( "code cannot be null or empty" );
              }
            code = code.Trim();
            this.InitFacility( facilityID );
            try
            {
                ICollection allRelationshipTypes = this.AllTypesOfRelationships( facilityID );
                foreach (RelationshipType relationship in allRelationshipTypes)
                {
                    if (relationship.Code.Equals(code) )
                    {
                        selectedRelationshipType = relationship;
                        break;
                    }
                }
                if (selectedRelationshipType == null)
                {
                    selectedRelationshipType = CodeWith<RelationshipType>(facilityID,code);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("RelationshipTypePBARBroker failed to initialize.", e, c_log);
            }
            return selectedRelationshipType;
        }

      
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// GetTypesOfRelationships with facilityID and typeofRole
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="typeOfRole"></param>
        /// <returns></returns>
       
        public ICollection GetTypesOfRelationships(long facilityID, int typeOfRole)
        {
            ICollection typesOfRelationships = null;
            var key = CacheKeys.CACHE_KEY_FOR_TYPESOFRELATIONSHIPS;
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
                    sb.Append("CALL " + SP_SELECT_ALL_RELATIONSHIPS + "(");
                    if (facilityID != 0)
                    {
                        sb.Append(PARAM_FACILITYID);
                        sb.Append("," + PARAM_TYPE_OF_ROLE);
                    }
                    sb.Append(")");

                    cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                    if (facilityID != 0)
                        cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                    cmd.Parameters[PARAM_TYPE_OF_ROLE].Value = typeOfRole;

                    read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        RelationshipType relationshipType = new RelationshipType();
                        relationshipType.Code = read.GetString(1).TrimEnd();
                        relationshipType.Description = read.GetString(2).TrimEnd();

                        oList.Add(relationshipType);
                    }
                    typesOfRelationships = oList;
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("RelationshipTypePBARBroker failed to initialize", e, c_log);
                }
                return typesOfRelationships;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_RELATIONSHIPS;
                typesOfRelationships = cacheManager.GetCollectionBy(key, 
                           typeOfRole,
                           loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("RelationshipTypePBARBroker failed to initialize", e, c_log);
            }
            return typesOfRelationships;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RelationshipTypePBARBroker()
            : base()
        {
        }
        public RelationshipTypePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public RelationshipTypePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( RelationshipTypePBARBroker ) );
        #endregion

        #region Constants
        private const int    
            ALL_TYPES = 0;

        private const string    
            SP_SELECT_ALL_RELATIONSHIPS = "SELECTALLRELATIONSHIPS",
            SP_SELECT_RELATIONSHIP_WITH = "SELECTRELATIONSHIPWITH",
            PARAM_TYPE_OF_ROLE          = "@P_TYPESOFROLE";
        #endregion
    }
}
