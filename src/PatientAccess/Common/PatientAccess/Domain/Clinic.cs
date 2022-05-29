using System;

//using Extensions.ClassLibrary.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Clinic  : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }
        #endregion

        #region Properties

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
        #endregion

        #region Construction and Finalization
        public Clinic()
        {
        }

        public Clinic( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
//        private string i_code = String.Empty ;
        private string i_Name = String.Empty ;
      
        #endregion

        #region Constants
        #endregion
    }
}

