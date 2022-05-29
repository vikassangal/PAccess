using PatientAccess.Domain;
using PatientAccess.UI.CommonControls.ServiceCategory.Presenter;
using System.Collections;

namespace PatientAccess.UI.CommonControls.ServiceCategory.View
{
    public interface IServiceCategoryView
    {
        Account Model_Account { get; set; }
        ServiceCategoryPresenter ServiceCategoryPresenter { get; set; }
        bool EnableServiceCategory { set; }
        bool ShowMe { set; }
        string SelectedServiceCategory { get; set; }
        void LoadServiceCategory(ArrayList serviceCategories);
        void ClearServiceCategory();
        void SetNormalBgColor();
        void MakeRequiredBgColor();
    }
}