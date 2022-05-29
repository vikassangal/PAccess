using PatientAccess.Domain;

namespace PatientAccess.UI.ClinicalViews
{
    public interface IRightCareRightPlacePresenter
    {
        void UpdateView();
        void UpdateRightCareRightPlace( YesNoFlag rightCareRightPlace );
        void UpdateLeftOrStayed( LeftOrStayed leftOrStayed );
        void UpdateLeftWithoutBeingSeen( YesNoFlag leftWithoutBeingSeen );
        void UpdateLeftWithoutFinancialClearance( YesNoFlag leftWithoutFinancialClearance );
        void EvaluateViewRules();
    }
}