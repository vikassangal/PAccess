using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class WorklistSettings : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods

        public DateRange GetDateRange()
        {
            var dateRange = new DateRange();

            if ( WorklistSelectionRange != null )
            {
                switch ( WorklistSelectionRange.Oid )
                {
                    case WorklistSelectionRange.ALL:

                        dateRange.FromDate = DateTime.MinValue;
                        dateRange.ToDate = DateTime.MaxValue;
                        break;

                    case WorklistSelectionRange.TOMORROW:

                        dateRange.FromDate = DateTime.Today + TimeSpan.FromDays( 1 );
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 1 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.TODAY:

                        dateRange.FromDate = DateTime.Today;
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 1 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.NEXT_3_DAYS:

                        dateRange.FromDate = DateTime.Today;
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 4 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.NEXT_10_DAYS:

                        dateRange.FromDate = DateTime.Today;
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 11 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.YESTERDAY:

                        dateRange.FromDate = DateTime.Today - TimeSpan.FromDays( 1 );
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 1 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.LAST_3_DAYS:

                        dateRange.FromDate = DateTime.Today - TimeSpan.FromDays( 3 );
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 4 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.LAST_10_DAYS:

                        dateRange.FromDate = DateTime.Today - TimeSpan.FromDays( 10 );
                        dateRange.ToDate = dateRange.FromDate + TimeSpan.FromDays( 11 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    case WorklistSelectionRange.DATE_RANGE:

                        //take the date part only so that we can start from the beginning of the day
                        dateRange.FromDate = FromDate.Date;

                        //include the entire day for the to date
                        dateRange.ToDate = ToDate.Date + TimeSpan.FromDays( 1 ) - TimeSpan.FromMilliseconds( 1 );
                        break;

                    default:

                        dateRange.FromDate = DateTime.MinValue;
                        dateRange.ToDate = DateTime.MaxValue;
                        break;
                }
            }

            else
            {
                dateRange.FromDate = DateTime.MinValue;
                dateRange.ToDate = DateTime.MaxValue;
            }

            return dateRange;
        }

        #endregion

        #region Properties
        public string BeginningWithLetter
        {
            get
            {
                return i_BeginningWithLetter;
            }
            set
            {
                i_BeginningWithLetter = value;
            }
        }

        public string EndingWithLetter
        {
            get
            {
                return i_EndingWithLetter;
            }
            set
            {
                i_EndingWithLetter = value;
            }
        }

        public DateTime FromDate
        {
            get
            {
                return i_FromDate;
            }
            set
            {
                i_FromDate = value;
            }
        }

        public long SortedColumn
        {
            get
            {
                return i_SortedColumn;
            }
            set
            {
                i_SortedColumn = value;
            }
        }

        public long SortedColumnDirection
        {
            get
            {
                return i_SortedColumnDirection;
            }
            set
            {
                i_SortedColumnDirection = value;
            }
        }
        public string UserID
        {
            get
            {
                return i_UserID;
            }
            set
            {
                i_UserID = value;
            }
        }
        public DateTime ToDate
        {
            get
            {
                return i_ToDate;
            }
            set
            {
                i_ToDate = value;
            }
        }
        public long WorkListID
        {
            get
            {
                return i_WorkListID;
            }
            set
            {
                i_WorkListID = value;
            }
        }
        public WorklistSelectionRange WorklistSelectionRange
        {
            get
            {
                return i_Range;
            }
            set
            {
                i_Range = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public WorklistSettings()
        {
            i_Range = new WorklistSelectionRange();
        }

        public WorklistSettings( string startLetters, string endLetters,
            DateTime startDate, DateTime endDate,
            WorklistSelectionRange range, long sortedColumn, long sortedColumnDirection )
        {
            this.i_BeginningWithLetter = startLetters;
            this.i_EndingWithLetter = endLetters;
            this.i_FromDate = startDate;
            this.i_ToDate = endDate;
            this.i_Range = range;
            this.i_SortedColumn = sortedColumn;
            this.i_SortedColumnDirection = sortedColumnDirection;
        }
        public WorklistSettings( string startLetters, string endLetters,
            DateTime startDate, DateTime endDate,
            WorklistSelectionRange range, long sortedColumn, long sortedColumnDirection,
            long worklistID )
        {
            this.i_BeginningWithLetter = startLetters;
            this.i_EndingWithLetter = endLetters;
            this.i_FromDate = startDate;
            this.i_ToDate = endDate;
            this.i_Range = range;
            this.i_SortedColumn = sortedColumn;
            this.i_SortedColumnDirection = sortedColumnDirection;
            this.i_WorkListID = worklistID;
        }
        #endregion

        #region Data Elements
        private DateTime i_FromDate;
        private DateTime i_ToDate;
        private string i_BeginningWithLetter = String.Empty;
        private string i_EndingWithLetter = String.Empty;
        private long i_SortedColumn = 1;
        private long i_SortedColumnDirection = 1;
        private string i_UserID = String.Empty;
        private long i_WorkListID = 0L;
        private WorklistSelectionRange i_Range;
        #endregion

        #region Constants
        public static long SORTED_COLUMN_DIRECTION_ASC = 1,
                           SORTED_COLUMN_DIRECTION_DESC = 2;
        #endregion
    }
}
