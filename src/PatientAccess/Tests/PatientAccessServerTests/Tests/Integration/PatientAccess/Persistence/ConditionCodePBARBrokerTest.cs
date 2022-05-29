using System;
using System.Collections.Generic;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class ConditionCodePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants

        private static readonly DateTime FeatureStartDate = DateTime.Today.AddDays( -5 );
        private readonly DateTime AdmitDateAfterFeatureStartDate = FeatureStartDate.AddDays( 10 );
        private readonly DateTime AdmitDateBeforeFeatureStartDate = FeatureStartDate.AddDays( -20 );

        #endregion

        #region Test Methods

        [Test]
        public void TestAllSelectableConditionCodes()
        {
            Assert.IsTrue( MasterList.Count > 0, "No condition code found" );
        }

        [Test]
        public void TestConditionCodesForValid()
        {
            const string validCode = "A0";
            var conditionCode = broker.ConditionCodeWith( ACO_FACILITYID, validCode );

            Assert.AreEqual( validCode, conditionCode.Code, "Code  should be A0" );

            Assert.AreEqual( "TRICARE EXT PARTNER PGM", conditionCode.Description, "Description should be - TRICARE EXT PARTNER PGM" );

            Assert.IsTrue( conditionCode.IsValid );
        }

        [Test]
        public void TestConditionCodesForBlank()
        {
            string blank = String.Empty;
            var conditionCode = broker.ConditionCodeWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, conditionCode.Code, "Code  should be blank" );
            Assert.AreEqual( blank, conditionCode.Description, "Description should be blank" );
            Assert.IsTrue( conditionCode.IsValid );
        }

        [Test]
        public void TestConditionCodesFor90()
        {
            const string Code90 = "90";
            const string Code90_Description = "EXPANDED ACCESS APPROVAL";
            var conditionCode = broker.ConditionCodeWith(ACO_FACILITYID, Code90);

            Assert.AreEqual(Code90, conditionCode.Code, "Code  should be 90 ");
            Assert.AreEqual(Code90_Description, conditionCode.Description, "Description is Invalid");
            Assert.IsTrue( conditionCode.IsValid );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestConditionCodesForNull()
        {
            broker.ConditionCodeWith( ACO_FACILITYID, null );
        }

        [Test]
        public void TestFilterSelectableConditionCodes_ForInpatient_AndAdmitDateGreaterThanFeatureDate_ShouldReturnP7InList()
        {
            bool p7ExistsInMasterList = MasterList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInMasterList, "P7 Condition Code does not exist in the master condition codes list." );

            account = new Account
            {
                KindOfVisit = VisitType.Inpatient,
                AdmitDate = AdmitDateAfterFeatureStartDate,
                Facility = new Facility { Oid = 900 }
            };

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            Manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

            var filteredList = Manager.FilterConditionCodesMasterList( MasterList );

            bool p7ExistsInFilteredList = filteredList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInFilteredList, "P7 Condition Code does not exist in the filtered condition codes list." );
        }

        [Test]
        public void TestFilterSelectableConditionCodes_ForInpatient_AndAdmitDateLesserThanFeatureDate_ShouldNotReturnP7InList()
        {
            bool p7ExistsInMasterList = MasterList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInMasterList, "P7 Condition Code does not exist in the master condition codes list." );

            account = new Account
            {
                KindOfVisit = VisitType.Inpatient,
                AdmitDate = AdmitDateBeforeFeatureStartDate,
                Facility = new Facility { Oid = 900 }
            };

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            Manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

            var filteredList = Manager.FilterConditionCodesMasterList( MasterList );

            bool p7ExistsInFilteredList = filteredList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsFalse( p7ExistsInFilteredList, "P7 Condition Code should not exist in the filtered condition codes list." );
        }

        [Test]
        public void TestFilterSelectableConditionCodes_ForNullPatientType_ShouldThrowArgumentNullException()
        {
            bool p7ExistsInMasterList = MasterList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInMasterList, "P7 Condition Code does not exist in the master condition codes list." );

            account = new Account
            {
                KindOfVisit = null,
                AdmitDate = AdmitDateAfterFeatureStartDate,
                Facility = new Facility { Oid = 900 }
            };

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();

            Manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

            var filteredList = Manager.FilterConditionCodesMasterList( MasterList );

            bool p7ExistsInFilteredList = filteredList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsFalse( p7ExistsInFilteredList, "P7 Condition Code should not exist in the filtered condition codes list." );
        }

        [Test]
        public void TestFilterSelectableConditionCodes_ForNonInpatient_AndAdmitDateLesserThanFeatureDate_ShouldNotReturnP7InList()
        {
            bool p7ExistsInMasterList = MasterList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInMasterList, "P7 Condition Code does not exist in the master condition codes list." );
            
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();

            account = new Account
            {
                AdmitDate = AdmitDateBeforeFeatureStartDate,
                Facility = new Facility { Oid = 900 }
            };

            foreach ( VisitType patientType in PatientTypes )
            {

                account.KindOfVisit = patientType;

                Manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

                var filteredList = Manager.FilterConditionCodesMasterList( MasterList );

                bool p7ExistsInFilteredList = filteredList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

                Assert.IsFalse( p7ExistsInFilteredList, "P7 Condition Code should not exist in the filtered condition codes list." );
            }
        }

        [Test]
        public void TestFilterSelectableConditionCodes_ForNonInpatient_AndAdmitDateGreaterThanFeatureDate_ShouldNotReturnP7InList()
        {
            bool p7ExistsInMasterList = MasterList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

            Assert.IsTrue( p7ExistsInMasterList, "P7 Condition Code does not exist in the master condition codes list." );
            account = new Account
            {
                AdmitDate = AdmitDateAfterFeatureStartDate,
                Facility = new Facility { Oid = 900 }
            };

            foreach ( VisitType patientType in PatientTypes )
            {

                account.KindOfVisit = patientType;
                var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();

                Manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

                var filteredList = Manager.FilterConditionCodesMasterList( MasterList );

                bool p7ExistsInFilteredList = filteredList.Any( x => x.Code == ConditionCode.ADMITTED_AS_IP_FROM_ER );

                Assert.IsFalse( p7ExistsInFilteredList, "P7 Condition Code should not exist in the filtered condition codes list." );
            }
        }

        #endregion

        #region Properties

        private IList<ConditionCode> MasterList { get; set; }

        #endregion

        #region Data Elements
        private static IConditionCodeBroker broker;
        private Account account = new Account();

        private readonly VisitType[] PatientTypes = 
                                            { VisitType.Outpatient,
                                              VisitType.NonPatient,
                                              VisitType.Recurring,
                                              VisitType.PreRegistration,
                                              VisitType.Emergency,
                                              new VisitType()
                                            };

        #endregion

        #region Construction
        public ConditionCodePBARBrokerTests()
        {
            broker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            MasterList = broker.AllSelectableConditionCodes( ACO_FACILITYID ).Cast<ConditionCode>().ToList();
        }

        private EmergencyToInPatientTransferCodeManager Manager { get; set; }

        #endregion
    }
}