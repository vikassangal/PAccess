using System;
using PatientAccess.Domain;
using NUnit.Framework;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.RegulatoryViews
{
    [TestFixture]
    [Category("Fast")]
    public class COSHandlerTests
    {
        [Test]
        public void TestHandleCOSIsYes_ForNewAndExistingPatient_AutoPopulatesHospCommToYes_And_PatientPortalToYes()
        {
            var cosService = new ConditionOfService();
            cosService.Code = ConditionOfService.YES;
            long mrn = 0;
            var COSHandler = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, cosService,mrn);
            COSHandler.HandleCOSSignedSelected();
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableEmail());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableEmailReason());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableHospComm());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnablePatientPortal());
        }

        [Test]
        public void TestHandleCOSIsNo_ForNewPatient_AutoPopulatesHospCommToNo_And_PatientPortalToNo()
        {
            var cosService = new ConditionOfService();
            cosService.Code = ConditionOfService.NOT_AVAILABLE;
            long mrn = 0;
            var COSHandler = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, cosService,mrn);
            COSHandler.HandleCOSSignedSelected();
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableEmail());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableEmailReason());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableHospComm());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableHospComm());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.SetEmailReasonToPatientDeclined());
        }
        [Test]
        public void TestHandleCOSIsUnable_ForExistingPatient_AutoPopulatesHospCommToNo_And_PatientPortalToNo()
        {
            var cosService = new ConditionOfService();
            cosService.Code = ConditionOfService.UNABLE;
            long mrn = 225154;
            var COSHandler = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, cosService,mrn);
            COSHandler.HandleCOSSignedSelected();
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableEmail());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableEmailReason());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnableHospComm());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.EnablePatientPortal());
        }
        [Test]
        public void TestHandleCOSIsRefused_ForExistingPatient_AutoPopulatesHospCommToNo_And_PatientPortalToNo()
        {
            var cosService = new ConditionOfService();
            cosService.Code = ConditionOfService.REFUSED;
            long mrn = 225155;
            var COSHandler = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, cosService, mrn);
            COSHandler.HandleCOSSignedSelected();
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableEmail());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableEmailReason());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisableHospComm());
            COSHandler.RegulatoryView.AssertWasCalled(view => view.DisablePatientPortal());
        }
        private COSSignedHandler GetPresenterWithMockView(Activity activity, VisitType patientType,ConditionOfService COSService, long mrn)
        {
            var view = MockRepository.GenerateMock<IRegulatoryView>();
            var account = GetAccount(activity, patientType);
            account.COSSigned.Code = COSService.Code;
            account.Patient.MedicalRecordNumber = mrn;
            return new COSSignedHandler(view, RuleEngine.GetInstance(), account);
        }
        #region Support Methods
        private static Account GetAccount(Activity activity, VisitType visitType)
        {

            return new Account
            {
                Activity = activity,
                AdmitDate = new DateTime(2013, 07, 20),
                AccountCreatedDate = new DateTime(2013, 07, 20),
                KindOfVisit = visitType,
                
            };
        }
#endregion
    }
}
