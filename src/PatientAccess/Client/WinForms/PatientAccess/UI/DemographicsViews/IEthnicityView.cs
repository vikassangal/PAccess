using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IEthnicityView
    {
        void PopulateEthnicity(IDictionary<Ethnicity, ArrayList> ethnicityDictionary);
        Ethnicity Ethnicity { get; set; }
        Account ModelAccount { get; set; }
        void SetDeactivatedBgColor();
        void ProcessInvalidCodeEvent();
        void SetEthnicityAsRequiredColor();
        void SetEthnicityAsPreferredColor();
        void SetNormalBgColor();
        IEthnicityViewPresenter EthnicityViewPresenter { get; set; }
        void SetSizeForEthnicityDropdownButton();
       
    }
}
