using System;
using PatientAccess.Domain;
using PatientAccess.Utilities;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.Views;
namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class HIEShareDataPresenter : IHIEShareDataPresenter
    {
        #region Constructors
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public HIEShareDataPresenter(IHIEShareDataFlagView view, IShareHIEDataFeatureManager featureManager,IMessageBoxAdapter messageBoxAdapter, Account account,INotifyPCPFeatureManager featureManagerPCP)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(featureManager, "featureManager");

            View = view;
            FeatureManager = featureManager;
            FeatureManagerPCP = featureManagerPCP;
            MessageBoxAdapter = messageBoxAdapter;
            Account = account;
        }

        public HIEShareDataPresenter()
        {

        }

        #endregion

        #region public methods

        public void UpdateShareDataWithPublicHIE()
        {

            Account.ShareDataWithPublicHieFlag = View.ShareDataWithPublicHIE;

            RunRules();
        }

        public void UpdateShareDataWithPCP()
        {
            Account.ShareDataWithPCPFlag = View.ShareDataWithPCP;
            RunRules();
        }

        public void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool rightToRestrictchecked)
        {
            bool ShareHIEDataEnabled = FeatureManager.IsShareHieDataEnabledforaccount(Account);
            YesNoFlag DefaultShareDataWithHIE = FeatureManager.DefaultShareHieDataForFacility(Account.Facility);

            if (ShareHIEDataEnabled)
            {
                if ( ! rightToRestrictchecked )
                {
                    if ( ! View.IsShareDataPublicHIEEnabled )
                    {
                        Account.ShareDataWithPublicHieFlag = DefaultShareDataWithHIE;
                        SetShareDataWithPublicHIEAsBlank();
                    }
                }
                else
                {
                    if ( View.IsShareDataPublicHIEEnabled )
                    {
                        SetShareDataPublicHIEAsNo(UIErrorMessages.PATIENT_REQUESTED_RIGHT_TO_RESTRICT_FLAG_MSG);
                    }
                }

                RunRules();
            }
           
        }

        public void AutoPopulateShareDataWithPublicHIEForShareDatawithPCP(bool selectedShareDataWithPCP)
        {
            bool ShareHIEDataEnabled = FeatureManager.IsShareHieDataEnabledforaccount(Account);

            if (ShareHIEDataEnabled)
            {
                if (selectedShareDataWithPCP)
                {
                    if ( ! View.IsShareDataPublicHIEEnabled )
                    {
                        SetShareDataWithPublicHIEAsBlank();
                    }
                }
                else
                {
                    if ( View.IsShareDataPublicHIEEnabled )
                    {
                        SetShareDataPublicHIEAsNo(UIErrorMessages.SHARE_DATA_WITH_PCP_FLAG_MSG);
                    }
                }

                RunRules();
            }
           
        }

        public void UpdateView()
        {
            HideShareDataWithPublicHIE();

            ShowShareDataWithPublicHIE();
            ShowShareDataWithPCP();
        }
        public void HandleNotifyPCP()
        {
            View.ShareDataWithPCP = null;
            YesNoFlag DefaultNotifyPCPValue = FeatureManagerPCP.DefaultNotifyPCPForFacility(Account.Facility);
            Account.ShareDataWithPCPFlag = DefaultNotifyPCPValue;
            ShowShareDataWithPCP();
        }
        #endregion

        #region private methods
        private void NotifyPCPDataRequiredEventhandler(object sender, EventArgs e)
        {
            View.SetNotifyPCPDataRequiredBgColor();
        }
        private void ShareHIEDataRequiredEventhandler(object sender, EventArgs e)
        {
            View.SetShareDataWithPublicHIEAsRequired();
        }

        private void RunRules()
        {
            View.SetShareDataWithPublicHIEToNormalColor();
            View.SetNotifyPCPToNormalColor();
            RuleEngine.GetInstance().OneShotRuleEvaluation<NotifyPCPDataRequired>(Account, NotifyPCPDataRequiredEventhandler);
            RuleEngine.GetInstance().OneShotRuleEvaluation<ShareDataWithPublicHIERequired>(Account, ShareHIEDataRequiredEventhandler);
        }

       private void SetShareDataPublicHIEAsNo(string message)
        {
            if ( ! IsShareDataPublicHIESetToNo )
            {
                View.ShareDataWithPublicHIE = YesNoFlag.No;
                View.DisableShareDatawithPublicHIE();
                ShowHIEMessage(message);
            }
            else if ( IsShareDataPublicHIESetToNo )
            {
                View.DisableShareDatawithPublicHIE();
                ShowHIEMessage(message);
            }
        }

        private void ShowHIEMessage(string message)
        {
            if ( View.ShowHIEMessage )
            {
                ShowMessage(message);
            }
        }

        private void SetShareDataWithPublicHIEAsBlank()
        {
            if ( !View.IsShareDataPublicHIEEnabled &&
                ! Account.RightToRestrict.IsYes )
            {
                View.ShareDataWithPublicHIE = null;
                ShowShareDataWithPublicHIE();
                View.EnableShareDataWithPublicHIE();
            }
        }

        private void ShowMessage(string errorMessage)
        {
            MessageBoxAdapter.ShowMessageBox(errorMessage, "Warning",
                        MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Warning,
                        MessageBoxAdapterDefaultButton.Button1);
        }

        private bool IsShareDataPublicHIESetToNo
        {
            get { return Account != null && Account.ShareDataWithPublicHieFlag != null &&
                       Account.ShareDataWithPublicHieFlag.IsNo;}
        }
        private bool IsShareDataWithPCPSetToNo
        {
            get
            {
                return Account != null && Account.ShareDataWithPCPFlag != null && 
                     Account.ShareDataWithPCPFlag.IsNo;
            }
        }
        private bool IsShareDataWithPCPSetToYes
        {
            get
            {
                return Account != null && Account.ShareDataWithPCPFlag != null &&
                     Account.ShareDataWithPCPFlag.IsYes;
            }
        }
        private void HideShareDataWithPublicHIE()
        {
            View.HideShareDataWithPublicHIE();
            View.HideShareDataWithPCP();
        }
        
        private void ShowShareDataWithPublicHIE()
        {
            bool ShareHIEDataEnabled = FeatureManager.IsShareHieDataEnabledforaccount(Account);
            YesNoFlag DefaultShareDataWithHIE = FeatureManager.DefaultShareHieDataForFacility(Account.Facility);
            if (Account != null && ShareHIEDataEnabled)
            {
                var selectedHIEValue = View.ShareDataWithPublicHIE;
                View.ShowShareDataWithPublicHIE();
                View.PopulateShareDataWithPublicHIE(ShareDataWithPublicHie);
                if ((Account != null && Account.ShareDataWithPublicHieFlag != null) && (!String.IsNullOrEmpty(Account.ShareDataWithPublicHieFlag.Code.Trim())))
                {
                    if(Account.RightToRestrict.IsYes)
                    {
                        View.ShareDataWithPublicHIE = YesNoFlag.No;
                    }
                    if (Account.Activity.IsMaintenanceActivity()
                            || Account.Activity.IsShortMaintenanceActivity()
                            || Account.Activity.IsPostMSEActivity()
                            || Account.Activity.IsEditPreMseActivity()
                            || Account.Activity.IsEditUCCPreMSEActivity()
                            || Account.Activity.IsUccPostMSEActivity()
                            )
                    {
                        View.ShareDataWithPublicHIE = Account.ShareDataWithPublicHieFlag;
                    }
                    else
                    {
                        if (selectedHIEValue == null)
                        {                            
                           View.ShareDataWithPublicHIE = DefaultShareDataWithHIE;
                            
                        }
                        else
                        {
                            View.ShareDataWithPublicHIE = Account.ShareDataWithPublicHieFlag;
                        }
                    }
                }
                else
                {
                    View.ShareDataWithPublicHIE = Account.ShareDataWithPublicHieFlag;
                }
            }
            else
            {
                View.ShareDataWithPublicHIE = DefaultShareDataWithHIE;
            }
        }

        private void ShowShareDataWithPCP()
        {
            bool NotifyPCPDataEnabled = FeatureManagerPCP.IsNotifyPCPEnabledforaccount(Account);
            YesNoFlag DefaultNotifyPCPValue = FeatureManagerPCP.DefaultNotifyPCPForFacility(Account.Facility);
            if (Account != null)
            {
                if (NotifyPCPDataEnabled)
                {
                    var selectedPCPValue = View.ShareDataWithPCP;
                    View.ShowShareDataWithPCP();
                    View.PopulateShareDataWithPCP(ShareDataWithPcpFlag);
                    if ((Account != null && Account.ShareDataWithPCPFlag != null) && (!string.IsNullOrEmpty(Account.ShareDataWithPCPFlag.Code.Trim())))
                    {
                        if (Account.Activity.IsMaintenanceActivity()
                            || Account.Activity.IsShortMaintenanceActivity()
                            || Account.Activity.IsPostMSEActivity()
                            || Account.Activity.IsEditPreMseActivity()
                            || Account.Activity.IsEditUCCPreMSEActivity()
                            || Account.Activity.IsUccPostMSEActivity()
                            )
                        {
                            View.ShareDataWithPCP = Account.ShareDataWithPCPFlag;
                        }
                        else
                        {
                            if (selectedPCPValue == null)
                            {
                                View.ShareDataWithPCP = DefaultNotifyPCPValue;
                            }
                            else
                            {
                                View.ShareDataWithPCP = Account.ShareDataWithPCPFlag;
                            }
                        }
                    }
                    else
                    {
                        View.ShareDataWithPCP = Account.ShareDataWithPCPFlag;
                    }
                }
                else
                {
                    View.ShareDataWithPCP = YesNoFlag.No;
                }
            }
        }
        
        #endregion

        #region Properties
        
        public IHIEShareDataFlagView View { get; private set; }

        private IShareHIEDataFeatureManager FeatureManager { get; set; }
        private INotifyPCPFeatureManager FeatureManagerPCP { get; set; }

        private Account Account { get; set; }

        private static readonly YesNoFlag[] ShareDataWithPublicHie =
        {
            YesNoFlag.Blank,
            YesNoFlag.Yes,
            YesNoFlag.No
        };

        private static readonly YesNoFlag[] ShareDataWithPcpFlag =
        {
            YesNoFlag.Blank,
            YesNoFlag.Yes,
            YesNoFlag.No
        };

        #endregion Properties
    }
}
