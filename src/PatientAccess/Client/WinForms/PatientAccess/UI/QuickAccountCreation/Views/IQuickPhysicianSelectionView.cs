namespace PatientAccess.UI.QuickAccountCreation.Views
{
    public interface IQuickPhysicianSelectionView
    {
        
        void DisplayAdmittingPhysician( string admittingPhysicianText );
        void DisplayReferringPhysician( string referringPhysicianText );
        void RunRules();
        void ClearSpecifyPhysicianPanel();
        string AdmPhysicianNumber { get; set; }
        string RefPhysicianNumber { get; set; }
        void SetNormalReferringPhysicianTextBox();
        void SetBackColorReferringPhysicianTextBox();
        void SetNormalAdmissionPhysicianTextBox();
        void SetErrorAdmissionPhysicianTextBox();
        void SetErrorReferringPhysicianTextBox();

    }
}
