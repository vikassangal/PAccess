using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TimeZoneNotFoundException = Peradigm.Framework.Domain.Time.Exceptions.TimeZoneNotFoundException;

namespace Peradigm.Framework.Domain.Time
{
    [Serializable]
    public class USDateTime : ICloneable, IComparable, IFormattable, ISerializable   
    {
        #region Constants
        private const string SHORT_DATE_FORMAT      = "d Z",
                             LONG_DATE_FORMAT       = "D Z",
                             SHORT_TIME_FORMAT      = "t Z",
                             LONG_TIME_FORMAT       = "T Z",
                             TO_STRING_FORMAT       = "M/d/yyyy H:m:s.fff Z";

        private const string SERIALIZATION_NAME     = "USDateTime";

        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static USDateTime Now( USTimeZone timeZone )
        {
            return new USDateTime( timeZone, timeZone.Now );
        }

        public USDateTime AsLocal()
        {
            USTimeZone localZone = USTimeZone.LocalTimeZone();
            DateTime localDate = this.TimeZone.ToLocalTime( this.primValue );
            return new USDateTime( localZone, localDate );
        }

        private USDateTime AsUniversal()
        {
            USTimeZone universalZone = USTimeZone.UniversalTimeZone();
            DateTime universalDate = this.TimeZone.ToUniversalTime( this.primValue );
            return new USDateTime( universalZone, universalDate );
        }

        public DateTime AsDateTime()
        {
            return this.primValue;
        }

        public static USDateTime Today( USTimeZone timeZone )
        {
            return new USDateTime( timeZone, timeZone.Today );
        }

        public virtual object Clone()
        {
            USDateTime clone = new USDateTime();
            clone.primTimeZone = this.TimeZone;
            clone.primValue = this.primValue;

            return clone;
        }

        public USDateTime Add( TimeSpan value )
        {
            DateTime universalTime = this.AsUniversalDateTime().Add( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddDays( double value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddDays( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddHours( double value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddHours( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddMilliseconds( double value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddMilliseconds( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddMinutes( double value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddMinutes( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddMonths( int value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddMonths( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddSeconds( double value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddSeconds( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddTicks( long value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddTicks( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public USDateTime AddYears( int value )
        {
            DateTime universalTime = this.AsUniversalDateTime().AddYears( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public TimeSpan Subtract( USDateTime other )
        {
            if( this.TimeZone == other.TimeZone )
            {
                return this.primValue.Subtract( other.primValue );
            }
            else
            {
                return this.AsUniversalDateTime().Subtract( other.AsUniversalDateTime() );
            }
        }

        public USDateTime Subtract( TimeSpan value )
        {
            DateTime universalTime = this.AsUniversalDateTime().Subtract( value );
            DateTime newTime = this.TimeZone.FromUniversalTime( universalTime );
            return new USDateTime( this.TimeZone, newTime );
        }

        public virtual int CompareTo( object value )
        {
            if( value == null )
            {
                return 1;
            }

            if( value is USDateTime )
            {
                USDateTime other = (USDateTime)value;
                return this.CompareTo( other );
            }
            else
            {
                throw new ArgumentException( "Value is not USDateTime", value.ToString() );
            }
        }

        public override bool Equals( object value )
        {
            return this.CompareTo( value ) == 0;
        }

        private int CompareTo( USDateTime other )
        {
            if( this.TimeZone == other.TimeZone )
            {
                return this.primValue.CompareTo( other.primValue );
            }
            else
            {
                return this.AsUniversalDateTime().CompareTo( other.AsUniversal() );
            }
        }

        public override int GetHashCode()
        {
            return this.TimeZone.GetHashCode() ^ this.primValue.GetHashCode();
        }

        public string ToString( string format, IFormatProvider formatProvider )
        {
            USDateTimeFormatInfo usDateformatter = null;
            if( formatProvider != null && formatProvider is USDateTimeFormatInfo )
            {
                usDateformatter = (USDateTimeFormatInfo)formatProvider;
            }
            else
            {
                usDateformatter = new USDateTimeFormatInfo();
            }
            return usDateformatter.Format( format, this, usDateformatter );
        }

        public string ToShortDateString()
        {

            return this.ToString( SHORT_DATE_FORMAT, null ); 
        }

        public string ToShortTimeString()
        {
            return this.ToString( SHORT_TIME_FORMAT, null ); 
        }

        public string ToLongDateString()
        {
            return this.ToString( LONG_DATE_FORMAT, null ); 
        }

        public string ToLongTimeString()
        {
            return this.ToString( LONG_TIME_FORMAT, null ); 
        }

        public override string ToString()
        {
            return this.ToString( TO_STRING_FORMAT, null ); 
        }

        public string ToString( string format )
        {
            return this.ToString( format, null ); 
        }

        private static USDateTime Parse( string s )
        {
            string value = s.Trim();
            string zoneString = null;

            USTimeZone zone = USDateTime.LocateTimeZone( value, out zoneString );
            string dateString = null;

            if( zone == null )
            {
                zone = USTimeZone.LocalTimeZone();
                dateString = value;
            }
            else
            {
                int zoneIndex = value.LastIndexOf( zoneString );
                dateString = value.Substring( 0, zoneIndex - 1 );
            }
            
            DateTime date = DateTime.Parse( dateString );
            return new USDateTime( zone, date );
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue( SERIALIZATION_NAME, this.ToString() );
        }

        #endregion

        #region Properties
        public USDateTime Date
        {
            get
            {
                return new USDateTime( this.TimeZone, this.primValue.Date );
            }
        }

        public int Day
        {
            get
            {
                return this.primValue.Day;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return this.primValue.DayOfWeek;
            }
        }

        public int DayOfYear
        {
            get
            {
                return this.primValue.DayOfYear;
            }
        }

        public int Hour
        {
            get
            {
                return this.primValue.Hour;
            }
        }

        public int Millisecond
        {
            get
            {
                return this.primValue.Millisecond;
            }
        }

        public int Month
        {
            get
            {
                return this.primValue.Month;
            }
        }

        public int Second
        {
            get
            {
                return this.primValue.Second;
            }
        }

        public int Year
        {
            get
            {
                return this.primValue.Year;
            }
        }

        public long Ticks
        {
            get
            {
                return this.primValue.Ticks;
            }
        }

        public TimeSpan TimeOfDay
        {
            get
            {
                return this.primValue.TimeOfDay;
            }
        }

        [XmlAttribute( AttributeName = "Date" )]
        public string DateWithTimeZone
        {
            get
            {
                return this.ToString();
            }
            set
            {
                USDateTime usDateTime = USDateTime.Parse( value );
                this.primTimeZone = usDateTime.TimeZone;
                this.primValue = usDateTime.primValue;
            }
        }
        #endregion

        #region Private Methods
        private DateTime AsUniversalDateTime()
        {
            return this.TimeZone.ToUniversalTime( this.primValue );
        }

        private static USTimeZone LocateTimeZone( string usDateTimeString, out string zoneSpecification )
        {
            string[] pieces = usDateTimeString.Split( null );
            int piecesCount = pieces.Length;
            zoneSpecification = String.Empty;

            if( piecesCount < 1 )
            {
                throw new FormatException( "Invalid USDateTime representation " + usDateTimeString );
            }

            string zoneNameString = pieces[piecesCount - 1],
                   zoneDescriptionString = null;
            USTimeZone timeZone = null;

            if( piecesCount > 3 )
            {
                zoneDescriptionString = String.Format( "{0} {1} {2}",
                                                       pieces[piecesCount - 3],
                                                       pieces[piecesCount - 2],
                                                       pieces[piecesCount - 1] );
            }
            try
            {
                zoneSpecification = zoneNameString;
                timeZone = USTimeZone.TimeZoneFor( zoneNameString );
            }
            catch ( TimeZoneNotFoundException )
            {
            }
            if( timeZone == null && zoneDescriptionString != null )
            {
                try
                {
                    zoneSpecification = zoneDescriptionString;
                    timeZone = USTimeZone.TimeZoneFor( zoneDescriptionString );
                }
                catch ( TimeZoneNotFoundException )
                {
                }
            }
            return timeZone;
        }
        #endregion

        #region Private Properties
        public USTimeZone TimeZone
        {
            get
            {
                return primTimeZone;
            }
        }

        private DateTime primValue
        {
            get
            {
                return i_Value;
            }
            set
            {
                i_Value = value;
            }
        }

        private USTimeZone primTimeZone
        {
            get
            {
                return i_TimeZone;
            }
            set
            {
                i_TimeZone = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public USDateTime( USTimeZone timeZone, DateTime value )
        {
            this.primTimeZone = timeZone;
            this.primValue = value;
        }

        public USDateTime()
        {
        }

        protected USDateTime( SerializationInfo info, StreamingContext context )
        {
            string usDateTimeString = info.GetString( SERIALIZATION_NAME );
            USDateTime usDateTime = USDateTime.Parse( usDateTimeString );
            
            this.primTimeZone = usDateTime.primTimeZone;
            this.primValue = usDateTime.primValue;
        }
        #endregion

        #region Data Elements
        private DateTime i_Value;
        private USTimeZone i_TimeZone;
        #endregion
    }
}
