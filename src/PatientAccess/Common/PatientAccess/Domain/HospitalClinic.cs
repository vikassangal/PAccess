using System;

namespace PatientAccess.Domain
{
	//TODO: Create XML summary comment for HospitalClinic
    [Serializable]
    public class HospitalClinic : CodedReferenceValue
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

        public string PreAdmitTest
        {
            get
            {
                return i_PreAdmitTest;
            }
            private set
            {
                i_PreAdmitTest = value;
            }
        }


        public string SiteCode
        {
            get { return i_SiteCode; }
            set { i_SiteCode = value; }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public HospitalClinic()
        {
        }
        public HospitalClinic( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public HospitalClinic( long oid, DateTime version, string description, 
            string code, string preAdmitTest, string siteCode )
            : base( oid, version, description, code )
        {
            this.PreAdmitTest = preAdmitTest;
            this.SiteCode = siteCode;

        }
        public HospitalClinic( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        private string i_Name = String.Empty ;
        private string i_PreAdmitTest = string.Empty;
        private string i_SiteCode = string.Empty;


        #endregion

        #region Constants
        #endregion
    }
}
