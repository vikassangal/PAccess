using System;
using System.Collections.Specialized;
using NUnit.Framework;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ClinicalTrialsFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ClinicalTrialsFeatureManagerTests
    {
        [Test]
        [ExpectedException( typeof( FormatException ) )]
        public void ShouldWeEnableClinicalResearchFields_WhenStartDateIsEmptyString_ShouldThrowException()
        {
            var mockAppSettings = new NameValueCollection();
            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] = string.Empty;
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( DateTime.Now, DateTime.Today);
        }

        [Test]
        public void ShouldWeEnableClinicalResearchFields_WhenAdmitDateIsTheSameAsTheFeatuteStartDate_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            DateTime admitDate = DateTime.Now;
            DateTime featureStartDate = admitDate; 

            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] = featureStartDate.ToString();
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            bool enableClinicalResearchFields = clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, DateTime.Today );
            Assert.IsTrue( enableClinicalResearchFields );
        }

        [Test]
        public void ShouldWeEnableClinicalResearchFields_WhenAdmitDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var mockAppSettings = new NameValueCollection();
            
            DateTime admitDate = DateTime.Now;
            DateTime featureStartDate = admitDate + TimeSpan.FromDays(2);

            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] =featureStartDate.ToString();
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            bool enableClinicalResearchFields = clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, DateTime.Today);
            Assert.IsFalse(enableClinicalResearchFields);
            
        }

        [Test]
        public void ShouldWeEnableClinicalResearchFields_WhenAdmitDateIsAafterTheFeatuteStartDate_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            DateTime admitDate = DateTime.Now;
            DateTime featureStartDate = admitDate - TimeSpan.FromDays( 2 );

            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] = featureStartDate.ToString();
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            bool enableClinicalResearchFields = clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, DateTime.Today);
            Assert.IsTrue( enableClinicalResearchFields );
        }

        [Test]
        public void ShouldWeEnableClinicalResearchFields_WhenAdmitDateIsMinValue_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            DateTime admitDate = DateTime.MinValue;
            DateTime featureStartDate = admitDate + TimeSpan.FromDays( 2 );

            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] = featureStartDate.ToString();
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            bool enableClinicalResearchFields = clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, DateTime.Today);
            Assert.IsTrue( enableClinicalResearchFields );

        }

        [Test]
        public void ShouldWeEnableClinicalResearchFields_WhenAdmitDateFeatureDateAndTodayAreSame_ShouldReturnFalse()
        {
            var mockAppSettings = new NameValueCollection();

            DateTime admitDate = DateTime.Now;
            DateTime featureStartDate = admitDate + TimeSpan.FromDays( 2 );

            DateTime today = admitDate;
 
            mockAppSettings[ClinicalTrialsFeatureManager.CLINICALRESEARCHFIELDS_START_DATE] = featureStartDate.ToString();
            var clinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( mockAppSettings );
            bool enableClinicalResearchFields = clinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, today );
            Assert.IsFalse( enableClinicalResearchFields );
        }
    }
}