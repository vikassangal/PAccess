using System;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for ITimeBroker.
    /// </summary>
    public interface ITimeBroker
    {		
        DateTime TimeAt( int gmtOffset, int dstOffset );    
    }
}