using System;
using System.Diagnostics;
using System.Reflection;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ContactPoint : PersistentModel, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            ContactPoint cp = obj as ContactPoint;

            bool e = cp.EmailAddress == null && this.EmailAddress == null ? true :
                cp.EmailAddress == null && this.EmailAddress != null ? false :
                cp.EmailAddress != null && this.EmailAddress == null ? false :
                cp.EmailAddress.Equals( this.EmailAddress );

            bool p = cp.PhoneNumber == null && this.PhoneNumber == null ? true :
                cp.PhoneNumber == null && this.PhoneNumber != null ? false :
                cp.PhoneNumber != null && this.PhoneNumber == null ? false :
                cp.PhoneNumber.Equals( this.PhoneNumber );

            bool cellp = cp.CellPhoneNumber == null && this.CellPhoneNumber == null ? true :
                cp.CellPhoneNumber == null && this.CellPhoneNumber != null ? false :
                cp.CellPhoneNumber != null && this.CellPhoneNumber == null ? false :
                cp.CellPhoneNumber.Equals( this.CellPhoneNumber );

            bool adr = cp.Address == null && this.Address == null ? true :
                cp.Address == null && this.Address != null ? false :
                cp.Address != null && this.Address == null ? false :
                cp.Address.Equals( this.Address );

            bool conp = cp.TypeOfContactPoint == null && this.TypeOfContactPoint == null ? true :
                cp.TypeOfContactPoint == null && this.TypeOfContactPoint != null ? false :
                cp.TypeOfContactPoint != null && this.TypeOfContactPoint == null ? false :
                cp.TypeOfContactPoint.Equals( this.TypeOfContactPoint );

            return
                adr &&
                e &&
                p &&
                cellp &&
                conp;
        }

        public override object Clone()
        {

            ContactPoint newObject = new ContactPoint();
            newObject.EmailAddress = this.EmailAddress;

            if( this.Address != null )
            {
                newObject.Address = (Address)this.Address.Clone();
            }

            if( this.PhoneNumber != null )
            {
                newObject.PhoneNumber = (PhoneNumber)this.PhoneNumber.Clone();
            }

            if( this.CellPhoneNumber != null )
            {
                newObject.CellPhoneNumber = (PhoneNumber)this.CellPhoneNumber.Clone();
            }

            newObject.TypeOfContactPoint = new TypeOfContactPoint( this.TypeOfContactPoint.Oid,
                                                                   this.TypeOfContactPoint.Description );
            return newObject;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsTrivial()
        {
            bool trivial = true;

            if( this.Address != null && this.Address.OneLineAddressLabel() != string.Empty )
                trivial = false;

            if( this.PhoneNumber != null && this.PhoneNumber.AsUnformattedString() != string.Empty )
                trivial = false;

            if( this.CellPhoneNumber != null && this.CellPhoneNumber.AsFormattedString() != string.Empty )
                trivial = false;

            if( this.EmailAddress != null && this.EmailAddress.ToString().Trim() != string.Empty )
                trivial = false;

            return trivial;
        }
        #endregion

        #region Properties
        public Address Address
        {
            get
            {
                return i_Address;
            }
            set
            {
                this.SetAndTrack<Address>( ref this.i_Address, value, MethodBase.GetCurrentMethod() );
            }
        }

        public EmailAddress EmailAddress
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

        public PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                this.SetAndTrack<PhoneNumber>( ref this.i_PhoneNumber, value, MethodBase.GetCurrentMethod() );
            }
        }

        public PhoneNumber CellPhoneNumber
        {
            get
            {
                return i_CellPhoneNumber;
            }
            set
            {
                i_CellPhoneNumber = value;
            }
        }

        public TypeOfContactPoint TypeOfContactPoint
        {
            get
            {
                return i_TypeOfContactPoint;
            }
            set
            {
                i_TypeOfContactPoint = value;
            }
        }

        public CellPhoneConsent CellPhoneConsent
        {
            get { return i_CellPhoneConsent; }
            set
            {
                Debug.Assert(value != null);
                SetAndTrack<CellPhoneConsent>(ref i_CellPhoneConsent, value, MethodBase.GetCurrentMethod());
            }
        }

        public EmailReason EmailReason
        {
            get { return i_EmailReason; }
            set
            {
                i_EmailReason = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ContactPoint()
        {
        }
        public ContactPoint( TypeOfContactPoint typeOfContactPoint )
        {
            if (typeOfContactPoint != null)
            {
                TypeOfContactPoint = typeOfContactPoint;
            }
            else
            {
                TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();
            }
        }

        public ContactPoint( Address address, PhoneNumber phoneNumber, EmailAddress emailAddress, TypeOfContactPoint typeOfContactPoint )
            : this( typeOfContactPoint )
        {
            if (address != null)
            {
                Address = address;
            }
            if( phoneNumber != null )
            {
                PhoneNumber = phoneNumber;
            }
            if (emailAddress != null)
            {
                EmailAddress = emailAddress;
            }
        }

        public ContactPoint( Address address, PhoneNumber phoneNumber, PhoneNumber cellPhoneNumber, EmailAddress emailAddress, TypeOfContactPoint typeOfContactPoint )
            : this( typeOfContactPoint )
        {
            if (address != null)
            {
                Address = address;
            }
            if (phoneNumber != null)
            {
                PhoneNumber = phoneNumber;
            }
            if (cellPhoneNumber != null)
            {
                CellPhoneNumber = cellPhoneNumber;
            }
            if (emailAddress != null)
            {
                EmailAddress = emailAddress;
            }
        }

        #endregion

        #region Data Elements
        private Address i_Address = new Address();
        private EmailAddress i_EmailAddress = new EmailAddress();
        private PhoneNumber i_PhoneNumber = new PhoneNumber();
        private PhoneNumber i_CellPhoneNumber = new PhoneNumber();
        private TypeOfContactPoint i_TypeOfContactPoint = new TypeOfContactPoint();
        private CellPhoneConsent i_CellPhoneConsent = new CellPhoneConsent();
        private EmailReason i_EmailReason = new EmailReason();
        #endregion

        #region Constants
        #endregion
    }
}
