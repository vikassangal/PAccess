
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class RegulatoryPresenter
    {
        private readonly IRegulatoryView RegulatoryView;
        private Account Account { get; set; }
        public RegulatoryPresenter(IRegulatoryView regulatoryView, Account account)
        {
            Guard.ThrowIfArgumentIsNull(regulatoryView, "regulatoryView");
            Guard.ThrowIfArgumentIsNull(account, "account");

            RegulatoryView = regulatoryView;
            Account = account;
          
        }

        public void SetShareDataWithHIEAndPCPLocation()
        {
            ShareHIEDataFeatureManager shareHieDataFeatureManager = new ShareHIEDataFeatureManager();
            if (shareHieDataFeatureManager.IsShareHieDataEnabledforaccount(Account))
            {
                RegulatoryView.SetHIEShareDataAndPCPFlagLocation();

            }
        }
    }
 }
