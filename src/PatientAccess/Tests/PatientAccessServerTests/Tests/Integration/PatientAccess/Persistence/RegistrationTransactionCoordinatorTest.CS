using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    [Category( "Fast" )]
    public class RegistrationTransactionCoordinatorTest
    {
        [Test]
        public void TestInsertStrategies_PreRegistrationFlag_ShouldBeEmpty_NewShortReg()
        {
            Account account = new Account();
            account.IsNew = true;
            account.Activity = new ShortRegistrationActivity();
            
            SelfPayCoverage coverage = new SelfPayCoverage();
            //coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            account.Insurance.AddCoverage( coverage);
            account.Insurance.SetAsPrimary( coverage );
            TransactionCoordinator registrationTransactionCoordinator =
                new RegistrationTransactionCoordinator( account );
            registrationTransactionCoordinator.AppUser = User.GetCurrent();
            string preRegistrationFlag = "";
            bool foundFlag = false;
            foreach ( SqlBuilderStrategy strategy in registrationTransactionCoordinator.InsertStrategies )
            {
                if ( strategy != null && strategy.GetType() == typeof( InsuranceInsertStrategy ) )
                {
                    preRegistrationFlag = ((InsuranceInsertStrategy) strategy).PreRegistrationFlag;
                    foundFlag = true;
                }
            }
            Assert.AreEqual(foundFlag, true, "InsertInsuranceStrategy should be found.");
            Assert.AreEqual( preRegistrationFlag, "", "PreRegistrationFlag should be empty." );

        }

        [Test()]
        public void TestInsertStrategies_PreRegistrationFlag_ShouldBeEmpty_NewStdReg()
        {
            Account account = new Account();
            account.IsNew = true;
            account.Activity = new RegistrationActivity();

            SelfPayCoverage coverage = new SelfPayCoverage();
            //coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            account.Insurance.AddCoverage( coverage );
            account.Insurance.SetAsPrimary( coverage );
            TransactionCoordinator registrationTransactionCoordinator =
                new RegistrationTransactionCoordinator( account );
            registrationTransactionCoordinator.AppUser = User.GetCurrent();
            string preRegistrationFlag = "";
            bool foundFlag = false;
            foreach ( SqlBuilderStrategy strategy in registrationTransactionCoordinator.InsertStrategies )
            {
                if ( strategy != null && strategy.GetType() == typeof( InsuranceInsertStrategy ) )
                {
                    preRegistrationFlag = ( (InsuranceInsertStrategy)strategy ).PreRegistrationFlag;
                    foundFlag = true;
                }
            }
            Assert.AreEqual( foundFlag, true, "InsertInsuranceStrategy should be found." );
            Assert.AreEqual( preRegistrationFlag, "", "PreRegistrationFlag should be empty." );

        }

        [Test()]
        public void TestInsertStrategies_PreRegistrationFlag_ShouldBeP_ActivateReg()
        {
            Account account = new Account();
            account.IsNew = true;
            account.Activity = new ActivatePreRegistrationActivity();

            SelfPayCoverage coverage = new SelfPayCoverage();
            //coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            account.Insurance.AddCoverage( coverage );
            account.Insurance.SetAsPrimary( coverage );
            TransactionCoordinator registrationTransactionCoordinator =
                new RegistrationTransactionCoordinator( account );
            registrationTransactionCoordinator.AppUser = User.GetCurrent();
            string preRegistrationFlag = "";
            bool foundFlag = false;
            foreach ( SqlBuilderStrategy strategy in registrationTransactionCoordinator.InsertStrategies )
            {
                if ( strategy != null && strategy.GetType() == typeof( InsuranceInsertStrategy ) )
                {
                    preRegistrationFlag = ( (InsuranceInsertStrategy)strategy ).PreRegistrationFlag;
                    foundFlag = true;
                }
            }
            Assert.AreEqual( foundFlag, true, "InsertInsuranceStrategy should be found." );
            Assert.AreEqual( preRegistrationFlag, "P", "PreRegistrationFlag should be 'p'." );

        }

        [Test()]
        public void TestInsertStrategies_PreRegistrationFlag_ShouldBeP_OldStdReg()
        {
            Account account = new Account();
            account.IsNew = false;
            account.Activity = new ActivatePreRegistrationActivity();

            SelfPayCoverage coverage = new SelfPayCoverage();
            //coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            account.Insurance.AddCoverage( coverage );
            account.Insurance.SetAsPrimary( coverage );
            TransactionCoordinator registrationTransactionCoordinator =
                new RegistrationTransactionCoordinator( account );
            registrationTransactionCoordinator.AppUser = User.GetCurrent();
            string preRegistrationFlag = "";
            bool foundFlag = false;
            foreach ( SqlBuilderStrategy strategy in registrationTransactionCoordinator.InsertStrategies )
            {
                if ( strategy != null && strategy.GetType() == typeof( InsuranceInsertStrategy ) )
                {
                    preRegistrationFlag = ( (InsuranceInsertStrategy)strategy ).PreRegistrationFlag;
                    foundFlag = true;
                }
            }
            Assert.AreEqual( foundFlag, true, "InsertInsuranceStrategy should be found." );
            Assert.AreEqual( preRegistrationFlag, "P", "PreRegistrationFlag should be 'p'." );

        }

        [Test()]
        public void TestInsertStrategies_PreRegistrationFlag_ShouldBeP_OldShortReg()
        {
            Account account = new Account();
            account.IsNew = false ;
            account.Activity = new ActivatePreRegistrationActivity();

            SelfPayCoverage coverage = new SelfPayCoverage();
            //coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            account.Insurance.AddCoverage( coverage );
            account.Insurance.SetAsPrimary( coverage );
            TransactionCoordinator registrationTransactionCoordinator =
                new RegistrationTransactionCoordinator( account );
            registrationTransactionCoordinator.AppUser = User.GetCurrent();
            string preRegistrationFlag = "";
            bool foundFlag = false;
            foreach ( SqlBuilderStrategy strategy in registrationTransactionCoordinator.InsertStrategies )
            {
                if ( strategy != null && strategy.GetType() == typeof( InsuranceInsertStrategy ) )
                {
                    preRegistrationFlag = ( (InsuranceInsertStrategy)strategy ).PreRegistrationFlag;
                    foundFlag = true;
                }
            }
            Assert.AreEqual( foundFlag, true, "InsertInsuranceStrategy should be found." );
            Assert.AreEqual( preRegistrationFlag, "P", "PreRegistrationFlag should be 'p'." );

        }
    }
}
