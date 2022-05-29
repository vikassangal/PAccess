using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

namespace Extensions.Persistence
{
    /// <summary>
    /// CachingBroker serves as a base class for brokers with the TPF 
    /// that cache items after retrieval to enhance performance of the
    /// persistence layer.  Objects materialized can be stored in the
    /// application cache (HttpRuntime.Cache) for later retrieval.
    /// 
    /// The services provided by the CachingBroker allow items added
    /// to the cache to be cached for one hour with no notification 
    /// when the cached item expires.
    /// 
    /// Keys used by CachingBrokers should be registered to allow
    /// the framework of the CachingBroker to maintain the cache
    /// correctly.
    /// </summary>
    [Serializable]
    abstract public class CachingBroker : AbstractBroker
    {
        #region Constants
        private const CacheDependency NO_DEPENDENCIES      = null;
        private const CacheItemRemovedCallback NO_CALLBACK = null;
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Remove all my registered Cache keys from the cache.
        /// </summary>
        public void ClearCache()
        {
            foreach( string key in this.Keys )
            {
                this.RemoveCacheItem( key );
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Answer the suitable cache for storing objects.
        /// </summary>
        protected Cache Cache        
        {
            get
            {
                HttpRuntime rt = new HttpRuntime();
                return HttpRuntime.Cache;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Add an object to the cache.  Items are cached with no absolute 
        /// expiration, a one hour sliding expiration window, and no notifications.
        /// </summary>
        /// <param name="key">
        /// Key used to access the object from the cache.
        /// </param>
        /// <param name="value">
        /// The object to cache.
        /// </param>
        /// <returns>
        /// The object cached.
        /// </returns>
        protected object CacheItem( string key, object value )
        {
            TimeSpan oneHour = new TimeSpan( 0, 1, 0, 0, 0 );
            this.Cache.Add( key, 
                            value, 
                            NO_DEPENDENCIES, 
                            Cache.NoAbsoluteExpiration,
                            oneHour,
                            CacheItemPriority.High,
                            NO_CALLBACK );
            return value;
        }

        /// <summary>
        /// Register a key that will be used to cache items.  Registered
        /// keys will be available for clean up using the ClearCache method.
        /// </summary>
        /// <param name="key">
        /// Key that will be used when caching objects.
        /// </param>
        /// <returns>
        /// The key that was registered.
        /// </returns>
        protected string RegisterKey( string key )
        {
            if( !this.Keys.Contains( key ) )
            {
                this.Keys.Add( key );
            }
            return key;
        }

        /// <summary>
        /// Remove an object from my cache.
        /// </summary>
        /// <param name="key">
        /// The key describing the object to remove.
        /// </param>
        /// <returns>
        /// The object that was removed.
        /// </returns>
        private object RemoveCacheItem( string key )
        {
            object cachedObject = this.Cache[key];
            //if( null != cachedObject )
            {
                this.Cache.Remove( key );
            }
            return cachedObject;
        }

        /// <summary>
        /// Unregister a key that will be used to cache items.
        /// </summary>
        /// <param name="key">
        /// Key that will be used when caching objects.
        /// </param>
        /// <returns>
        /// The key that was registered.
        /// </returns>
        protected string UnregisterKey( string key )
        {
            if( this.Keys.Contains( key ) )
            {
                this.Keys.Remove( key );
            }
            return key;
        }
        #endregion

        #region Private Properties
        /// <summary>
        /// My collection of registered keys.
        /// </summary>
        private ArrayList Keys
        {
            //Marked as internal because NUnit tests need visibility for testing purposes.
            get
            {
                return i_Keys;
            }
            set
            {
                i_Keys = value;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construct a new instance of the broker that will read the connection
        /// information from the AppSettings value, CXN_PROPERTY_NAME.
        /// </summary>
        public CachingBroker() 
            : base()
        {
            this.Keys = new ArrayList();
        }

        /// <summary>
        /// Construct and initialize an instance of the broker using a supplied
        /// connection string.
        /// </summary>
        /// <param name="cxnString">
        /// The connection string required for the broker to connection to 
        /// its underlying persistence mechanism.
        /// </param>
        public CachingBroker( string cxnString )
            : base( cxnString )
        {
            this.Keys = new ArrayList();
        }

        /// <summary>
        /// Construct and initialize an instance of the broker using a supplied
        /// open connection.
        /// </summary>
        /// <param name="txn">
        /// An existing transaction into which all commands will enlist.
        /// </param>
        public CachingBroker( SqlTransaction txn )
            : base( txn )
        {
            this.Keys = new ArrayList();
        }
        #endregion

        #region Data Elements
        private ArrayList i_Keys;
        #endregion
    }
}
