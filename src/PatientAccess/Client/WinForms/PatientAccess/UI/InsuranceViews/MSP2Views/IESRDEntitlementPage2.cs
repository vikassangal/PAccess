using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MSP2Views
{
    public interface IESRDEntitlementPage2
    {
        ESRDEntitlement EsrdEntitlement { get; set; }
        void ClearDialysisCenterNames();
        void PopulateDialysisCenterNames(IEnumerable<string> dialysisCenterNames);
        void SetDialysisCenterName(string dialysisCenterName);
        void EnableDialysisCenterNames();
        void DisableDialysisCenterNames();
        void SetDialysisCenterNameRequired();
        void SetDialysisCenterNameNormal();
        bool GHP();
        void EnablePanels(bool ghp);
        void ResetSelections();
        void ResetDialysisCenterSelection();
        bool ReceivedMaintenanceDialysisTreatment { get; set; }
        string SelectedDialysisCenter { get; set; }
        void UpdateView();
        bool DialysisCenterNamesEnabled { get; }
    }
}
