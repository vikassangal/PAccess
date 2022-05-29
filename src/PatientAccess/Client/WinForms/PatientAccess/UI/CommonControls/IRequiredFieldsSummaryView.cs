using System.Collections.Generic;

namespace PatientAccess.UI.CommonControls
{
    public interface IRequiredFieldsSummaryView
    {
        string HeaderText { set; get; }
        string Text { get; set; }
        RequiredFieldsSummaryPresenter Presenter { get; set; }
        void Update( List<RequiredFieldItem> itemInLists );
        void ShowAsModalDialog( object owner );
        void Hide();
    }
}