using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for EmploymentStatusPBARBrokerTests.
    /// </summary>

    [TestFixture()]
    public class EmploymentStatusPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown EmploymentStatusPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpEmploymentStatusBrokerTests()
        {
            employmentStatusBroker = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownEmploymentStatusPBARBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAllEmploymentStatuses()
        {
            ArrayList statuses = (ArrayList)employmentStatusBroker.AllTypesOfEmploymentStatuses( ACO_FACILITYID );

            Assert.IsNotNull( statuses, "No Statuses were found" );
            EmploymentStatus foundEmpStatus = null;
            foreach( EmploymentStatus stat in statuses )
            {
                if( stat.Code == "1" )
                {
                    foundEmpStatus = stat;
                }
            }
            Assert.IsNotNull( foundEmpStatus, "Did not Find expected Employment Status" );
            Assert.AreEqual( "EMPLOYED FULL TIME",
                             foundEmpStatus.Description.Trim(),
                             "First entry should be EMPLOYED FULL TIME" );
        }

        [Test()]
        public void TestSpecificEmploymentStatus()
        {
            EmploymentStatus status = null;

            status = employmentStatusBroker.EmploymentStatusWith( ACO_FACILITYID, "5" );
            Assert.IsNotNull( status, "Did not find expected Employment Status" );
            Assert.AreEqual( "RETIRED", status.Description, "Code for 5 Should be RETIRED" );
        }

        [Test()]
        public void TestEmploymentStatusForBlank()
        {
            string blank = String.Empty;
            EmploymentStatus es = employmentStatusBroker.EmploymentStatusWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, es.Code, "Code should be blank" );

            Assert.AreEqual( blank, es.Description, "Description should be blank" );

            Assert.IsTrue( es.IsValid, "Should be a valid" );

            EmploymentStatus es1 = employmentStatusBroker.EmploymentStatusWith( ACO_FACILITYID, " " );
            Assert.IsNotNull( es1 );
        }

        [Test()]
        public void TestEmploymentStatusForInvalid()
        {
            EmploymentStatus es = employmentStatusBroker.EmploymentStatusWith( ACO_FACILITYID, "DZ" );

            Assert.IsFalse( es.IsValid, "Should be in valid" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IEmploymentStatusBroker employmentStatusBroker = null;
        #endregion
    }
}