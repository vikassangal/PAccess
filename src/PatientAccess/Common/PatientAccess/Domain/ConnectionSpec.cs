using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ConnectionSpec
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format( TO_STRING_MSG,
                this.HospitalCode,
                this.HospitalName,
                this.DatabaseName,
                this.ServerIP
                );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        public string HospitalCode
        {
            private get
            {
                return i_HospitalCode;
            }
            set
            {
                i_HospitalCode = value;
            }
        }
        public string HospitalName
        {
            private get
            {
                return i_HospitalName;
            }
            set
            {
                i_HospitalName = value;
            }
        }
        public string DatabaseName
        {
            get
            {
                return i_DatabaseName;
            }
            set
            {
                i_DatabaseName = value;
            }
        }
        public string ServerIP
        {
            get
            {
                return i_ServerIP;
            }
            set
            {
                i_ServerIP = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return i_ConnectionString;
            }
            set
            {
                i_ConnectionString = value;
            }
        }

        #endregion

        #region Construction and Finalization
        public ConnectionSpec()
        {
        }
        #endregion

        #region Data Elements

        private string i_ServerIP           = string.Empty;
        private string i_DatabaseName       = string.Empty;
        private string i_HospitalName       = string.Empty;
        private string i_HospitalCode       = string.Empty;
        private string i_ConnectionString   = string.Empty;
        
        #endregion

        #region Constants

        private const string TO_STRING_MSG  = "Connection Spec For: {0}-{1}, Database: {2} on {3}";
        
#endregion
    }
}