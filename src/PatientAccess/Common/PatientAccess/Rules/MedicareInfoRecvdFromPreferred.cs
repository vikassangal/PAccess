using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareInfoRecvdFromPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicareInfoRecvdFromPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareInfoRecvdFromPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareInfoRecvdFromPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareInfoRecvdFromPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareInfoRecvdFromPreferredEvent = null;   
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
            if( context.GetType() != typeof( GovernmentMedicareCoverage ) )
            {
                return true;
            }
            Coverage aCoverage = context as Coverage;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.InformationReceivedSource == null 
                || aCoverage.InformationReceivedSource.Description == string.Empty )
            {
                if( this.FireEvents && MedicareInfoRecvdFromPreferredEvent != null )
                {
                    MedicareInfoRecvdFromPreferredEvent( this, null );
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
        public MedicareInfoRecvdFromPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
