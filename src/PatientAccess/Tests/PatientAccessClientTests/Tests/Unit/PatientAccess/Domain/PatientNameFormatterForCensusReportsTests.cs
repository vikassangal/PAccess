using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PatientNameFormatterForCensusReportsTests
    {
        #region Data Elements
        readonly HospitalService psychHSV = new HospitalService
            {
                Code = HospitalService.PSYCH_NON_LOCKED,
                Description = "PSYCH_NON_LOCKED"
            };

        readonly HospitalService nonPsychHSV = new HospitalService
            {
                Code = HospitalService.HSV57,
                Description = "OUTPT_IN_BED_NON_OBS"
            };

        readonly AccountProxy accountProxy = new AccountProxy
            {
                Patient = new Patient
                    {
                        FirstName = "SAM",
                        LastName = "THOMAS",
                        MiddleInitial = "M"
                    }
            };
        #endregion

        #region Test Methods

        [Test]
        public void TestPatientCensusName_HSVisPsychCode()
        {
            accountProxy.HospitalService = psychHSV;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, false ).GetFormattedPatientName();
            
            Assert.AreEqual( patientName, String.Empty, "Patient name should be blank for Psych patients" );
        }

        [Test]
        public void TestPatientCensusName_HSVisNonPsychCode()
        {
            accountProxy.HospitalService = nonPsychHSV;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, false ).GetFormattedPatientName();
            
            Assert.AreEqual( patientName, accountProxy.Patient.FormattedName, "Patient name should be in the format 'LastName, FirstName M' for non-Psych patient" );
        }

        [Test]
        public void TestPatientCensusName_HSVisPsychCode_WithConfidentialStatus_ForUserScreen()
        {
            accountProxy.HospitalService = psychHSV;
            accountProxy.Confidential = CONFIDENTIAL_STATUS;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, false ).GetFormattedPatientName();
            
            Assert.AreEqual( patientName, String.Empty, "Patient Name should be blank for user screen" );
        }

        [Test]
        public void TestPatientCensusName_HSVisPsychCode_WithConfidentialStatus_ForPrintPreviewScreen()
        {
            accountProxy.HospitalService = psychHSV;
            accountProxy.Confidential = CONFIDENTIAL_STATUS;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, true ).GetFormattedPatientName();
            
            Assert.AreEqual( patientName, String.Empty, "Patient Name should be blank for print preview screen" );
        }

        [Test]
        public void TestPatientCensusName_HSVisNonPsychCode_WithConfidentialStatus_ForUserScreen()
        {
            accountProxy.HospitalService = nonPsychHSV;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, false ).GetFormattedPatientName();

            Assert.AreEqual( patientName, accountProxy.Patient.FormattedName, "Patient name should be in the format 'LastName, FirstName M' for non-Psych patient" );
        }

        [Test]
        public void TestPatientCensusName_HSVisNonPsychCode_WithConfidentialStatus_ForPrintPreviewScreen()
        {
            accountProxy.HospitalService = nonPsychHSV;
            accountProxy.Confidential = CONFIDENTIAL_STATUS;
            var patientName = new PatientNameFormatterForCensusReports( accountProxy, true ).GetFormattedPatientName();

            Assert.AreEqual( patientName, CONFIDENTIAL_PATIENT_NAME, "Patient name should be 'OCCUPIED'" );
        }

        #endregion

        #region Constants

        private const String CONFIDENTIAL_STATUS = "C";
        private const String CONFIDENTIAL_PATIENT_NAME = "OCCUPIED";

        #endregion
    }
}