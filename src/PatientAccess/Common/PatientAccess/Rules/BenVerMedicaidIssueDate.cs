using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerMedicaidIssueDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerMedicaidIssueDate : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerMedicaidIssueDateEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerMedicaidIssueDateEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerMedicaidIssueDateEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerMedicaidIssueDateEvent = null;  
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
            if( context == null || context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {
                return true;
            }
            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;
            if( aCoverage == null )
            {
                return false;
            }

			Facility fac = new Facility();

			if( this.AssociatedControl != null)
			{
				fac = this.AssociatedControl as Facility;
			}
            
            if( aCoverage.IssueDate == DateTime.MinValue 
                &&  fac.MedicaidIssueDateRequired )
            {
                if( this.FireEvents && BenVerMedicaidIssueDateEvent != null )
                {
                    BenVerMedicaidIssueDateEvent( this, null );
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
        public BenVerMedicaidIssueDate()
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( BenVerMedicaidIssueDate ) );
        #endregion

        #region Constants
        #endregion
    }
}
