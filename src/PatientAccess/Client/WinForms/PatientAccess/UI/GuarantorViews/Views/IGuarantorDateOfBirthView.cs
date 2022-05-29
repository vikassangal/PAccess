 using System;

namespace PatientAccess.UI.GuarantorViews.Views
{
    public interface IGuarantorDateOfBirthView
    {
        void ShowMe();
        void HideMe();
        void Populate(DateTime dateOfBirth);
        string UnmaskedText { get; set; }
        void FocusMe();
        void SetErrorColor();
        void SetNormalColor();
        void SetPreferredColor();
        void SetRequireedColor();
    }
}
