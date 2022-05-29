using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for NonStaffPhysicianNPIRequiredTest
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class NonStaffPhysicianNPIRequiredTests
    {
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType()
        {
            NonStaffPhysicianNPIRequired ruleUnderTest = new NonStaffPhysicianNPIRequired();
            object inValidObjectType = new object();
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( ruleResult );
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestCanBeAppliedToWithNullContext()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired();
            ruleUnderTest.CanBeAppliedTo( null );
        }

        [Test]
        public void TestCanBeAppliedToWithNullAssociatedControl()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = null };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( new Physician() );
            Assert.IsTrue( ruleResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenAssociatedControlIsNotActivity_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new object() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( new Physician() );
            Assert.IsTrue( ruleResult );
        }

        [Test]
        public void TestIsValidActivity_ForAllValidActivityTypes_ShouldReturnTrue()
        {
            ArrayList validList = new ArrayList( ValidActivities );

            foreach ( ActivityType activityType in Enum.GetValues( typeof( ActivityType ) ) )
            {
                var isValidActivity = NonStaffPhysicianNPIRequired.IsValidActivity( CodeAndTypeHelper.GetActivityFrom( activityType ) );
                if ( validList.Contains( activityType ) )
                {
                    Assert.IsTrue( isValidActivity );
                }
            }
        }

        [Test]
        public void TestIsValidActivity_ForAllInvalidActivityTypes_ShouldReturnFalse()
        {
            ArrayList validList = new ArrayList( ValidActivities );

            foreach ( ActivityType activityType in Enum.GetValues( typeof( ActivityType ) ) )
            {
                var isValidActivity = NonStaffPhysicianNPIRequired.IsValidActivity( CodeAndTypeHelper.GetActivityFrom( activityType ) );
                if ( !validList.Contains( activityType ) )
                {
                    Assert.IsFalse( isValidActivity, activityType + " is not a valid activity for Non Staff Physician NPI Required" );
                }
            }
        }

        [Test]
        public void TestCanBeAppliedTo_ForInValidActivity_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new DischargeActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( new Physician() );
            Assert.IsTrue( ruleResult );
        }

        [Test]
        public void TestCanBeAppliedTo_ForValidActivityAndNotNonStaffPhysician_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new RegistrationActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( new Physician { PhysicianNumber = 111 } );
            Assert.IsTrue( ruleResult );
        }

        [Test]
        public void TestCanBeAppliedTo_ForValidActivityAndNonStaffPhysicianNPIIsEmpty_ShouldReturnFalse()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new RegistrationActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo( 
                new Physician { PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER } );
            Assert.IsFalse( ruleResult );
        }

        [Test]
        public void TestCanBeAppliedTo_ForValidActivityAndNonStaffPhysicianNPIIsNotEmpty_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new RegistrationActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo(
                new Physician { PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER,
                                NPI = "1111111111" 
                              } );
            Assert.IsTrue( ruleResult );
        }
        [Test]
        public void TestCanBeAppliedTo_ForTransferErToOutPatientActivityAndNonStaffPhysicianNPIIsNotEmpty_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new TransferERToOutpatientActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo(
                new Physician
                {
                    PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER,
                    NPI = "1111111111"
                });
            Assert.IsTrue(ruleResult);
        }
        [Test]
        public void TestCanBeAppliedTo_ForTRansferOutPatientToERPatientActivityAndNonStaffPhysicianNPIIsNotEmpty_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new TransferOutpatientToERActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo(
                new Physician
                {
                    PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER,
                    NPI = "1111111111"
                });
            Assert.IsTrue(ruleResult);
        }
        [Test]
        public void TestCanBeAppliedTo_ForTRansferOutPatientToERPatientActivityAndNonStaffPhysicianNPIIsEmpty_ShouldReturnTrue()
        {
            var ruleUnderTest = new NonStaffPhysicianNPIRequired { AssociatedControl = new TransferOutpatientToERActivity() };
            bool ruleResult = ruleUnderTest.CanBeAppliedTo(
                new Physician
                {
                    PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER,
                    NPI = String.Empty
                });
            Assert.IsFalse(ruleResult);
        }
        #region Data Elements

        private readonly ActivityType[] ValidActivities = 
                                            { ActivityType.PreRegistrationActivity,
                                              ActivityType.RegistrationActivity,
                                              ActivityType.PreMSERegisterActivity,
                                              ActivityType.PostMSERegistrationActivity,
                                              ActivityType.AdmitNewbornActivity,
                                              ActivityType.PreAdmitNewbornActivity,
                                              ActivityType.MaintenanceActivity,
                                              ActivityType.EditAccountActivity,
                                              ActivityType.EditPreMseActivity,
                                              ActivityType.PreRegistrationWithOfflineActivity,
                                              ActivityType.RegistrationWithOfflineActivity,
                                              ActivityType.PreMSERegistrationWithOfflineActivity,
                                              ActivityType.AdmitNewbornWithOfflineActivity,
                                              ActivityType.PreAdmitNewbornWithOfflineActivity,
                                              ActivityType.ActivatePreRegistrationActivity,
                                              ActivityType.TransferOutToInActivity,
                                              ActivityType.TransferERToOutpatientActivity,
                                              ActivityType.TransferOutpatientToERActivity, 
                                            };
        #endregion
    }
}
