using System;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.ClinicalViews
{
    public class RightCareRightPlacePresenter : IRightCareRightPlacePresenter
    {
        public RightCareRightPlacePresenter( IRightCareRigtPlaceView view, IRightCareRightPlaceFeatureManager featureManager, Account account )
        {
            View = view;
            FeatureManager = featureManager;
            RuleEngine = RuleEngine.GetInstance();
            Account = account;
        }

        public IRightCareRigtPlaceView View { get; private set; }
        private RuleEngine RuleEngine { get; set; }

        private IRightCareRightPlaceFeatureManager FeatureManager { get; set; }

        #region IRightCareRightPlacePresenter Members

        public void UpdateView()
        {
            bool isRcrpVisibleForFacilityAndVisitType = FeatureManager.ShouldRCRPFieldsBeVisible( this.Account.Facility, this.Account.KindOfVisit, this.Account.Activity );
            bool isRcrpFeatureEnabledForAccountCreatedDate = FeatureManager.IsFeatureEnabledFor( this.Account.AccountCreatedDate, DateTime.Today );
            bool isRcrpFeatureEnabledForAdmitDate = FeatureManager.IsFeatureEnabledFor( this.Account.AdmitDate, DateTime.Today );
            if ( isRcrpVisibleForFacilityAndVisitType )
            {
                ShowRCRPFieldsVisible();
                PopulateRCRPFields();

                if ( isRcrpFeatureEnabledForAccountCreatedDate && isRcrpFeatureEnabledForAdmitDate )
                {
                    ShowRCRPFieldsEnabled();
                }
                else
                {
                    if ( !isRcrpFeatureEnabledForAccountCreatedDate )
                    {
                        ClearRCRPFields();
                    }
                    ShowRCRPFieldsDisabled();
                }
            }
            else
            {
                ShowRCRPFieldsNotVisible();
            }

            EvaluateViewRules();
        }

        public void UpdateRightCareRightPlace( YesNoFlag rightCareRightPlace )
        {
            Account.RightCareRightPlace.RCRP = rightCareRightPlace;
            EvaluateViewRules();
            UpdateLeftOrStayed();
        }

        public void UpdateLeftWithoutBeingSeen( YesNoFlag leftWithoutBeingSeen )
        {
            Account.LeftWithOutBeingSeen = leftWithoutBeingSeen;
        }

        public void UpdateLeftWithoutFinancialClearance( YesNoFlag leftWithoutFinancialClearance )
        {
            Account.LeftWithoutFinancialClearance = leftWithoutFinancialClearance;
        }

        public void EvaluateViewRules()
        {
            RuleEngine.EvaluateRule( typeof( RightCareRightPlaceRequired ), Account );
            RuleEngine.EvaluateRule( typeof( LeftOrStayedRequired ), Account );
        }

        public void UpdateLeftOrStayed( LeftOrStayed leftOrStayed )
        {
            Account.RightCareRightPlace.LeftOrStayed = leftOrStayed;
            EvaluateViewRules();
        }

        public Account Account
        {
            get; 
            private set;
        }

        #endregion

        internal void UpdateLeftOrStayed()
        {
            if ( Account.Activity.GetType() == typeof( MaintenanceActivity ) ||
                 Account.Activity.GetType() == typeof( EditAccountActivity ) )
            {
                View.PopulateLeftOrStayed();
                bool blnEnable = ( Account.RightCareRightPlace.RCRP.Code == YesNoFlag.CODE_YES );
                View.LeftOrStayedEnabled = blnEnable;
            }

            else
            {
                if ( Account.RightCareRightPlace.RCRP.Code.Equals( YesNoFlag.CODE_YES ) )
                {
                    View.PopulateLeftOrStayed();
                    View.LeftOrStayedEnabled = true;
                }
                else
                {
                    View.ClearLeftOrStayed();
                    View.LeftOrStayedEnabled = false;
                }
            }
            EvaluateViewRules();
        }

        private void PopulateRCRPFields()
        {
            View.PopulateRCRP();
            View.PopulateLeftWithoutBeingSeenField();
            View.PopulateLeftWithoutFinancialClearance();
        }

        private void ShowRCRPFieldsVisible()
        {
            View.RCRPVisible = true;
            View.LeftOrStayedVisible = true;
            View.LeftWithoutBeingSeenVisible = true;
            View.LeftWithoutFinancialClearanceVisible = true;
        }

        private void ShowRCRPFieldsNotVisible()
        {
            View.RCRPVisible = false;
            View.LeftOrStayedVisible = false;
            View.LeftWithoutBeingSeenVisible = false;
            View.LeftWithoutFinancialClearanceVisible = false;
        }

        private void ShowRCRPFieldsEnabled()
        {
            View.RCRPEnabled = true;
            UpdateLeftOrStayed();
            View.LeftWithoutBeingSeenEnabled = true;
            View.LeftWithoutFinancialClearanceEnabled = true;
        }

        private void ShowRCRPFieldsDisabled()
        {
            View.RCRPEnabled = false;
            View.LeftOrStayedEnabled = false;
            View.LeftWithoutBeingSeenEnabled = false;
            View.LeftWithoutFinancialClearanceEnabled = false;
        }

        private void ClearRCRPFields()
        {
            View.ClearRCRP();
            View.ClearLeftOrStayed();
            View.ClearLeftWithoutBeingSeen();
            View.ClearLeftWithoutFinancialClearance();
        }
    }
}