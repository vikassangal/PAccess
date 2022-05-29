using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for PhysicianPatientsSearchCriteria.
	/// </summary>
	[Serializable]
	public class PhysicianPatientsSearchCriteria : SearchCriteria
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
        public int Admitting 
        {
            get
            {
                return i_Admitting;
            }
            set
            {
                i_Admitting = value;
            }
        }
        public int Attending 
        {
            get
            {
                return i_Attending;
            }
            set
            {
                i_Attending = value;
            }
        }
        public int Referring 
        {
            get
            {
                return i_Referring;
            }
            set
            {
                i_Referring = value;
            }
        }
        public int Consulting 
        {
            get
            {
                return i_Consulting;
            }
            set
            {
                i_Consulting = value;
            }
        }
        public int Operating 
        {
            get
            {
                return i_Operating;
            }
            set
            {
                i_Operating = value;
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
        public PhysicianPatientsSearchCriteria( Facility facility,
                                                long physicianNumber,
                                                int admitting,
                                                int attending,
                                                int referring,
                                                int consulting,
                                                int operating )
        : base( facility.Code )
        {
            this.i_Facility         = facility;
            this.i_PhysicianNumber  = physicianNumber;
            this.i_Admitting        = admitting;
            this.i_Attending        = attending;
            this.i_Referring        = referring;
            this.i_Consulting       = consulting;
            this.i_Operating        = operating;
        }

        #endregion

        #region Data Elements
        private long            i_PhysicianNumber;
        private int             i_Admitting;
        private int             i_Attending;
        private int             i_Referring;
        private int             i_Consulting;
        private int             i_Operating;
        private Facility        i_Facility;
        #endregion

        #region Constants
        #endregion
	}
}
