using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.ShortRegistration
{
    [UsedImplicitly]
    public class ShortAccountViewProxy : IAccountView
    {
        #region Events

        public event EventHandler OnRepeatActivity = delegate { };
        public event EventHandler OnEditAccount = delegate { };
        public event EventHandler OnCloseActivity = delegate { };

        #endregion

        public IAccountView GetIInstance()
        {
            return anAccountView;
        }

        public void ActivateAccount()
        {
        }

        public void ActivateClinicalTab()
        {
        }

        public void ActivateContactsTab()
        {
        }

        public void ActivateCounselingTab()
        {
        }

        public void ActivateDemographicsTab()
        {
        }

        public void ActivateDiagnosisTab()
        {
        }

        public void ActivateDocumentsTab()
        {
        }

        public void ActivateEmploymentTab()
        {
        }

        public void ActivateGuarantorTab()
        {
        }

        public void ActivateInsuranceTab()
        {
        }

        public void ActivatePaymentTab()
        {
        }

        public void ActivateRegulatoryTab()
        {
        }

        public void SetModel(Account anAccount)
        {
        }

        public Account GetModel()
        {
            return anAccountView.GetModel();
        }

        private BackgroundWorker GetAccountBackgroundWorker()
        {
            return anAccountView.SavingAccountBackgroundWorker;
        }

        public bool IsMedicareAdvisedForPatient()
        {
            return anAccountView.IsMedicareAdvisedForPatient();
        }

        public IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter
        {
            get { return anAccountView.RequiredFieldsSummaryPresenter; }
        }

        public void ShowPanel()
        {
        }

        public void HidePanel()
        {
        }

        public void SetActivatingTab( string value )
        {
        }

        public void StartBenefitsResponsePollTimer()
        {
        }

        public void StopBenefitsResponsePollTimer()
        {
        }

        public void UpdateView()
        {
        }

        public void Dispose()
        {
        }

        public void Show()
        {
        }

        public void DisplayLocationRequiredFieldSummary()
        {
        }

        public string ActiveContext
        {
            set { anAccountView.ActiveContext = value; }
        }

        public bool EnableInsuranceTab
        {
            set { anAccountView.EnableInsuranceTab = value; }
        }

        public BackgroundWorker SavingAccountBackgroundWorker
        {
            get { return GetAccountBackgroundWorker(); }
        }

        public Account Model_Account
        {
            get { return GetModel(); }
        }

        public DockStyle Dock
        {
            get { return anAccountView.Dock; }
            set { anAccountView.Dock = value; }
        }

        public object Model
        {
            get { return anAccountView.Model; }
            set { anAccountView.Model = value; }
        }

        public bool MedicareOver65Checked
        {
            get { return anAccountView.MedicareOver65Checked; }
            set { anAccountView.MedicareOver65Checked = value; }
        }

        public bool WasInvokedFromWorklistItem
        {
            set { anAccountView.WasInvokedFromWorklistItem = value; }
        }

        public bool Over65Check
        {
            set { anAccountView.Over65Check = value; }
        }

        IAccountView anAccountView = ShortAccountView.GetInstance();
    }
}

