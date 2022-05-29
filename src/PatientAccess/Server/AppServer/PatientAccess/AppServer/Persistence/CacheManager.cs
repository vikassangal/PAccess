using System;
using System.Collections;
using System.Web.Caching;
using Extensions.Persistence;

namespace PatientAccess.Persistence
{
    public delegate ICollection LoadCacheDelegate();

    // because the underlaying classes (CodesBroker and PBARCodesBroker) support Brokers of different
    // types there needs to be separate versions of these. 
    public delegate ICollection GLoadCacheDelegate<T>(CodesBroker.ReadDataDelegate<T> readMethod);
    public delegate ICollection GLoadKeyedCacheDelegate<T>(long id, CodesBroker.ReadDataDelegate<T> readMethod);
    
    public delegate ICollection PLoadCacheDelegate<T>(PBARCodesBroker.ReadDataDelegate<T> readMethod);
    public delegate ICollection PLoadKeyedCacheDelegate<T>(long id, PBARCodesBroker.ReadDataDelegate<T> readMethod);
    // this 'special' version is to support Hub specific calls 
    public delegate ICollection PLoadKeyedCacheDelegateH<T>(string id, PBARCodesBroker.ReadDataDelegate<T> readMethod);

    public class CacheManager : CachingBroker
    {
        /// <summary>
        /// Get a Collection of objects from the cache
        /// </summary>
        /// <param name="key">the key to the cache which will be used to reference 
        ///     the collection of objects</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <returns>A collection of objects</returns>
        public ICollection GetCollectionBy(string key, LoadCacheDelegate doLoadblock)
        {
            ICollection aCollection = (ICollection)Cache[key];
            
            if (aCollection == null)
            {
                aCollection = insertCollection(key, key, doLoadblock);
            }
            return aCollection;
        }
        
        /// <summary>
        /// A generic method used to get a collection of object from the cache. It uses the parameters to 
        /// load the data if the collection does not exist in cache.
        /// </summary>
        /// <typeparam name="T">The Type of object being handled</typeparam>
        /// <param name="key">The key to the cache which will be used to reference the collection of objects</param>
        /// <param name="doLoadblock">The method used to read the collection from the database if it does
        /// not alread exist in Cache.</param>
        /// <returns>A Collection of objects of type T</returns>
        public ICollection GetCollectionBy<T>(string key, GLoadCacheDelegate<T> doLoadblock)
        {
            ICollection aCollection = (ICollection)Cache[key];
            
            if (aCollection == null)
            {
                aCollection = insertCollection(key, key, doLoadblock );
            }
            return aCollection;
        }

        /// <summary>
        /// A generic method used to get a collection of object from the cache. It uses the parameters to 
        /// load the data if the collection does not exist in cache.
        /// </summary>
        /// <typeparam name="T">The Type of object being handled</typeparam>
        /// <param name="key">The Key to the Cache which is used to reference the collection of objects</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <param name="readMethod">The delegate used to create the objects by reading the data from the data reader</param>
        /// <returns>A Collection of objects of type T</returns>
        public ICollection GetCollectionBy<T>(string key, 
            GLoadCacheDelegate<T> doLoadblock, 
            CodesBroker.ReadDataDelegate<T> readMethod )
        {
            ICollection aCollection = (ICollection)Cache[key];
            
            if (aCollection == null)
            {
                aCollection = insertCollection(key, key, doLoadblock, readMethod );
            }
            return aCollection;
        }

        /// <summary>
        /// get a collection of objects whose cache key has a secondary key
        /// </summary>
        /// <param name="key">The Key to the Cache which is used to reference the collection of objects</param>
        /// <param name="subKey">A Secondary part of the key for cases where there are multiple collections of
        /// the same type of object that are of the same type but which are different in some way from 
        /// each other. For example ReligiousCongregations are keyed by the string RELIGIOUSCONGREGATION 
        /// Appended with a facilityID. The Sub key is normally the facilityID. this version permits the use
        /// of a string as the 2ndary part of the key</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <returns>a Collection of objects</returns>
        public ICollection GetCollectionBy(string key, object subKey, LoadCacheDelegate doLoadblock)
        {
            string qualifiedKey = CacheKeys.KeyFor(key, subKey.ToString());
            ICollection aCollection = (ICollection)Cache[qualifiedKey];
            
            if (aCollection == null)
            {
                aCollection = insertCollection(key, qualifiedKey, doLoadblock);
            }
            return aCollection;
        }

        /// <summary>
        /// Get a collection from the cache.
        ///     this version takes a version of the doLoad delegate that requires an extra key parameter.
        ///     this parameter is the actual keyID which in most cases is facilityID
        /// </summary>
        /// <typeparam name="T">The Type of object being handled</typeparam>
        /// <param name="key">The Key to the Cache which is used to reference the collection of objects</param>
        /// <param name="id">A Secondary part of the key for cases where there are multiple collections of
        /// the same type of object that are of the same type but which are different in some way from 
        /// each other. For example ReligiousCongregations are keyed by the string RELIGIOUSCONGREGATION 
        /// Appended with a facilityID. The Sub key is normally the facilityID</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <returns>A collection of objects of type T</returns>
        public ICollection GetCollectionBy<T>(string key, long id, PLoadKeyedCacheDelegate<T> doLoadblock)
        {
            string qualifiedKey = CacheKeys.KeyFor(key, id.ToString());
            ICollection aCollection = (ICollection)Cache[qualifiedKey];
            
            if (aCollection == null)
            {
                aCollection = insertKeyedCollection<T>(key, qualifiedKey, id, doLoadblock);
            }
            return aCollection;
        }
        public ICollection GetCollectionBy<T>(string key, string id, PLoadKeyedCacheDelegateH<T> doLoadblock)
        {
            string qualifiedKey = CacheKeys.KeyFor(key, id);
            ICollection aCollection = (ICollection)Cache[qualifiedKey];
            
            if (aCollection == null)
            {
                // there are deamons here
                // for this version the id parameter is a HUB name which is only used to look in
                // the cache. It should not be used to look in the database. The database connection
                // is taken from the member attribute which is set in the initFacility method.
                aCollection = insertKeyedCollection<T>(key, qualifiedKey, null, doLoadblock);
            }
            return aCollection;
        }

        /// <summary>
        /// <typeparam name="T">The Type of object being handled</typeparam>
        /// <param name="key">The Key to the Cache which is used to reference the collection of objects</param>
        /// <param name="id">A Secondary part of the key for cases where there are multiple collections of
        /// the same type of object that are of the same type but which are different in some way from 
        /// each other. For example ReligiousCongregations are keyed by the string RELIGIOUSCONGREGATION 
        /// Appended with a facilityID. The Sub key is normally the facilityID</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <param name="readMethod">the delegate used to create a single object of type T by reading
        /// data from a reader</param>
        /// <returns>A collection of objects of type T</returns>
        /// </summary>
        public ICollection GetCollectionBy<T>(string key, long id, 
            PLoadKeyedCacheDelegate<T> doLoadblock, 
            PBARCodesBroker.ReadDataDelegate<T> readMethod)
        {
            string qualifiedKey = CacheKeys.KeyFor(key, id.ToString());
            ICollection aCollection = (ICollection)Cache[qualifiedKey];
            
            if (aCollection == null)
            {
                aCollection = insertKeyedCollection<T>(key, qualifiedKey, id, doLoadblock, readMethod);
            }
            return aCollection;
        }

        public ICollection GetCollectionBy<T>(string key, PLoadCacheDelegate<T> doLoadblock, PBARCodesBroker.ReadDataDelegate<T> readMethod)
        {
            ICollection aCollection = null;

            lock (key)
            {
                aCollection = (ICollection) Cache[key];
                if (aCollection == null)
                {
                    aCollection = doLoadblock(readMethod);
                    this.Cache.Insert(key, aCollection, null, DateTime.UtcNow.AddHours(CacheKeys.CacheExpirationIntervalFor(key)),
                        Cache.NoSlidingExpiration);
                }
            }
			
            return aCollection;
        }

        /// <summary>
        /// Get a collection of objects whose cache key has both secondary and tertiary names
        /// </summary>
        /// <param name="key">The Key to the Cache which is used to reference the collection of objects</param>
        /// <param name="subKey">A Secondary part of the key for cases where there are multiple collections of
        /// the same type of object that are of the same type but which are different in some way from 
        /// each other. For example ReligiousCongregations are keyed by the string RELIGIOUSCONGREGATION 
        /// Appended with a facilityID. The Sub key is normally the facilityID</param>
        /// <param name="subkey2">An additional portion of the cache key name</param>
        /// <param name="doLoadblock">The delegate used to read the data from the database if the collection 
        /// is not already in cache.</param>
        /// <returns>A collection of objects</returns>
        public ICollection GetCollectionBy(string key, object subKey, object subkey2, LoadCacheDelegate doLoadblock)
        {
            string qualifiedKey = CacheKeys.KeyFor(key, subKey, subkey2);
            ICollection aCollection = (ICollection)Cache[qualifiedKey];
            
            if (aCollection == null)
            {
                aCollection = insertCollection(key, qualifiedKey, doLoadblock);
            }
            return aCollection;
        }

        /// <summary>
        /// Insert a collection of objects into the cache first checking to see if the list is already
        ///     in the cache. If it is not call the delegate (last parameters) to load the data. 
        /// </summary>
        /// <param name="baseKey"></param>
        /// <param name="qualifiedKey"></param>
        /// <param name="doLoadblock"></param>
        /// <returns></returns>
        private ICollection insertCollection(string baseKey, string qualifiedKey, LoadCacheDelegate doLoadblock)
        {
            ICollection aCollection = null;

            lock (qualifiedKey)
            {
                if ((aCollection = (ICollection)Cache[qualifiedKey]) == null)
                {
                    aCollection = doLoadblock();
                    this.Cache.Insert(qualifiedKey,
                        aCollection,
                        null,
                        DateTime.UtcNow.AddHours(CacheKeys.CacheExpirationIntervalFor(baseKey)),
                        Cache.NoSlidingExpiration);
                }
            }
            return aCollection;
        }

        /// <summary>
        /// a Generic version of the previous method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseKey"></param>
        /// <param name="qualifiedKey"></param>
        /// <param name="doLoadblock"></param>
        /// <returns></returns>
        private ICollection insertCollection<T>(string baseKey, string qualifiedKey, GLoadCacheDelegate<T> doLoadblock)
        {
            return insertCollection(baseKey,qualifiedKey, doLoadblock, null);
        }
        private ICollection insertCollection<T>(string baseKey, string qualifiedKey, 
            GLoadCacheDelegate<T> doLoadblock, CodesBroker.ReadDataDelegate<T> readMethod)
        {
            ICollection aCollection = null;

            lock (qualifiedKey)
            {
                if ((aCollection = (ICollection)Cache[qualifiedKey]) == null)
                {
                    aCollection = doLoadblock(readMethod);
                    this.Cache.Insert(qualifiedKey,
                        aCollection,
                        null,
                        DateTime.UtcNow.AddHours(CacheKeys.CacheExpirationIntervalFor(baseKey)),
                        Cache.NoSlidingExpiration);
                }
            }
            return aCollection;
        }

        private ICollection insertKeyedCollection<T>(string baseKey, string qualifiedKey, long id,
            PLoadKeyedCacheDelegate<T> doLoadblock)
        {
            return insertKeyedCollection<T>(baseKey,qualifiedKey,id,doLoadblock, null);
        }
        private ICollection insertKeyedCollection<T>(string baseKey, string qualifiedKey, string id,
            PLoadKeyedCacheDelegateH<T> doLoadblock)
        {
            return insertKeyedCollection<T>(baseKey,qualifiedKey,id,doLoadblock, null);
        }
        private ICollection insertKeyedCollection<T>(string baseKey, string qualifiedKey, long id, 
            PLoadKeyedCacheDelegate<T> doLoadblock, 
            PBARCodesBroker.ReadDataDelegate<T> readMethod)
        {
            ICollection aCollection = null;

            lock (qualifiedKey)
            {
                if ((aCollection = (ICollection)Cache[qualifiedKey]) == null)
                {
                    aCollection = doLoadblock(id,readMethod);
                    this.Cache.Insert(qualifiedKey,
                        aCollection,
                        null,
                        DateTime.UtcNow.AddHours(CacheKeys.CacheExpirationIntervalFor(baseKey)),
                        Cache.NoSlidingExpiration);
                }
            }
            return aCollection;
        }
        private ICollection insertKeyedCollection<T>(string baseKey, string qualifiedKey, string id, 
            PLoadKeyedCacheDelegateH<T> doLoadblock, 
            PBARCodesBroker.ReadDataDelegate<T> readMethod)
        {
            ICollection aCollection = null;

            lock (qualifiedKey)
            {
                if ((aCollection = (ICollection)Cache[qualifiedKey]) == null)
                {
                    aCollection = doLoadblock(id,readMethod);
                    this.Cache.Insert(qualifiedKey,
                        aCollection,
                        null,
                        DateTime.UtcNow.AddHours(CacheKeys.CacheExpirationIntervalFor(baseKey)),
                        Cache.NoSlidingExpiration);
                }
            }
            return aCollection;
        }
    }
}