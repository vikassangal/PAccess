using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Auditing.FusNotes.Formatters;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class COSSignedFusNoteFormatterTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown COSSignedFusNoteFormatterTests
        [TestFixtureSetUp]
        public static void SetUpCOSSignedFusNoteFormatterTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownCOSSignedFusNoteFormatterTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestFormatWhenActivityCodeIsRPUTS()
        {
            //create input to the system under test(the formatter)
            Account anAccount = new Account();
            anAccount.COSSigned = new ConditionOfService( 0, DateTime.Now, ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE );
            const string activityCode = "RPUTS";
            FusNote noteToFormat = GetFusNoteFor( activityCode, anAccount );

            //setup the system under test for testing
            var formatterUnderTest = new COSSignedFUSNoteFormatter();
            formatterUnderTest.Context = noteToFormat;

            //exercise the system under test
            IList formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual( formattedNotes[0], "COS not signed", String.Format( "Incorrect message in the fus note for {0}", activityCode ) );
        }

        [Test]
        public void TestFormatWhenActivityCodeIsRPRTS()
        {
            //create input to the system under test(the formatter)
            Account anAccount = new Account();
            anAccount.COSSigned = new ConditionOfService( 0, DateTime.Now, ConditionOfService.REFUSED_DESCRIPTION, ConditionOfService.REFUSED );
            const string activityCode = "RPRTS";
            FusNote noteToFormat = GetFusNoteFor( activityCode, anAccount );

            //setup the system under test for testing
            var formatterUnderTest = new COSSignedFUSNoteFormatter();
            formatterUnderTest.Context = noteToFormat;

            //exercise the system under test
            IList formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual( formattedNotes[0], "COS not signed", String.Format( "Incorrect message in the fus note for {0}", activityCode ) );
        }
        [Test]
        public void TestFormatWhenActivityCodeIsRPNAS()
        {
            Account anAccount = new Account();
            anAccount.COSSigned = new ConditionOfService( 0, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );
            const string activityCode = "RPNAS";
            FusNote noteToFormat = GetFusNoteFor( activityCode, anAccount );
            
            //setup the system under test for testing
            var formatterUnderTest = new COSSignedFUSNoteFormatter();
            formatterUnderTest.Context = noteToFormat;
           
            //exercise the system under test
            IList formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual( formattedNotes[0], "COS not signed", String.Format( "Incorrect message in the fus note for {0}", activityCode ) );

        }
        [Test]
        public void TestFormatWhenActivityCodeIsIICOS()
        {
            Account anAccount = new Account();
            anAccount.COSSigned = new ConditionOfService(0, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE);
            const string activityCode = "IICOS";
            FusNote noteToFormat = GetFusNoteFor( activityCode, anAccount );

            //setup the system under test for testing
            var formatterUnderTest = new COSSignedFUSNoteFormatter();
            formatterUnderTest.Context = noteToFormat;

            //exercise the system under test
            IList formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual( formattedNotes[0], "COS SIGNED:No, Patient Not Available to Sign ", String.Format( "Incorrect message in the fus note for {0}", activityCode ) );
        }
        [Test]
        public void TestFormatWhenActivityCodeIsICOSC()
        {
            
            //create input to the system under test(the formatter)
            Account anAccount = new Account();
            anAccount.COSSigned = new ConditionOfService(0, DateTime.Now, ConditionOfService.YES_DESCRIPTION, ConditionOfService.YES);
            const string activityCode = "ICOSC";
            FusNote noteToFormat = GetFusNoteFor(activityCode, anAccount);

            //setup the system under test for testing
            var formatterUnderTest = new COSSignedFUSNoteFormatter();
            formatterUnderTest.Context = noteToFormat;
            //exercise the system under test
            IList formattedNotes = formatterUnderTest.Format();

            //verify that the system under test behaved the way we expected it too
            Assert.AreEqual(formattedNotes[0], "COS SIGNED:Yes ", String.Format("Incorrect message in the fus note for {0}", activityCode));
        }

        #endregion

        #region Support Methods

        private static FusNote GetFusNoteFor(string activityCode, Account anAccount)
        {
            FusActivity fusActivity = new FusActivity();
            fusActivity.Code = activityCode;
            return new FusNote(anAccount, null, fusActivity);
        }
      

        #endregion

        #region Data Elements
 
        #endregion
    }
}