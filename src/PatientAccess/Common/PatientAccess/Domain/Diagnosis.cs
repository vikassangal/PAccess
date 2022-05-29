using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Diagnosis : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Possible Condition types are Illness,Pregnancy,Accident and Crime. Default is Illness.
        /// </summary>
        public Condition Condition 
        {
            get
            {
                return i_Condition;
            }
            set
            {
                i_Condition = value;
            }
        }
        /// <summary>
        /// General description of the Complaint 
        /// </summary>
        public string ChiefComplaint 
        {
            get
            {
                return i_ChiefComplaint;
            }
            set
            {
                i_ChiefComplaint = value;
            }
        }
        /// <summary>
        /// General description of the Complaint 
        /// </summary>
        public string Procedure
        {
            get
            {
                return i_Procedure;
            }
            set
            {
                i_Procedure = value;
            }
        }

        /// <summary>
        /// PrivateAccommodationRequested
        /// </summary>
        public bool isPrivateAccommodationRequested
        {
            get
            {
                return i_PrivateAccommodationRequested;
            }
            set
            {
                i_PrivateAccommodationRequested = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Diagnosis()
        {
        }
        #endregion

        #region Data Elements
        private string i_ChiefComplaint = String.Empty;
        private string i_Procedure = String.Empty;
        private Condition i_Condition = new UnknownCondition();
        private bool i_PrivateAccommodationRequested;
        

        #endregion

        #region Constants
        #endregion
    }
}




