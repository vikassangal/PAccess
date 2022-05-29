using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI;
using PatientAccess.UI.PatientSearch;

namespace Tests.Unit.PatientAccess.UI.PatientSearch
{
    [Ignore]
    [TestFixture]
    [Category( "Fast" )] 
    public class PatientAccountViewPresenterTest
    {
        
        [Test]
        public void SetActivatePreregButton_WithPreregPatient_ActivateAccountButtonsShouldEnabled()
        {
            RegistrationActivity currrentActivity = new RegistrationActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);
            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT);
            account.DerivedVisitType = Account.OUT_PRE;
           
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;
            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();
            
            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter) presenter).SelectedAccount = account; 
            presenter.EnableButtonsForActivity(view);
            
            Assert.IsTrue(view.Cbm.Command("btnActivatePrereg").Enabled == true, "Activate Account - Standard Registration button should be enabled.");
            Assert.IsTrue(view.Cbm.Command("btnActivatePreregShort").Enabled == true, "Activate Account - Diagnosis Registration button should be enabled.");
        }

        [Test]
        public void SetActivatePreregButton_WithNonPreregPatient_ActivateAccountButtonsShouldDisabled()
        {
            RegistrationActivity currrentActivity = new RegistrationActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);

            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.RECURRING_PATIENT_DESC, VisitType.RECURRING_PATIENT);
            account.DerivedVisitType = Account.OUT_PRE;
            
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;

            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();

            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter)presenter).SelectedAccount = account;
            presenter.EnableButtonsForActivity(view);

            Assert.IsTrue(view.Cbm.Command("btnActivatePrereg").Enabled == false, "Activate Account - Standard Registration button should be enabled.");
            Assert.IsTrue(view.Cbm.Command("btnActivatePreregShort").Enabled == false, "Activate Account - Diagnosis Registration button should be enabled.");
        }

        [Test]
        public void SetConvertShortPreregButton_WithStdPreregPatient_ConvertShortPreregButtonsShouldEnabled()
        {
            MaintenanceActivity currrentActivity = new MaintenanceActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);

            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT);
            account.DerivedVisitType = Account.OUT_REC;
            account.IsShortRegistered = false;
            
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;
            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();

            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter)presenter).SelectedAccount = account;
            presenter.EnableButtonsForActivity(view);

            Assert.IsTrue(view.Cbm.Command("btnConvertToDiagPrereg").Enabled == true, "Convert Account - Diagnostic Preregistration button should be enabled.");
        }

        [Test]
        public void SetConvertShortPreregButton_WithShortPreregPatient_ConvertShortPreregButtonsShouldDisabled()
        {
            MaintenanceActivity currrentActivity = new MaintenanceActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);

            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT);
            account.DerivedVisitType = Account.PRE_CAN;
            account.IsShortRegistered = true;
            
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;
            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();

            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter)presenter).SelectedAccount = account;
            presenter.EnableButtonsForActivity(view);

            Assert.IsTrue(view.Cbm.Command("btnConvertToDiagPrereg").Enabled == false, "Convert Account - Diagnostic Preregistration button should be disabled.");
        }
        [Test]
       public void SetContinueButton_WithUrgentCarePatient_CancelOutpatientDischargeStatus_EnableButtonsShouldDisabled()
        {
            CancelOutpatientDischargeActivity currrentActivity = new CancelOutpatientDischargeActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);

            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT);
            account.FinancialClass = new FinancialClass { Code = FinancialClass.MED_SCREEN_EXM_CODE };
            account.HospitalService = new HospitalService(0, PersistentModel.NEW_VERSION, "MED_OBSV_PT_IN_BED", "58");
            account.HospitalService.DayCare = "Y";
            account.DischargeDate = DateTime.Now.AddDays(-4);
            
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;
            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();

            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter)presenter).SelectedAccount = account;
            presenter.EnableButtonsForActivity(view);

            Assert.IsTrue(view.Cbm.Command("btnContinue").Enabled == false, "Continue - button should be disabled.");
        }
        [Test]
        public void SetContinueButton_WithUrgentCarePatient_CancelOutpatientDischargeStatus_EnableButtonsShouldEnabled()
        {
            CancelOutpatientDischargeActivity currrentActivity = new CancelOutpatientDischargeActivity();
            MasterPatientIndexView parent = new MasterPatientIndexView();
            parent.CurrentActivity = currrentActivity;
            Facility facility = facilityBroker.FacilityWith("ACO");
            User patientAccessUser = User.GetCurrent();
            patientAccessUser.Facility = facility;
            PatientsAccountsView view = new PatientsAccountsView(parent);

            AccountProxy account = new AccountProxy();
            account.KindOfVisit = new VisitType(2, DateTime.Now, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT);
            account.FinancialClass = new FinancialClass { Code = FinancialClass.UNINSURED_CODE };
            account.HospitalService = new HospitalService(0, PersistentModel.NEW_VERSION, "MED_OBSV_PT_IN_BED", "58");
            account.HospitalService.DayCare = "Y";
            account.DischargeDate = DateTime.Now.AddDays(-4);
            
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.TenetID = 5001896;
            securityUser.UPN = "patientaccess.user03";
            patientAccessUser.SecurityUser = securityUser;
            MessageBoxAdapter msgboxAdapter = new MessageBoxAdapter();

            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter(view, facility, msgboxAdapter,
                                                                                      currrentActivity);
            ((IPatientsAccountsPresenter)presenter).SelectedAccount = account;
            presenter.EnableButtonsForActivity(view);

            Assert.IsTrue(view.Cbm.Command("btnContinue").Enabled == true, "Continue - button should be Enabled.");
        }

        #region Data Elements
        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        #endregion

    }


}
