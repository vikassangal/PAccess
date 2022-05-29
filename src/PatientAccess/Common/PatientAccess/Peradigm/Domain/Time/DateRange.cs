using System;
using System.Collections;
using Peradigm.Framework.Domain.Exceptions;
using Peradigm.Framework.Domain.Time.Exceptions;

namespace Peradigm.Framework.Domain.Time
{
	public enum PeriodTypes
	{
		Month = 1,
		Quarter = 3,
		Year = 12
	}
	/// <summary>
	/// Summary description for DateRange.
	/// </summary>
	[Serializable]
    public class DateRange : IComparable
	{
        #region Constants
        private const string
            ERROR_DATERANGE = "Start date must be less than or equal to end date",
            ERROR_DATERANGE_SPLIT = "Split point must be between start date and end date";
        #endregion

        #region Event Handlers
		#endregion

		#region Methods
        public bool Abuts( DateRange range )
        {
            return !this.Intersects( range ) && this.GapBetween( range ).IsZeroLength();
        }
        public int CompareTo( object obj )
        {            
            int comparison = 0;
            DateRange other = obj as DateRange;

            if( other != null )
            {
                if( !this.StartDate.Equals( other.StartDate ) )
                {
                    comparison = this.StartDate.CompareTo( other.StartDate );
                }
                else
                {
                    comparison = this.EndDate.CompareTo( other.EndDate );
                }
            }
            else
            {
                throw new InvalidParameterException( this, obj );
            }         
   
            return comparison;
        }

        /// <summary>
        /// Returns true if the supplied time belongs to the date range, 
        /// both ends included.
        /// </summary>
        private bool Contains( DateTime time )
        {            
            return ( time < this.EndDate ) && ( time > this.StartDate );
        }

	    private bool Contains( DateRange range )
        {
            return this.Contains( range.StartDate ) && this.Contains( range.EndDate );
        }

        public override bool Equals( object obj )
        {
            bool equal = false;

            DateRange other = obj as DateRange;

            if( other != null )
            {
                equal = this.StartDate.Equals( other.StartDate ) && this.EndDate.Equals( other.EndDate );
            }

            return equal;
        }

	    private DateRange GapBetween( DateRange range )
        {            
            DateRange gapRange;
            DateRange lowerRange;
            DateRange higherRange;
            
            if( this.Intersects( range ) )
            {
                gapRange = DateRange.Empty();
            }
            else
            {
                if( this.CompareTo( range ) < 0 )
                {
                    lowerRange = this;
                    higherRange = range;
                }
                else
                {
                    lowerRange = range;
                    higherRange = this;
                }

                gapRange = new DateRange( lowerRange.EndDate, higherRange.StartDate );
            }

            return gapRange;
        }

        public override int GetHashCode()
        {
            return this.StartDate.GetHashCode();
        }

	    private bool Intersects( DateRange range )
        {
            return this.Contains( range.StartDate ) || this.Contains( range.EndDate ) || this.Contains( range );
        }
        
        /// <summary>
        /// Returns true if the date range occurred in the past
        /// </summary>
        public bool IsCompletePeriod()
        {
            return this.EndDate < DateTime.Today;
        }

        public bool IsEmpty()
        {
            return DateRange.Empty().Equals( this );
        }

        /// <summary>
        /// Returns true if the range's time length is zero. 
        /// </summary>
        /// <returns></returns>
        private bool IsZeroLength()
        {
            return this.StartDate == this.EndDate;
        }

        /// <summary>
        /// Join two date ranges into one
        /// </summary>
        /// <param name="dateRange">The date range with which to join</param>
        /// <returns>The resulting date range</returns>        
        public DateRange JoinWith( DateRange dateRange )
        {
            DateRange newRange = null;
            int comparison = this.CompareTo( dateRange );
            
            if( comparison.Equals( -1 ) )
            {
                newRange = new DateRange( this.StartDate, dateRange.EndDate );                
            }
            else if( comparison.Equals( 1 ) )
            {
                newRange = new DateRange( dateRange.StartDate, this.EndDate );
            }
            else
            {
                newRange = new DateRange( this.StartDate, this.EndDate );
            }

            return newRange;
        }

        /// <summary>
        /// Break the date range up into it's monthly components
        /// </summary>
        /// <returns>An ArrayList of full month DateRange objects</returns>
        public ArrayList MonthsInRange()
        {
            int inc = 0;
            ArrayList months = new ArrayList();            
            DateTime startMonth = this.StartDate;
            
            startMonth = startMonth.AddDays ( ( -1 * ( startMonth.Day - 1 ) ) );

            while( startMonth <= this.EndDate )
            {
                DateRange newRange = new DateRange( this.StartDate.AddMonths( inc ), 
                                                    PeriodTypes.Month );
                months.Add( newRange );
                
                startMonth = startMonth.AddMonths( 1 );
                inc ++;                
            }           
            
            return months;
        }

        /// <summary>
        /// Splits the date range into two ranges, second one containing 
        /// the point of split.
        /// </summary>
        public DateRange[] Split( DateTime at )
        {
            if( at <= this.StartDate || at >= this.EndDate )
            {
                throw new EnterpriseException( ERROR_DATERANGE_SPLIT );
            }
   
            DateRange firstRange = new DateRange( this.StartDate, at );
            DateRange secondRange = new DateRange( at, this.EndDate );
            DateRange[] ranges = { firstRange, secondRange };
            return ranges;
        }

        public override string ToString()
        {
            return this.StartDate.ToString() + "-" + this.EndDate.ToString();
        }
		#endregion

        #region Class Methods

	    private static DateRange Empty()
        {
            DateRange empty = new DateRange();
            empty.i_EndDate = DateTime.MinValue;
            empty.i_StartDate = DateTime.MaxValue;
            
            return empty;
        }

        #endregion

		#region Properties
		public DateTime StartDate
		{
			get
			{
				return i_StartDate;
			}
		    private set
			{                
                if( this.EndDate == DateTime.MinValue || value <= this.EndDate )
                {
                    i_StartDate = value;
                }
                else
                {
                    throw new EnterpriseException( ERROR_DATERANGE, Severity.Low );
                }
			}
		}

		public DateTime EndDate
		{
			get
			{
				return i_EndDate;
			}
		    private set
			{
                if( this.StartDate == DateTime.MinValue || this.StartDate <= value )
                {
                    i_EndDate = value;
                }
                else
                {
                    throw new EnterpriseException( ERROR_DATERANGE, Severity.Low );
                }                   
			}
		}
		#endregion

		#region Private Methods
		private void InitializeRangeFor( DateTime dateTime, PeriodTypes periodType )
		{
			int numberOfMonths = (int) periodType;
			int month = dateTime.Month;
			int year = dateTime.Year;
			
			int endMonth = ( (int) ( ( month - 1 ) / numberOfMonths ) + 1 ) * numberOfMonths;
			int lastDayOfRange = DateTime.DaysInMonth( year, endMonth );
			this.EndDate = new DateTime( year, endMonth , lastDayOfRange );

			int startMonth = endMonth - numberOfMonths + 1;
			this.StartDate = new DateTime( year, startMonth, 1 );
		}
		#endregion

		#region Private Properties
		#endregion
	
		#region Construction and Finalization
		private DateRange()
            : this( DateTime.Today, DateTime.Today )
		{
		}

        public DateRange( DateTime startDate, DateTime endDate )
        {
            this.StartDate = startDate;
            this.EndDate = endDate;                                                                                                       
        }

		private DateRange( DateTime dateTime, PeriodTypes periodType )
		{
			InitializeRangeFor( dateTime, periodType );
		}
		#endregion

		#region Data Elements
		private DateTime i_StartDate;
		private DateTime i_EndDate;
		#endregion
    }
}