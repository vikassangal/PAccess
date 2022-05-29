using System;
using System.Collections.Generic;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientSearchResponse.
    /// </summary>
    [Serializable]
    public class PatientSearchResponse
    {
        #region Event Handlers
        #endregion

        #region Methods        

        public List<PatientSearchResult> GetResultsOfType( TypeOfName resultType )
        {

            List<PatientSearchResult> results = 
                PatientSearchResults.FindAll( delegate( PatientSearchResult target )
                                                { return target.Name.TypeOfName == resultType; } );

            return results;

        }
        
        #endregion

        #region Properties

        public PatientSearchResultStatus PatientSearchResultStatus
        {
            get
            {
                return i_PatientSearchResultStatus;
            }
            set
            {
                i_PatientSearchResultStatus = value;
            }
        }

        public List<PatientSearchResult> PatientSearchResults
        {
            get
            {
                return i_PatientSearchResults;
            }
            set
            {
                i_PatientSearchResults = value;
            }
        }

        public string Message
        {
            get
            {
                return i_Message;
            }
            private set
            {
                i_Message = value;
            }
        }

        public EMPIResultStatus EMPIResultStatus { get; set; }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public bool HasEMPIResults { get; set; }

        public PatientSearchResponse()
        {
            HasEMPIResults = false;
        }

        private PatientSearchResponse( PatientSearchResultStatus searchStatus )
        {
            HasEMPIResults = false;
            this.PatientSearchResultStatus = searchStatus;
        }

        public PatientSearchResponse( List<PatientSearchResult> patientSearchResults, PatientSearchResultStatus searchStatus )
            : this( searchStatus )
        {
            this.PatientSearchResults = patientSearchResults;
        }

        public PatientSearchResponse( List<PatientSearchResult> patientSearchResults, PatientSearchResultStatus searchStatus, string message )
            : this( patientSearchResults, searchStatus )
        {
            this.Message = message;
        }

        #endregion

        #region Data Elements

        private List<PatientSearchResult>   i_PatientSearchResults = new List<PatientSearchResult>();
        private string                      i_Message = String.Empty;
        private PatientSearchResultStatus   i_PatientSearchResultStatus = PatientSearchResultStatus.Unknown;
        
        #endregion

        #region Constants
        #endregion
    }
}
