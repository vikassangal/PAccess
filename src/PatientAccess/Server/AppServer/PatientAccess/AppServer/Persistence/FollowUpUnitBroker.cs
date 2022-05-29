using System;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FollowUpUnitBroker.
    /// </summary>
   [Serializable]
    public class FollowUpUnitBroker : CachingBroker, IFollowUpUnitBroker
    {
		#region Constants 

        private const string
            COL_FOLLOWUPUNITID      = "Id",
            COL_NAME                = "Name";
       private const string
           LOADINGFOLLOWUPUNIT         = "LOADINGFOLLOWUPUNIT";
       private const string 
            SP_SELECTALLFOLLOWUPUNITS  = "Facility.SelectAllFollowUpUnits";

		#endregion Constants 

		#region Fields 

        private static readonly ILog c_log = 
           LogManager.GetLogger( typeof( FollowUpUnitBroker ) );

		#endregion Fields 

		#region Constructors 

        public FollowUpUnitBroker( string cxnString )
            : base( cxnString )
        {
        }


        public FollowUpUnitBroker( SqlTransaction txn )
            : base( txn )
        {
        }


        public FollowUpUnitBroker()
            : base()
        {
        }

		#endregion Constructors 

		#region Methods 

        public IList AllFollowUpUnits()
        {
            ArrayList followUpUnits  = null;
            string key = CacheKeys.CACHE_KEY_FOR_FOLLOWUPUNITS;

            LoadCacheDelegate LoadData = delegate()
            {
                        
                        SafeReader reader = null;
                        SqlCommand cmd = null;
                        
                        try
                        {
                            followUpUnits = new ArrayList();

                            cmd = this.CommandFor( SP_SELECTALLFOLLOWUPUNITS );

                            reader = this.ExecuteReader(cmd); 
                
                            while( reader.Read() )
                            {
                                long followUpUnitId = reader.GetInt32( COL_FOLLOWUPUNITID );
                                string description = reader.GetString( COL_NAME );

                                FollowupUnit fuu = new FollowupUnit(
                                    followUpUnitId,
                                    ReferenceValue.NEW_VERSION,
                                    description);
                                

                                followUpUnits.Add( fuu );
                            }
                        }
                        catch( Exception e )
                        {
                            Console.Error.WriteLine( e );
                            string msg = "FollowUpUnit cache failed to initialize.";
                            throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
                        }
                        finally
                        {
                            this.Close(reader);
                            this.Close(cmd);
                        }
                    
            return followUpUnits;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                followUpUnits = (ArrayList)cacheManager.GetCollectionBy(key, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("FollowupUnitBroker failed to initialize", e, c_log);
            }

            return followUpUnits;
        }


       public bool FollowupUnitIsBeingLoaded(string fuuCode)
       {
           Hashtable ht = this.GetLoadingFollowupUnits();

           if( ht.ContainsKey(fuuCode) )
           {
               return true;
           }
           return false;
       }


        public FollowupUnit FollowUpUnitWith( long followUpUnitID )
        {
            FollowupUnit selectedFuu = null;
            try
            {

                IList allFollowUpUnits = this.AllFollowUpUnits();

                foreach( FollowupUnit fuu in allFollowUpUnits )
                {
                    if( fuu.Oid == followUpUnitID )
                    {
                        selectedFuu = fuu;
                        break;
                    }
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "FollowupUnitBroker failed to initialize", e, c_log );
            }
            finally
            {
            }
            return selectedFuu;
        }
       private Hashtable GetLoadingFollowupUnits()
       {
           Hashtable loadingFollowupUnits = new Hashtable();

           string loadingFollowupUnitList = ConfigurationManager.AppSettings[LOADINGFOLLOWUPUNIT];

           if( loadingFollowupUnitList != null && loadingFollowupUnitList.Length > 0 )
           {
               string[] followupUnitArray = loadingFollowupUnitList.Split(",".ToCharArray(),100);

               foreach( string str in followupUnitArray )
               {
                   loadingFollowupUnits.Add(str,str);
               }
           }
           return loadingFollowupUnits;
       }

		#endregion Methods 
    }
}
