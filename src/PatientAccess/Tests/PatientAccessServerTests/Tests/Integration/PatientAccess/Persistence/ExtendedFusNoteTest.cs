using System;
using System.Data;
using System.Configuration;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
	[Ignore()]
    public class ExtendedFusNoteTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown BedTests
        [TestFixtureSetUp()]
        public static void SetUpFusNoteTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            ;
            i_ACOFacility = fb.FacilityWith( ACO_FACILITYID );
              
            dbConnection = new iDB2Connection();
            dbConnection.ConnectionString = i_ACOFacility.ConnectionSpec.ConnectionString;
            dbConnection.Open(); 
        }

        [TestFixtureTearDown()]
        public static void TearDownFusNoteTests()
        {
            
            if( dbConnection != null )
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
        #endregion

        #region Test Methods
        [Test()]
        [Ignore()]
        public void TestCREFP()
        {
            try
            {
                
                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.Payment = new Payment();
                anAccount.Payment.ZeroPaymentReason = new ZeroPaymentReason( 0, DateTime.Now, "REFUSED TO PAY" );
                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
             
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote CREFPFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "CREFP" )
                    {
                        CREFPFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( CREFPFusNote, "Invalid Fus Note" );
            }
            catch(Exception ex)
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        [Test()]
        [Ignore()]
        public void TestRUPPD()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.TotalCurrentAmtDue = 100;
                Coverage coverage = new SelfPayCoverage();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
                anAccount.Insurance.AddCoverage( coverage );
                anAccount.Insurance.SetAsPrimary(coverage);
                dbTransaction = dbConnection.BeginTransaction();
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RUPPDFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RUPPD" )
                    {
                        RUPPDFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RUPPDFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        [Test()]
        [Ignore()]
        public void TestRFCMO()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.Payment = new Payment();
                anAccount.Payment.IsCurrentAccountPayment = true;

                anAccount.OriginalMonthlyPayment = 100;
                anAccount.MonthlyPayment = 110;
                
                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );

                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RFCMOFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RFCMO" )
                    {
                        RFCMOFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RFCMOFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }

        [Test()]
        [Ignore()]
        public void TestRNPCA()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.TotalCurrentAmtDue = 100;
                Coverage coverage = new CommercialCoverage();
                Authorization authorization = new Authorization();
                authorization.AuthorizationRequired.Code = YesNotApplicableFlag.CODE_NOTAPPLICABLE;

                ( (CoverageGroup)coverage ).Authorization = authorization;
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
                anAccount.Insurance.AddCoverage( coverage );
                anAccount.Insurance.SetAsPrimary( coverage );
                dbTransaction = dbConnection.BeginTransaction();
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RNPCAFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RNPCA" )
                    {
                        RNPCAFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RNPCAFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }

        [Test()]
        [Ignore()]
        public void TestRNPCAAndIRATA()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.TotalCurrentAmtDue = 100;
                Coverage coverage = new CommercialCoverage();
                Authorization authorization = new Authorization();
                authorization.AuthorizationRequired.Code = YesNotApplicableFlag.CODE_NOTAPPLICABLE;
                authorization.AuthorizationStatus.Code ="A";
                ( (CoverageGroup)coverage ).Authorization = authorization;
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
                anAccount.Insurance.AddCoverage( coverage );
                anAccount.Insurance.SetAsPrimary( coverage );
                dbTransaction = dbConnection.BeginTransaction();
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RNPCAFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RNPCA" )
                    {
                        RNPCAFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RNPCAFusNote, "Invalid Fus Note" );

                FusNote IRATAFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "IRATA" )
                    {
                        IRATAFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( IRATAFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }

        [Test()]
        [Ignore()]
        public void TestInsuranceVerificationFusNotes()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.TotalCurrentAmtDue = 100;
                Coverage coverage = new CommercialCoverage();
                coverage.WriteVerificationEntryFUSNote = true;
                coverage.InformationReceivedSource = new InformationReceivedSource( 0, DateTime.Now, "Phone Verification", InformationReceivedSource.PHONE_VERIFICATION_OID.ToString() );
                Coverage secondaryCoverage = new CommercialCoverage();
                secondaryCoverage.WriteVerificationEntryFUSNote = true;
                secondaryCoverage.InformationReceivedSource = new InformationReceivedSource( 0, DateTime.Now, "Phone Verification", InformationReceivedSource.PHONE_VERIFICATION_OID.ToString() );
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
                anAccount.Insurance.AddCoverage( coverage );
                anAccount.Insurance.SetAsPrimary( coverage );
                anAccount.Insurance.AddCoverage( secondaryCoverage );
                anAccount.Insurance.SetAsSecondary( secondaryCoverage );
                dbTransaction = dbConnection.BeginTransaction();
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RINVBFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RVINB" )
                    {
                        RINVBFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RINVBFusNote, "Invalid Fus Note" );

                FusNote RINVSFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RVINS" )
                    {
                        RINVSFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RINVSFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }

        [Test()]
        [Ignore()]
        public void TestIICOS()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.COSSigned = new ConditionOfService( 0, DateTime.Now, ConditionOfService.YES_DESCRIPTION, ConditionOfService.YES );
                
                
                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );

                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote IICOSFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "IICOS" )
                    {
                        IICOSFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( IICOSFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        [Test()]
        [Ignore()]
        public void TestRSSVS()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.ValuablesAreTaken = new YesNoFlag( "Y" );


                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );

                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote RSSVSFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "RSSVS" )
                    {
                        RSSVSFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( RSSVSFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        [Test()]
        [Ignore()]
        public void TestIHIPP()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.OptOutHealthInformation = true;

                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );

                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote IHIPPFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "IHIPP" )
                    {
                        IHIPPFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( IHIPPFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        [Test()]
        [Ignore()]
        public void TestINPOF()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.Patient = new Patient();
                anAccount.Patient.NoticeOfPrivacyPracticeDocument = new NoticeOfPrivacyPracticeDocument();
                anAccount.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus( SignatureStatus.SIGNED ); 
                
                dbTransaction = dbConnection.BeginTransaction();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );

                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote INPOFFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "INPOF" )
                    {
                        INPOFFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( INPOFFusNote, "Invalid Fus Note" );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        
        [Test()]
        [Ignore()]
        public void TestTenetPlanFusNotes()
        {
            try
            {

                Account anAccount = new Account();
                anAccount.Facility = i_ACOFacility;
                anAccount.AccountNumber = 461517;
                anAccount.TotalCurrentAmtDue = 100;
                Coverage coverage = new CommercialCoverage();
                InsurancePBARBroker insurancePBARBroker = new InsurancePBARBroker();
                CommercialInsurancePlan plan = new CommercialInsurancePlan();
                plan.Payor = new Payor();
                plan.Payor.Code = "LO9";
                plan.PlanSuffix = "3Q";

                coverage.InsurancePlan = plan;
                Coverage secondaryCoverage = new CommercialCoverage();
                Activity activity = new RegistrationActivity();
                TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
                anAccount.Insurance.AddCoverage( coverage );
                anAccount.Insurance.SetAsPrimary( coverage );
                anAccount.Insurance.AddCoverage( secondaryCoverage );
                anAccount.Insurance.SetAsSecondary( secondaryCoverage );
                dbTransaction = dbConnection.BeginTransaction();
                txncoord.Account = anAccount;
                txncoord.AppUser.PBAREmployeeID = "PACCESS";
                txncoord.WriteFUSNotesForAccount();
                FusNote REIFAFusNote = null;
                foreach( FusNote fusNote in anAccount.FusNotes )
                {
                    if( fusNote.FusActivity.Code == "REIFA" )
                    {
                        REIFAFusNote = fusNote;
                    }
                }
                Assert.IsNotNull( REIFAFusNote, "Invalid Fus Note" );

              
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
       
        private static Facility i_ACOFacility;
        private static IDbConnection dbConnection;
        private static IDbTransaction dbTransaction;
       
        #endregion
    }
}