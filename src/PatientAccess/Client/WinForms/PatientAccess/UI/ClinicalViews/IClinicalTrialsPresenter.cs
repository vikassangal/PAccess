using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.ClinicalViews
{
    public interface IClinicalTrialsPresenter
    {
        Account Account { get; set; }
        IResearchStudyBroker StudyBroker { get; }
        IClinicalTrialsDetailsView DetailsView { get; set; }
        ClinicalTrialsDetailsPresenter DetailsPresenter { get; set;}
        void HandleClinicalResearchFields( DateTime admitDate );
        void EvaluateClinicalResearchFieldRules();
        void ShowDetails();
        void UserChangedPatientInClinicalTrialsTo(YesNoFlag flag);
    }
}