using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompClaimAddressVerifiedPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompClaimAddressVerifiedPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompClaimAddressVerifiedPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompClaimAddressVerifiedPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompClaimAddressVerifiedPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {   
            this.WorkersCompClaimAddressVerifiedPreferredEvent = null;            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;
            if( aCoverage == null )
            {
                return true;
            }
            YesNoFlag flag = new YesNoFlag();
            flag.SetBlank();

            if( aCoverage.ClaimsAddressVerified == null
                || aCoverage.ClaimsAddressVerified.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && WorkersCompClaimAddressVerifiedPreferredEvent != null )
                {
                    WorkersCompClaimAddressVerifiedPreferredEvent( this, null );
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public WorkersCompClaimAddressVerifiedPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
