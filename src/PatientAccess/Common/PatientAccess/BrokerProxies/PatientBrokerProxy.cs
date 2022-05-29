using System;
using System.Collections;
 
using Extensions.PersistenceCommon;
 
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for PatientBrokerProxy.
    /// </summary>
    //TODO: Create XML summary comment for PatientBrokerProxy
    [Serializable]
    public class PatientBrokerProxy : AbstractBrokerProxy, IPatientBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        public MedicalGroupIPA GetIpaForPatient( Patient patient )
        {
            throw new NotImplementedException();
        }

        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        public VisitType PatientTypeWith( long facilityID, string code )
        {
            throw new NotImplementedException();
        }


        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        public long MRNForAccount( long accountNumber, string facilityCode )
        {
            throw new NotImplementedException();
        }


        public long GetNewMrnFor( Facility facility )
        {
            return i_PatientBroker.GetNewMrnFor( facility );
        }


        public Patient SparsePatientWith( long mrn, string facilityCode )
        {
            return i_PatientBroker.SparsePatientWith( mrn, facilityCode );
        }


        public PatientSearchResponse GetPatientSearchResponseFor( PatientSearchCriteria patientCriteria )
        {
            return i_PatientBroker.GetPatientSearchResponseFor( patientCriteria );
        }
        public PatientSearchResponse GetPatientSearchResponseFor( GuarantorSearchCriteria guarantorCriteria )
        {
            return i_PatientBroker.GetPatientSearchResponseFor( guarantorCriteria );
        }

        public Patient PatientFrom( PatientSearchResult patientResult )
        {
            return i_PatientBroker.PatientFrom( patientResult );
        }

        public ICollection AllPatientTypes( long facilityID )
        {
            var cacheKey = "PATIENT_BROKER_ALL_PATIENT_TYPES" + "_AND_FACILITY_" + facilityID;
            ICollection patientTypes = (ICollection)this.Cache[cacheKey];

            if ( patientTypes == null )
            {
                lock (cacheKey)
                {
                    patientTypes = i_PatientBroker.AllPatientTypes( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey,
                            patientTypes );
                    }
                }
            }

            return patientTypes;
        }

        public VisitType PatientTypeWith( string code, long facilityID )
        {

            VisitType visitType = null;

            ICollection patientTypes = AllPatientTypes( facilityID );

            if ( patientTypes != null )
            {
                foreach ( VisitType visit in patientTypes )
                {
                    if ( visit.Code == code )
                    {
                        visitType = visit;
                    }
                }
            }

            // CodeReview: in the scenario where no PT is found in the client cache, we should defer to the remote broker 
            // (rather than creating a new instance).

            if ( visitType == null )
            {
                visitType = new VisitType( PersistentModel.NEW_OID, DateTime.Now, code, code );
                visitType.IsValid = false;
            }

            return visitType;
        }

        public ArrayList PatientTypesFor(string activityType, string associatedActivityType, string kindOfVisitCode, string financialClassCode, string locationBedCode, long facilityID)
        {
            return i_PatientBroker.PatientTypesFor( activityType, associatedActivityType, kindOfVisitCode, financialClassCode,
                locationBedCode, facilityID);
        }

        public ArrayList PatientTypesForWalkinAccount( long facilityID )
        {
            return i_PatientBroker.PatientTypesForWalkinAccount( facilityID );
        }
        public ArrayList PatientTypesForUCCAccount(long facilityID)
        {
            return i_PatientBroker.PatientTypesForUCCAccount(facilityID);
        }

        public Boolean IsPatientSequestered(Account account)
        {
            return i_PatientBroker.IsPatientSequestered(account);
        }

        public int PatientAgeFor( DateTime dateOfBirth, string facilityCode )
        {
            return i_PatientBroker.PatientAgeFor( dateOfBirth, facilityCode );
        }

        public RecentAccountDetails GetMostRecentAccountDetailsFor(long medicalRecordNumber, Facility facility)
        {
            return i_PatientBroker.GetMostRecentAccountDetailsFor(medicalRecordNumber, facility);
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements
        private readonly IPatientBroker i_PatientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
        #endregion


    }
}
