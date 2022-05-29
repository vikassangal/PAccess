using System;
using System.Globalization;

namespace Peradigm.Framework.Domain.Time
{
//TODO: Create XML summary comment for ConstantUTCOffSetStrategy
    [Serializable]
    public class ConstantUTCOffSetStrategy : DaylightStrategy
    {
        #region Constants
        #endregion

        #region Methods
        public override string TimeZoneDescriptionFor( DateTime time, USTimeZone timeZone )
        {
            return timeZone.StandardDescription;
        }

        public override string TimeZoneNameFor( DateTime time, USTimeZone timeZone )
        {
            return timeZone.StandardName;
        }

        public override TimeSpan UTCOffsetFor( DateTime time, USTimeZone timeZone )
        {
            return timeZone.StandardUTCOffset;
        }

        public override DaylightTime DaylightChangesFor( int year )
        {
            return new DaylightTime( DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ConstantUTCOffSetStrategy()
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
