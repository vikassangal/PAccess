 
using System.Collections; 
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DemographicsViews.Presenters; 

namespace PatientAccess.UI.DemographicsViews.Views
{
    public interface IGenderView
    {
        void SetNormal();
        void MakeGenderRequired();
        void MakeGenderControlError();
        void ProcessInvalidCode();
        GenderViewPresenter GenderViewPresenter { get; set; }
        GenderControl GenderControl { get;  }
      void UpdateGenderOnView(DictionaryEntry gender); 
    }
}
