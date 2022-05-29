using System;
using System.Collections;
using System.Globalization;
using Peradigm.Framework.Domain.Time.Exceptions;

namespace Peradigm.Framework.Domain.Time
{
//TODO: Create XML summary comment for ChangingUTCOffSetStrategy
    [Serializable]
    public class ChangingUTCOffSetStrategy : DaylightStrategy
    {
        #region Constants
        private const int
            DAYLIGHT_SAVINGS_OFFSET = 1;
        #endregion

        #region Methods
        public override string TimeZoneDescriptionFor( DateTime time, USTimeZone timeZone )
        {
            string description = timeZone.StandardDescription;

            if( USTimeZone.IsDaylightSavingTime( time, this.DaylightChangesFor( time.Year ) ) )
            {
                description = timeZone.DaylightDescription;
            }

            return description;
        }

        public override string TimeZoneNameFor( DateTime time, USTimeZone timeZone )
        {
            string name = timeZone.StandardName;

            if( USTimeZone.IsDaylightSavingTime( time, this.DaylightChangesFor( time.Year ) ) )
            {
                name = timeZone.DaylightName;
            }

            return name;
        }

        public override TimeSpan UTCOffsetFor( DateTime time, USTimeZone timeZone )
        {
            TimeSpan offSet = timeZone.StandardUTCOffset;

            if( USTimeZone.IsDaylightSavingTime( time, this.DaylightChangesFor( time.Year ) ) )
            {
                offSet = offSet.Add( new TimeSpan( DAYLIGHT_SAVINGS_OFFSET, 0, 0 ) );
            }

            return offSet;
        }

        public override DaylightTime DaylightChangesFor( int year )
        {
            DateTime daylightStart = this.DaylightStartTimeFor( year );
            DateTime daylightEnd = this.StandardStartTimeFor( year );
            return new DaylightTime( daylightStart, daylightEnd, -TimeSpan.FromHours( 1 ) );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private DateTime DaylightStartTimeFor( int year )
        {            
			DaylightChangeRule daylightStart = this.DaylightStartRuleFor( year );
			return daylightStart.DateFor( year );
        }

        private DateTime StandardStartTimeFor( int year )
        {           
			DaylightChangeRule daylightEnd = this.DaylightEndRuleFor( year );
			return daylightEnd.DateFor( year );
		}       

		private DaylightChangeRule DaylightStartRuleFor( int year )
		{
			foreach( DateRange dateRange in DaylightStartRules.Keys )
			{
				if( dateRange.StartDate.Year <= year && year <= dateRange.EndDate.Year )
				{
					return (DaylightChangeRule)DaylightStartRules[dateRange];
				}
			}
			throw new DaylightChangeRuleNotFoundException( year );
		}

		private DaylightChangeRule DaylightEndRuleFor( int year )
		{
			foreach( DateRange dateRange in DaylightEndRules.Keys )
			{
				if( dateRange.StartDate.Year <= year && year <= dateRange.EndDate.Year )
				{
					return (DaylightChangeRule)DaylightEndRules[dateRange];
				}
			}
			throw new DaylightChangeRuleNotFoundException( year );
		}
        #endregion

        #region Private Properties
		private static Hashtable DaylightStartRules
		{
			get
			{
				return s_DaylightStartRules;
			}
			set
			{
				s_DaylightStartRules = value;
			}
		}

		private static Hashtable DaylightEndRules
		{
			get
			{
				return s_DaylightEndRules;
			}
			set
			{
				s_DaylightEndRules = value;
			}
		}		
		#endregion

        #region Construction and Finalization
		public ChangingUTCOffSetStrategy()
		{
		}

        static ChangingUTCOffSetStrategy()
        {
			DaylightStartRules = new Hashtable();

			DaylightChangeRule firstSundayOfApril = new DaylightChangeRule( 4, 1, 0, 2, 0, 0 );
			DaylightChangeRule secondSundayOfMarch = new DaylightChangeRule( 3, 2, 0, 2, 0, 0 );

			DaylightStartRules.Add( new DateRange( DateTime.MinValue, new DateTime( 2006, 12, 31 ) ), 
								    firstSundayOfApril );
			DaylightStartRules.Add( new DateRange( new DateTime( 2007, 1, 1 ), DateTime.MaxValue ), 
									secondSundayOfMarch );

			DaylightEndRules = new Hashtable();

			DaylightChangeRule lastSundayOfOctober = new DaylightChangeRule( 10, 5, 0, 2, 0, 0 );
			DaylightChangeRule firstSundayOfNovember = new DaylightChangeRule( 11, 1, 0, 2, 0, 0 );

			DaylightEndRules.Add( new DateRange( DateTime.MinValue, new DateTime( 2006, 12, 31 ) ),
								  lastSundayOfOctober ); 
			DaylightEndRules.Add( new DateRange( new DateTime( 2007, 1, 1 ), DateTime.MaxValue ), 
								  firstSundayOfNovember );
		}
        #endregion

        #region Data Elements
		private static Hashtable s_DaylightStartRules;
		private static Hashtable s_DaylightEndRules;
		#endregion    
    }
}