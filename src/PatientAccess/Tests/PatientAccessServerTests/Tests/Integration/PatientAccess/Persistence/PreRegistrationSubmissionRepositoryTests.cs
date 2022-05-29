using System;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Nhibernate;
using PatientAccess.Persistence.OnlinePreregistration;
using NUnit.Framework;
using Tests.Utilities.OnlinePreRegistration;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    [Category( "Fast" )]
    public class PreRegistrationSubmissionRepositoryTests
    {
        [Test]
        public void TestSessionFactoryCreation()
        {
            Assert.IsNotNull( NHibernateInitializer.SessionFactory );
        }

        [Test]
        public void TestDeleteSubmission()
        {
            var submissionRepository = new PreRegistrationSubmissionRepository();

            var submission = GetSubmissionForFacility( DhfFacilityCode );

            var submissionId = submissionRepository.Save( submission );

            submissionRepository.Delete( submission );

            var retrievedSubmission = submissionRepository.GetById( submissionId );

            Assert.IsNotNull( submissionId );
            Assert.IsNull( retrievedSubmission );
        }

        [Test]
        public void TestDeleteSubmissionById()
        {
            var submissionRepository = new PreRegistrationSubmissionRepository();

            var submission = GetSubmissionForFacility( DhfFacilityCode );

            var submissionId = submissionRepository.Save( submission );

            submissionRepository.Delete( submission.Id );

            var retrievedSubmission = submissionRepository.GetById( submissionId );

            Assert.IsNotNull( submissionId );
            Assert.IsNull( retrievedSubmission );
        }

        [Test]
        public void TestSaveSubmission()
        {
            var submissionRepository = new PreRegistrationSubmissionRepository();
            var submissionToSave = GetSubmissionForFacility( DhfFacilityCode );
            string expectedFacility = DataCreator.GetFacilityCode( submissionToSave );
            var savedSubmissionId = submissionRepository.Save( submissionToSave );

            var retreivedSubmission = submissionRepository.GetById( savedSubmissionId );

            string actualFacilityCode = DataCreator.GetFacilityCode( retreivedSubmission );

            Assert.AreEqual( expectedFacility, actualFacilityCode );
            Assert.IsNotNull( savedSubmissionId );
        }

        [Test]
        public void TestDeleteSubmissionsWithAdmitTimeOlderThan()
        {
            var submissionRepository = new PreRegistrationSubmissionRepository();

            var submission = GetSubmissionForFacility( DhfFacilityCode );
            submission.AdmitDate = DateTime.Now - TimeSpan.FromDays( 6 );
            submissionRepository.Save( submission );

            submissionRepository.DeleteSubmissionsWithAdmitTimeOlderThan( 5 );
            var subb = submissionRepository.GetById( submission.Id );
            
            Assert.IsNull( subb );
        }

        [Test]
        public void TestGetByFacilityAndSearchCriteria()
        {
            var submissionRepository = new PreRegistrationSubmissionRepository();
            var expected = GetSubmissionForFacility( DhfFacilityCode );
            var savedSubmissionId = submissionRepository.Save( expected );

            var actual = submissionRepository.GetSubmissionItemsBy( DhfFacilityCode, GetAllEnclusiveCriteria() ).First( x => x.Id == savedSubmissionId );

            AssertAreEqual( expected, actual );
        }

        private static void AssertAreEqual( PreRegistrationSubmission expected, OnlinePreRegistrationItem actual )
        {
            var expectedName = new Name( expected.FirstName, expected.LastName, expected.MiddleInitial ).AsFormattedName();
            Assert.AreEqual( expectedName, actual.PatientName );
            Assert.AreEqual( expected.Gender, actual.Gender );
            Assert.AreEqual( expected.DateOfBirth.Date, actual.DateOfBirth.Date );
            Assert.AreEqual( expected.ReturningPatient, actual.VisitedBefore );
            Assert.AreEqual( expected.SSN, actual.Ssn );
            Assert.AreEqual( expected.Address, actual.Address );
            Assert.AreEqual( expected.AdmitDate.Date, actual.AdmitDate.Date );
        }

        [SetUp]
        public void SetUp()
        {
            DeleteAllSubmissions();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteAllSubmissions();
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            DeleteAllSubmissions();
        }

        private static void DeleteAllSubmissions()
        {
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                session.CreateSQLQuery( "DELETE from Messaging.PreRegistrationSubmissions" ).ExecuteUpdate();
                tx.Commit();
            }
        }

        private static PreRegistrationSubmission GetSubmissionForFacility( long facilityId )
        {
            var message = DataCreator.GetMessageXmlDocumentFromSampleDocument();

            var preRegistrationSubmission = new PreRegistrationSubmission
            {
                FacilityId = facilityId,
                FirstName = "firstName",
                Gender = "FEMALE",
                LastName = "lastName",
                DateOfBirth = new DateTime( 1999, 1, 1 ),
                SSN = "123-12-1234",
                Address = "some address",
                AdmitDate = DateTime.Now,
                DateTimeReceived = DateTime.Now,
                Message = message

            };
            return preRegistrationSubmission;
        }

        private const long DhfFacilityCode = 54;

        private static WorklistSettings GetAllEnclusiveCriteria()
        {
            var range = new WorklistSelectionRange { Oid = WorklistSelectionRange.ALL };
            return new WorklistSettings( "A", "Z", DateTime.MinValue, DateTime.MaxValue, range, 0, 0 );
        }
    }
}
