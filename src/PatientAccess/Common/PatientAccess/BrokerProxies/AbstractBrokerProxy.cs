using System;
using System.Web;
using System.Web.Caching;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class AbstractBrokerProxy : object
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Answer the suitable cache for storing objects.
        /// </summary>
        protected Cache Cache        
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
