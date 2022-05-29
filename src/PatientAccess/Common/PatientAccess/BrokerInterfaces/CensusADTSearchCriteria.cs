using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for CensusADTSearchCriteria.
    /// Contains ADT Census Search Criteria Parameters.
    /// </summary>
    [Serializable]
    public class CensusADTSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string StartTime 
        {
            get
            {
                return i_StartTime;
            }
            set
            {
                i_StartTime = value;
            }
        }

        public string ADTActivity 
        {
            get
            {
                return i_ADTActivity;
            }
            set
            {
                i_ADTActivity = value;
            }
        }

        public string NursingStations 
        {
            get
            {
                return i_NursingStations;
            }
            set
            {
                i_NursingStations = value;
            }
        }

        public Facility Facility
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

        public string ADTSortColumn
        {
            get
            {
                return i_ADTSortColumn;
            }
            set
            {
                i_ADTSortColumn = value;
            }
        }

        #endregion

        #region Private Methods

        public override ValidationResult Validate()
        {
            return null;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CensusADTSearchCriteria( string startTime, 
                                        string adtActivity, 
                                        string nursingStations,
                                        string adtSortColumn,
                                        Facility facility )
        : base(facility.Code)
        {
            i_ADTActivity              = adtActivity;
            i_StartTime                = startTime;
            i_NursingStations          = nursingStations;
            i_ADTSortColumn            = adtSortColumn; 
            i_Facility                 = facility;
        }

        #endregion

        #region Data Elements
        private string                  i_ADTActivity;
        private string                  i_StartTime;
        private string                  i_NursingStations;
        private string                  i_ADTSortColumn;
        private Facility                i_Facility;
        #endregion

        #region Constants
        #endregion
    }
}