
namespace PatientAccess.UI.DiagnosisViews
{
    public interface IAlternateCareFacilityPresenter
    {
        void HandleAlternateCareFacility();
        void UpdateAlternateCareFacility( string alternateCareFacility );
        void EvaluateAlternateCareFacilityRule();
    }
}