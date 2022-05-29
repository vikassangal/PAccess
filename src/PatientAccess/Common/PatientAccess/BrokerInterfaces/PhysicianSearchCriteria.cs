using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for PhysicianSearchCriteria
	/// This class is used for searching by Physician First Name,
	/// Last Name, Physician Number.
	/// </summary>
	[Serializable]
	public class PhysicianSearchCriteria : SearchCriteria
	{
        #region Event Handlers
        #endregion

        #region Methods

        public override ValidationResult Validate()
        {
            return null;
        }

        #endregion

        #region Properties
        public string FirstName 
        {
            get
            {
                return i_FirstName;
            }
            set
            {
                i_FirstName = value;
            }
        }
        public string LastName 
        {
            get
            {
                return i_LastName;
            }
            set
            {
                i_LastName = value;
            }
        }
        public long PhysicianNumber 
        {
            get
            {
                return i_PhysicianNumber;
            }
            set
            {
                i_PhysicianNumber = value;
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
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PhysicianSearchCriteria( Facility facility,
                                       string firstName, 
                                       string lastName, 
                                       long physicianNumber )
        : base( facility.Code )
        {
            this.i_Facility         = facility;
            this.i_FirstName        = firstName;
            this.i_LastName         = lastName;
            this.i_PhysicianNumber  = physicianNumber;
        }
        #endregion

        #region Data Elements
        private string          i_FirstName;
        private string          i_LastName;
        private long            i_PhysicianNumber;
        private Facility        i_Facility;
        #endregion

        #region Constants
        #endregion
	}
}
