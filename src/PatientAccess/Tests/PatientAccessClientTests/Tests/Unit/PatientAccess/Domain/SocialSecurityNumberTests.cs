using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SocialSecurityNumberTests
    {
        #region Constants

        private const string
            VALID_AREA_NO = "789",
            VALID_GROUP_NO = "65",
            VALID_SERIES_NO = "3215",
            VALID_SSN = VALID_AREA_NO + VALID_GROUP_NO + VALID_SERIES_NO;

        #endregion

        #region Test Methods

        [Test]
        public void TestConstructors()
        {
            var ssn = new SocialSecurityNumber(VALID_SSN);

            Assert.AreEqual(VALID_SSN, ssn.ToString(), "SocailSecurityNumber should be " + VALID_SSN);
        }

        [Test]
        [ExpectedException(typeof (ApplicationException))]
        public void TestNullSSN()
        {
            new SocialSecurityNumber(null);
        }

        [Test]
        public void TestShortSSN()
        {
            var ssn = new SocialSecurityNumber("1112233");
            Assert.IsNotNull(ssn, "Failed to create SSN object");
            Assert.IsTrue(ssn.IsPartialSSN);
        }

        [Test]
        public void TestEmptySSN()
        {
            var ssn = new SocialSecurityNumber(string.Empty);
            Assert.AreEqual("--", ssn.AsFormattedString(), "SSN should be -- ");
        }

        [Test]
        public void TestAsFormattedString()
        {
            var ssn = new SocialSecurityNumber("999999999");
            Assert.AreEqual("999-99-9999", ssn.AsFormattedString(), "SSN should be 999-99-9999");

            var ssn2 = new SocialSecurityNumber("000112222");
            Assert.AreEqual("000-11-2222", ssn2.AsFormattedString(), "SSN should be 000-11-2222");

            var ssn3 = new SocialSecurityNumber("1112233");
            Assert.AreEqual("--", ssn3.AsFormattedString(), "SSN should be --");
        }

        [Test]
        public void TestDisplayStringAndPrintString()
        {
            var ssn = new SocialSecurityNumber("999999999");
            Assert.AreEqual(ssn.DisplayString, ssn.PrintString, "Strings should be same.");
        }

        [Test]
        public void TestProperties()
        {
            var ssn = new SocialSecurityNumber(VALID_SSN);

            Assert.AreEqual(VALID_AREA_NO, ssn.AreaNumber, "Area should be " + VALID_AREA_NO);
            Assert.AreEqual(VALID_GROUP_NO, ssn.GroupNumber, "Area should be " + VALID_GROUP_NO);
            Assert.AreEqual(VALID_SERIES_NO, ssn.Series, "Area should be " + VALID_SERIES_NO);
        }

        [Test]
        public void TestGetHashCode()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            Assert.IsTrue(ssn1.GetHashCode() != 0, "ssn1 and ssn2 should be equivalent");
        }

        [Test]
        public void TestEqualsString()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            var ssn2 = new SocialSecurityNumber(VALID_SSN);
            var ssn3 = new SocialSecurityNumber("000112222");

            Assert.IsTrue(ssn1.Equals(ssn2.AsFormattedString()), "ssn1 and ssn2 should be equivalent");

            string ssn2String = ssn2.AreaNumber + ssn2.GroupNumber + ssn2.Series;
            Assert.IsTrue(ssn1.Equals(ssn2String), "ssn1 and ssn2 should be equivalent");
            Assert.IsTrue(!ssn1.Equals(ssn3.AsFormattedString()), "ssn1 and ssn2 should be equivalent");

            string ssn3String = ssn3.AreaNumber + ssn2.GroupNumber + ssn2.Series;
            Assert.IsTrue(!ssn1.Equals(ssn3String), "ssn1 and ssn2 should be equivalent");
        }

        [Test]
        public void TestEquals()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            var ssn2 = new SocialSecurityNumber(VALID_SSN);
            var ssn3 = new SocialSecurityNumber("000112222");
            const SocialSecurityNumber ssn4 = null;

            Assert.IsTrue(ssn1.Equals(ssn2), "ssn1 and ssn2 should be equivalent");
            Assert.IsTrue(!ssn1.Equals(ssn3), "ssn1 and ssn3 should be equivalent");
            Assert.IsTrue(!ssn1.AreaNumber.Equals(ssn3.AreaNumber), "ssn1 and ssn3 should not be equivalent");
            Assert.AreEqual(ssn1.AreaNumber, ssn2.AreaNumber, "ssn1 and ssn2 should be equivalent");
            Assert.IsTrue(ssn1.AreaNumber == ssn2.AreaNumber, "ssn1 and ssn2 should be equivalent");
            Assert.IsTrue(ssn1.AreaNumber != ssn3.AreaNumber, "ssn1 and ssn3 should be not equivalent");
            Assert.IsFalse(ssn1.Equals(ssn4), "ssn1 and ssn4 should not be equivalent");
        }

        [Test]
        public void TestEqualOperator()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            var ssn2 = new SocialSecurityNumber(VALID_SSN);

            Assert.IsTrue(ssn1 == ssn2, "ssn1 and ssn2 should be equivalent");
            Assert.AreEqual(ssn1.AreaNumber, ssn2.AreaNumber, "ssn1 and ssn2 Area Number should be equivalent");
            Assert.IsTrue(ssn1.GroupNumber == ssn2.GroupNumber, "ssn1 and ssn2 Group Number should be equivalent");
            Assert.IsTrue(ssn1.Series == ssn2.Series, "ssn1 and ssn2 Series should be equivalent");
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            var ssn2 = new SocialSecurityNumber("000112222");

            Assert.IsTrue(ssn1 != ssn2, "ssn1 and ssn3 should be equivalent");
            Assert.IsTrue(ssn1.AreaNumber != ssn2.AreaNumber, "ssn1 and ssn2 Area Number should be not equivalent");
            Assert.IsTrue(ssn1.GroupNumber != ssn2.GroupNumber, "ssn1 and ssn2 Group Number should be not equivalent");
            Assert.IsTrue(ssn1.Series != ssn2.Series, "ssn1 and ssn2 Series should be not equivalent");
        }

        [Test]
        public void TestDeepCopy()
        {
            var ssn1 = new SocialSecurityNumber(VALID_SSN);
            var ssn2 = (SocialSecurityNumber) ssn1.DeepCopy();

            Assert.IsTrue(ssn1.Equals(ssn2), "ssn1 and ssn2 should be equivalent");
        }

        [Test]
        public void TestIsDefaultSsn()
        {
            SocialSecurityNumber defaultSsn = SocialSecurityNumber.NonFloridaNoneSSN;
            bool validSsn = defaultSsn.IsDefaultSsn();
            Assert.IsTrue(validSsn, "Default SSN");

            var ssn = new SocialSecurityNumber("123456789");
            bool invalidSsn = ssn.IsDefaultSsn();
            Assert.IsFalse(invalidSsn, "Not a Default SSN");
        }

        [Test]
        public void TestIsComplete()
        {
            var ssn = new SocialSecurityNumber(VALID_SSN);
            Assert.IsTrue(ssn.IsComplete, "This SSN should have been complete, i.e. of the correct length");

            ssn = new SocialSecurityNumber("01");
            Assert.IsTrue(!ssn.IsComplete, "This SSN should not be complete, i.e. of an incorrect length");
        }

        #endregion

        #region Properties
        #endregion

        #region Data Elements
        #endregion
    }
}