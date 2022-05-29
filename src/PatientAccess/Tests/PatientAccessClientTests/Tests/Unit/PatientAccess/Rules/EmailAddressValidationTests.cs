
using System.Text.RegularExpressions;
using NUnit.Framework;
using PatientAccess.Utilities;

namespace Tests.Unit.PatientAccess.Rules
{
    class EmailAddressValidationTests
    {
        [SetUp]
        [Test]
        public void TestValidEmailAddressFormatUnderscoreDomain_valid()
        {
            var emailAddressText = "Test@dell_India.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result == true, " Test@dell_India.com as Email address should be valid, false");
        }
        [Test]
        public void TestValidEmailAddressFormat_HyphenDomain_Valid()
        {
            var emailAddressText = "Test@dell-india.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result == true, " Test@dell-india.com as Email address should be valid, false");
        }
        [Test]
        public void TestValidEmailAddressFormat_DotInDomain_Valid()
        {
            var emailAddressText = "Test@dell.india.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result == true, " Test@dell.india.com as Email address should be valid, false");
        }
        [Test]
        public void TestinValidEmailAddressFormat_DollarinDomain_Invalid()
        {
            var emailAddressText = "Test@dell$india.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsFalse(result == true, " Test@dell_India.com as Email address should be valid, false");
        }
        [Test]
        public void TestinValidEmailAddressFormat_MultipleAmpersand_Invalid()
        {
            var emailAddressText = "Test@dell@india.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsFalse(result == true, " Test@dell@india.com as Email address should be invalid, false");
        }
        [Test]
        public void TestinValidEmailAddressFormat_ConsecutivesDots_Invalid()
        {
            var emailAddressText = "Test@dell..india.com";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsFalse(result , " Test@dell..india.com as Email address should be invalid, false");
        }
        [Test]
        public void TestinValidEmailAddressFormat_SingleCharInDomain_Valid()
        {
            var emailAddressText = "A@B.CC";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result , " A@B.CC as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_TwoCharInDomain_Valid()
        {
            var emailAddressText = "A@BC.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@BC.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Underscore_Alone_In_Domain_Valid()
        {
            var emailAddressText = "A@_.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@_.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Hyphen_Alone_In_Domain_Valid()
        {
            var emailAddressText = "A@-.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@-.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Hyphen_At_BeginningOfDomain_Valid()
        {
            var emailAddressText = "A@-BC.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@-BC.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Underscore_At_BeginningOfDomain_Valid()
        {
            var emailAddressText = "A@_BC.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@_BC.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Hyphen_At_EndOfDomain_Valid()
        {
            var emailAddressText = "A@BC-.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@BC-.DD as Email address should be valid, true");
        }
        [Test]
        public void TestinValidEmailAddressFormat_Underscore_At_EndOfDomain_Valid()
        {
            var emailAddressText = "A@BC_.DD";
            var emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            var result = emailValidationExpression.IsMatch(emailAddressText);
            Assert.IsTrue(result, "A@BC_.DD as Email address should be valid, true");
        }
    }
}
