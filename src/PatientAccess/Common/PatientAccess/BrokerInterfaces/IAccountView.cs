using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IAccountView.
	/// </summary>
	public interface IAccountView
	{
        IAccountView GetIInstance();
        
        void ActivateAccount();
        void ActivateClinicalTab();
        void ActivateContactsTab();
        void ActivateCounselingTab();
        void ActivateDemographicsTab();
        void ActivateDiagnosisTab();
        void ActivateDocumentsTab();
        void ActivateEmploymentTab();
        void ActivateGuarantorTab();
        void ActivateInsuranceTab();
        void ActivatePaymentTab();
        void ActivateRegulatoryTab();
        
        void SetModel(Account anAccount);
        Account GetModel();
	    DockStyle Dock { get; set; }

	    bool IsMedicareAdvisedForPatient();
        IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter { get; }
	    bool MedicareOver65Checked { get; set; }
	    Account Model_Account { get; }
	    BackgroundWorker SavingAccountBackgroundWorker { get; }
	    bool EnableInsuranceTab { set; }
	    string ActiveContext { set; }
	    bool Over65Check { set; }
	    object Model { get; set; }
	    bool WasInvokedFromWorklistItem { set; }
	    void ShowPanel();
	    void HidePanel();
	    void DisplayLocationRequiredFieldSummary();
	    event EventHandler OnRepeatActivity;
	    event EventHandler OnEditAccount;
	    event EventHandler OnCloseActivity;
	    void UpdateView();
	    void Show();
	    void Dispose();

	    /// <summary>
	    /// stopResponsePollTimer - stop the polling timer
	    /// </summary>
	    void StopBenefitsResponsePollTimer();

	    /// <summary>
	    /// startResponsePollTimer - start the timer to poll every few (currently 10) seconds
	    /// </summary>
	    void StartBenefitsResponsePollTimer();

	    void SetActivatingTab( string value );
	}
}

namespace PatientAccess.UI
{
    public interface IRequiredFieldsSummaryPresenter
    {
        event EventHandler TabSelectedEvent;
        bool IsTabEventRegistered( EventHandler eventHandler );
        void ShowViewAsDialog( IWin32Window owner );
    }
}
