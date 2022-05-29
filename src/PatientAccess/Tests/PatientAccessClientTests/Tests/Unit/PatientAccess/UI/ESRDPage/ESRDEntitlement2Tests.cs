using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews.MSP2;
using PatientAccess.UI.InsuranceViews.MSP2Presenters;
using PatientAccess.UI.InsuranceViews.MSP2Views;

namespace Tests.Unit.PatientAccess.UI.ESRDPage
{
    [TestFixture]
    public class EsrdEntitlement2Tests
    {
        
        [Test, Sequential]
        [Category( "Fast" )]
        public void Test1HandleDialysisCenterNamesActivityIsRegistrationWhenDialysiCenterNameShouldBeDisabled(
            [Values(true,true,false)]bool ghp,
            [Values(true,false,false)]bool receivedMaintenanceTreatments)
        {
            var presenter = GetPresenterWith(_erVisitType, new RegistrationActivity(), DateTime.Today, ghp);
            var view = presenter.View;

            view.UpdateView();
            view.ReceivedMaintenanceDialysisTreatment = receivedMaintenanceTreatments;

            Assert.IsFalse(view.DialysisCenterNamesEnabled);
        }
        [Test]
        public void Test2HandleDialysisCenterNamesActivityIsRegistrationWhenDialysiCenterNameShouldBeEnabled()
        {
            var presenter = GetPresenterWith(_erVisitType, new RegistrationActivity(), DateTime.Today, false );
            var view = presenter.View;

            view.UpdateView();
            view.ReceivedMaintenanceDialysisTreatment = true ;

            Assert.IsTrue(view.DialysisCenterNamesEnabled);
        }
        
        #region Constants
        #endregion

        private readonly VisitType _erVisitType = new VisitType( PersistentModel.NEW_OID,
                                                               PersistentModel.NEW_VERSION,
                                                               VisitType.EMERGENCY_PATIENT_DESC,
                                                               VisitType.EMERGENCY_PATIENT );
        

        private static ESRDEntitlementPagePresenter GetPresenterWith( VisitType visitType, Activity activity,  DateTime admitDate, bool GHP )
        {
            var account = GetAccount( visitType, activity,  admitDate , GHP );
            var view = GetStubEsrdEntitlementView(account, GHP);

            return new ESRDEntitlementPagePresenter(view, account);
        }

        private static IESRDEntitlementPage2 GetStubEsrdEntitlementView( Account account , bool GHP)
        {
            var msp2Dialog = new MSP2Dialog(account);
           
            _esrdEntitlementPage1 = new ESRDEntitlementPage1("ESRDEntitlementPage1", msp2Dialog.wizardContainer, account );
            _esrdEntitlementPage1.CheckBoxYesNoGroup1.rbYes.Checked = GHP;
            msp2Dialog.wizardContainer.AddWizardPage(_esrdEntitlementPage1);
            var esrdView = new ESRDEntitlementPage2("ESRDEntitlementPage2", msp2Dialog.wizardContainer)
                               {EsrdEntitlement = account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement};
            return esrdView;
        }
      
        private static Account GetAccount( VisitType visitType, Activity activity,   DateTime admitDate, bool GHP )
        {
            var facility = new Facility( 54,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF" );

            var account = new Account
                              {
                                  Facility = facility,
                                  Activity = activity,
                                  AdmitDate = admitDate,
                                  KindOfVisit = visitType,
                                  MedicareSecondaryPayor = new global::PatientAccess.Domain.MedicareSecondaryPayor()
                              };
            User.SetCurrentUserTo(User.NewInstance());
            User.GetCurrent().Facility = account.Facility;

            var esrdEntitlement = new ESRDEntitlement();
            esrdEntitlement.GroupHealthPlanCoverage = GHP ? GHPTrue() : GHPFalse();
            account.MedicareSecondaryPayor.MedicareEntitlement = esrdEntitlement;
             return account;
        }
        private  static YesNoFlag GHPTrue()
        {
            var ghp = new YesNoFlag();
            ghp.SetYes();
            return ghp;
        }
        private static YesNoFlag GHPFalse()
        {
            var ghp = new YesNoFlag();
            ghp.SetNo();
            return ghp;
        }
        #region Data Elements

        private static ESRDEntitlementPage1 _esrdEntitlementPage1;
        #endregion
    }
}