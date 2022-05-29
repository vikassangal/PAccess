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
    public class AddressCountyRequired : AddressFieldsRequired
    {
        #region Events

        public event EventHandler AddressCountyRequiredEvent;

        #endregion

        #region Event Handlers

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            AddressCountyRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            AddressCountyRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            AddressCountyRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || AssociatedControl == null ||
                context.GetType() != typeof (Address))
            {
                return true;
            }
            var address = (Address) context;
            var capturePhysicalAddress = (bool) AssociatedControl;

            if(!capturePhysicalAddress)
            {
                return true;
            }

            string countyCode = String.Empty;

            if (address.County != null)
            {
                countyCode = ((Address) context).County.Code;
            }

            if (address.State == null || String.IsNullOrEmpty(address.State.Code))
            {
                return true;
            }

            if ((string.IsNullOrEmpty(countyCode)
                 || countyCode == "0")
                &&
                (address.IsUnitedStatesAddress()) && (address.State != null && AddressStateHasCounties(address.State.Code)))
            {
                if (FireEvents && AddressCountyRequiredEvent != null)
                {
                    AddressCountyRequiredEvent(this, null);
                }

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

        #endregion
    }
}