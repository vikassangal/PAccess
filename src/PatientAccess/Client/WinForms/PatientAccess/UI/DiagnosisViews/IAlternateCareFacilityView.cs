using PatientAccess.Domain;

namespace PatientAccess.UI.DiagnosisViews
{
    public interface IAlternateCareFacilityView
    {
        Account Model { get; set; }
        void PopulateAlternateCareFacility();
        void ShowAlternateCareFacilityEnabled();
        void ShowAlternateCareFacilityDisabled();
        void ShowAlternateCareFacilityVisible();
        void ShowAlternateCareFacilityNotVisible();
        void ClearAlternateCareFacility();
    }
}
