using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{

    public class SpanCodeBrokerProxy : AbstractBrokerProxy, ISpanCodeBroker
    {

        #region Event Handlers
        #endregion

        #region ISpanCodeBroker Member Methods

        public ICollection AllSpans( long facilityID )
        {
            var cacheKey = "SPANCODE_BROKER_PROXY_ALL_SPANS_" + "_AND_FACILITY_" + facilityID;
            ICollection allSpans = (ICollection)this.Cache[cacheKey];

            if ( null == allSpans )
            {
                lock (cacheKey)
                {
                    allSpans = this.i_SpanCodeBroker.AllSpans( facilityID ) ;
                    if (null == this.Cache[cacheKey])
                    {
                        this.Cache.Insert(cacheKey, allSpans);
                    }
                }
            }
            
            return allSpans ;
        }

        public SpanCode SpanCodeWith( long facilityID, string code )
        {
            return this.i_SpanCodeBroker.SpanCodeWith( facilityID, code ) ;
        }

        #endregion


        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public SpanCodeBrokerProxy()
        {
        }

        #endregion

        #region Data Elements
        private ISpanCodeBroker i_SpanCodeBroker = BrokerFactory.BrokerOfType< ISpanCodeBroker >() ;
        #endregion

      

    }

}
