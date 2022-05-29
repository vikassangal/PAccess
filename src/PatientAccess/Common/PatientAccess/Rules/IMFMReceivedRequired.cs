using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for IMFM Received Required
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class IMFMReceivedRequired : LeafRule
    {
        # region private properties and variables

        private ICOBReceivedAndIMFMReceivedFeatureManager cobReceivedAndIMFMReceivedFeatureManager;

        private Account anAccount;

        private ICOBReceivedAndIMFMReceivedFeatureManager COBReceivedAndIMFMReceivedFeatureManager
        {
            get { return cobReceivedAndIMFMReceivedFeatureManager; }
            set { cobReceivedAndIMFMReceivedFeatureManager = value; }
        }

        #endregion

        # region Event Handlers

        public event EventHandler COBReceivedAndIMFMReceivedRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            COBReceivedAndIMFMReceivedRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            COBReceivedAndIMFMReceivedRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            COBReceivedAndIMFMReceivedRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }

            anAccount = context as Account;
            if (anAccount == null)
            {
                return true;
            }

            COBReceivedAndIMFMReceivedFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();

            if ((COBReceivedAndIMFMReceivedFeatureManager.IsIMFMReceivedEnabledForAccount(anAccount))
                && String.IsNullOrEmpty(anAccount.IMFMReceived.Code.Trim()))
            {
                if ( FireEvents && COBReceivedAndIMFMReceivedRequiredEvent != null )
                {
                    COBReceivedAndIMFMReceivedRequiredEvent(this, null);
                }
                return false;
            }
            return true;
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

    }
}
