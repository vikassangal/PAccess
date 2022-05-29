using System;
using System.Linq;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Persistence;

namespace Tests.Unit.PatientAccess.Persistence
{
    [TestFixture()]
    public class RFCMOFusNoteTest
    {
        [Test()]
        public void TestInsertFinancialFUSNotesInto_WhenNumberOfMonthlyPaymentsIsLessThan4_ShouldNotGenerateRFCMOFUSNote()
        {
            try
            {
                var anAccount = new Account
                {
                    AccountNumber = 461517,
                    Payment = new Payment { IsCurrentAccountPayment = true },
                    OriginalMonthlyPayment = 100,
                    MonthlyPayment = 110,
                    NumberOfMonthlyPayments = 2
                };

                var activity = new RegistrationActivity();
                anAccount.Activity = activity;
                var txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);

                txncoord.InsertFinancialFUSNotesInto(anAccount);

                var RFCMOFusNoteIsGenerated = anAccount.FusNotes.Cast<FusNote>().Any(fusNote => fusNote.FusActivity.Code == "RFCMO");

                Assert.IsFalse(RFCMOFusNoteIsGenerated, "RFCMO Fus Note should not be generated");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Test()]
        public void TestInsertFinancialFUSNotesInto_WhenNumberOfMonthlyPaymentsIsMoreThan3_ShouldGenerateRFCMOFUSNote()
        {
            try
            {
                var anAccount = new Account
                {
                    AccountNumber = 461517,
                    Payment = new Payment {IsCurrentAccountPayment = true},
                    OriginalMonthlyPayment = 100,
                    MonthlyPayment = 110,
                    NumberOfMonthlyPayments = 6
                };

                var activity = new RegistrationActivity();
                anAccount.Activity = activity;
                var txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);

                txncoord.InsertFinancialFUSNotesInto(anAccount);

                var RFCMOFusNoteIsGenerated =
                    anAccount.FusNotes.Cast<FusNote>().Any(fusNote => fusNote.FusActivity.Code == "RFCMO");

                Assert.IsTrue(RFCMOFusNoteIsGenerated, "RFCMO Fus Note should be generated");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
