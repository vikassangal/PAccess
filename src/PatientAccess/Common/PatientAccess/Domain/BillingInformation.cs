using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class BillingInformation :ContactPoint
    {
        #region Event Handlers
        #endregion

        #region Methods
         
        public override object Clone()
        {
            BillingInformation bi   = new BillingInformation();

            bi.BillingCOName        = this.BillingCOName;
            bi.BillingName          = this.BillingName;
            bi.CellPhoneNumber      = (PhoneNumber)this.CellPhoneNumber.Clone();
            bi.EmailAddress         = (EmailAddress)this.EmailAddress.Clone();
            bi.PhoneNumber          = (PhoneNumber)this.PhoneNumber.Clone();
            bi.Address              = (Address)this.Address.Clone();
            bi.Area                 = this.Area;
            bi.TypeOfContactPoint   = (TypeOfContactPoint)this.TypeOfContactPoint.Clone();  

            return bi;
        }

        #endregion

        #region Properties
        public string BillingCOName
        {
            get
            {
                return i_BillingCOName;
            }
            set
            {
                i_BillingCOName = value;
            }
        }
        public string BillingName
        {
            get
            {
                return i_BillingName;
            }
            set
            {
                i_BillingName = value;
            }
        }
        public string Area
        {
            get
            {
                return i_Area;
            }
            set
            {
                i_Area = value;
            }
        }

       

       

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BillingInformation()
        {
        }
        public BillingInformation( Address address, PhoneNumber phoneNumber, EmailAddress emailAddress, TypeOfContactPoint typeOfContactPoint )           
            :base(   address,  phoneNumber,  emailAddress,  typeOfContactPoint )  
        {
           
        }
           
        #endregion

        #region Data Elements
        private string i_BillingCOName  = String.Empty;
        private string i_BillingName  = String.Empty;
        private string i_Area  = String.Empty;

        
        #endregion

        #region Constants
        #endregion
    }
}

