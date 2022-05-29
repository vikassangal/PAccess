using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for WorklistBroker.
    /// </summary>
    //TODO: Create XML summary comment for WorklistBroker
    [Serializable]
    public class WorklistSettingsBroker : CachingBroker, IWorklistSettingsBroker
    {
		#region Constants 

        private const long DEFAULT_USER   = 9999999;

        private const string PARAM_USERID = "@UserId",
                             PARAM_STARTINGLETTERS = "@StartingLetters",
                             PARAM_ENDINGLETTERS = "@EndingLetters",
                             PARAM_STARTDATE = "@StartDate",
                             PARAM_ENDDATE = "@EndDate",
                             PARAM_RANGETYPEID = "@RangeTypeId",
                             PARAM_WORKLISTID = "@WorklistId",
                             PARAM_SORTEDCOLUMN = "@SortedColumn",
                             PARAM_SORTEDCOLUMNDIR = "@SortedColumnDirection",
                             COL_ID = "SelectionRangeId",
                             COL_DESC = "Description",
                             COL_RANGE = "RangeInDays",
                             COL_STARTLETTERS = "StartLetters",
                             COL_ENDLETTERS = "EndLetters",
                             COL_STARTDATE = "StartDate",
                             COL_ENDDATE = "EndDate",
                             COL_WORKLISTSELECTIONANGEID = "SelectionRangeId",
                             COL_SORTEDCOLUMN = "SortedColumn",
                             COL_SORTEDCOLUMNDIRECTION = "SortedColumnDirection",
                             COL_WORKLISTID = "WorklistId",
                             COL_WORKLISTNAME = "WorklistName";
        private const long
            PREREGWORKLISTID = 1L,
            POSTREGWORKLISTID = 2L,
            INSURANCEVERIFICATIONWORKLISTID = 3L,
            PATIENTLIABILITYWORKLISTID = 4L,
            EMERGENCYDEPARMENTWORKLISTID = 5L,
            NOSHOWWORKLISTID = 6L,
            ONLINEPREREGWORKLISTID = 7L;
        private const string
            SP_GETALLWORKLISTRANGES = "Worklist.SelectAllWorklistRanges",
            SP_SELECTRANGESBYWORKLIST = "Worklist.SelectRangesByWorklist",
            SP_SELECTWORKLISTSETTINGS = "Worklist.SelectWorklistSettings",
            SP_SAVEWORKLISTSETTINGS = "Worklist.SaveWorklistSettings",
            SP_GETALLWORKLISTS = "Worklist.SelectAllWorklists";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger = 
            LogManager.GetLogger( typeof( WorklistSettingsBroker ) );

		#endregion Fields 

		#region Constructors 

        public WorklistSettingsBroker( string cxnString )
            : base( cxnString )
        {
        }


        public WorklistSettingsBroker( SqlTransaction txn )
            : base( txn )
        {
        }


        public WorklistSettingsBroker()
            : base()
        {
        }

		#endregion Constructors 

		#region Methods 

        public WorklistSettings DefaultEmergencyDepartmentWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.EmergencyDepartmentWorklist() );
        }


        public WorklistSettings DefaultInsuranceVerificationWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.InsuranceVerificationWorklist() );
        }


        public WorklistSettings DefaultNoShowWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.NoShowWorklist() );
        }


        public WorklistSettings DefaultPatientLiabilityWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.PatientLiabilityWorklist() );
        }


        public WorklistSettings DefaultPostRegWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.PostRegWorklist() );
        }


        public WorklistSettings DefaultPreRegWorklistSettings()
        {
            return this.WorklistSettings( DEFAULT_USER, this.PreRegWorklist() );
        }

        public WorklistSettings DefaultOnlinePreRegWorklistSettings()
        {
            return WorklistSettings( DEFAULT_USER, OnlinePreRegWorklist() );
        }


        public Worklist EmergencyDepartmentWorklist()
        {
            return this.WorklistWith( EMERGENCYDEPARMENTWORKLISTID );
        }


        /// <summary>
        /// Get Pre Reg Work list Settings
        /// One Method for each type of worklist
        /// </summary>
        /// <param name="tenetID">The tenet ID.</param>
        /// <returns></returns>
        public WorklistSettings EmergencyDepartmentWorklistSettings( long tenetID )
        {
            return this.WorklistSettings( tenetID, this.EmergencyDepartmentWorklist() );
        }


        /// <summary>
        /// Get all of the worklists 
        /// </summary>
        /// <returns>An ArrayList of all worklists</returns>
        public ArrayList GetAllWorkLists()
        {
            string key = CacheKeys.CACHE_KEY_FOR_WORKLISTS;

            LoadCacheDelegate LoadData = delegate()
            {
                ArrayList worklists = new ArrayList();
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    cmd = this.CommandFor( SP_GETALLWORKLISTS );


                    reader = this.ExecuteReader( cmd );
                    while( reader.Read() )
                    {
                        long id = reader.GetInt32( "Id" );

                        string worklistName = reader.GetString( "Name" );
                        Worklist newWorklist = new Worklist( id, ReferenceValue.NEW_VERSION, worklistName );

                        worklists.Add( newWorklist );
                    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close( reader );
                    base.Close( cmd );
                }
                return worklists;
            };

            ArrayList wlists;

            try
            {
                CacheManager manager = new CacheManager();
                wlists = (ArrayList)manager.GetCollectionBy( key, LoadData );

            }
            catch( Exception e )
            {

                throw BrokerExceptionFactory.BrokerExceptionFrom( "WorklistBroker.GetAllWorkLists failed to initialize", e, _logger );
            }

            return wlists;
        }


        public ArrayList GetEmergencyDepartmentWorklistRanges()
        {
            Worklist worklist = this.EmergencyDepartmentWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        public ArrayList GetInsuranceVerificationWorklistRanges()
        {
            Worklist worklist = this.InsuranceVerificationWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
            //            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        public ArrayList GetNoShowWorklistRanges()
        {
            Worklist worklist = NoShowWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        public ArrayList GetOnlinePreRegWorklistRanges()
        {
            Worklist worklist = OnlinePreRegWorklist();
            return ( ArrayList )( GetRangesByWorklist() )[worklist.Oid];
        }


         public ArrayList GetPatientLiabilityWorklistRanges()
        {
            Worklist worklist = this.PatientLiabilityWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        public ArrayList GetPostRegWorklistRanges()
        {
            Worklist worklist = this.PostRegWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        /// <summary>
        /// Get the list of ranges that are valid for preRegistration
        /// </summary>
        /// <returns></returns>
        public ArrayList GetPreRegWorklistRanges()
        {
            Worklist worklist = this.PreRegWorklist();
            return (ArrayList)(GetRangesByWorklist())[worklist.Oid];
        }


        public Worklist InsuranceVerificationWorklist()
        {
            return this.WorklistWith( INSURANCEVERIFICATIONWORKLISTID );
        }


        public WorklistSettings InsuranceVerificationWorklistSettings( long tenetID )
        {
            return this.WorklistSettings( tenetID, this.InsuranceVerificationWorklist() );
        }


        public Worklist NoShowWorklist()
        {
            return WorklistWith(NOSHOWWORKLISTID);
        }

        public WorklistSettings NoShowWorklistSettings( long tenetID )
        {
            return WorklistSettings(tenetID, NoShowWorklist());
        }


        public Worklist OnlinePreRegWorklist()
        {
            return WorklistWith( ONLINEPREREGWORKLISTID );
        }

        public WorklistSettings OnlinePreRegWorklistSettings( long tenetID )
        {
            return WorklistSettings( tenetID, OnlinePreRegWorklist() );
        }

        public Worklist PatientLiabilityWorklist()
        {
            return this.WorklistWith( PATIENTLIABILITYWORKLISTID );
        }


        public WorklistSettings PatientLiabilityWorklistSettings( long tenetID )
        {
            return this.WorklistSettings( tenetID ,this.PatientLiabilityWorklist() );
        }


        public Worklist PostRegWorklist()
        {
            return this.WorklistWith( POSTREGWORKLISTID );
        }


        public WorklistSettings PostRegWorklistSettings( long tenetID )
        {
            return this.WorklistSettings( tenetID, this.PostRegWorklist() );
        }


        public Worklist PreRegWorklist()
        {
            return this.WorklistWith( PREREGWORKLISTID );
        }


        public WorklistSettings PreRegWorklistSettings( long tenetID )
        {
            return this.WorklistSettings( tenetID, this.PreRegWorklist() );
        }


        /// <summary>
        /// Save Worklist Settings Section
        /// one method for each type of worklist
        /// </summary>
        /// <param name="tenetID">The tenet ID.</param>
        /// <param name="worklistSettings">The worklist settings.</param>
        public void SaveEmergencyDepartmentWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.EmergencyDepartmentWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SaveInsuranceVerificationWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.InsuranceVerificationWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SaveNoShowWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.NoShowWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SaveOnlinePreRegWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = OnlinePreRegWorklist();
            SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SavePatientLiabilityWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.PatientLiabilityWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SavePostRegWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.PostRegWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        public void SavePreRegWorklistSettings( long tenetID, WorklistSettings worklistSettings )
        {
            Worklist worklist = this.PreRegWorklist();
            this.SaveWorklistSettings( tenetID, worklistSettings, worklist );
        }


        /// <summary>
        /// Save the settings for a particular worklist
        /// </summary>
        /// <param name="tenetID">The user's Tenet ID whose settings are being saved</param>
        /// <param name="worklistSettings">The Settings</param>
        /// <param name="worklist">The worklist that these settings belong to.</param>
        public void SaveWorklistSettings( long tenetID, 
                                          WorklistSettings worklistSettings, 
                                          Worklist worklist )
        {
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(SP_SAVEWORKLISTSETTINGS);
       
                SqlParameter userIDParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_USERID, SqlDbType.Int ) );
                userIDParam.Value = tenetID;

                SqlParameter startingLetterParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_STARTINGLETTERS, SqlDbType.VarChar ) );
                startingLetterParam.Value = worklistSettings.BeginningWithLetter;

                SqlParameter endingLetterParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_ENDINGLETTERS, SqlDbType.VarChar ) );
                endingLetterParam.Value = worklistSettings.EndingWithLetter;
                          
                if(worklistSettings.FromDate != DateTime.MinValue)
                {
                    SqlParameter startDateParam = cmd.Parameters.Add(
                        new SqlParameter(PARAM_STARTDATE, SqlDbType.DateTime));
                    startDateParam.Value = worklistSettings.FromDate;
                }

                if(worklistSettings.ToDate != DateTime.MinValue)
                {
                    SqlParameter endDateParam = cmd.Parameters.Add(
                        new SqlParameter(PARAM_ENDDATE, SqlDbType.DateTime));
                    endDateParam.Value = worklistSettings.ToDate;
                }

                SqlParameter worklistIDParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_WORKLISTID, SqlDbType.Int ) );
                worklistIDParam.Value = worklist.Oid;

                SqlParameter rangeTypeIDParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_RANGETYPEID, SqlDbType.Int ) );

                if(worklistSettings.WorklistSelectionRange == null)
                    Console.WriteLine("WorklistSelectionRange is Missing");
                if(cmd.Parameters[PARAM_RANGETYPEID] == null)
                    Console.WriteLine("Parameter is not found");

            
                if( worklistSettings.WorklistSelectionRange != null)
                {
                    rangeTypeIDParam.Value =worklistSettings.WorklistSelectionRange.Oid;

                }
                else
                {
                    rangeTypeIDParam.Value = DBNull.Value;
                }

                SqlParameter sortedColumnParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_SORTEDCOLUMN, SqlDbType.VarChar ) );
                sortedColumnParam.Value = worklistSettings.SortedColumn;
                
                SqlParameter sortedColumnDirParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_SORTEDCOLUMNDIR, SqlDbType.VarChar ) );
                sortedColumnDirParam.Value = worklistSettings.SortedColumnDirection;     

                cmd.ExecuteNonQuery();
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
            }
            finally
            {
                base.Close( cmd );
            }
        }


        public WorklistSelectionRange WorklistSelectionRangeWith(long rangeID)
        {
            WorklistSelectionRange result = null;

            ArrayList list = this.GetAllWorklistRanges();
            foreach( WorklistSelectionRange range in list )
            {
                if( range.Oid == rangeID)
                    result = range;
            }
            return result;
        }


        /// <summary>
        /// Get a worklist with a particular ID
        /// </summary>
        /// <param name="worklistID"></param>
        /// <returns>Thw worklist whose OID = the worklistID parameters</returns>
        public Worklist WorklistWith(long worklistID)
        {
            Worklist foundWorklist = null;
            ArrayList worklists = this.GetAllWorkLists();
            foreach( Worklist worklist in worklists )
            {
                if( worklist.Oid == worklistID )
                {
                    foundWorklist = (Worklist)worklist.Clone();
                }
            }
            return foundWorklist;
        }
        /// <summary>
        /// Get a list of all available worklist ranges
        /// </summary>
        /// <returns>A List of all worklist Ranges</returns>
        private ArrayList GetAllWorklistRanges()
        {
            string key = CacheKeys.CACHE_KEY_FOR_RANGES;
            LoadCacheDelegate LoadData = delegate()
            {
                ArrayList ranges = new ArrayList();
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {

                    cmd = this.CommandFor( SP_GETALLWORKLISTRANGES );

                    reader = this.ExecuteReader( cmd );
                    while( reader.Read() )
                    {
                        //                                long id = reader.GetInt64(COL_ID);
                        long id = reader.GetInt32( "Id" ) ;

                        string desc = reader.GetString( COL_DESC );
                        long range = 0;
                        if( reader.GetValue( COL_RANGE ) != DBNull.Value )
                        {
                            range = reader.GetInt32( COL_RANGE ) ;
                        }

                        WorklistSelectionRange worklistselectionRange =
                            new WorklistSelectionRange( id, desc, range );
                        Console.WriteLine( "WorklistSelectionRange: " + id + " " + desc );
                        ranges.Add( worklistselectionRange );
                    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close( reader );
                    base.Close( cmd );
                }
                return ranges;
            };

            ArrayList allwlistranges;

            try
            {
                CacheManager manager = new CacheManager();
                allwlistranges = (ArrayList)manager.GetCollectionBy( key, LoadData );

            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "WorklistBroker.GetAllWorklistRanges failed to initialize", e, _logger );
            }

            return allwlistranges;
        }


        private Hashtable GetRangesByWorklist()
        {
            string key = CacheKeys.CACHE_KEY_FOR_WORKLISTRANGES;
            LoadCacheDelegate LoadData = delegate()
            {
                Hashtable worklistRanges = new Hashtable();
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    cmd = this.CommandFor( SP_SELECTRANGESBYWORKLIST );
                    reader = this.ExecuteReader( cmd );
                    long range = 0;

                    while( reader.Read() )
                    {
                        long worklistID = reader.GetInt32( COL_WORKLISTID ) ;
                        long id = reader.GetInt32( COL_ID ) ;
                        string desc = reader.GetString( COL_DESC );
                        if( reader.GetValue( COL_RANGE ) != DBNull.Value )
                        {
                            range = reader.GetInt32( COL_RANGE );
                        }

                        WorklistSelectionRange worklistselectionRange =
                            new WorklistSelectionRange( id, desc, range );

                        ArrayList ranges = (ArrayList)worklistRanges[worklistID];
                        if( ranges == null )
                        {
                            ranges = new ArrayList();
                            worklistRanges.Add( worklistID, ranges );
                        }
                        ranges.Add( worklistselectionRange );
                    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close( reader );
                    base.Close( cmd );
                }

                return worklistRanges;
            };

            Hashtable wlistranges;

            try
            {
                CacheManager manager = new CacheManager();
                wlistranges = (Hashtable)manager.GetCollectionBy( key, LoadData );

            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "WorklistBroker.GetRangesByWorklist failed to initialize", e, _logger );
            }

            return wlistranges;
        }


        private WorklistSettings WorklistSettings( long tenetID, Worklist worklist )
        {
            WorklistSettings ws = null;
            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(SP_SELECTWORKLISTSETTINGS);
                SqlParameter userIDParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_USERID, SqlDbType.Int ) );
                userIDParam.Value = tenetID;

                SqlParameter worklistIDIDParam = cmd.Parameters.Add(
                    new SqlParameter( PARAM_WORKLISTID, SqlDbType.Int ) );
                worklistIDIDParam.Value = worklist.Oid;


                reader = this.ExecuteReader(cmd);

                while(reader.Read())
                {
                    long worklistSelectionRangeID = reader.GetInt32(COL_WORKLISTSELECTIONANGEID);
                    string startLetters = reader.GetString(COL_STARTLETTERS);
                    string endLetters = reader.GetString(COL_ENDLETTERS);
                    DateTime startDate = reader.GetDateTime(COL_STARTDATE);
                    DateTime endDate = reader.GetDateTime(COL_ENDDATE);
                    long sortedColumn = reader.GetInt32(COL_SORTEDCOLUMN);
                    long sortedColumnDirection = reader.GetInt32(COL_SORTEDCOLUMNDIRECTION);

                    WorklistSelectionRange range = this.WorklistSelectionRangeWith(worklistSelectionRangeID);
                    ws = new WorklistSettings(startLetters,endLetters,
                        startDate, endDate, range, sortedColumn, sortedColumnDirection);
                }
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return ws;
        }

		#endregion Methods 
    }
}
