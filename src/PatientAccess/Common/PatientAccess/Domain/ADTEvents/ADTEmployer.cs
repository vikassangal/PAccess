using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.ADTEvents
{
    /// <summary>
    /// Summary description for ADTEmployer
    /// </summary>
    [Serializable]
    public class ADTEmployer : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
     

       
        #endregion

        #region Properties
       
        public string EmployerName
        {
            get
            {
                return i_EmployerName;
            }
            set
            {
                i_EmployerName = value;
            }

        }
        public string EmployeeID
        {
            get
            {
                return i_EmployeeID;
            }
            set
            {
                i_EmployeeID = value;
            }

        }
        public string Occupation
        {
            get
            {
                return i_Occupation;
            }
            set
            {
                i_Occupation = value;
            }

        }

        
        public ContactPoint EmployerContactpoint
        {
            get
            {
                return i_EmployerContactpoint;
            }
            set
            {
                i_EmployerContactpoint = value;
            }
        }
        public int EmployerID
        {
            get
            {
                return i_EmployerID;
            }
            set
            {
                i_EmployerID = value;
            }
        }

        public string  EmploymentStatusCode
        {
            get
            {
                return i_EmploymentStatusCode;
            }
            set
            {
                i_EmploymentStatusCode = value;
            }
        }
    
        #endregion

        #region Private Methods
   

        #endregion

        #region Construction and Finalization
      
        public ADTEmployer() : base()
        {
        }
        #endregion

        #region Data Elements
        private string       i_EmployerName = string.Empty;
        private string       i_EmployeeID = string.Empty;
        private string       i_Occupation = string.Empty;
        private ContactPoint i_EmployerContactpoint   = new ContactPoint();
        private int          i_EmployerID  ;
        private string       i_EmploymentStatusCode = string.Empty;
     
        #endregion

        #region Constants
        #endregion
    }
}
