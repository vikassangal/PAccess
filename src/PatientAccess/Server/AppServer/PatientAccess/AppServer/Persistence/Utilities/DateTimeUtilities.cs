using System;
using System.Data.SqlTypes;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence.Utilities
{
    /// <summary>
    /// Summary description for DateTimeUtilities.
    /// </summary>
    public class DateTimeUtilities
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static DateTime DateTimeFromPacked(long d)
        {
            if(d == 0)
                return(new DateTime(1,1,1));
            string s = (Math.Abs(d)).ToString();
            return DateTimeFromString(s);
        }
        public static DateTime DateTimeFromString(string d, string time)
        {
            if(d == "0")
                    return new DateTime(0);

            if(time.Length > 4)
                throw new DateFormatException("The time " + time + " is not in a recognizable format");

            DateTime dt = DateTimeFromString(d);
            
            while(time.Length < 4)
                time = "0" + time;
            int hour = Int32.Parse(time.Substring(0,2));
            int minute = Int32.Parse(time.Substring(2,2));
            return(new DateTime(dt.Year,dt.Month,dt.Day,hour,minute,0));
        }

        public static DateTime DateTimeFromString(string date)
        {
            if(date == "0" || date == "00000000" || date.Trim().Length == 0)
                    return new DateTime(0);

            if(date.Length <= 4 || date.Length > 8)
            {
                throw new DateFormatException("The date " + date + " is not in a recognizable format");
            }

            string origDateStr = date;

            if(date.Length == 5)
                date = "0" + date;

            if(date.Length == 7)
                date = "0" + date;

            int m = Int32.Parse(date.Substring(0,2));
            int d = Int32.Parse(date.Substring(2,2));
            int y = 0;
            if(date.Length == 6)
            {
                y = Int32.Parse(date.Substring(4,2));
                DateTime curDate = DateTime.Now;
                int calcYear = curDate.Year - 99;
                string calcYearStr = calcYear.ToString().Substring(2,2);
                int calcYearPart = Int32.Parse(calcYearStr);
                if( y > calcYearPart )
                {
                    y = 1900 + y;
                }
                else
                {
                    y = 2000 + y;
                } 
            }
            else
            {
                y = Int32.Parse(date.Substring(4,4));
            }

            return(validDateTime(y,m,d));
        }


        public static DateTime DateTimeForYYMMDDFormat(long lDate)
        {
           
            if( lDate == 0 )
            {
                return new DateTime(0);
            }
            string date = (Math.Abs(lDate)).ToString();

            if( date.Length < 3 || date.Length > 6 )
            {
                throw new DateFormatException("The date " + date + " is not in a recognizable format");
            }

            date = date.PadLeft( 6, '0' );

            int y = Int32.Parse(date.Substring(0,2));
            int m = Int32.Parse(date.Substring(2,2));
            int d = Int32.Parse(date.Substring(4,2));

            y = LongYearFromPartYear( y );
        
            return( validDateTime(y,m,d) );
        }

        public static DateTime DateTimeForYYDDMMFormat(long lDate)
        {
            string date = (Math.Abs(lDate)).ToString();

            if( date.Length < 3 || date.Length > 6 )
            {
                throw new DateFormatException("The date " + date + " is not in a recognizable format");
            }

            date = date.PadLeft( 6, '0' );

            int y = Int32.Parse(date.Substring(0,2));
            int d = Int32.Parse(date.Substring(2,2));
            int m = Int32.Parse(date.Substring(4,2));

            y = LongYearFromPartYear( y );

            return( validDateTime(y,m,d) );
        }


        public static DateTime DateTimeForYYYYMMDDFormat(long lDate)
        {
            string date = (Math.Abs(lDate)).ToString();

            if( lDate == 0 )
            {
                return new DateTime(0);
            }

            if( date.Length != 8 )
            {
                throw new DateFormatException("The date " + date + " is not in a recognizable format");
            }

            int y = Int32.Parse(date.Substring(0,4));
            int m = Int32.Parse(date.Substring(4,2));
            int d = Int32.Parse(date.Substring(6,2));

            return( validDateTime(y,m,d));
        }

        public static DateTime DateTimeForYYYYMMDDFormat(string sDate)
        {
            if( sDate == null || sDate.Equals(String.Empty) )
            {
                return new DateTime(0);
            }
            if( sDate.Trim().Length > 0 && sDate.Trim().Equals("0") )
            {
                return new DateTime(0);
            }
            string trimmedDate = sDate.Trim();

            if( trimmedDate.Length != 8 )
            {
                throw new DateFormatException("The date " + trimmedDate + " is not in a recognizable format");
            }

            int y = Int32.Parse(trimmedDate.Substring(0,4));
            int m = Int32.Parse(trimmedDate.Substring(4,2));
            int d = Int32.Parse(trimmedDate.Substring(6,2));

            return( validDateTime(y,m,d) );
        }

        /// <summary>
        /// Convert a 2 digit year to a 4 digit year using standard Y2K
        /// Mainframe technique
        /// </summary>
        /// <param name="partYear"></param>
        /// <returns></returns>
        public static int LongYearFromPartYear(int partYear)
        {
            int year = partYear;
            DateTime curDate = DateTime.Now;
            int calcYear = curDate.Year - 99;
            string calcYearStr = calcYear.ToString().Substring(2,2);
            int calcYearPart = Int32.Parse(calcYearStr);
            if( year > calcYearPart )
            {
                year = 1900 + year;
            }
            else
            {
                year = 2000 + year;
            }

            return year;
        }

        public static int PackedDateFromDate(DateTime dt)
        {
            string day = dt.Day.ToString();
            string month = dt.Month.ToString();
            string year = dt.ToString("yy");

            if ( day.Length == 1)
            {
                day = "0" + day;
            }

            string dateStr = month + day + year;

            return (Int32.Parse(dateStr));
        }

        public static bool IsValidDateTime( string MM, string DD, string YYYY )
        {
            try
            {
                Convert.ToDateTime( MM + "/" + DD + "/" + YYYY );   
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime DateTimeForMMDDYYFormat( long lDate )
        {
            if( lDate == 0 )
            {
                return DateTime.MinValue;
            }

            string date = (Math.Abs(lDate)).ToString();

            if( date.Length < 3 || date.Length > 6 )
            {
                throw new DateFormatException( "The date " + date + " is not in a recognizable format" );
            }

            date  = date.PadLeft( 6, '0' );

            int m = Int32.Parse( date.Substring( 0, 2 ) );
            int d = Int32.Parse( date.Substring( 2, 2 ) );
            int y = Int32.Parse( date.Substring( 4, 2 ) );

            y = LongYearFromPartYear( y );

            return( validDateTime( y, m, d ) );
        }

        public static DateTime FullDateTime( long lDate, int hour, int minute, int second )
        {
            DateTime dateTime;
            try
            {
                dateTime = DateTimeForYYMMDDFormat( lDate );

                dateTime = new DateTime(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day,
                    hour,
                    minute,
                    second );
            }
            catch 
            {
                dateTime = new DateTime();
            }
            return dateTime;
        }

        /// <summary>
        /// The sql server 2005 datetime data type has a narrower range for dates than the .Net framework DateTime type. 
        /// This method determines whether the input object is within that range.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if input is within the SQL server datetime range; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidSqlDateTime( DateTime date )
        {
            return ( ( date >= (DateTime)SqlDateTime.MinValue ) && ( date <= (DateTime)SqlDateTime.MaxValue ) );
        }

        public static DateRange BringWithInSqlServerDateTimeRange( DateRange dateRange )
        {
            if ( !IsValidSqlDateTime( dateRange.FromDate ) )
            {
                dateRange.FromDate = (DateTime)SqlDateTime.MinValue;
            }

            if ( !IsValidSqlDateTime( dateRange.ToDate ) )
            {
                dateRange.ToDate = (DateTime)SqlDateTime.MaxValue;
            }
            return dateRange;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private static DateTime validDateTime(int y, int m, int d)
        {
            DateTime newDateTime;

            try
            {
                newDateTime = new DateTime( y,m,d );
            }
            catch (ArgumentOutOfRangeException)
            {
                c_log.Error("Invalid Date detected " + 
                    y + " " + m + " " + d );
                newDateTime = DateTime.MinValue;
            }

            return( newDateTime );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DateTimeUtilities()
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger(typeof(DateTimeUtilities));
        #endregion
    }
}
