using System;
using System.Web.Caching;

namespace PatientAccess
{
    public interface ICache
    {
        /// <summary>
        /// Gets the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        object Get(string propertyName);
        
        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Insert(string key, object value);
        
        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        void Insert(string key, object value, 
                    CacheDependency dependencies, 
                    DateTime absoluteExpiration,
                    TimeSpan slidingExpiration);

        object Remove(string key);

        /// <summary>
        /// Clears all.
        /// </summary>
        void ClearAll();
    }
}