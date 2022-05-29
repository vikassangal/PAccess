using System;

namespace PatientAccess.Domain.Parties
{
    //TODO: Create XML summary comment for PhysicianRelationship
    [Serializable]
    public class PhysicianRelationship : Object
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public PhysicianRole PhysicianRole
        {
            get
            {
                return i_PhysicianRole;
            }
            set
            {
                i_PhysicianRole = value;
            }
        }

        public Physician Physician
        {
            get
            {
                return i_Physician;
            }
            set
            {
                i_Physician = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PhysicianRelationship()
        {
        }

        public PhysicianRelationship( PhysicianRole aRole, Physician aPhysician )
        {
            PhysicianRole = aRole;
            Physician = aPhysician;
        }
        #endregion

        #region Data Elements
        private PhysicianRole i_PhysicianRole;
        private Physician i_Physician;
        #endregion

        #region Constants

        public const string
            REFERRING_PHYSICIAN = "REF",
            ADMITTING_PHYSICIAN = "ADM",
            ATTENDING_PHYSICIAN = "ATT",
            OPERATING_PHYSICIAN = "OPR",
            PRIMARYCARE_PHYSICIAN = "PCP";
        #endregion
    }
}
