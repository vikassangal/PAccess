using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Employment : PersistentModel, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        
        public override object DeepCopy()
        {
            MemoryStream stream = null;
            BinaryFormatter formatter = null;
            object deserialized = null;

            try
            {
                stream = new MemoryStream();
                formatter = new BinaryFormatter();
                formatter.Serialize( stream, this );
                stream.Position = 0;
                deserialized = formatter.Deserialize( stream );
            }
            finally
            {
                if( null != stream )
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            
            return deserialized;
        }

        public override object Clone()
        {
            Employment newObject = new Employment();
            newObject.Occupation = (string) this.Occupation.Clone();
            newObject.EmployeeID = (string) this.EmployeeID.Clone();
            newObject.RetiredDate = this.RetiredDate;

            if( this.Employer != null )
            {
                // TLG 06/04/2006
                // don't know why this is not the only line of code needed... but, refactored to
                // keep from throwing an exception

                newObject.Employer = (Employer) this.Employer.Clone();

                if( this.Employer.PartyContactPoint != null )
                {
                    newObject.Employer.PartyContactPoint = (ContactPoint) this.Employer.PartyContactPoint.Clone();
                    
                    if( this.Employer.PartyContactPoint.Address != null )
                    {
                        newObject.Employer.PartyContactPoint.Address = (Address) this.Employer.PartyContactPoint.Address.Clone();
                    }
                }
            }

            if( this.Employee != null )
            {
                newObject.Employee = (Person) this.Employee.Clone();
            }

            if( this.PhoneNumber != null )
            {
                newObject.PhoneNumber = (PhoneNumber) this.PhoneNumber.Clone();
            }
            if( this.Status != null )
            {
                newObject.Status.Oid            = this.Status.Oid;
                newObject.Status.Code           = this.Status.Code;
                newObject.Status.Description    = this.Status.Description;
            }

            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            Employment emp = obj as Employment;

            bool address = (emp.Employer == null && this.Employer == null ? true :
                emp.Employer == null || this.Employer == null ? false :
                emp.Employer.PartyContactPoint == null && this.Employer.PartyContactPoint == null ? true :
                emp.Employer.PartyContactPoint == null || this.Employer.PartyContactPoint == null ? false :
                emp.Employer.PartyContactPoint.Address == null && this.Employer.PartyContactPoint.Address == null ? true : 
                emp.Employer.PartyContactPoint.Address == null || this.Employer.PartyContactPoint.Address == null ? false :
                emp.Employer.PartyContactPoint.Address == this.Employer.PartyContactPoint.Address );

            bool phone = (emp.PhoneNumber == null && this.PhoneNumber == null ? true : 
                emp.PhoneNumber == null || this.PhoneNumber == null ? false :
                emp.PhoneNumber == this.PhoneNumber );

            bool status = (emp.Status == null && this.Status == null ? true : 
                emp.Status == null || this.Status == null ? false :
                emp.Status.Oid == this.Status.Oid);

            bool employer = (emp.Employer == null && this.Employer == null ? true : 
                emp.Employer == null || this.Employer == null ? false :
                emp.Employer.Equals( this.Employer ));

            bool contact = (emp.Employer == null && this.Employer == null ? true :
                emp.Employer == null || this.Employer == null ? false :
                emp.Employer.PartyContactPoint == null && this.Employer.PartyContactPoint == null ? true : 
                emp.Employer.PartyContactPoint == null || this.Employer.PartyContactPoint == null ? false :
                emp.Employer.PartyContactPoint == this.Employer.PartyContactPoint );

            int comparison = DateTime.Compare( emp.RetiredDate, this.RetiredDate );
            bool date = (comparison == 0);

            bool result = (address && phone && status && employer && contact &&
                           emp.Occupation == this.Occupation &&
                           emp.EmployeeID == this.EmployeeID &&
                           date);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties


        public Person Employee 
        {
            get
            {
                return i_Employee;
            }
            private set
            {
                i_Employee = value;
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

        public PhoneNumber PhoneNumber 
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }

        public EmploymentStatus Status 
        {
            get
            {
                return i_Status;
            }
            set
            {
                i_Status = value;
            }
        }

//        public IEmployer Employer 
        public Employer Employer
            {
            get
            {
                return i_Employer;
            }
            set
            {
                i_Employer = value;
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

        public DateTime RetiredDate
        {
            get
            {
                return i_RetiredDate;
            }
            set
            {
                i_RetiredDate = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Employment()
        {
        }

        public Employment( Person employee )
        {
            this.Employee = employee;
        }
        #endregion

        #region Data Elements

        private Employer            i_Employer      = new Employer();
        private Person              i_Employee;
        private string              i_EmployeeID    = String.Empty;
        private PhoneNumber         i_PhoneNumber   = new PhoneNumber();
        private string              i_Occupation    = String.Empty;
        private EmploymentStatus    i_Status        = new EmploymentStatus();
        private ContactPoint        i_ContactPoint  =  new ContactPoint();
        private DateTime            i_RetiredDate ;
        #endregion

        #region Constants
        #endregion
    }
}