using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Auditing.FusNotes.Formatters;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category("Fast")]
    public class RGAPMFusNoteFormatterTests
    {
        #region Constants
        private const int PARAM_OID = 0;
        #endregion

        #region SetUp and TearDown RGAPMFusNoteFormatterTests

        #endregion

        #region Test Methods

        [Test]
        public void TestFormat_WhenActivityCodeIsRGAPM()
        {
            //create input to the system under test(the formatter)
            var anAccount = new Account
            {
                Guarantor = new Guarantor(),
               OldGuarantorCellPhoneConsent = previousCellPhoneConsent
            };
            var contactPoint = new ContactPoint(TypeOfContactPoint.NewMobileContactPointType())
            {
                CellPhoneNumber = cellPhoneNumber,
                CellPhoneConsent = cellPhoneConsent
            };
            anAccount.Guarantor.AddContactPoint(contactPoint);


            const string activityCode = "RGAPM";
            var noteToFormat = GetFusNoteFor(activityCode, anAccount);

            //setup the system under test for testing
            var formatterUnderTest = new RGAPMFUSNoteFormatter { Context = noteToFormat };

            //exercise the system under test
            var formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual(formattedNotes[0], CONSENT_FUS_NOTE_FORMAT, String.Format("Incorrect message in the fus note for {0}", activityCode));
        }
        #endregion

        #region Support Methods

        private static FusNote GetFusNoteFor(string activityCode, Account anAccount)
        {
            var fusActivity = new FusActivity {Code = activityCode};
            return new FusNote(anAccount, null, fusActivity);
        }

        #endregion

        #region Data Elements

        private readonly PhoneNumber cellPhoneNumber = new PhoneNumber("987213654");

        private readonly CellPhoneConsent cellPhoneConsent = new CellPhoneConsent(PARAM_OID, DateTime.Now,
            CellPhoneConsent.VERBAL_CONSENT_DESCRIPTION,
            CellPhoneConsent.VERBAL_CONSENT);

         private readonly CellPhoneConsent previousCellPhoneConsent = new CellPhoneConsent(PARAM_OID, DateTime.Now,
            CellPhoneConsent.WRITTEN_CONSENT_DESCRIPTION,
            CellPhoneConsent.WRITTEN_CONSENT);
       
        #endregion

        # region Constants

        private const string CONSENT_FUS_NOTE_FORMAT = "Guar Cell Consent Flg Fr: W To: V" ;

        #endregion
    }
}