using System;
using System.Data;
using IBM.Data.DB2.iSeries;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Specialized;
using log4net;

namespace PatientAccess.Persistence.Specialized
{
    /// <summary>
    /// Extends default broker to add clinical trial flag value to
    /// the Patient instance
    /// </summary>
    public class ClinicalTrialsPatientPBARBroker : PatientPBARBroker
    {
		#region Constants 

        private const string SQL_QUERY_FOR_CLINICAL_TRIAL_VALUE_ON_PATIENT =
            "SELECT MDCTBF FROM HPADMDP WHERE MDHSP# = @P_FACILITYID AND MDMRC# = @P_MRN";

		#endregion Constants 

		#region Fields 

        private static readonly ILog Logger =
            LogManager.GetLogger( typeof( ClinicalTrialsAccountPBARBroker ) );

		#endregion Fields 

		#region Methods 

        /// <summary>
        /// Creates a Patient instance based on <see cref="PatientSearchResult"/>
        /// </summary>
        /// <param name="patientResult">The patient result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>patientResult</c> is null.</exception>
        public override Patient PatientFrom( PatientSearchResult patientResult )
        {
            Patient newPatient =
                base.PatientFrom( patientResult );

            this.HandleIfFacilityIsClinicalTrialsEnabled( ref newPatient );

            return newPatient;
        }


        /// <summary>
        /// Gets a sparse patient instance
        /// </summary>
        /// <param name="mrn">The MRN.</param>
        /// <param name="facilityCode">The facility code.</param>
        /// <returns></returns>
        public override Patient SparsePatientWith( long mrn, string facilityCode )
        {
            Patient newSparsePatient =
                base.SparsePatientWith( mrn, facilityCode );

            this.HandleIfFacilityIsClinicalTrialsEnabled( ref newSparsePatient );

            return newSparsePatient;
        }


        /// <summary>
        /// Handles if facility is clinical trials enabled.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void HandleIfFacilityIsClinicalTrialsEnabled( ref Patient patient )
        {

            // For offline registrations, it is possible this will be a null value
            if( patient == null )
            {
               return;                
            }

            Facility facility = patient.Facility;

            if( !facility.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED ) )
            {
                return;
            }

            iDB2Command db2Command = this.CommandFor( SQL_QUERY_FOR_CLINICAL_TRIAL_VALUE_ON_PATIENT,
                                                      CommandType.Text, facility );

            db2Command.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;
            db2Command.Parameters[PARAM_MRN].Value = patient.MedicalRecordNumber;

            try
            {
                patient[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS] =
                    db2Command.ExecuteScalar().ToString();
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                Close( db2Command );
            }
        }

		#endregion Methods 
    }
}
