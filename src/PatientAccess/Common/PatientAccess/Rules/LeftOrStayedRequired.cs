using System;
using System.Configuration;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// /// Applies Required rule to the Account.IsPatientInClinicalResearchStudy property; 
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LeftOrStayedRequired : LeafRule
    {
        [NonSerialized]
        private RightCareRightPlaceFeatureManager _rightCareRightPlaceFeatureManager;

        private RightCareRightPlaceFeatureManager RightCareRightPlaceFeatureManager
        {
            get { return _rightCareRightPlaceFeatureManager; }
            set { _rightCareRightPlaceFeatureManager = value; }
        }

        #region Events

        public event EventHandler LeftOrStayedRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            LeftOrStayedRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            LeftOrStayedRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            LeftOrStayedRequiredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || !( context is IAccount ) )
            {
                return true;
            }

            Account model = ( Account )context;
            if( model == null )
            {
                return true;
            }
            RightCareRightPlaceFeatureManager = new RightCareRightPlaceFeatureManager( ConfigurationManager.AppSettings );

            if ( RightCareRightPlaceFeatureManager.ShouldRCRPFieldsBeVisible( model.Facility, model.KindOfVisit, model.Activity ) && 
                 RightCareRightPlaceFeatureManager.IsFeatureEnabledFor( model.AccountCreatedDate, DateTime.Today ) &&
                 model.RightCareRightPlace != null && model.RightCareRightPlace.RCRP != null && 
                 model.RightCareRightPlace.RCRP.Code.Equals( YesNoFlag.CODE_YES ) &&
                 model.RightCareRightPlace.LeftOrStayed.Code.Trim() == string.Empty )
            {
                if ( FireEvents && LeftOrStayedRequiredEvent != null )
                {
                    LeftOrStayedRequiredEvent( this, null );
                }

                return false;
            }

            return true;
        }

        public override void ApplyTo( object context )
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
