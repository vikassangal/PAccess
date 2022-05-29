using System;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for Patient.
    /// </summary>
    [Serializable]
    public class PatientContextHeaderData
    {
        #region Event Handlers
        #endregion

        #region Methods

        #endregion

        #region Properties

        public Name PatientName { get; set; }

        public SocialSecurityNumber SSN { get; set; }

        public DateTime DOB { get; set; }

        public Gender Sex { get; set; }

        public long MRN { get; set; }

        public string AKA { get; set; }


        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }

}
