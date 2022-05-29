using System;
using System.Text;

namespace Peradigm.Framework.Domain.Time
{
    [Serializable]
    public class USDateTimeFormatInfo : IFormatProvider, ICustomFormatter
    {
        #region Constants
        private const string SHORT_TIME_ZONE_FORMAT     = "Z",
                             FULL_TIME_ZONE_FORMAT      = "ZZ";
        private static readonly string[] ZONE_FORMATS = { SHORT_TIME_ZONE_FORMAT,
                                                            FULL_TIME_ZONE_FORMAT };
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public object GetFormat(Type formatType)
        {
            if( typeof(ICustomFormatter).Equals(formatType) ) 
            {
                return this;
            }

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if( arg == null )
            {
                throw new ArgumentNullException( "arg" );
            }
 
            if( format != null && arg is USDateTime )
            {
                string shortFormat, zoneFormat;
                this.SplitFormatWithTimeZone( format, out shortFormat, out zoneFormat );

                return this.FormatWithTimeZone( (USDateTime)arg, shortFormat, zoneFormat );
            }
 
            if ( arg is IFormattable )
            {
                return ((IFormattable)arg).ToString( format, formatProvider );
            }
            else
            {
                return arg.ToString();
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void SplitFormatWithTimeZone( string format, out string shortFormat, out string zoneString  )
        {
            string[] formatPieces = format.Split( null );
            string last = formatPieces[ formatPieces.Length - 1];
            int index = Array.IndexOf( ZONE_FORMATS, last );
            
            if( index != -1 )
            {
                zoneString = last;
                shortFormat = format.Remove( format.Length - last.Length, last.Length ).Trim();
            }
            else
            {
                shortFormat = format;
                zoneString = SHORT_TIME_ZONE_FORMAT;
            }
        }

        private string FormatWithTimeZone( USDateTime arg, string shortFormat, string zoneFormat )
        {
            string shortResult = arg.AsDateTime().ToString( shortFormat );
            string zoneDescription = String.Empty;

            switch( zoneFormat )
            {
                case SHORT_TIME_ZONE_FORMAT : 
                    zoneDescription = USTimeZone.NameFor( arg ); 
                    break;

                case FULL_TIME_ZONE_FORMAT : 
                    zoneDescription = USTimeZone.DescriptionFor( arg ); 
                    break;
            }
            return new StringBuilder( shortResult ).Append(' ').Append( zoneDescription ).ToString();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public USDateTimeFormatInfo()
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
