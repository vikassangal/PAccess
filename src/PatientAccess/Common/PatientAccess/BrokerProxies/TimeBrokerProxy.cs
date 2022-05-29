using System;
using System.Configuration;
using System.Web;
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.BrokerProxies
{
    /// <summary>
    /// This class caches the delta between the local time and the server time to
    /// approximate the server time in order to reduce the calls
    /// made to the remote server based <see cref="TimeBroker"/>
    /// </summary>
    public class TimeBrokerProxy : ITimeBroker
    {
		#region Constants 

        private const int DEFAULT_CACHE_TIMEOUT_IN_MINUTES = 5;
        private const string CACHE_KEY_NAME = "CACHE_KEY_TIMEBROKER_TIMOUT_IN_MINUTES";
        internal const string CACHE_KEY_FORMAT_STRING = "55B67B63-BF3E-4cb5-A07F-243792156623-{0}-{1}-DeltaKey";

		#endregion Constants 

		#region Fields 

        private static volatile object _myLock = new object();

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBrokerProxy"/> class.
        /// </summary>
        /// <param name="timeBroker">The time broker.</param>
        /// <param name="cache">The cache.</param>
        public TimeBrokerProxy( ITimeBroker timeBroker, ICache cache )
        {
            TimeBroker = timeBroker;
            Cache = cache;
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        /// <value>The cache.</value>
        private ICache Cache { get; set; }

        /// <summary>
        /// Gets the cache timeout in minutes.
        /// </summary>
        /// <value>The cache timeout in minutes.</value>
        internal int CacheTimeoutInMinutes
        {
            get
            {
                int timeoutInMinutes;
                
                var timeout =
                    ConfigurationManager.AppSettings[CACHE_KEY_NAME];
                
                if( !int.TryParse(timeout, out timeoutInMinutes) )
                {
                    timeoutInMinutes = DEFAULT_CACHE_TIMEOUT_IN_MINUTES;
                }

                return timeoutInMinutes;
            }
        }

        /// <summary>
        /// Gets or sets the time broker.
        /// </summary>
        /// <value>The time broker.</value>
        private ITimeBroker TimeBroker { get; set; }

		#endregion Properties 

		#region Public Methods 

        /// <summary>
        /// Calculate approximate server time using the cached delta between the
        /// server time and the local time.
        /// </summary>
        /// <param name="gmtOffset">The GMT offset.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <returns></returns>
        public DateTime TimeAt( int gmtOffset, int dstOffset )
        {
            var snapShot = GetServerTimeSnapShotFor( gmtOffset, dstOffset );

            return snapShot.CalculateLocalTime();
        }

		#endregion Public Methods 

		#region Private Methods 

        /// <summary>
        /// Gets the server time snap shot for.
        /// </summary>
        /// <param name="gmtOffset">The GMT offset.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <returns></returns>
        private ServerTimeSnapshot GetServerTimeSnapShotFor( int gmtOffset, int dstOffset )
        {
            //adding GUID suffix to make the key unique
            var snapshotKey = 
                String.Format( CACHE_KEY_FORMAT_STRING,
                               gmtOffset,
                               dstOffset );

            var snapshot = 
                HttpRuntime.Cache.Get( snapshotKey ) as ServerTimeSnapshot;

            if ( snapshot == null )
            {
                lock ( _myLock )
                {
                    // checking again as something might have been inserted in the 
                    // cache before we reached here
                    snapshot = Cache.Get( snapshotKey ) as ServerTimeSnapshot;

                    if ( snapshot == null )
                    {
                        snapshot = GetServerTimeFromAppServer( gmtOffset, dstOffset );

                        // We'll expire the entry at set intervals. This will still reduce
                        // the call volume and will help with DST issues.
                        Cache.Insert( snapshotKey, 
                                      snapshot, 
                                      null, 
                                      DateTime.MaxValue, 
                                      TimeSpan.FromMinutes( DEFAULT_CACHE_TIMEOUT_IN_MINUTES ));
                    }
                }
            }
            
            return snapshot;
        }

		#endregion Private Methods 

		#region Internal Methods 

        /// <summary>
        /// This method is made accessible only for testing purposes. It is not meant for usage from client classes.
        /// </summary>
        /// <param name="gmtOffset">The GMT offset.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <returns></returns>
        private ServerTimeSnapshot GetServerTimeFromAppServer( int gmtOffset, int dstOffset )
        {
            var snapShot =
                new ServerTimeSnapshot
                    {
                        // There is going to be some innacuracy here becaus of network lag
                        ServerDateTime = TimeBroker.TimeAt(gmtOffset, dstOffset),
                        TimeSnapshotCreated = DateTime.UtcNow
                    };

            return snapShot;
        }

		#endregion Internal Methods 
    }
}
