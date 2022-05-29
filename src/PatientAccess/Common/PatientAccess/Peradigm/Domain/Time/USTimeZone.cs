using System;
using System.Collections;
using System.Globalization;
using TimeZoneNotFoundException = Peradigm.Framework.Domain.Time.Exceptions.TimeZoneNotFoundException;

namespace Peradigm.Framework.Domain.Time
{
    [Serializable]
    public class USTimeZone : Object
    {
        #region Constants
        private const string
            UNIVERSAL_TIME_ZONE_STANDARD_NAME        = "GMT",
            UNIVERSAL_TIME_ZONE_STANDARD_DESCRIPTION = "Greenwich Mean Time",

            ATLANTIC_TIME_ZONE_STANDARD_NAME        = "AST",
            ATLANTIC_TIME_ZONE_STANDARD_DESCRIPTION = "Atlantic Standard Time",
           
            EASTERN_TIME_ZONE_STANDARD_NAME         = "EST",
            EASTERN_TIME_ZONE_STANDARD_DESCRIPTION  = "Eastern Standard Time",
            EASTERN_TIME_ZONE_DAYLIGHT_NAME         = "EDT",
            EASTERN_TIME_ZONE_DAYLIGHT_DESCRIPTION  = "Eastern Daylight Time",

            CENTRAL_TIME_ZONE_STANDARD_NAME         = "CST",
            CENTRAL_TIME_ZONE_STANDARD_DESCRIPTION  = "Central Standard Time",
            CENTRAL_TIME_ZONE_DAYLIGHT_NAME         = "CDT",
            CENTRAL_TIME_ZONE_DAYLIGHT_DESCRIPTION  = "Central Daylight Time",

            MOUNTAIN_TIME_ZONE_STANDARD_NAME        = "MST",
            MOUNTAIN_TIME_ZONE_STANDARD_DESCRIPTION = "Mountain Standard Time",
            MOUNTAIN_TIME_ZONE_DAYLIGHT_NAME        = "MDT",
            MOUNTAIN_TIME_ZONE_DAYLIGHT_DESCRIPTION = "Mountain Daylight Time",

            PACIFIC_TIME_ZONE_STANDARD_NAME         = "PST",
            PACIFIC_TIME_ZONE_STANDARD_DESCRIPTION  = "Pacific Standard Time",
            PACIFIC_TIME_ZONE_DAYLIGHT_NAME         = "PDT",
            PACIFIC_TIME_ZONE_DAYLIGHT_DESCRIPTION  = "Pacific Daylight Time",

            ALASKAN_TIME_ZONE_STANDARD_NAME         = "AKST",
            ALASKAN_TIME_ZONE_STANDARD_DESCRIPTION  = "Alaskan Standard Time",

            HAWAII_TIME_ZONE_STANDARD_NAME          = "HST",
            HAWAII_TIME_ZONE_STANDARD_DESCRIPTION   = "Hawaiian-Aleutian Standard Time",

            SAMOA_TIME_ZONE_STANDARD_NAME           = "SST",
            SAMOA_TIME_ZONE_STANDARD_DESCRIPTION    = "Samoa Standard Time",

            NO_DAYLIGHT_NAME                        = "",
            NO_DAYLIGHT_DESCRIPTION                 = "";

        private const bool
            OBSERVES_DAY_LIGHT_SAVINGS          = true,
            DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS  = false;

        private const int
            UNIVERSAL_TIME_ZONE_UTC_OFFSET      = 0,
            ATLANTIC_TIME_ZONE_UTC_OFFSET       = -4,
            EASTERN_TIME_ZONE_UTC_OFFSET        = -5,
            CENTRAL_TIME_ZONE_UTC_OFFSET        = -6,
            MOUNTAIN_TIME_ZONE_UTC_OFFSET       = -7,
            PACIFIC_TIME_ZONE_UTC_OFFSET        = -8,
            ALASKAN_TIME_ZONE_UTC_OFFSET        = -9,
            HAWAII_TIME_ZONE_UTC_OFFSET         = -10,
            SAMOA_TIME_ZONE_UTC_OFFSET          = -11;
        #endregion

        #region Class Methods
        public static ArrayList AllTimeZones()
        {
            return USTimeZone.TimeZones.Clone() as ArrayList;
        }

        private static USTimeZone TimeZoneFor( string name, bool observesDaylightSavings )
        {
            USTimeZone timeZone = null;
            
            foreach( USTimeZone tz in USTimeZone.TimeZones )
            {
                if( tz.ObservesDaylightSavings == observesDaylightSavings && 
                    ( name == tz.StandardName ||
                      name == tz.DaylightName || 
                      name == tz.StandardDescription ||
                      name == tz.DaylightDescription ) )
                {
                    timeZone = tz;
                }
            }

            if( timeZone == null )
            {
                throw new TimeZoneNotFoundException();
            }

            return timeZone;
        }

        /// <summary>
        /// Returns timezone for the name provided, first tries to match
        /// timezone which observes daylight savings.
        /// </summary>
        public static USTimeZone TimeZoneFor( string name )
        {
            USTimeZone timeZone = null;
            try
            {
                timeZone = USTimeZone.TimeZoneFor( name, true );
            }
            catch ( TimeZoneNotFoundException )
            {
            }
            
            if( timeZone == null )
            {
                timeZone = USTimeZone.TimeZoneFor( name, false );
            }

            return timeZone;
        }

        public static USTimeZone UniversalTimeZone()
        {
            return USTimeZone.TimeZoneFor( UNIVERSAL_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone AtlanticTimeZone()
        {
            return USTimeZone.TimeZoneFor( ATLANTIC_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone EasternTimeZone()
        {
            return USTimeZone.TimeZoneFor( EASTERN_TIME_ZONE_STANDARD_NAME, 
                                           OBSERVES_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone EasternNoDaylightSavingsTimeZone()
        {
            return USTimeZone.TimeZoneFor( EASTERN_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone CentralTimeZone()
        {
            return USTimeZone.TimeZoneFor( CENTRAL_TIME_ZONE_STANDARD_NAME, 
                                           OBSERVES_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone MountainTimeZone()
        {
            return USTimeZone.TimeZoneFor( MOUNTAIN_TIME_ZONE_STANDARD_NAME, 
                                           OBSERVES_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone MountainNoDaylightSavingsTimeZone()
        {
            return USTimeZone.TimeZoneFor( MOUNTAIN_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone PacificTimeZone()
        {
            return USTimeZone.TimeZoneFor( PACIFIC_TIME_ZONE_STANDARD_NAME, 
                                           OBSERVES_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone AlaskanTimeZone()
        {
            return USTimeZone.TimeZoneFor( ALASKAN_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone HawaiiAleutianTimeZone()
        {
            return USTimeZone.TimeZoneFor( HAWAII_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone SamoaTimeZone()
        {
            return USTimeZone.TimeZoneFor( SAMOA_TIME_ZONE_STANDARD_NAME, 
                                           DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
        }

        public static USTimeZone LocalTimeZone()
        {
            TimeZone currentTimeZone = TimeZone.CurrentTimeZone;

            bool observesDaylightSavings = !currentTimeZone.DaylightName.Equals( currentTimeZone.StandardName );
            return USTimeZone.TimeZoneFor( currentTimeZone.StandardName, observesDaylightSavings );
        }

        public static bool IsDaylightSavingTime( DateTime time, DaylightTime daylightTime )
        {            
            return TimeZone.IsDaylightSavingTime( time, daylightTime );
        }

        private static void InitializeTimeZones()
        {
            USTimeZone universalTZ = new USTimeZone( UNIVERSAL_TIME_ZONE_STANDARD_NAME, 
                                                    UNIVERSAL_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                    new TimeSpan( UNIVERSAL_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                    DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );

            USTimeZone atlanticTZ = new USTimeZone( ATLANTIC_TIME_ZONE_STANDARD_NAME, 
                                                    ATLANTIC_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                    new TimeSpan( ATLANTIC_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                    DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
            
            USTimeZone easternTZ = new USTimeZone( EASTERN_TIME_ZONE_STANDARD_NAME, 
                                                    EASTERN_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                    EASTERN_TIME_ZONE_DAYLIGHT_NAME, 
                                                    EASTERN_TIME_ZONE_DAYLIGHT_DESCRIPTION, 
                                                    new TimeSpan( EASTERN_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                    OBSERVES_DAY_LIGHT_SAVINGS );
            
            USTimeZone easternNoDaylightTZ = new USTimeZone( EASTERN_TIME_ZONE_STANDARD_NAME, 
                                                             EASTERN_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                             new TimeSpan( EASTERN_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                             DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
            
            USTimeZone centralTZ = new USTimeZone( CENTRAL_TIME_ZONE_STANDARD_NAME, 
                                                   CENTRAL_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                   CENTRAL_TIME_ZONE_DAYLIGHT_NAME, 
                                                   CENTRAL_TIME_ZONE_DAYLIGHT_DESCRIPTION, 
                                                   new TimeSpan( CENTRAL_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                   OBSERVES_DAY_LIGHT_SAVINGS );
            
            USTimeZone mountainTZ = new USTimeZone( MOUNTAIN_TIME_ZONE_STANDARD_NAME, 
                                                    MOUNTAIN_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                    MOUNTAIN_TIME_ZONE_DAYLIGHT_NAME, 
                                                    MOUNTAIN_TIME_ZONE_DAYLIGHT_DESCRIPTION, 
                                                    new TimeSpan( MOUNTAIN_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                    OBSERVES_DAY_LIGHT_SAVINGS );
            
            USTimeZone mountainNoDaylightTZ = new USTimeZone( MOUNTAIN_TIME_ZONE_STANDARD_NAME, 
                                                              MOUNTAIN_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                              new TimeSpan( MOUNTAIN_TIME_ZONE_UTC_OFFSET, 0, 0 ),
                                                              DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
            
            USTimeZone pacificTZ = new USTimeZone( PACIFIC_TIME_ZONE_STANDARD_NAME, 
                                                   PACIFIC_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                   PACIFIC_TIME_ZONE_DAYLIGHT_NAME, 
                                                   PACIFIC_TIME_ZONE_DAYLIGHT_DESCRIPTION, 
                                                   new TimeSpan( PACIFIC_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                   OBSERVES_DAY_LIGHT_SAVINGS );
            
            USTimeZone alaskanTZ = new USTimeZone( ALASKAN_TIME_ZONE_STANDARD_NAME, 
                                                   ALASKAN_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                   new TimeSpan( ALASKAN_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                   DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
            
            USTimeZone hawaiiTZ = new USTimeZone( HAWAII_TIME_ZONE_STANDARD_NAME, 
                                                  HAWAII_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                  new TimeSpan( HAWAII_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                  DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );
            
            USTimeZone samoaTZ = new USTimeZone( SAMOA_TIME_ZONE_STANDARD_NAME, 
                                                 SAMOA_TIME_ZONE_STANDARD_DESCRIPTION, 
                                                 new TimeSpan( SAMOA_TIME_ZONE_UTC_OFFSET, 0, 0 ), 
                                                 DOES_NOT_OBSERVE_DAY_LIGHT_SAVINGS );

            USTimeZone.TimeZones.Add( universalTZ );
            USTimeZone.TimeZones.Add( atlanticTZ );
            USTimeZone.TimeZones.Add( easternTZ );
            USTimeZone.TimeZones.Add( easternNoDaylightTZ );
            USTimeZone.TimeZones.Add( centralTZ );
            USTimeZone.TimeZones.Add( mountainTZ );
            USTimeZone.TimeZones.Add( mountainNoDaylightTZ );
            USTimeZone.TimeZones.Add( pacificTZ ); 
            USTimeZone.TimeZones.Add( alaskanTZ );
            USTimeZone.TimeZones.Add( hawaiiTZ );
            USTimeZone.TimeZones.Add( samoaTZ );
        }

        public static string NameFor( USDateTime time )
        {
            return time.TimeZone.NameFor( time.AsDateTime() );
        }

        public static string DescriptionFor( USDateTime time )
        {
            return time.TimeZone.DescriptionFor( time.AsDateTime() );
        }

        public static TimeSpan UTCOffsetFor( USDateTime time )
        {
            return time.TimeZone.DaylightStrategy.UTCOffsetFor( time.AsDateTime(), time.TimeZone );
        }

        public static bool IsDaylightSavingTime( USDateTime time )
        {
            return USTimeZone.IsDaylightSavingTime( time.AsDateTime(), time.TimeZone.DayLightChangesFor( time.Year ) );
        }

        public static USDateTime ToUniversalTime( USDateTime time )
        {
            return new USDateTime( USTimeZone.UniversalTimeZone(), 
                time.TimeZone.ToUniversalTime( time.AsDateTime() ) );
        }

        public static USDateTime ToLocalTime( USDateTime time )
        {
            return new USDateTime( USTimeZone.LocalTimeZone(), time.TimeZone.ToLocalTime( time.AsDateTime() ) );
        }
        #endregion
        
        #region Instance Methods
        public override bool Equals(object obj)
        {
            USTimeZone other = obj as USTimeZone;
            if( other != null )
            {
                return this.DaylightName.Equals( other.DaylightName ) &&
                       this.StandardName.Equals( other.StandardName );
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.StandardName.GetHashCode() ^ this.DaylightName.GetHashCode();
        }

        public override string ToString()
        {
            return this.StandardName;
        }


        private string NameFor( DateTime time )
        {
            return this.DaylightStrategy.TimeZoneNameFor( time, this );
        }

        private string DescriptionFor( DateTime time )
        {
            return this.DaylightStrategy.TimeZoneDescriptionFor( time, this );
        }

        private TimeSpan UTCOffsetFor( DateTime time )
        {
            return this.DaylightStrategy.UTCOffsetFor( time, this );
        }

        private DaylightTime DayLightChangesFor( int year )
        {       
           return this.DaylightStrategy.DaylightChangesFor( year );
        }

        public bool IsDaylightSavingsTime( DateTime time )
        {
            return USTimeZone.IsDaylightSavingTime( time, this.DayLightChangesFor( time.Year ) );
        }

        public DateTime ToUniversalTime( DateTime time )
        {
            return time.Add( -this.UTCOffsetFor( time ) );
        }

        public DateTime FromUniversalTime( DateTime universalTime )
        {
           return universalTime.Add( this.UTCOffsetFor( universalTime ) );
        }

        public DateTime ToLocalTime( DateTime time )
        {
            return this.ConvertTo( time, USTimeZone.LocalTimeZone() );
        }

        private DateTime ConvertTo( DateTime time, USTimeZone toTimeZone )
        {
            DateTime universalTime = this.ToUniversalTime( time );
            return toTimeZone.FromUniversalTime( universalTime );
        }

        private DateTime ConvertFrom( DateTime time, USTimeZone fromTimeZone )
        {
            DateTime universalTime = fromTimeZone.ToUniversalTime( time );
            return this.FromUniversalTime( universalTime );
        }

        public USDateTime ConvertFrom( USDateTime from )
        {
            DateTime date = this.ConvertFrom( from.AsDateTime(), from.TimeZone );
            return new USDateTime( this, date );
        }
        #endregion

        #region Properties
        public DateTime Today
        {
            get 
            {
                return this.Now.Date;
            }
        }

        public DateTime Now
        {
            get
            {
                return this.ConvertFrom( DateTime.Now, USTimeZone.LocalTimeZone() );
            }
        }

        public string StandardName
        {
            get
            {
                return i_StandardName;
            }
            private set
            {
                i_StandardName = value;
            }
        }

        public string DaylightName
        {
            get
            {
                return i_DaylightName;
            }
            private set
            {
                i_DaylightName = value;
            }
        }

        public string StandardDescription
        {
            get
            {
                return i_StandardDescription;
            }
            private set
            {
                i_StandardDescription = value;
            }
        }

        public string DaylightDescription
        {
            get
            {
                return i_DaylightDescription;
            }
            private set
            {
                i_DaylightDescription = value;
            }
        }

        public TimeSpan StandardUTCOffset
        {
            get
            {
                return i_StandardUTCOffset;
            }
            private set
            {
                i_StandardUTCOffset = value;
            }
        }

        private bool ObservesDaylightSavings
        {
            get
            {
                return i_ObservesDaylightSavings;
            }
            set
            {
                i_ObservesDaylightSavings = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private static ArrayList TimeZones
        {
            get
            {
                return i_TimeZones;
            }
            set
            {
                i_TimeZones = value;
            }
        }        

        private DaylightStrategy DaylightStrategy
        {
            get
            {
                return i_DaylightStrategy;
            }
            set
            {
                i_DaylightStrategy = value;
            }
        }
        #endregion

        #region Construction and Finalization
        static USTimeZone()
        {
            USTimeZone.TimeZones = new ArrayList();
            USTimeZone.InitializeTimeZones();
        }

        public USTimeZone()
        {
            //Default Constructor allows class to be serialized
        }

        private USTimeZone( string standardName, string standardDescription, TimeSpan utcOffset, bool observesDayLightSavings )
            : this( standardName, standardDescription, NO_DAYLIGHT_NAME, NO_DAYLIGHT_DESCRIPTION, utcOffset, observesDayLightSavings )
        {
        }
        
        private USTimeZone( string standardName, string standardDescription, string daylightName, string daylightDescription, TimeSpan utcOffset, bool observesDaylightSavings )
        {
            this.StandardName = standardName;
            this.StandardDescription = standardDescription;
            this.DaylightName = daylightName;
            this.DaylightDescription = daylightDescription;
            this.StandardUTCOffset = utcOffset;
            this.ObservesDaylightSavings = observesDaylightSavings;

            if( observesDaylightSavings )
            {
                this.DaylightStrategy = new ChangingUTCOffSetStrategy();
            }
            else
            {
                this.DaylightStrategy = new ConstantUTCOffSetStrategy();
            }
        }
        #endregion

        #region Data Elements
        private string i_StandardName;
        private string i_StandardDescription;
        private string i_DaylightName;
        private string i_DaylightDescription;
        private TimeSpan i_StandardUTCOffset;
        private bool i_ObservesDaylightSavings;
        private static ArrayList i_TimeZones;
        private DaylightStrategy i_DaylightStrategy;
        #endregion
    }
}