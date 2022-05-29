using System;

namespace Peradigm.Framework.Domain.Time
{
	//TODO: Create XML summary comment for ChangingUTCOffsetRule
	[Serializable]
	internal class DaylightChangeRule : object
	{
		#region Event Handlers
		#endregion

		#region Methods
		public DateTime DateFor( int year )
		{
			DateTime firstOfMonth = new DateTime( year, this.Month, 1, this.Hour, this.Minute, this.Second );
			int weekNumber = ( this.DayOfWeek < (int)firstOfMonth.DayOfWeek ) ? 
							   this.WeekNumber : this.WeekNumber - 1;

			DateTime startTime = firstOfMonth.AddDays( 7 * weekNumber - (int)firstOfMonth.DayOfWeek + this.DayOfWeek );
			
			if( startTime.Month > this.Month )
			{
				startTime.AddDays( - startTime.Day );
			}

			return startTime;
		}
		#endregion

		#region Properties

	    private int Month
		{
			get
			{
				return i_Month;
			}
			set
			{
				i_Month = value;
			}
		}

	    private int WeekNumber
		{
			get
			{
				return i_WeekNumber;
			}
			set
			{
				i_WeekNumber = value;
			}
		}

	    private int DayOfWeek
		{
			get
			{
				return i_DayOfWeek;
			}
			set
			{
				i_DayOfWeek = value;
			}
		}

	    private int Hour
		{
			get
			{
				return i_Hour;
			}
			set
			{
				i_Hour = value;
			}
		}

	    private int Minute
		{
			get
			{
				return i_Minute;
			}
			set
			{
				i_Minute = value;
			}
		}

	    private int Second
		{
			get
			{
				return i_Second;
			}
			set
			{
				i_Second = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public DaylightChangeRule()
		{
		}

		public DaylightChangeRule( int month, int weekNumber, int dayOfWeek, int hour, int minute, int second )
		{
			this.Month		= month;
			this.WeekNumber	= weekNumber;
			this.DayOfWeek	= dayOfWeek;
			this.Hour		= hour;
			this.Minute		= minute;
			this.Second		= second;
		}
		#endregion

		#region Data Elements
		private int i_Month;
		private int i_WeekNumber;
		private int i_DayOfWeek;
		private int i_Hour;
		private int i_Minute;
		private int i_Second;
		#endregion

		#region Constants
		#endregion
	}
}
