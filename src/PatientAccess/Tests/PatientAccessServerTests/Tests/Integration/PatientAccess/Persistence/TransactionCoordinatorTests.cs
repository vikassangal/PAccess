using System;
using Extensions;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TransactionCoordinatorTests.
    /// </summary>

    //TODO: Create XML summary comment for TransactionCoordinatorTests
    [TestFixture]
    [Category( "Fast" )]
    public class TransactionCoordinatorTests
    {

        #region Constants

        #endregion

        #region SetUp and TearDown TransactionCoordinatorTests
        [SetUp]
        public void SetUpTransactionCoordinatorTests()
        {
            this.UnitUnderTest = TransactionCoordinator.TransactionCoordinatorFor( new RegistrationActivity() );
            Model.IsTrackingEnabled = true;
        }

        [TearDown]
        public void TearDownTransactionCoordinatorTests()
        {
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestCreateCoordinators()
        {
            Activity activity = new CancelInpatientDischargeActivity();
            TransactionCoordinator txncoord = 
                TransactionCoordinator.TransactionCoordinatorFor(activity);

            Assert.IsNotNull(txncoord,"Cancel Discharge TxnCoordinator not created");
            Assert.AreEqual(typeof(CancelDischargeTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to create Cancel Discharge TXN Coordinator");

            activity = new CancelPreRegActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"Cancel preReg TxnCoordinator not created");
            Assert.AreEqual(typeof(CancelPreRegTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to create Cancel PreReg TXN Coordinator");

            activity = new EditDischargeDataActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"Edit Discharge TxnCoordinator not created");
            Assert.AreEqual(typeof(EditDischargeInfoTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to Edit Discharge TXN Coordinator");

            activity = new DischargeActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"Edit Discharge TxnCoordinator not created");
            Assert.AreEqual(typeof(DischargeTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to Discharge TXN Coordinator");

            activity = new MaintenanceActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"Maintenance TxnCoordinator not created");
            Assert.AreEqual(typeof(MaintenanceTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to Maintenance TXN Coordinator");

            activity = new PreMSERegisterActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"PreMSE TxnCoordinator not created");
            Assert.AreEqual(typeof(PreMSETransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to pre MSE TXN Coordinator");

            activity = new RegistrationActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"registration TxnCoordinator not created");
            Assert.AreEqual(typeof(RegistrationTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to Registration TXN Coordinator");

            activity = new PreRegistrationActivity();
            txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            Assert.IsNotNull(txncoord,"pre registration TxnCoordinator not created");
            Assert.AreEqual(typeof(PreregistrationTransactionCoordinator),txncoord.GetType(), 
                            "In correct type return to Pre Registration TXN Coordinator");
        }
        [Test()]
        public void Test_InsertConditionOfServiceSignedFusNote_ForRPUTS()
        {

            Account anAccount = new Account();
            ConditionOfService cosUnable = new ConditionOfService( 0, DateTime.Now, ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE );
            anAccount.COSSigned = cosUnable;
            this.UnitUnderTest.InsertConditionOfServiceSignedFusNote( anAccount );
            FusNote RPUTSFusNote = null;
            FusNote IICOSFusNote = null;
            foreach( FusNote fusNote in anAccount.FusNotes )
            {
                if( fusNote.FusActivity.Code == "RPUTS" )
                {
                    RPUTSFusNote = fusNote;
                }
                if( fusNote.FusActivity.Code == "IICOS" )
                {
                    IICOSFusNote = fusNote;
                }
            }
            Assert.IsNotNull( RPUTSFusNote, "Invalid Fus Note, RPUTS Fus note should be generated" );
            Assert.IsNotNull( IICOSFusNote, "Invalid Fus Note, IICOS Fus noteShould be generated" );
        }
        [Test()]
        public void Test_InsertConditionOfServiceSignedFusNote_ForRPRTS()
        {

            Account anAccount = new Account();
            ConditionOfService cosRefused = new ConditionOfService( 0, DateTime.Now, ConditionOfService.REFUSED_DESCRIPTION, ConditionOfService.REFUSED );
            anAccount.COSSigned = cosRefused;
            this.UnitUnderTest.InsertConditionOfServiceSignedFusNote( anAccount );
            FusNote RPRTSFusNote = null;
            FusNote IICOSFusNote = null;
            foreach( FusNote fusNote in anAccount.FusNotes )
            {
                if( fusNote.FusActivity.Code == "RPRTS" )
                {
                    RPRTSFusNote = fusNote;
                }
                if( fusNote.FusActivity.Code == "IICOS" )
                {
                    IICOSFusNote = fusNote;
                }
            }
            Assert.IsNotNull( RPRTSFusNote, "Invalid Fus Note, RPRTS Fus note should be generated" );
            Assert.IsNotNull( IICOSFusNote, "Invalid Fus Note, IICOS Fus note Should be generated" );
        }
        [Test()]
        public void Test_InsertConditionOfServiceSignedFusNote_ForRPNAS()
        {

            Account anAccount = new Account();
            ConditionOfService cosNA = new ConditionOfService( 0, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );
            anAccount.COSSigned = cosNA;
            this.UnitUnderTest.InsertConditionOfServiceSignedFusNote( anAccount );
            FusNote RPNASFusNote = null;
            FusNote IICOSFusNote = null;
            foreach( FusNote fusNote in anAccount.FusNotes )
            {
                if( fusNote.FusActivity.Code == "RPNAS" )
                {
                    RPNASFusNote = fusNote;
                }
                if( fusNote.FusActivity.Code == "IICOS" )
                {
                    IICOSFusNote = fusNote;
                }
            }
            Assert.IsNotNull( RPNASFusNote, "Invalid Fus Note, RPNAS Fus note Should be generated" );
            Assert.IsNotNull( IICOSFusNote, "Invalid Fus Note, IICOS Fus note Should be generated" );
        }
        [Test()]
        public void Test_InsertConditionOfServiceSignedFusNote_ForICOSC()
        {

            Account anAccount = new Account();
            ConditionOfService cosYes = new ConditionOfService( 0, DateTime.Now, ConditionOfService.YES_DESCRIPTION, ConditionOfService.YES );
            anAccount.COSSigned = cosYes;
            this.UnitUnderTest.InsertConditionOfServiceSignedFusNote( anAccount );
            FusNote ICOSCFusNote = null;
            foreach( FusNote fusNote in anAccount.FusNotes )
            {
                if( fusNote.FusActivity.Code == "ICOSC" )
                {
                    ICOSCFusNote = fusNote;
                }
            }
            Assert.IsNotNull( ICOSCFusNote, "Invalid Fus Note, ICOSC Fus note should be generated" );
        }
        [Test()]
        public void Test_InsertConditionOfServiceSignedFusNote_ForBlank()
        {

            Account anAccount = new Account();
            ConditionOfService cosBlank = new ConditionOfService( 0, DateTime.Now, ConditionOfService.BLANK, ConditionOfService.BLANK );
            anAccount.COSSigned = cosBlank;
            this.UnitUnderTest.InsertConditionOfServiceSignedFusNote( anAccount );
            FusNote BlankFusNote = null;
            foreach( FusNote fusNote in anAccount.FusNotes )
            {
                if( fusNote.FusActivity.Code == "RPRTS" ||
                    fusNote.FusActivity.Code == "RPUTS" ||
                    fusNote.FusActivity.Code == "RPNAS" ||
                    fusNote.FusActivity.Code == "IICOS" ||
                    fusNote.FusActivity.Code == "ICOSC"
                    )
                {
                    BlankFusNote = fusNote;
                }
            }
            //No fus notes will be generated when COS value is Blank
            Assert.IsNull( BlankFusNote, "Invalid Fus Note, No fus notes should be generated when COS value is blank" );
        }

        [Test,Sequential]
        [Category( "Fast" )]
        public void Test_InsertNppSignedFusNote_GenerateINPOFDependOnSignatureStatus(
            [Values("S","U")] String signatureStatusCode,
            [Values(true,false)] bool inpofGenerated)
        {
            NoticeOfPrivacyPracticeDocument doc = new NoticeOfPrivacyPracticeDocument
                                                      {
                                                          SignatureStatus = new SignatureStatus(signatureStatusCode),
                                                          SignedOnDate = DateTime.Now
                                                      };
            Account anAccount = new Account
                                    {
                                        Patient = new Patient { NoticeOfPrivacyPracticeDocument = doc }
                                    };
            
            this.UnitUnderTest.InsertNppSignedFusNote(anAccount);
            
            bool inpofFound = false;
            
            foreach ( FusNote fusNote in anAccount.FusNotes )
            {
                if ( fusNote.FusActivity.Code == "INPOF" )
                {
                    inpofFound = true;
                }
            }
            Assert.IsTrue(inpofFound == inpofGenerated, string.Format("INPOF Fus note should {0}be generated",inpofGenerated?"":"not " ));
        }

        [Test, Sequential]
        [Category( "Fast" )]
        public void Test_InsertNppSignedFusNote_GenerateINPOFDependOnSigndOnDate(
            [Values( "04/23/2012", "01/01/0001" )] DateTime signedDate,
            [Values( true, false )] bool inpofGenerated )
        {
            NoticeOfPrivacyPracticeDocument doc = new NoticeOfPrivacyPracticeDocument
            {
                SignatureStatus = new SignatureStatus( "S" ),
                SignedOnDate = signedDate
            };
            Account anAccount = new Account
            {
                Patient = new Patient { NoticeOfPrivacyPracticeDocument = doc }
            };

            this.UnitUnderTest.InsertNppSignedFusNote( anAccount );

            bool inpofFound = false;

            foreach ( FusNote fusNote in anAccount.FusNotes )
            {
                if ( fusNote.FusActivity.Code == "INPOF" )
                {
                    inpofFound = true;
                }
            }
            Assert.IsTrue( inpofFound == inpofGenerated, string.Format( "INPOF Fus note should {0}be generated", inpofGenerated ? "" : "not " ) );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private TransactionCoordinator UnitUnderTest { get; set; }
        #endregion
    }
}