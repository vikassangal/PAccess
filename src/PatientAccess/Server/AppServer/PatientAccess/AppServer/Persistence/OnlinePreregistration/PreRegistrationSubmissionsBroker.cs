using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Locking;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.PreRegistration;
using PatientAccess.Messaging;
using PatientAccess.RemotingServices;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class PreRegistrationSubmissionsBroker : MarshalByRefObject, IPreRegistrationSubmissionsBroker
    {
        private static readonly ILog Logger = LogManager.GetLogger( typeof( PreRegistrationSubmissionsBroker ) );

        public IEnumerable<OnlinePreRegistrationItem> GetSubmittedMessagesList( long facilityCode, WorklistSettings criteria )
        {
            Guard.ThrowIfArgumentIsNull( criteria, "criteria" );
            Guard.ThrowIfArgumentIsNull( criteria.WorklistSelectionRange, "criteria.WorklistSelectionRange" );
            
            var userId = GetUserId();
            
            Logger.Info( string.Format( "User [{0}] requested submissions for Facility[{1}] with range id [{2}]", userId, facilityCode, criteria.WorklistSelectionRange.Oid ) );
 
            var repository = new PreRegistrationSubmissionRepository();
            
            //need to create a List, cannot send an IEnumerable<T> over the remoting boundary
            var onlinePreRegistrationItems = new List<OnlinePreRegistrationItem>( repository.GetSubmissionItemsBy( facilityCode, criteria ) );
            
            var lockBroker = BrokerFactory.BrokerOfType<IOfflineLockBroker>();
            
            var locks = lockBroker.GetLocksOfType( ResourceType.OnlinePreregistrationSubmission );
            
            SetLockStatusOn(onlinePreRegistrationItems, locks);

            Logger.Info( string.Format( "User [{0}] received submissions for Facility[{1}] with range id [{2}]", userId, facilityCode, criteria.WorklistSelectionRange.Oid ) );

            return onlinePreRegistrationItems;
        }

        public PreRegistrationData GetSubmissionInformationForNewAccount( Patient existingPatient, Guid submissionId )
        {
            Guard.ThrowIfArgumentIsNull( existingPatient, "existingPatient" );

            var repository = new PreRegistrationSubmissionRepository();

            PreRegistrationData preRegistrationData = null;

            var userId = GetUserId();

            Logger.Info( string.Format( "User [{0}] requested submission data for patient MRN [{1}] and submission id[{2}]", 
                userId, existingPatient.MedicalRecordNumber, submissionId ) );

            var submission = repository.GetById( submissionId );

            if ( submission != null )
            {
                preRegistration preRegistration = GetPreRegistration( submission );
                var patientFactory = new PatientFactory( preRegistration );
                var suppInfoFactory = new SupplementalInformationFactory( preRegistration );


                preRegistrationData = new PreRegistrationData( 
                    patientFactory.UpdateExistingPatientWithNewAccount( existingPatient ), 
                    suppInfoFactory.Build(), submission.Id );
            }

            Logger.Info( string.Format( "User [{0}] received submission data for patient MRN [{1}] and submission id[{2}]",
                userId, existingPatient.MedicalRecordNumber, submissionId ) );

            return preRegistrationData;
        }

        public PreRegistrationData GetDataFor( Guid submissionId )
        {
            var userId = GetUserId();

            Logger.Info( string.Format( "User [{0}] requested submission data for submission id [{1}]", userId, submissionId ) );

            var repository = new PreRegistrationSubmissionRepository();

            PreRegistrationData preRegistrationData = null;

            var submission = repository.GetById( submissionId );

            if ( submission != null )
            {
                preRegistration preRegistration = GetPreRegistration( submission );
                var patientFactory = new PatientFactory( preRegistration );
                var suppInfoFactory = new SupplementalInformationFactory( preRegistration );

                preRegistrationData = new PreRegistrationData( patientFactory.BuildNewPatient(), suppInfoFactory.Build(), submission.Id );
            }

            Logger.Info( string.Format( "User [{0}] received submission data for submission id [{1}]", userId, submissionId ) );

            return preRegistrationData;
        }

        public void DeleteSubmission( Guid id )
        {
            var userId = GetUserId();

            Logger.Info( string.Format( "Deleting message with id [{0}] for user id [{1}]", id, userId ) );

            var repository = new PreRegistrationSubmissionRepository();

            repository.Delete( id );

            Logger.Info( string.Format( "Deleted message with id [{0}] for user id [{1}]", id, userId ) );
        }

        private preRegistration GetPreRegistration( PreRegistrationSubmission submission )
        {
            var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );
            XmlDocument submissionMessage = submission.Message;
            XmlReader reader = new XmlNodeReader( submissionMessage.DocumentElement );
            var preRegistration = (preRegistration)xmlSerializer.Deserialize( reader );

            if ( IsOlderVersionMessage( preRegistration ) )
            {
                var facilityCode = GetFacilityCodeBasedOnOlderSchema( submissionMessage );
                preRegistration.facility = new facilityType { code = facilityCode };
            }

            return preRegistration;
        }

        private static bool IsOlderVersionMessage( preRegistration preRegistration )
        {
            return preRegistration.facility == null;
        }

        private static string GetFacilityCodeBasedOnOlderSchema( XmlDocument document )
        {
            var message = XDocument.Parse( document.OuterXml );
            var facilityCodeElementName = message.Root.Name.Namespace + "facilityCode";
            return message.Descendants( facilityCodeElementName ).First().Value;
        }

        private static string GetUserId()
        {
            var callContextAccesor = new CallContextAccessor();
            return callContextAccesor.GetContext().UserId;
        }

        private static void SetLockStatusOn( IEnumerable<OnlinePreRegistrationItem> submissions, IEnumerable<OfflineLock> locks )
        {
            foreach ( var submission in submissions )
            {
                var submissionId = submission.Id.ToString();

                var isLocked = locks.Count( x => x.Handle == submissionId ) == 1;

                submission.IsLocked = isLocked;
            }
        }
    }
}
