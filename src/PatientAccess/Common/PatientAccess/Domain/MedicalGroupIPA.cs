using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class MedicalGroupIPA  : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void AddClinic( Clinic aClinic )
        {
            this.i_Clinics.Add( aClinic );
        }

        #endregion

        #region Properties

        public ICollection Clinics 
        {
            get
            {
                return (ICollection)this.PrimClinics.Clone();
            }
        }
        public string Code
        {
            get
            {
                return i_code;
            }
            set
            {
                i_code = value;
            }
        }
        public string Name
        {
            get
            {
                return i_Name;
            }
            set
            {
                i_Name = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        private ArrayList PrimClinics
        {
            get
            {
                return this.i_Clinics;
            }
            set
            {
                this.i_Clinics = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public MedicalGroupIPA()
        {
        }
        #endregion

        #region Data Elements
        private string i_code = String.Empty ;
        private string i_Name = String.Empty ;
        private ArrayList i_Clinics    = new ArrayList();
        
        #endregion

        #region Constants
        #endregion
    }
}

