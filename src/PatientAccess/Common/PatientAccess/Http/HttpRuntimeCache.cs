using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace PatientAccess.Http
{
    public class HttpRuntimeCache : ICache
    {
        public HttpRuntimeCache()
        {
            this.Cache = HttpRuntime.Cache;
        }

        private Cache Cache { get; set; }

        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key,value);
        }

        public void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            HttpRuntime.Cache.Insert(key,value,dependencies,absoluteExpiration,slidingExpiration);
        }

        public object Remove(string key)
        {
            return this.Cache.Remove(key);
        }

        public void ClearAll()
        {
            foreach ( DictionaryEntry variable in HttpRuntime.Cache )
            {
                this.Cache.Remove((string) variable.Key);
            }
        }
    }
}