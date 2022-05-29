using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Extensions.Persistence;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class AnnouncementBroker : AbstractBroker, IAnnouncementBroker
	{
		#region Constants 

        private const string DBCOLUMN_ROLE_ID = "RoleId";

        private const string DBCOLUMN_FACILITY_ID = "FacilityId";

        private const string DBCOLUMN_ANNOUNCEMENT_ID = "Id";

        private const string DBCOLUMN_SAVE_DATE = "SaveDate";

        private const string DBCOLUMN_STOP_DATE = "StopDate";

        private const string DBCOLUMN_DESCRIPTION = "Description";

        private const string DBCOLUMN_AUTHOR = "Author";

        private const string DELIMITER = ",";
        
        private const string DBPARAMETER_ANNOUNCEMENTID = "@AnnouncementId";

        private const string DBPARAMETER_STOP_DATE = "@StopDate";

        private const string DBPARAMETER_SAVE_DATE = "@SaveDate";

        private const string DBPARAMETER_DESCRIPTION = "@Description";

        private const string DBPARAMETER_AUTHOR = "@Author";

        private const string DBPARAMETER_FACILITIES_LIST = "@FacilitiesList";

        private const string DBPARAMETER_DELIMITER = "@Delimiter";

        private const string DBPARAMETER_FACILITY_ID = "@FacilityId";

        private const string DBPARAMETER_ROLES_LIST = "@RolesList";

        private const string DBPARAMETER_CURRENT_DATE = "@CurrentDate";

        private const string DBPROCEDURE_SAVE_ANNOUNCEMENTS = "Announcement.SaveAnnouncement";

        private const string DBPROCEDURE_SELECT_CURRENT_ANNOUNCEMENTFOR = "Announcement.SelectCurrentAnnouncementsFor";

        private const string DBPROCEDURE_SELECT_ALL_ANNOUNCEMENTFOR = "Announcement.SelectAllAnnouncementsFor";

        #endregion Constants 

		#region Fields 

        private static readonly ILog _logger = 
            LogManager.GetLogger( typeof( AnnouncementBroker ) );

        private IRoleBroker _roleBroker = 
            BrokerFactory.BrokerOfType< IRoleBroker >();

        private IFacilityBroker _facilityBroker = 
            BrokerFactory.BrokerOfType< IFacilityBroker >();

        #endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementBroker"/> class.
        /// </summary>
        /// <param name="connectionString">The CXN string.</param>
        public AnnouncementBroker( string connectionString ) : base( connectionString )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementBroker"/> class.
        /// </summary>
        /// <param name="sqlTransaction">The TXN.</param>
        public AnnouncementBroker( SqlTransaction sqlTransaction ) : base( sqlTransaction )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementBroker"/> class.
        /// </summary>
        public AnnouncementBroker(): base()
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

        /// <summary>
        /// Gets or sets the role broker.
        /// </summary>
        /// <value>The role broker.</value>
        private IRoleBroker RoleBroker
        {
            get
            {
                return this._roleBroker;
            }
            set
            {
                this._roleBroker = value;
            }
        }

        /// <summary>
        /// Gets or sets the facility broker.
        /// </summary>
        /// <value>The facility broker.</value>
        private IFacilityBroker FacilityBroker
        {
            get
            {
                return this._facilityBroker;
            }
            set
            {
                this._facilityBroker = value;
            }
        }

        #endregion Properties 

		#region Methods 

        /// <summary>
        /// Returns all the Announcements with corresponding Roles & facilities.
        /// </summary>
        /// <param name="facilitiesToManage"></param>
        /// <returns></returns>
        public ICollection AllAnnouncementsFor( ICollection facilitiesToManage )
        {
            ArrayList announcements = new ArrayList();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {
                //concatenates all the facilities using delimiter.
                string facilityList = 
                    CreateDelimitedStringUsing( facilitiesToManage, DELIMITER,
                                                delegate(Facility facility) { return facility.Oid.ToString(); });

                if( !String.IsNullOrEmpty( facilityList ) )
                {
                    sqlCommand = this.CommandFor( DBPROCEDURE_SELECT_ALL_ANNOUNCEMENTFOR );

                    DateTime currentDate = DateTime.Now;

                    SqlParameter facilityListparam = 
                        new SqlParameter( DBPARAMETER_FACILITIES_LIST, facilityList );
                    sqlCommand.Parameters.Add( facilityListparam );

                    SqlParameter currentDateParam =
                        new SqlParameter(DBPARAMETER_CURRENT_DATE, currentDate);
                    sqlCommand.Parameters.Add( currentDateParam );

                    reader = this.ExecuteReader( sqlCommand );

                    announcements = BuildAnnouncementsFrom( reader );

                }
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                base.Close( reader );
                base.Close( sqlCommand );
            }

            return announcements;
        }


        /// <summary>
        /// Fetches all the announcement for specific roles and current facility
        /// </summary>
        /// <param name="facilityRoles"></param>
        /// <param name="currentFacility"></param>
        /// <returns></returns>
        public ICollection CurrentAnnouncementsFor( ICollection facilityRoles, Facility currentFacility )
        {
            ArrayList announcements = new ArrayList();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {

                //concatenates all the roles using the delimiter.
                string roleList = CreateDelimitedStringUsing( facilityRoles, DELIMITER,
                                                              delegate(Role role) { return role.Oid.ToString(); });

                if( !String.IsNullOrEmpty( roleList )  )
                {

                    ITimeBroker timeBroker = 
                        BrokerFactory.BrokerOfType< ITimeBroker >() ;
                    
                    DateTime currentDate = 
                        timeBroker.TimeAt( currentFacility.GMTOffset,
                                           currentFacility.DSTOffset );

                    sqlCommand = this.CommandFor( DBPROCEDURE_SELECT_CURRENT_ANNOUNCEMENTFOR );

                    SqlParameter facilityIdparam = 
                        new SqlParameter( DBPARAMETER_FACILITY_ID, currentFacility.Oid );
                    sqlCommand.Parameters.Add( facilityIdparam );

                    SqlParameter roleListParam = 
                        new SqlParameter( DBPARAMETER_ROLES_LIST, roleList );
                    sqlCommand.Parameters.Add( roleListParam );

                    SqlParameter currentDateParam = 
                        new SqlParameter( DBPARAMETER_CURRENT_DATE, currentDate );
                    sqlCommand.Parameters.Add( currentDateParam );

                    reader = this.ExecuteReader( sqlCommand );

                    announcements = this.BuildAnnouncementsFrom( reader );

                }
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                Close( reader );
                Close( sqlCommand );
            }

            return announcements;
        }

        /// <summary>
        /// Builds the announcements from.
        /// </summary>
        /// <param name="safeReader">The safe reader.</param>
        /// <returns></returns>
        private ArrayList BuildAnnouncementsFrom( SafeReader safeReader )
        {
            ArrayList announcements = new ArrayList();
            Announcement currentAnnouncement = null;
            List<long> announcementFacilityIds = new List<long>();
            List<long> announcementRoleIds = new List<long>();
            
            while( safeReader.Read() )
            {
                Announcement newAnnouncement = 
                    AnnouncementFrom( safeReader );
                Role newRole =
                    this.RoleBroker.RoleWith( safeReader.GetInt32( DBCOLUMN_ROLE_ID ) );
                Facility newFacility = 
                    this.FacilityBroker.FacilityWith( safeReader.GetInt32( DBCOLUMN_FACILITY_ID ) );

                if( currentAnnouncement == null ||
                    newAnnouncement.AnnouncementID != currentAnnouncement.AnnouncementID )
                {
                    currentAnnouncement = newAnnouncement;
                    announcements.Add( newAnnouncement );
                    announcementFacilityIds.Clear();
                    announcementRoleIds.Clear();
                }
                
                if( !announcementFacilityIds.Contains( newFacility.Oid ) )
                {
                    currentAnnouncement.Facilities.Add( newFacility );
                }

                if( !announcementRoleIds.Contains( newRole.Oid ) )
                {
                    currentAnnouncement.Roles.Add( newRole );
                }

            }

            return announcements;
        }


        /// <summary>
        /// Saves the announcement.
        /// </summary>
        /// <param name="announcementToSave">The announcement to save.</param>
        public int SaveAnnouncement( Announcement announcementToSave )
        {

            int newAnnouncementId;
            SqlCommand sqlCommand = new SqlCommand();

            try
            {
                string roles = CreateDelimitedStringUsing( announcementToSave.Roles, DELIMITER,
                                                           delegate( Role role ) { return role.Oid.ToString(); } );
                string facilities = CreateDelimitedStringUsing( announcementToSave.Facilities, DELIMITER, 
                                                                delegate(Facility facility){return facility.Oid.ToString();} );

                sqlCommand = this.CommandFor( DBPROCEDURE_SAVE_ANNOUNCEMENTS );

                SqlParameter announcementIdParameter = 
                    new SqlParameter( DBPARAMETER_ANNOUNCEMENTID, announcementToSave.AnnouncementID );
                sqlCommand.Parameters.Add( announcementIdParameter );

                SqlParameter stopDateParameter =
                    new SqlParameter( DBPARAMETER_STOP_DATE, announcementToSave.StopDate );
                sqlCommand.Parameters.Add( stopDateParameter );
                
                SqlParameter descriptionParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_DESCRIPTION, SqlDbType.VarChar ) );
                descriptionParameter.Value = announcementToSave.Description;

                SqlParameter authorParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_AUTHOR, SqlDbType.VarChar ) );
                authorParameter.Value = announcementToSave.Author;

                SqlParameter saveDateParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_SAVE_DATE, SqlDbType.DateTime ) );
                saveDateParameter.Value = announcementToSave.SaveDate;
                
                SqlParameter rolesListParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_ROLES_LIST, SqlDbType.VarChar ) );
                rolesListParameter.Value = roles;

                SqlParameter facilitiesListParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_FACILITIES_LIST, SqlDbType.VarChar ) );
                facilitiesListParameter.Value = facilities;

                SqlParameter delimiterParameter = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_DELIMITER, SqlDbType.VarChar ) );
                delimiterParameter.Value = DELIMITER;

                newAnnouncementId = sqlCommand.ExecuteNonQuery();
                
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                base.Close(sqlCommand);
            }

            return newAnnouncementId;
        }


        /// <summary>
        /// Populate Announcement Object from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Announcement AnnouncementFrom( SafeReader reader )
        {
            long announcmentId = 
                reader.GetInt32( DBCOLUMN_ANNOUNCEMENT_ID );
            DateTime saveDate = 
                reader.GetDateTime( DBCOLUMN_SAVE_DATE );
            DateTime stopDate = 
                reader.GetDateTime( DBCOLUMN_STOP_DATE );
            string description = 
                reader.GetString( DBCOLUMN_DESCRIPTION );
            string author = 
                reader.GetString( DBCOLUMN_AUTHOR );
            
            Announcement announcement = 
                new Announcement( announcmentId, 
                                  stopDate, 
                                  description, 
                                  author, 
                                  saveDate );

            return announcement;
        }


        private delegate string ExtractValue<T>( T valueHolder );

        /// <summary>
        /// Creates the delimited string using.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="extractValue">The extract value.</param>
        /// <returns></returns>
        private static string CreateDelimitedStringUsing<T>( ICollection collection, string delimiter, ExtractValue<T> extractValue )
        {
            StringBuilder result = new StringBuilder( string.Empty );
            foreach( T collectionMember in collection )
            {
                if( !result.ToString().Equals( String.Empty ) )
                {
                    result.Append( delimiter );
                }
                result.Append( extractValue( collectionMember ) );
            }
            
            return result.ToString();
        }

		#endregion Methods 
	}
}
