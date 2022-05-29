using PatientAccess.BenefitsValidationFusProxy;

namespace PatientAccess.Services
{
    public interface IBenefitsValidationFusService
    {
        void CancelAsync(object userState);
        dropFusNoteResponse dropFusNote(dropFusNote dropFusNote1);
        void dropFusNoteAsync(dropFusNote dropFusNote1);
        void dropFusNoteAsync(dropFusNote dropFusNote1, object userState);
        event dropFusNoteCompletedEventHandler dropFusNoteCompleted;
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
