using PatientAccess.UI.CommonControls.Suffix.Presenters;

namespace PatientAccess.UI.CommonControls.Suffix.Views
{
    public interface ISuffixView
    { 
        SuffixPresenter SuffixPresenter { get; set; }
        void UpdateSuffix(string suffix);
        void ClearItems();
        void AddSuffix(string suffix);
        void ClearSuffix();
    }
}