using PatientAccess.Domain;

namespace PatientAccess.UI.ClinicalViews
{
    public interface IRightCareRigtPlaceView 
    {
        Account Model_Account { get; set; }
        bool RCRPVisible { set; get; }
        bool RCRPEnabled { set; get; }
        void PopulateRCRP();
        bool LeftOrStayedVisible { set; get; }
        bool LeftOrStayedEnabled { set; get; }
        void PopulateLeftOrStayed();
        bool LeftWithoutBeingSeenVisible { set; get; }
        bool LeftWithoutBeingSeenEnabled { set; get; }
        void PopulateLeftWithoutBeingSeenField();
        bool LeftWithoutFinancialClearanceVisible { set; get; }
        bool LeftWithoutFinancialClearanceEnabled { set; get; }
        void PopulateLeftWithoutFinancialClearance();
        void ClearRCRP();
        void ClearLeftOrStayed();
        void ClearLeftWithoutBeingSeen();
        void ClearLeftWithoutFinancialClearance();
    }
}