using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain.ADTEvents
{
    /// <summary>
    /// Summary description for ADTGuarantor
    /// </summary>
    [Serializable]
    public class ADTGuarantor : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
     

       
        #endregion

        #region Properties
       
       
        public string MedicalRecordNumber
        {
            get
            {
                return i_MedicalRecordNumber;
            }
            set
            {
                i_MedicalRecordNumber = value;
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
     
        public int PersonID
        {
            get
            {
                return i_PersonID;
            }
            set
            {
                i_PersonID = value;
            }
        }
        public int VisitID
        {
            get
            {
                return i_VisitID;
            }
            set
            {
                i_VisitID = value;
            }
        }
        public int PatientID
        {
            get
            {
                return i_PatientID;
            }
            set
            {
                i_PatientID = value;
            }
        }
      
        public string DriversLicenseNumber
        {
            get
            {
                return i_DriversLicenseNumber.Substring( 0, 
                    i_DriversLicenseNumber.Length - 2 );
            }
            set
            {
                i_DriversLicenseNumber = value;
            }
        }
        public string DriversLicenseStateCode
        {
            get
            {
                return i_DriversLicenseNumber.Substring(
                    i_DriversLicenseNumber.Length - 2, 2 );
            }
            set
            {
                i_DriversLicenseStateCode = value;
            }
        }
     
        public string EmailAddress
        {
            get
            {
                return i_EmailAddress;
            }
            set
            {
                i_EmailAddress = value;
            }
        }
       
       
        public string GenderCode
        {
            get
            {
                return i_GenderCode;
            }
            set
            {
                i_GenderCode = value;
            }

        }
        public string SocialSecurityNumber
        {
            get
            {
                return i_SocialSecurityNumber;
            }
            set
            {
                i_SocialSecurityNumber = value;
            }

        }
        public string RelationShipCode
        {
            get
            {
                return i_RelationShipCode;
            }
            set
            {
                i_RelationShipCode = value;
            }

        }
        public string TypeOfRole
        {
            get
            {
                return i_TypeOfRole;
            }
            set
            {
                i_TypeOfRole = value;
            }

        }

        public void AddContactPoint( ContactPoint aContactPoint )
        {
            if( !this.ContactPoints.Contains( aContactPoint ) )
            {
                this.ContactPoints.Add( aContactPoint );
            }
        }

        private ArrayList ContactPoints
        {
            get
            {
                return i_ContactPoints;
            }
            set
            {
                i_ContactPoints = value;
            }
        }
        /// <summary>
        /// /////////////
        /// </summary>
        /// 
        public void AddName( Name aName )
        {
            if( !this.Names.Contains( aName ) )
            {
                this.Names.Add( aName );
            }
        }


        private ArrayList Names
        {
            get
            {
                return i_Names;
            }
            set
            {
                i_Names = value;
            }
        }
        public  int GuarantorNumber
        {
            get
            {
                return i_GuarantorNumber;
            }
            set
            {
                i_GuarantorNumber = value;
            }
        }
        #endregion

        #region Private Methods
   

        #endregion

        #region Construction and Finalization
      
        public ADTGuarantor() : base()
        {
        }
        #endregion

        #region Data Elements
        private         int  i_GuarantorNumber  ;
        private string    i_MedicalRecordNumber = string.Empty;
        private Facility    i_Facility = new Facility();
        private string    i_RelationShipCode = string.Empty;
        private string    i_TypeOfRole = string.Empty;
     
        private ArrayList i_Names    = new ArrayList();
        private ArrayList i_ContactPoints    = new ArrayList();
        private int      i_PersonID  ;
        private int      i_VisitID  ;
        private int      i_PatientID  ;
       
        private string   i_DriversLicenseNumber = string.Empty;
        private string   i_DriversLicenseStateCode = string.Empty;
     
        private string   i_EmailAddress = string.Empty;
     
        private string   i_GenderCode = string.Empty;
        private string   i_SocialSecurityNumber = string.Empty;     

     
        #endregion

        #region Constants
        #endregion
    }
}
