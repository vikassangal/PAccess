using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class SelectedAccountForTransferOutToInTests
    {
        #region Constants

        private readonly long TRANSFER_MEDICALRECORDNUMBER = 409611,
                              TRANSFER_ACCOUNTNUMBER = 2530279;

        private readonly long PREREG_MEDICALRECORDNUMBER = 409627,
                              PREREG_ACCOUNTNUMBER = 2530589;

        private readonly long PREMSE_MEDICALRECORDNUMBER = 409637,
                              PREMSE_ACCOUNTNUMBER = 2530694;
        private readonly long INPATIENT_MEDICALRECORDNUMBER = 409611,
                              INPATIENT_ACCOUNTNUMBER = 2530279;

        #endregion

        #region SetUp and TearDown SelectedAccountForTransferOutToInTests

        #endregion

        #region Test Methods

        [Test]
        [Description("VeryLongExecution")]
        public void TestSelectedAccountForTransferOutToInActivity()
        {
            var account = GetAccount( new TransferOutToInActivity(), TRANSFER_MEDICALRECORDNUMBER,TRANSFER_ACCOUNTNUMBER );
            Assert.AreEqual("2", account.KindOfVisit.Code, "Outpatient Kind Of Visit is expected.");
            Account selectedAccount = AccountActivityService.SelectedAccountFor(account);
            Assert.IsNotNull(selectedAccount);
            Assert.IsNotNull(selectedAccount.AdmittingPhysician);
            Assert.AreEqual("1", selectedAccount.KindOfVisit.Code, "Inpatient kind of visit is expected."); 
        }

        [Test]
        [Description("VeryLongExecution")]
        public void TestSelectedAccountForPostMSEInActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), PREMSE_MEDICALRECORDNUMBER, PREMSE_ACCOUNTNUMBER);
            Assert.AreEqual("2", account.KindOfVisit.Code, "Outpatient Kind Of Visit is expected.");
            Account selectedAccount = AccountActivityService.SelectedAccountFor(account);
            Assert.IsNotNull(selectedAccount);
            Assert.IsNotNull(selectedAccount.AdmittingPhysician);
            Assert.AreEqual("0", selectedAccount.KindOfVisit.Code, "PreAdmit kind of visit is expected.");
        }

        [Test]
        [Description("VeryLongExecution")]
        public void TestSelectedAccountForPreRegActivatePreReg()
        {
            var account = GetAccount(new ActivatePreRegistrationActivity(), PREREG_MEDICALRECORDNUMBER, PREREG_ACCOUNTNUMBER);
            Assert.AreEqual("2", account.KindOfVisit.Code, "Outpatient Kind Of Visit is expected.");
            Account selectedAccount = AccountActivityService.SelectedAccountFor(account);
            Assert.IsNotNull(selectedAccount);
            Assert.IsNotNull(selectedAccount.AdmittingPhysician);
            Assert.AreEqual("0", selectedAccount.KindOfVisit.Code, "PreAdmit kind of visit is expected.");
        }

        [Test]
        [Description("VeryLongExecution")]
        public void TestSelectedAccountForCancelInpatientStatus()
        {
            var account = GetAccount(new CancelInpatientStatusActivity(), INPATIENT_MEDICALRECORDNUMBER, INPATIENT_ACCOUNTNUMBER);
            Assert.AreEqual("2", account.KindOfVisit.Code, "Outpatient kind of visit is expected.");
            Account selectedAccount = AccountActivityService.SelectedAccountFor(account);
            Assert.IsNotNull(selectedAccount);
            Assert.IsNotNull(selectedAccount.AdmittingPhysician);
            Assert.AreEqual("1", selectedAccount.KindOfVisit.Code, "Inpatient kind of visit is expected.");
        }

        [Test]
        [Description("VeryLongExecution")]
        public void TestSelectedAccountForPreAdmitDiagStatus()
        {
            var account = GetAccount(new ShortRegistrationActivity(), PREREG_MEDICALRECORDNUMBER, PREREG_ACCOUNTNUMBER);
            account.KindOfVisit = VisitType.PreRegistration;
            account.Activity.AssociatedActivityType = typeof (ActivatePreRegistrationActivity);
            Assert.AreEqual("0", account.KindOfVisit.Code, "preadmit kind of visit is expected.");
            Account selectedAccount = AccountActivityService.SelectedAccountFor(account);
            Assert.IsNotNull(selectedAccount);
            Assert.IsNotNull(selectedAccount.AdmittingPhysician);
            Assert.AreEqual("", selectedAccount.KindOfVisit.Code, "kind of visit should be cleared.");
        }
        private Account GetAccount( Activity activity , long medicalRecordNumber, long accountNumber )
        {
            var patient = new Patient { MedicalRecordNumber = medicalRecordNumber };
            var account = new Account
            {
                Facility = facility,
                AccountNumber = accountNumber,
                Activity = activity ,
                KindOfVisit =
                    new VisitType(0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC,
                                  VisitType.OUTPATIENT),
                Patient = patient
            };

            User.GetCurrent().Facility = facility;
            User.GetCurrent().PBAREmployeeID = "PACCUSER";
            User.GetCurrent().WorkstationID = "WorkstationID";
            account.Activity.AppUser = User.GetCurrent();
            return account;
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private Facility facility  = new Facility(PersistentModel.NEW_OID,
                                                 PersistentModel.NEW_VERSION,
                                                 "DOCTORS HOSPITAL DALLAS",
                                                 "DHF");
        #endregion
    }
}