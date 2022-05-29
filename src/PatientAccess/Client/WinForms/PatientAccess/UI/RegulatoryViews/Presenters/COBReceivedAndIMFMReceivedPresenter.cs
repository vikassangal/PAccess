using PatientAccess.Rules;
using PatientAccess.Domain; 
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class COBReceivedAndIMFMReceivedPresenter : ICOBReceivedAndIMFMReceivedPresenter
    {

        #region Variables and Properties

        public readonly ICOBReceivedAndIMFMReceivedView View ;

        private readonly IRegulatoryView RegulatoryView;

        private Account Account { get; set; }

        private readonly ICOBReceivedAndIMFMReceivedFeatureManager COBReceivedAndIMFMReceivedFeatureManager ;

        #endregion

        #region Constructors

        public COBReceivedAndIMFMReceivedPresenter(ICOBReceivedAndIMFMReceivedView cobIMFMView, IRegulatoryView regulatoryView, ICOBReceivedAndIMFMReceivedFeatureManager cobIMFMFeatureManager, Account account)
        {
            Guard.ThrowIfArgumentIsNull(cobIMFMView, "cobIMFMView");
            Guard.ThrowIfArgumentIsNull(cobIMFMFeatureManager, "cobIMFMFeatureManager");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(regulatoryView, "regulatoryView");

            RegulatoryView = regulatoryView;
            View = cobIMFMView;
            Account = account;
            COBReceivedAndIMFMReceivedFeatureManager = cobIMFMFeatureManager;
            View.Presenter = this;
        }

        #endregion

        #region Private Methods
        
        private void UpdateCOBReceivedFlag()
        {
            if (COBReceivedAndIMFMReceivedFeatureManager.IsCOBReceivedEnabledForAccount(Account))
            {
                if (Account.KindOfVisit.IsPreRegistrationPatient)
                {
                    RegulatoryView.SetCOBReceivedLocationForPreregistrationAccount();
                }
                else
                {
                    RegulatoryView.SetCOBReceivedLocationForRegistrationAccount();
                }

                View.ShowCOBReceived();
                UpdateCOBReceivedSelection();
            }
            else
            {
                View.HideCOBReceived();
            }
        }

        private void UpdateIMFMReceivedFlag()
        {
            if (COBReceivedAndIMFMReceivedFeatureManager.IsIMFMReceivedEnabledForAccount(Account))
            {
                if (Account.KindOfVisit.IsPreRegistrationPatient)
                {
                    RegulatoryView.SetIMFMReceivedLocationForPreregistrationAccount();
                }
                else
                {
                    RegulatoryView.SetIMFMReceivedLocationForRegistrationAccount();
                }
                View.ShowIMFMReceived();
                UpdateIMFMReceivedSelection();
            }
            else
            {
                View.HideIMFMReceived();
            }
        }

        private void UpdateCOBReceivedSelection()
         {
            switch ( Account.COBReceived.Code )
                {
                    case YesNoFlag.CODE_YES:
                        View.SetCOBReceivedAsYes();
                        break;

                    case YesNoFlag.CODE_NO:
                        View.SetCOBReceivedAsNo();
                        break;

                    default:
                        View.COBUnSelected();
                        break;
              }
         }

        private void UpdateIMFMReceivedSelection()
        {
            switch ( Account.IMFMReceived.Code )
                {
                    case YesNoFlag.CODE_YES:
                        View.SetIMFMReceivedAsYes();
                        break;

                    case YesNoFlag.CODE_NO:
                        View.SetIMFMReceivedAsNo();
                        break;

                    default:
                        View.IMFMUnSelected();
                        break;
                }
          }

        #endregion

        #region Public Methods
        public void UpdateView()
        {
            HideCOBReceivedIMFMReceived();
            UpdateCOBReceivedFlag();
            UpdateIMFMReceivedFlag();
        }

        private void HideCOBReceivedIMFMReceived()
        {
            View.HideCOBReceived();
            View.HideIMFMReceived();
        }

        public void SetCOBReceivedAsYes()
        {
           Account.COBReceived.SetYes();
        }

        public void SetCOBReceivedAsNo()
        {
           Account.COBReceived.SetNo();
        }
        
        public void SetIMFMReceivedAsYes()
        {
           Account.IMFMReceived.SetYes();
        }

        public void SetIMFMReceivedAsNo()
        {
            Account.IMFMReceived.SetNo();
        }

        #endregion

    }
}