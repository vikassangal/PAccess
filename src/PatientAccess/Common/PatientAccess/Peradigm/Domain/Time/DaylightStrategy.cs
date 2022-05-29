using System;
using System.Globalization;

namespace Peradigm.Framework.Domain.Time
{
    [Serializable]
    public abstract class DaylightStrategy : Object
    {
        #region Constants
        #endregion

        #region Methods
        public abstract string TimeZoneNameFor( DateTime time, USTimeZone timeZone );
        public abstract string TimeZoneDescriptionFor( DateTime time, USTimeZone timeZone );
        public abstract TimeSpan UTCOffsetFor( DateTime time, USTimeZone timeZone );
        public abstract DaylightTime DaylightChangesFor( int year );
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DaylightStrategy()
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}