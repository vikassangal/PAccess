using System;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.AddressViews.Views
{
    public interface ICellPhoneConsentView
    {
        void ClearConsentSelectionValues();
        void PopulateConsentSelections(IEnumerable<CellPhoneConsent> consentsValues);
        void Enable();
        void Disable();
        CellPhoneConsent CellPhoneConsent { get; }
        void SetCellPhoneConsentNormal();
        void CellPhoneNumberUpdated();
        void CellPhoneConsentUpdated(); 
        int MobileAreaCodeLength { get; }
        int MobilePhoneNumberLength { get; }
        void GuarantorConsentPreferredEventHandler(object sender, EventArgs e);
    }
}