using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
    public interface IEmploymentView
    {
        Employment Model_Employment { get; set; }
        void SetControlState(bool state);
        void EnableClearButton(bool state);

    }
}
