using System;
using System.Globalization;
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Handling time related requests
    /// </summary>
    [Serializable]
    public class TimeBroker : MarshalByRefObject, ITimeBroker
    {
 
        #region public method       

        public DateTime TimeAt( int gmtOffset, int dstOffset )
        {
            DateTime currentAppSvrTime = DateTime.Now;

            return GetFacilityTime( gmtOffset, dstOffset, currentAppSvrTime, false );
        }

        /// <summary>
        /// currentAppSvrTime is considered as app sever local time with local TimeZone
        /// info.   
        /// </summary>
        /// <param name="dstOffset"></param>
        /// <param name="currentAppSvrTime"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        public DateTime TimeAt( int gmtOffset, int dstOffset, DateTime currentAppSvrTime )                 
        {
            return GetFacilityTime( gmtOffset, dstOffset, currentAppSvrTime, false );     
        }

        /// <summary>
        /// Parameter isUTCTime is to tell if the 2nd parameter currentAppSvrTime is UTC time or not.
        /// if isUTCTime is true, the 2nd parameter should be converted from explicit string using 
        /// ParseExact with offset defined, otherwise, when the UTC time is converted back to localTime,  
        /// the local TimeZone will be used including saving time or standard time info.  
        /// If isUTCTime is false, the 2nd parameter is considered as appsvr local time with local 
        /// TimeZone info.   
        /// </summary>
        /// <param name="dstOffset"></param>
        /// <param name="currentAppSvrTime"></param>
        /// <param name="isUTCTime"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        public DateTime TimeAt( int gmtOffset, int dstOffset, DateTime currentAppSvrTime, bool isUTCTime )                 
        {
            return GetFacilityTime( gmtOffset, dstOffset, currentAppSvrTime, isUTCTime );     
        }
        #endregion

        #region private method   
        private DateTime GetFacilityTime( int gmtOffset, int dstOffset, DateTime currentAppSvrTime, bool isUTCTime )                 
        {            
            DateTime currentAppSvrUTCTime;
            if( isUTCTime )
            {
                currentAppSvrUTCTime = currentAppSvrTime;
                currentAppSvrTime = currentAppSvrTime.ToLocalTime();
            }
            else
                currentAppSvrUTCTime = currentAppSvrTime.ToUniversalTime();            

            DateTime facilityTime = FacilityTimeFrom( currentAppSvrUTCTime, gmtOffset );  

            DateTime adjustedFacilityTime = ApplyDSTOffset( facilityTime, dstOffset, currentAppSvrTime ); 

            return adjustedFacilityTime;
        }    

        private DateTime FacilityTimeFrom( DateTime currentAppSvrUTCTime, int facilityUTCOffset )                 
        {             
            DateTime facilityTime = new DateTime( currentAppSvrUTCTime.AddHours( facilityUTCOffset ).Ticks );

            return facilityTime;
        }

        private DateTime ApplyDSTOffset( DateTime facilityTime, int dstOffset, DateTime currentAppSvrTime )                 
        {            
            int currentAppSvrYear = currentAppSvrTime.Year;
            DateTime facilityTimeTemp = facilityTime;

            TimeZone localZone = TimeZone.CurrentTimeZone;
            DaylightTime daylight = localZone.GetDaylightChanges( currentAppSvrYear );

            DateTime appSvrDSTStartTime = daylight.Start;
            DateTime appSvrDSTEndTime = daylight.End;
            
            if( facilityTime.Year >  currentAppSvrTime.Year )            
                facilityTimeTemp = facilityTime.AddYears( -1 );            
            else if( facilityTime.Year <  currentAppSvrTime.Year )            
                facilityTimeTemp = facilityTime.AddYears( 1 );            

            if( facilityTimeTemp >= appSvrDSTStartTime &&  facilityTimeTemp < appSvrDSTEndTime )
            {
                facilityTime = facilityTime.AddHours( dstOffset );
                if( facilityTime >= appSvrDSTEndTime )
                {
                    facilityTime = facilityTime.AddHours( - dstOffset );
                }
            }

            return facilityTime;
        }
        #endregion
        
        #region Construction and Finalization
        public TimeBroker()
        : base()
        {
        }
        #endregion
    }

}