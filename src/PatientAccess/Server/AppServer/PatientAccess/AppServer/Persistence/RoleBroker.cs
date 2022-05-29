using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Extensions.Persistence;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for RoleBroker.
	/// </summary>
	public class RoleBroker : AbstractBroker, IRoleBroker
    {

		#region Constants 

        private const string
            DBCOLUMN_DESCRIPTION = "Description";
        private const string
            DBCOLUMN_ROLEID = "Oid";
        private const string
            DBCOLUMN_ROLENAME = "Code";
        private const string
            PARAM_ROLEID = "@RoleId";
        private const string
            DBPROCEDURE_SELECTALLROLES = "Security.SelectAllRoles";
        private const string
            DBPROCEDURE_SELECTROLEWITH = "Security.SelectRoleWith";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger =
            LogManager.GetLogger( typeof( RoleBroker ) );

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBroker"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public RoleBroker( string connectionString )
            : base( connectionString )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBroker"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public RoleBroker( SqlTransaction transaction )
            : base(transaction)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBroker"/> class.
        /// </summary>
        public RoleBroker()
        {
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

        #endregion Properties 

		#region Methods 

        /// <summary>
        /// Get a list of all Roles.
        /// </summary>
        /// <returns></returns>
        public Hashtable AllRoles()
        {
            CacheManager cacheManager = new CacheManager();
            Hashtable roles = 
                cacheManager.GetCollectionBy( CacheKeys.CACHE_KEY_FOR_ROLES, 
                                              this.LoadAllRoles ) as Hashtable;

            return roles;
        }

        /// <summary>
        /// Loads all roles.
        /// </summary>
        /// <returns></returns>
        private Hashtable LoadAllRoles()
        {            
            Hashtable roles = new Hashtable();
            SafeReader safeReader = null;
            SqlCommand sqlCommand = null;
            
            try
            {

                Logger.Debug( "Loading Security Roles" );
               
                sqlCommand = this.CommandFor(DBPROCEDURE_SELECTALLROLES);

                safeReader = this.ExecuteReader(sqlCommand);

                while( safeReader.Read() )
                {
                    Role role = RoleFrom( safeReader );
                    roles.Add(role.Name, role);
                    Logger.DebugFormat( "Loaded {0} Role", role.Name );
                }

            }
            catch( Exception anyException )
            {
               throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger);
            }
            finally
            {
                base.Close( safeReader );
                base.Close( sqlCommand );
                Logger.Debug( "Finished Loading Security Roles" );
                
            }

            return roles;
        }


        /// <summary>
        /// Gets Role with id
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <returns></returns>
        public Role RoleWith( long oid )
        {
            Role role = null;
            SafeReader reader = null;
            SqlCommand sqlCommand = null;

            try
            {
                Hashtable allRoles = this.AllRoles();
                
                foreach( DictionaryEntry dictionaryEntry in allRoles )
                {
                    Role aRole = (Role)dictionaryEntry.Value;
                    if( aRole.Oid == oid )
                    {
                        role = aRole;
                        break;
                    }
                }

                if( role == null )
                {
                    sqlCommand = 
                        this.CommandFor(DBPROCEDURE_SELECTROLEWITH);
                    SqlParameter roleIdParam = 
                        sqlCommand.Parameters.Add(
                            new SqlParameter(PARAM_ROLEID, SqlDbType.Int));
                    roleIdParam.Value = oid;

                    reader = this.ExecuteReader(sqlCommand);

                    if( reader.Read() )
                    {
                        role = RoleFrom(reader);
                    }
                }
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(anyException, Logger);
            }
            finally
            {
                base.Close(reader);
                base.Close(sqlCommand);
            }

            return role;
        }

        /// <summary>
        /// Roles from.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static Role RoleFrom(SafeReader reader)
        {
            long id = reader.GetInt32( DBCOLUMN_ROLEID );
            string name = reader.GetString( DBCOLUMN_ROLENAME );
            string description = reader.GetString( DBCOLUMN_DESCRIPTION );

            Role role = new Role(id, name, description);

            return role;
        }

		#endregion Methods 

    }
}
