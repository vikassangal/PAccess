using System;
using System.Collections.Specialized;
using NUnit.Framework;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PrimaryCarePhysicianForPreMseFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class PrimaryCarePhysicianForPreMseFeatureManagerTests
    {
        [Test]
        [ExpectedException( typeof( FormatException ) )]
        public void ShouldWeEnablePCP_PhysicianForPre_MSE_WhenStartDateIsEmptyString_ShouldThrowException()
        {
            var mockAppSettings = new NameValueCollection();
            mockAppSettings[PrimaryCarePhysicianForPreMseFeatureManager.PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] = string.Empty;
            var featureManager = new PrimaryCarePhysicianForPreMseFeatureManager( mockAppSettings );
            featureManager.IsEnabledFor( DateTime.Now );
        }

        [Test]
        public void ShouldWeEnablePCP_PhysicianForPre_MSE_WhenAdmitDateIsTheSameAsTheFeatuteStartDate_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            var admitDate = DateTime.Now;
            var featureStartDate = admitDate;

            mockAppSettings[PrimaryCarePhysicianForPreMseFeatureManager.PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] = featureStartDate.ToString();
            var featureManager = new PrimaryCarePhysicianForPreMseFeatureManager( mockAppSettings );
            bool enabled = featureManager.IsEnabledFor( admitDate );

            Assert.IsTrue( enabled );
        }

        [Test]
        public void ShouldWeEnablePCP_PhysicianForPre_MSE_WhenAdmitDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var mockAppSettings = new NameValueCollection();

            var admitDate = DateTime.Now;
            var featureStartDate = admitDate + TimeSpan.FromDays( 2 );

            mockAppSettings[PrimaryCarePhysicianForPreMseFeatureManager.PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] = featureStartDate.ToString();
            var featureManager = new PrimaryCarePhysicianForPreMseFeatureManager( mockAppSettings );
            bool enabled = featureManager.IsEnabledFor( admitDate );

            Assert.IsFalse( enabled );

        }

        [Test]
        public void ShouldWeEnablePCP_PhysicianForPre_MSE_WhenAdmitDateIsAafterTheFeatuteStartDate_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            var admitDate = DateTime.Now;
            var featureStartDate = admitDate - TimeSpan.FromDays( 2 );

            mockAppSettings[PrimaryCarePhysicianForPreMseFeatureManager.PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] = featureStartDate.ToString();
            var featureManager = new PrimaryCarePhysicianForPreMseFeatureManager( mockAppSettings );
            bool enabled = featureManager.IsEnabledFor( admitDate );
            Assert.IsTrue( enabled );
        }

        [Test]
        public void ShouldWeEnablePCP_PhysicianForPre_MSE_WhenAdmitDateIsMinValue_ShouldReturnTrue()
        {
            var mockAppSettings = new NameValueCollection();

            var admitDate = DateTime.MinValue;
            var featureStartDate = admitDate + TimeSpan.FromDays( 2 );

            mockAppSettings[PrimaryCarePhysicianForPreMseFeatureManager.PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] = featureStartDate.ToString();
            var featureManager = new PrimaryCarePhysicianForPreMseFeatureManager( mockAppSettings );
            bool enabled = featureManager.IsEnabledFor( admitDate );
            Assert.IsTrue( enabled );

        }
    }
}