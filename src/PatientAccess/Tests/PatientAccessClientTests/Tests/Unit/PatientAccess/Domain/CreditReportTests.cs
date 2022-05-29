using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CreditReportTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CreditReportTests
        [TestFixtureSetUp()]
        public static void SetUpCreditReportTests()
        {
            cr = new CreditReport();

        }

        [TestFixtureTearDown()]
        public static void TearDownCreditReportTests()
        {
            cr = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCreditReports()
        {
            cr.CreditScore = 459;
            cr.Report = " Credit report not available";
            cr.IsInsured = true;
            cr.ServiceLastName = string.Empty;
            cr.ServiceMiddleName = string.Empty;
            cr.ServiceFirstName = string.Empty;
            cr.ServiceSSN = string.Empty;
            cr.ServicePhoneNumber = new PhoneNumber();
            cr.ServiceAddresses = new ArrayList();
            cr.ServiceHawkAlerts = new ArrayList();
            cr.FormatedHawkAlert = string.Empty;

            Assert.AreEqual(459, cr.CreditScore);
            Assert.AreEqual(" Credit report not available", cr.Report);
            Assert.AreEqual(true, cr.IsInsured);
            Assert.AreEqual(string.Empty, cr.ServiceLastName);
            Assert.AreEqual(string.Empty, cr.ServiceMiddleName);
            Assert.AreEqual(string.Empty, cr.ServiceFirstName);
            Assert.AreEqual(string.Empty, cr.ServiceSSN);
            Assert.AreEqual(string.Empty, cr.ServicePhoneNumber.Number);
            Assert.AreEqual(0, cr.ServiceAddresses.Count);
            Assert.AreEqual(0, cr.ServiceHawkAlerts.Count);
            Assert.AreEqual(string.Empty, cr.FormatedHawkAlert);
        }

        [Test()]
        public void TestCreditAdvice()
        {
            cr.IsInsured = true;

            cr.CreditScore = 550;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(cr.INSURED_SCORE_LESS_THAN_600, 550)
                );

            cr.CreditScore = 650;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(cr.INSURED_SCORE_IS_600_TO_699, 650)
                );

            cr.CreditScore = 750;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(CreditReport.INSURED_SCORE_IS_700_TO_850, 750)
                );

            cr.IsInsured = false;

            cr.CreditScore = 550;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(cr.UNINSURED_SCORE_LESS_THAN_600, 550)
                );

            cr.CreditScore = 650;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(cr.UNINSURED_SCORE_IS_600_TO_699, 650)
                );

            cr.CreditScore = 750;
            Assert.AreEqual(
                cr.CreditAdvice,
                string.Format(CreditReport.UNINSURED_SCORE_IS_700_TO_850, 750)
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CreditReport cr = null;
        #endregion
    }
}