using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class OccurrenceSpan : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public SpanCode SpanCode
        {
            get
            {
                return i_SpanCode;
            }
            set
            {
                i_SpanCode = value;
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
        public string Facility
        {
            get
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }

        public bool IsSystemGenerated { get; set; }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OccurrenceSpan()
        {
        }
        #endregion

        #region Data Elements
        private DateTime  i_FromDate;
        private DateTime  i_ToDate;
        private SpanCode i_SpanCode = new SpanCode();
        private string i_Facility;

        #endregion

        #region Constants
        #endregion
    }
}
