using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsurancePlanSSNRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsurancePlanSSNRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsurancePlanSSNRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsurancePlanSSNRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsurancePlanSSNRequiredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            this.InsurancePlanSSNRequiredEvent = null;    
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
            if( context == null )
            {
                return true;
            }
            if( context.GetType() != typeof( CoverageForCommercialOther )
                && context.GetType() != typeof( GovernmentOtherCoverage )
                && context.GetType() != typeof( OtherCoverage )
                && context.GetType().BaseType != typeof( CoverageForCommercialOther )
                && context.GetType().BaseType != typeof( GovernmentOtherCoverage )
                && context.GetType().BaseType != typeof( OtherCoverage ))
            {
                return true;
            }

            CoverageForCommercialOther aCoverage = context as CoverageForCommercialOther;

            if( aCoverage == null )
            {
                return true;
            }
            if( aCoverage.CertSSNID == String.Empty )
            {
                if( this.FireEvents && InsurancePlanSSNRequiredEvent != null )
                {
                    InsurancePlanSSNRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public InsurancePlanSSNRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
