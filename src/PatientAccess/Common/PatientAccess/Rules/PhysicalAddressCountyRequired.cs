using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address Zip
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicalAddressCountyRequired : AddressFieldsRequired
    {
        #region Methods

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null)
            {
                return true;
            }
            Address address;
            string contextType = context.GetType().ToString();
            switch (contextType)
            {
                case Account:
                    var anAccount = context as Account;
                    if (anAccount == null)
                    {
                        return true;
                    }
                    var physicalType = new TypeOfContactPoint(TypeOfContactPoint.PHYSICAL_OID, PHYSICAL_CONTACTPOINT);

                    ContactPoint physicalContactPoint = anAccount.Patient.ContactPointWith(physicalType);
                    address = physicalContactPoint.Address;
                    break;

                case Address:
                    address = context as Address;
                    if (address == null)
                    {
                        return true;
                    }
                    break;
                default:
                    return true;
            }
            string countyCode = String.Empty;

            if (address.County != null)
            {
                countyCode = address.County.Code;
            }

            if (address.State == null || String.IsNullOrEmpty(address.State.Code))
            {
                return true;
            }

            if ((string.IsNullOrEmpty(countyCode)
                 || countyCode == ZERO_COUNTY_CODE)
                &&
                (address.IsUnitedStatesAddress()) &&
                (address.State != null && AddressStateHasCounties(address.State.Code)))
            {
                return false;
            }

            return true;
        }

        private bool AddressStateHasCounties(String stateCode)
        {
            AddressBroker = new AddressBrokerProxy();
            return (AddressBroker.GetCountiesForAState(stateCode, User.GetCurrent().Facility.Oid).Count > 0);
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        #endregion

        #region Properties

        private IAddressBroker AddressBroker { get; set; }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements

        #endregion

        #region Constants

        private const string
            Account = "PatientAccess.Domain.Account",
            Address = "PatientAccess.Domain.Address",
            PHYSICAL_CONTACTPOINT = "Physical",
            ZERO_COUNTY_CODE = "0";

        #endregion
    }
}