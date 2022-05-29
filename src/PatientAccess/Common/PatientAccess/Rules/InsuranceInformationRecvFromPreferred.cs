using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuranceInformationRecvFromPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceInformationRecvFromPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuranceInformationRecvFromPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceInformationRecvFromPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceInformationRecvFromPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuranceInformationRecvFromPreferredEvent = null;   
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
            if( context == null || 
                context.GetType() != typeof( Coverage ) &&
                context.GetType().BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof ( Coverage )
                )
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
                if( this.FireEvents && InsuranceInformationRecvFromPreferredEvent != null )
                {
                    InsuranceInformationRecvFromPreferredEvent( this, null );
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
        public InsuranceInformationRecvFromPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
