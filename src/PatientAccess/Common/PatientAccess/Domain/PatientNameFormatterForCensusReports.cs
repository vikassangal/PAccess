using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientNameFormatterForCensusReports.
    /// </summary>
    public class PatientNameFormatterForCensusReports
    {
        #region Properties

        #endregion

        #region Construction and Finalization

        public PatientNameFormatterForCensusReports( AccountProxy accProxy, bool forPrintPreview )
        {
            Guard.ThrowIfArgumentIsNull( accProxy, "accProxy" );
            Guard.ThrowIfArgumentIsNull( accProxy.Patient, "accProxy.Patient" );
            Guard.ThrowIfArgumentIsNull( accProxy.HospitalService, "accProxy.HospitalService" );

            accountProxy = accProxy;
            forPrint = forPrintPreview;
        }

        #endregion

        #region Methods

        public string GetFormattedPatientName()
        {
            var patientName = accountProxy.Patient.FormattedName;

            if ( accountProxy.Patient != null && accountProxy.HospitalService.IsPsychCode() )
            {
                patientName = String.Empty;
            }

            else if ( accountProxy.Confidential != null && accountProxy.Confidential.Trim().Length > 0 )
            {
                patientName = forPrint ? CONFIDENTIAL_PATIENT_NAME : patientName;
            }

            return patientName;
        }
        #endregion

        #region Data Elements

        private readonly AccountProxy accountProxy;
        private readonly bool forPrint;

        #endregion

        #region Constants

        private const string CONFIDENTIAL_PATIENT_NAME = "OCCUPIED";

        #endregion

    }
}
