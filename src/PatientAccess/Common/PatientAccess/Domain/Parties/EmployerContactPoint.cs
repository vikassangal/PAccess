using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class EmployerContactPoint :ContactPoint
    {
        #region Event Handlers
        #endregion

        #region Methods
        
        #endregion

        #region Properties
        public long EmployerAddressNumber
        {
            get
            {
                return i_EmployerAddressNumber;
            }
            set
            {
                i_EmployerAddressNumber = value;
            }
        }
     
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmployerContactPoint()
        {
        }
        public EmployerContactPoint( Address address, PhoneNumber phoneNumber, EmailAddress emailAddress, TypeOfContactPoint typeOfContactPoint )           
            :base(   address,  phoneNumber,  emailAddress,  typeOfContactPoint )  
        {
           
        }
           
        #endregion

        #region Data Elements
        private long i_EmployerAddressNumber  ;
      
        #endregion

        #region Constants
        #endregion
    }
}

