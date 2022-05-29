using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Views;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
   public class HIEShareDataPresenterFactory
    {
       //This method will return presenter object depend upon account created date
       public static IHIEShareDataPresenter GetPresenter(IHIEShareDataFlagView view, IHIEConsentFeatureManager featureManager, Account account)
       {
           ShareHIEDataFeatureManager shareHieDataFeatureManager = new ShareHIEDataFeatureManager();
           NotifyPCPDataFeatureManager notifyPCPFeatureManager = new NotifyPCPDataFeatureManager();
           if (shareHieDataFeatureManager.IsShareHieDataEnabledforaccount(account))
           {
               return new HIEShareDataPresenter(view, shareHieDataFeatureManager, new MessageBoxAdapter(), account,notifyPCPFeatureManager);
           }
           else
           {
               return new HIEConsentPresenter(view, featureManager,account);
           }
       }
    }
}
