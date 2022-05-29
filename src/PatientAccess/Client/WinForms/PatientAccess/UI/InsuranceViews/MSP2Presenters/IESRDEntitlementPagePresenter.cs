namespace PatientAccess.UI.InsuranceViews.MSP2Presenters
{
    public interface IESRDEntitlementPagePresenter
    {
        #region Operations

        void HandleDialysisCenterNames();
        void UpdateDialysisCenterName(string dialysisCenterName);
        void SaveDialysisCenterName();
        void SetDialysisCenterNameColor();
        void EnablePanels();
        void SetDialysisCenterNameOnView();
        void EnableDiasableDialysisCenterNames();

        #endregion Operations
    }
}
