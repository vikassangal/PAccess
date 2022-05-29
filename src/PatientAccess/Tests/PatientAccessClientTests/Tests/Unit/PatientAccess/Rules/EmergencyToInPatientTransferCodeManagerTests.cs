using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    public class EmergencyToInPatientTransferCodeManagerTests
    {
        #region Constants

        private static readonly DateTime FeatureStartDate = DateTime.Today.AddDays( -5 );
        private readonly DateTime AdmitDateAfterFeatureStartDate = FeatureStartDate.AddDays( 10 );
        private readonly DateTime AdmitDateBeforeFeatureStartDate = FeatureStartDate.AddDays( -20 );

        #endregion


        #region Test Methods

        [Test]
        public void TestUpdateConditionCodes_AdmitDateGreaterThanFeatureDate_AndERPatient_ShouldAddCode()
        {
            var manager = GetManagerWithAccount( VisitType.Emergency, AdmitDateAfterFeatureStartDate, new TransferOutToInActivity(), true );

            var numberOfConditionCodesBefore = manager.Account.ConditionCodes.Count;
            manager.UpdateConditionCodes();
            var numberOfConditionCodesAfter = manager.Account.ConditionCodes.Count;

            Assert.IsTrue( numberOfConditionCodesAfter > numberOfConditionCodesBefore, "Condition code should be added to the account" );
        }

        [Test]
        public void TestUpdateConditionCodes_AdmitDateGreaterThanFeatureDate_AndInpatient_ShouldAddCode()
        {
            var manager = GetManagerWithAccount( VisitType.Inpatient, AdmitDateAfterFeatureStartDate, new TransferOutToInActivity(), false );
            var numberOfConditionCodesBefore = manager.Account.ConditionCodes.Count;
            manager.UpdateConditionCodes();
            var numberOfConditionCodesAfter = manager.Account.ConditionCodes.Count;

            Assert.IsFalse( numberOfConditionCodesAfter > numberOfConditionCodesBefore, "Condition code should not be added to the account" );

        }

        [Test]
        public void TestUpdateConditionCodes_AdmitDateLesserThanFeatureDate_AndERPatient_ShouldNotAddCode()
        {
            var manager = GetManagerWithAccount( VisitType.Emergency, AdmitDateBeforeFeatureStartDate, new TransferOutToInActivity(), true );

            var numberOfConditionCodesBefore = manager.Account.ConditionCodes.Count;
            manager.UpdateConditionCodes();
            var numberOfConditionCodesAfter = manager.Account.ConditionCodes.Count;

            Assert.IsFalse( numberOfConditionCodesAfter > numberOfConditionCodesBefore, "Condition code should not be added to the account" );
        }

        [Test]
        public void TestUpdateConditionCodes_AdmitDateIsMinValue_AndWasERPatient_ShouldAddCode()
        {
            var manager = GetManagerWithAccount( VisitType.Emergency, DateTime.MinValue, new TransferOutToInActivity(), true );

            var numberOfConditionCodesBefore = manager.Account.ConditionCodes.Count;
            manager.UpdateConditionCodes();
            var numberOfConditionCodesAfter = manager.Account.ConditionCodes.Count;

            Assert.IsTrue( numberOfConditionCodesAfter > numberOfConditionCodesBefore, "Condition code should be added to the account" );
        }


        [Test]
        public void TestShouldCodeBeRemoved_AdmitDateGreaterThanFeatureDate_AndIsERPatient_ReturnTrue()
        {
            var manager = GetManagerWithAccount( VisitType.Emergency, AdmitDateAfterFeatureStartDate );

            bool removeCode = manager.ShouldCodeBeRemoved();
            Assert.IsTrue( removeCode );
        }

        [Test]
        public void TestShouldCodeBeRemoved_AdmitDateGreaterThanFeatureDate_AndIsInPatient_ReturnFalse()
        {
            var manager = GetManagerWithAccount( VisitType.Inpatient, AdmitDateAfterFeatureStartDate );

            bool removeCode = manager.ShouldCodeBeRemoved();
            Assert.IsFalse( removeCode );
        }

        [Test]
        public void TestShouldCodeBeRemoved_AdmitDateGreaterThanFeatureDate_PatientTypeIsEmpty_ReturnFalse()
        {
            var manager = GetManagerWithAccount( new VisitType(), AdmitDateAfterFeatureStartDate );

            bool removeCode = manager.ShouldCodeBeRemoved();
            Assert.IsTrue( removeCode );
        }

        [Test]
        public void TestShouldCodeBeRemoved_AdmitDateLesserThanFeatureDate_AndIsInPatient_ReturnTrue()
        {
            var manager = GetManagerWithAccount( VisitType.Inpatient, AdmitDateBeforeFeatureStartDate );

            bool removeCode = manager.ShouldCodeBeRemoved();
            Assert.IsTrue( removeCode );
        }

        [Test]
        public void TestShouldCodeBeRemoved_AdmitDateGreaterThanFeatureDate_AndIsOutPatient_ReturnTrue()
        {
            var manager = GetManagerWithAccount( VisitType.Outpatient, AdmitDateAfterFeatureStartDate );

            bool removeCode = manager.ShouldCodeBeRemoved();
            Assert.IsTrue( removeCode );
        }


        [Test]
        public void TestRemoveP7ConditionCodeIfInvalidForPatientType_WhenInpatient_ShouldNotRemoveP7ConditionCode()
        {
            var account = new Account { KindOfVisit = VisitType.Inpatient, Facility = new Facility { Oid = 900 } };

            var cond1 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE };
            var cond2 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS };
            var cond3 = new ConditionCode { Code = ConditionCode.ADMITTED_AS_IP_FROM_ER };

            account.ConditionCodes.Add( cond1 );
            account.ConditionCodes.Add( cond2 );
            account.ConditionCodes.Add( cond3 );
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );
            manager.RemoveConditionCodeOnPatientTypeChange();

            Assert.IsTrue( account.ConditionCodes.Count == 3 );
            Assert.IsTrue( account.ConditionCodes.Contains( cond3 ) );
        }

        [Test]
        public void TestRemoveP7ConditionCodeIfInvalidForPatientType_WhenInpatientButNoP7_ShouldNotRemoveAnyConditionCode()
        {
            var account = new Account { KindOfVisit = VisitType.Inpatient, Facility = new Facility { Oid = 900 } };

            var cond1 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE };
            var cond2 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS };
            var cond3 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_DISABILITY_NO_GHP };

            account.ConditionCodes.Add( cond1 );
            account.ConditionCodes.Add( cond2 );
            account.ConditionCodes.Add( cond3 );

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );
            manager.RemoveConditionCodeOnPatientTypeChange();

            Assert.IsTrue( account.ConditionCodes.Count == 3 );
        }

        [Test]
        public void TestRemoveP7ConditionCodeIfInvalidForPatientType_WhenOutpatient_ShouldRemoveP7ConditionCode()
        {
            var account = new Account { KindOfVisit = VisitType.Outpatient, Facility = new Facility { Oid = 900 } };

            var cond1 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE };
            var cond2 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS };
            var cond3 = new ConditionCode { Code = ConditionCode.ADMITTED_AS_IP_FROM_ER };

            account.ConditionCodes.Add( cond1 );
            account.ConditionCodes.Add( cond2 );
            account.ConditionCodes.Add( cond3 );
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );
            manager.RemoveConditionCodeOnPatientTypeChange();

            Assert.IsTrue( account.ConditionCodes.Count == 2 );
            Assert.IsFalse( account.ConditionCodes.Contains( cond3 ) );
        }


        [Test]
        public void TestFilterConditionCodesMasterList_WhenOutpatient_ShouldRemoveP7ConditionCode()
        {
            var account = new Account { KindOfVisit = VisitType.Outpatient, Facility = new Facility { Oid = 900 } };

            var cond1 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE };
            var cond2 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS };
            var cond3 = new ConditionCode { Code = ConditionCode.ADMITTED_AS_IP_FROM_ER };

            IEnumerable<ConditionCode> conditionCodes = new List<ConditionCode> { cond1, cond2, cond3 };

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );
            conditionCodes = manager.FilterConditionCodesMasterList( conditionCodes );

            Assert.IsFalse( conditionCodes.Contains( cond3 ) );
        }

        [Test]
        public void TestFilterConditionCodesMasterList_WhenInpatient_ShouldNotRemoveP7ConditionCode()
        {
            var account = new Account { KindOfVisit = VisitType.Inpatient, Facility = new Facility { Oid = 900 } };

            var cond1 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE };
            var cond2 = new ConditionCode { Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS };
            var cond3 = new ConditionCode { Code = ConditionCode.ADMITTED_AS_IP_FROM_ER };

            IEnumerable<ConditionCode> conditionCodes = new List<ConditionCode> { cond1, cond2, cond3 };
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var manager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );
            conditionCodes = manager.FilterConditionCodesMasterList( conditionCodes );

            Assert.IsTrue( conditionCodes.Contains( cond3 ) );
        }

        #endregion

        #region Private Methods



        private static EmergencyToInPatientTransferCodeManager GetManagerWithAccount( VisitType patientType, DateTime admitDate, Activity activity = null, bool wasEmergencyPatient = false )
        {
            var account = new Account
            {
                KindOfVisit = patientType,
                Activity = activity,
                AdmitDate = admitDate,
                Facility = new Facility { Oid = 900 }
            };

            var accountBroker = MockRepository.GenerateMock<IAccountBroker>();

            accountBroker.Expect( x => x.WasAccountEverAnERType( Arg<Account>.Is.Anything ) ).Return( wasEmergencyPatient );
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var conditionCodeManager = new EmergencyToInPatientTransferCodeManager( FeatureStartDate, account, accountBroker, conditionCodeBroker );

            return conditionCodeManager;
        }

        #endregion

        #region Data Elements
        #endregion
    }
}
