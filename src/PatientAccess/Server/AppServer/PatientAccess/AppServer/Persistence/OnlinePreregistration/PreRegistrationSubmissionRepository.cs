using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Nhibernate;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class PreRegistrationSubmissionRepository : IPreRegistrationSubmissionRepository
    {
        private const int MaxResults = 100;

        public PreRegistrationSubmission GetById( Guid submissionId )
        {
            PreRegistrationSubmission retrievedSubmission;
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                retrievedSubmission = session.Get<PreRegistrationSubmission>( submissionId );
                tx.Commit();
            }
            return retrievedSubmission;
        }

        public void Delete( PreRegistrationSubmission submission )
        {
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction( IsolationLevel.Serializable ) )
            {
                session.Delete( submission );
                tx.Commit();
            }
        }

        public Guid Save( PreRegistrationSubmission submission )
        {
            Guid savedSubmissionId;
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                savedSubmissionId = (Guid)session.Save( submission );

                tx.Commit();
            }
            return savedSubmissionId;
        }

        public void Delete( Guid submissionId )
        {
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                var retrievedSubmission = session.Get<PreRegistrationSubmission>( submissionId );
                session.Delete( retrievedSubmission );
                tx.Commit();
            }
        }

        /// <summary>
        /// Gets the OnlinePreRegistrationItem based on the given <c>facilityId</c> and <c>criteria</c>.
        /// The method does not return the full<c>PreRegistrationSubmission</c> 
        /// because we want to avoid decrypting the xml message for each entry in the result
        /// </summary>
        /// <param name="facilityId">The facility id.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public IEnumerable<OnlinePreRegistrationItem> GetSubmissionItemsBy( long facilityId, WorklistSettings criteria )
        {
            Guard.ThrowIfArgumentIsNull( criteria, "criteria" );

            string startingCharacter = criteria.BeginningWithLetter.Substring( 0, 1 );
            string endingCharacter = criteria.EndingWithLetter.Substring( 0, 1 );

            string searchExpression = "[" + startingCharacter + "-" + endingCharacter + "]" + "%";

            var dateRange = criteria.GetDateRange();

            dateRange = DateTimeUtilities.BringWithInSqlServerDateTimeRange( dateRange );

            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {

                var results = session.QueryOver<PreRegistrationSubmission>()

                    .WhereRestrictionOn( p => p.LastName ).IsLike( searchExpression )
                    .Where( p => p.FacilityId == facilityId )
                    .And( x => x.AdmitDate >= dateRange.FromDate )
                    .And( x => x.AdmitDate <= dateRange.ToDate )

                    .Select(
                        submission => submission.Id,
                        submission => submission.FirstName,
                        submission => submission.LastName,
                        submission => submission.MiddleInitial,
                        submission => submission.Gender,
                        submission => submission.DateOfBirth,
                        submission => submission.ReturningPatient,
                        submission => submission.SSN,
                        submission => submission.Address,
                        submission => submission.AdmitDate

                    )
                    .Take( MaxResults )

                    .List<object[]>().Select( item => new OnlinePreRegistrationItem(
                                                          (Guid)item[0],
                                                          new Name( (string)item[1], (string)item[2], (string)item[3] ),
                                                          (string)item[4],
                                                          (DateTime)item[5],
                                                          (bool?)item[6],
                                                          (string)item[7],
                                                          (string)item[8],
                                                          (DateTime)item[9],
                                                          false ) );
                tx.Commit();
                return results;
            }
        }

        /// <exception cref="ArgumentException"><c>numberOfDays</c> is not positive</exception>
        public void DeleteSubmissionsWithAdmitTimeOlderThan( int numberOfDays )
        {
            Guard.ThrowIfIntegerIsNotPositive( numberOfDays, "numberOfDays" );
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                DateTime cutoffDate = DateTime.Now - TimeSpan.FromDays( numberOfDays );
                session.CreateQuery( "delete from PreRegistrationSubmission d where d.AdmitDate < :cutoffDate" ).SetDateTime( "cutoffDate", cutoffDate ).ExecuteUpdate();
                tx.Commit();
            }
        }
    }
}
