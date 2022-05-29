using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SelfPayPatientInfoUnavailablePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class SelfPayPatientInfoUnavailablePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler SelfPayPatientInfoUnavailablePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            SelfPayPatientInfoUnavailablePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            SelfPayPatientInfoUnavailablePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.SelfPayPatientInfoUnavailablePreferredEvent = null;   
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
            if( context == null || context.GetType() != typeof( SelfPayCoverage ) )
            {
                return true;
            }
            SelfPayCoverage aCoverage = context as SelfPayCoverage;
            if( aCoverage == null )
            {
                return false;
            }

            if( aCoverage.InsuranceInfoUnavailable == null
                || aCoverage.InsuranceInfoUnavailable.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && SelfPayPatientInfoUnavailablePreferredEvent != null )
                {
                    SelfPayPatientInfoUnavailablePreferredEvent( this, null );
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
        public SelfPayPatientInfoUnavailablePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
