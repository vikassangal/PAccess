using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEthnicityCode : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidEthnicityCodeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidEthnicityCodeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEthnicityCodeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidEthnicityCodeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;

            if (account.DischargeDate == DateTime.MinValue)
            {
                return true;
            }

            if (account.Patient != null &&
                (!account.Patient.IsEthnicityValid(account) ||
                 !account.Patient.IsDescentValid(account)))
            {
                if (this.FireEvents && InvalidEthnicityCodeEvent != null)
                {
                    this.InvalidEthnicityCodeEvent(this, null);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
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

        public InvalidEthnicityCode()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
