using System;
using System.Configuration;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// /// Applies Required rule to the Account.RightCareRightPlace property; 
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class RightCareRightPlaceRequired : LeafRule
    {
        [NonSerialized]
        private RightCareRightPlaceFeatureManager _rightCareRightPlaceFeatureManager;

        private RightCareRightPlaceFeatureManager RightCareRightPlaceFeatureManager
        {
            get { return _rightCareRightPlaceFeatureManager; }
            set { _rightCareRightPlaceFeatureManager = value; }
        }

        #region Events

        public event EventHandler RightCareRightPlaceRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            RightCareRightPlaceRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            RightCareRightPlaceRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            RightCareRightPlaceRequiredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || !( context is IAccount ) )
            {
                return true;
            }

            Account model = ( Account )context;
            if(model == null)
            {
                return true;
            }
            RightCareRightPlaceFeatureManager = new RightCareRightPlaceFeatureManager( ConfigurationManager.AppSettings );

            if ( RightCareRightPlaceFeatureManager.ShouldRCRPFieldsBeVisible( model.Facility, model.KindOfVisit ,model.Activity) &&
                 RightCareRightPlaceFeatureManager.IsFeatureEnabledFor( model.AccountCreatedDate, DateTime.Today ) &&
                  RightCareRightPlaceFeatureManager.IsFeatureEnabledFor(model.AdmitDate, DateTime.Today) && 
                 model.RightCareRightPlace != null && model.RightCareRightPlace.RCRP != null &&
                 model.RightCareRightPlace.RCRP.Code.Trim() == string.Empty )
            {
                if (FireEvents && RightCareRightPlaceRequiredEvent != null)
                {
                    RightCareRightPlaceRequiredEvent( this, null );
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
