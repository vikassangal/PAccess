using System;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for ExtendedFUSNote
    [Serializable]
    public class ExtendedFUSNote : FusNote, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public int CompareTo( object obj )
        {
            if( obj is ExtendedFUSNote )
            {
                ExtendedFUSNote fusNote = (ExtendedFUSNote)obj;

                int compareResult =  -1 * this.CreatedOn.CompareTo( fusNote.CreatedOn );

                if( compareResult == 0 
                    && this.FusActivity != null 
                    && fusNote.FusActivity != null )
                {
                    compareResult = -1 * this.FusActivity.Code.CompareTo( fusNote.FusActivity.Code );
                }

                return compareResult;
            }

            throw new ArgumentException( "Object is not an ExtendedFUSNote" );
        }

        #endregion

        #region Properties

        public DateTime Date1
        {
            get
            {
                return i_Date1;
            }
            set
            {
                i_Date1 = value;
            }
        }
        public DateTime Date2
        {
            get
            {
                return i_Date2;
            }
            set
            {
                i_Date2 = value;
            }
        }
        public decimal Dollar1
        {
            get
            {
                return i_Dollar1;
            }
            set
            {
                i_Dollar1 = value;
            }
        }
        public decimal Dollar2
        {
            get
            {
                return i_Dollar2;
            }
            set
            {
                i_Dollar2 = value;
            }
        }
        public string Month
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

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ExtendedFUSNote()
        {
        }
        public ExtendedFUSNote( FusActivity fusActivity, 
                                DateTime entryDateTime,
                                string keyedBy,
                                string remarks,
                                string text,
                                DateTime worklistDate,
                                decimal dollar1, 
                                decimal dollar2, 
                                DateTime date1, 
                                DateTime date2, 
                                string month )
        {
            this.FusActivity    = fusActivity;
            this.CreatedOn      = entryDateTime;
            this.UserID         = keyedBy;
            this.Remarks        = remarks;
            this.Text           = text;
            this.WorklistDate   = worklistDate;
            this.Dollar1        = dollar1;
            this.Dollar2        = dollar2;
            this.Date1          = date1;
            this.Date2          = date2;
            this.Month          = month;
        }

        #endregion

        #region Data Elements

        private DateTime    i_Date1 = DateTime.MinValue;
        private DateTime    i_Date2 = DateTime.MinValue;
        private decimal     i_Dollar1 = 0;
        private decimal     i_Dollar2 = 0;
        private string      i_Month;
       
        #endregion

        #region Constants
        #endregion
    }
}
