using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class PhysicalAddressRequired : LeafRule
    {
        #region Event Handlers

        public event EventHandler PhysicalAddressRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            PhysicalAddressRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            PhysicalAddressRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            PhysicalAddressRequiredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context.GetType() != typeof (Account))
            {
                return true;
            }

            var anAccount = context as Account;
            if (anAccount == null) return true;
            var physicalType = new TypeOfContactPoint(TypeOfContactPoint.PHYSICAL_OID, "Physical");

            var physicalContactPoint = anAccount.Patient.ContactPointWith(physicalType);
 
            if (physicalContactPoint != null
                && physicalContactPoint.Address != null
                && physicalContactPoint.Address.Address1 != null
                && physicalContactPoint.Address.Address1.Trim().Length > 0)
            {
                return true;
            }

            if (FireEvents && PhysicalAddressRequiredEvent != null)
            {
                PhysicalAddressRequiredEvent(this, new PropertyChangedArgs(AssociatedControl));
            }
            return false;
        }

        #endregion

        #region Properties

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