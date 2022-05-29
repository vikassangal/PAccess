using PatientAccess.BrokerInterfaces;
using PatientAccess.Http;

namespace PatientAccess.BrokerProxies
{
    public static class ProxyFactory
    {
        public static ITimeBroker GetTimeBroker()
        {
            var cache = new HttpRuntimeCache();
            var realTimeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            var timeBrokerProxy = new TimeBrokerProxy(realTimeBroker, cache);
            
            return timeBrokerProxy;
        }
    }
}