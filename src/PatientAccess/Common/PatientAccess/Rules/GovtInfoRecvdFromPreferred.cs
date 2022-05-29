using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GovtInfoRecvdFromPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GovtInfoRecvdFromPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GovtInfoRecvdFromPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GovtInfoRecvdFromPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GovtInfoRecvdFromPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GovtInfoRecvdFromPreferredEvent = null;   
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
            bool rc = true;

            if( context == null )
                return true;

            if( context.GetType() != typeof( WorkersCompensationCoverage ) 
                && context.GetType() != typeof( GovernmentOtherCoverage ) )
            {
                return true;
            }  

            if( context.GetType() == typeof( WorkersCompensationCoverage) )
            {
                WorkersCompensationCoverage wcCoverage = context as WorkersCompensationCoverage;

                if( wcCoverage == null )
                {
                    rc = false;
                }

                if( rc 
                    && ( wcCoverage.InformationReceivedSource == null 
                    || wcCoverage.InformationReceivedSource.Description == string.Empty ) )
                {
                    rc = false;
                }

            }
            else
            {
                GovernmentOtherCoverage gCoverage  = context as GovernmentOtherCoverage;

                if( gCoverage == null )
                {
                    rc = false;
                }

                if( rc 
                    && ( gCoverage.InformationReceivedSource == null
                    || gCoverage.InformationReceivedSource.Description == string.Empty ) )
                {
                    rc = false;
                }
            }

            if( !rc )
            {            
                if( this.FireEvents && GovtInfoRecvdFromPreferredEvent != null )
                {
                    GovtInfoRecvdFromPreferredEvent( this, null );
                }                
            }

            return rc;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GovtInfoRecvdFromPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
