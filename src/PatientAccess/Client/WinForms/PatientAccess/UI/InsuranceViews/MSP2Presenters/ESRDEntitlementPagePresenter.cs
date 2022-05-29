using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews.MSP2Views;

namespace PatientAccess.UI.InsuranceViews.MSP2Presenters
{
    internal class ESRDEntitlementPagePresenter : IESRDEntitlementPagePresenter
    {
        #region Constructors

        internal ESRDEntitlementPagePresenter( IESRDEntitlementPage2 view, Account modelAccount )
        {
            View = view;
            Account = modelAccount;
        }

        #endregion Constructors

        #region Properties

        public IESRDEntitlementPage2 View { get; private set; }

        private Account Account { get; set; } 

        #endregion Properties

        #region Public Methods
        public void EnablePanels()
        {
            if (View.GHP())
            {
                View.EnablePanels(true);
                View.DisableDialysisCenterNames();
                View.ResetDialysisCenterSelection();
                View.ClearDialysisCenterNames();

            }
            else
            {
                View.EnablePanels(false);
                View.ResetSelections();
            }
        }
        public void HandleDialysisCenterNames()
        {
            bool enbaleDialysisCenterName = View.ReceivedMaintenanceDialysisTreatment;
            EnableDiasableDialysisCenterNames();
            if (enbaleDialysisCenterName && !View.GHP())
            {
                PopulateDialysisCenterNames();
                SetDialysisCenterName();
            }
        }

        public void EnableDiasableDialysisCenterNames()
        {
            bool enbaleDialysisCenterName = View.ReceivedMaintenanceDialysisTreatment;
            if (enbaleDialysisCenterName && !View.GHP())
            {
                View.EnableDialysisCenterNames();
                View.SetDialysisCenterNameRequired();
                PopulateDialysisCenterNames();
            }

            else
            {
                View.ClearDialysisCenterNames();
                View.SetDialysisCenterNameNormal();
                View.DisableDialysisCenterNames();
            }
        }
        private void PopulateDialysisCenterNames()
        {
            var dialysisCenterBroker = BrokerFactory.BrokerOfType<IDialysisCenterBroker>();
            var allDialysisCenterNames = dialysisCenterBroker.AllDialysisCenterNames(User.GetCurrent().Facility.Oid);
            View.PopulateDialysisCenterNames( allDialysisCenterNames );
            
        }

        private void SetDialysisCenterName()
        {
            if (Account == null
                  || Account.MedicareSecondaryPayor == null
                  || (Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement) == null)
            {
                if (String.IsNullOrEmpty(View.SelectedDialysisCenter))
                {
                    return;
                }

                View.SetDialysisCenterName(View.SelectedDialysisCenter);
                return;
            }

            if (Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals(typeof(ESRDEntitlement)))
            {
                var esrdEntitlement = Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement;

                if (esrdEntitlement.DialysisCenterName != null)
                {
                    View.SetDialysisCenterName(esrdEntitlement.DialysisCenterName.Trim());
                }
            }
        }
        public void SetDialysisCenterNameOnView()
        {

            if (String.IsNullOrEmpty(View.SelectedDialysisCenter))
            {
                View.SetDialysisCenterName(String.Empty);
                return;
            }

            View.SetDialysisCenterName(View.SelectedDialysisCenter);
            return;
        }

        public void SaveDialysisCenterName()
        {
            if (View.EsrdEntitlement != null)
            {
                View.EsrdEntitlement.DialysisCenterName = View.SelectedDialysisCenter;
            }
        }

        public void UpdateDialysisCenterName(string dialysisCenterName)
        {
            View.SelectedDialysisCenter = dialysisCenterName;
        }
        public void  SetDialysisCenterNameColor()
        {
            if(String.IsNullOrEmpty(View.SelectedDialysisCenter))
            {
                View.SetDialysisCenterNameRequired();
            }
            else
            {
                View.SetDialysisCenterNameNormal();
            }
        }
        
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

        #region Data Elements

        private string selectedDialysisCenter = string.Empty;

        #endregion Data Elements

        #region Constants

        #endregion Constants
    }
}