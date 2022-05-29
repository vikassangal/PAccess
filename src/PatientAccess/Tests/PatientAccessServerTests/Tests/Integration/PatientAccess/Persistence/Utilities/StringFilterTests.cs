using System;
using PatientAccess.Utilities;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Utilities
{
    [TestFixture]
    [Category( "Fast" )]
    public class StringFilterTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown StringFilterTests
        [TestFixtureSetUp]
        public static void SetUpStringFilterTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownStringFilterTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestPadLeft()
        {
            const string input = "234234";
            const string result = "000234234";
            Assert.IsTrue( result == StringFilter.PadString( input, '0', 9, false ) ,
                           "Did not put leading zeros as expected." );
        }

        [Test]
        public void TestPadRight()
        {
            const string input = "234234";
            const string result = "234234000";
            Assert.IsTrue( result == StringFilter.PadString( input, '0', 9, true ),
                           "Did not put append zeros to right as expected." );
        }

        [Test]
        public void TestLengthofStringGreaterThan()
        {
            const string input = "123456789";
            const string result = "123456789";
            Assert.IsTrue( result == StringFilter.PadString( input, '0', 7, true ),
                           "Should have returned input string back." );
        }

        [Test]
        public void TestStripMiddleInitialFromFirstName()
        {
            string firstName = "FIRST NAME M";
            string strippedMiddleInitial = StringFilter.StripMiddleInitialFromFirstName( ref firstName );
            Assert.IsTrue( firstName == "FIRST NAME", "First Name not stripped off MiddleInitial." );
            Assert.IsTrue( strippedMiddleInitial == "M", "First Name not stripped off MiddleInitial." );
        }

        [Test]
        public void TestRegexFilter()
        {
            string res = StringFilter.mangleName("SOME-NAME");
            Assert.IsTrue(TestStringSomeName == res, "String SOME-NAME was not properly parsed.");
            
            res = StringFilter.mangleName("SOME&NAME");
            Assert.IsTrue(TestStringSomeName == res, "String SOME&NAME was not properly parsed.");
             
            res = StringFilter.mangleName("_SOME-NAME");
            Assert.IsTrue(TestStringSomeName == res, "String SOME-NAME was not properly parsed.");

            res = StringFilter.mangleName(TestStringSomeName);
            Assert.IsTrue(res == TestStringSomeName,"String mangling did not return the same value");
            
            res = StringFilter.mangleName(TestStringSomeNumberName);
            Assert.IsTrue(res == TestStringSomeNumberName,
                          "String mangling with numbers did not return the same value");
            
            res = StringFilter.mangleName(TestStringSomeName);
            Assert.IsTrue(res == TestStringSomeName,"String mangling did not return the same value");
            
        }

        [Test]
        public void TestRemoveHL7Chars()
        {
            const string testName = "||&&123 TEST ^~\\NAME";
            const string resultName = "123 TEST NAME";
            Assert.AreEqual( resultName, StringFilter.RemoveHL7Chars( testName ),
                           "HL7 Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetter()
        {
            const string testName = "@#$@#123 TEST NAME";
            const string resultName = "TEST NAME";
            Assert.AreEqual( resultName, StringFilter.RemoveFirstCharNonLetter( testName ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetter_WithAllInvalidCharacters()
        {
            const string input = "@#$@#123";
            Assert.AreEqual( String.Empty, StringFilter.RemoveFirstCharNonLetter( input ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterAndRestNonLetter()
        {
            const string testName = "1@#$TEST.NAME#$% -11";
            const string resultName = "TESTNAME";
            Assert.AreEqual( resultName, StringFilter.RemoveFirstCharNonLetterAndRestNonLetter( testName ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetter_WithSingleCharacterInputString()
        {
            const string input = "@";
            string result = String.Empty;
            Assert.AreEqual( result, StringFilter.RemoveFirstCharNonLetter( input ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNonNumber()
        {
            const string testName = "@#$@#1 TEST NAME";
            const string resultName = "1 TEST NAME";
            Assert.AreEqual( resultName, StringFilter.RemoveFirstCharNonLetterNonNumber( testName ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonNumberAndRestNonNumber()
        {
            const string testName = "@#$@#1 TEST NAME2 3 4";
            const string resultName = "1234";
            Assert.AreEqual( resultName, StringFilter.RemoveFirstCharNonNumberAndRestNonNumber( testName ),
                           "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber()
        {
            const string testName = "@!1@#$TEST.NAME#$% -11";
            const string resultName = "1TESTNAME11";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod()
        {
            const string testName = "1@#$TEST.NAME#$% -11 TEST";
            const string resultName = "TEST.NAME - TEST";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod()
        {
            const string testName = "@1@#$TEST.NAME#$% -11 TEST";
            const string resultName = "1TEST.NAME -11 TEST";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash()
        {
            const string testName = "@1@#$TST.NM#$% -11 /TEST";
            const string resultName = "1TST.NM -11 /TEST";
            Assert.AreEqual( resultName,
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod()
        {
            const string testName = "  @1@#$TEST.NAME#$% -11 TEST";
            const string resultName = "  1TEST.NAME -11 TEST";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod_WhenStringHasNoSpaceInFront()
        {
            const string testName = "@1@#$TEST.NAME#$% -11 TEST";
            const string resultName = "1TEST.NAME -11 TEST";
            Assert.AreEqual( resultName,
                StringFilter.RemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterAndRestNonLetterSpaceAndHyphen()
        {
            const string testName = "1@#$TEST#$%-22 NAME";
            const string resultName = "TEST- NAME";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceAndHyphen( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen()
        {
            const string testName = "1@#$TEST#$%-11 NAME";
            const string resultName = "TEST-11 NAME";
            Assert.AreEqual( resultName,
                StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen()
        {
            const string testName = "1@#$TEST#$%-11 NAME";
            const string resultName = "1TEST-11 NAME";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma()
        {
            const string testName = "1@#$TEST.,NAME#$% -11 TEST";
            const string resultName = "TEST,NAME - TEST";
            Assert.AreEqual( resultName, 
                StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma( testName ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveAllNonEmailSpecialCharacters()
        {
            const string testEmail = "||123~TEST_^\\NAME$%@ABC.NET";
            const string resultName = "123~TEST_NAME$%@ABC.NET";
            Assert.AreEqual( resultName,
                StringFilter.RemoveAllNonEmailSpecialCharacters( testEmail ),
                "Invalid Special Characters not stripped." );
        }

        [Test]
        public void TestRemoveAllNonEmailSpecialCharacters_WithPeriodBeforeAtSymbol()
        {
            const string testEmail = "||123.||TEST.^\\NAME@ABC.NET";
            const string resultEmail = "123.TEST.NAME@ABC.NET";
            Assert.AreEqual( resultEmail,
                StringFilter.RemoveAllNonEmailSpecialCharacters( testEmail ),
                "Invalid Special Characters not stripped." );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private const string TestStringSomeName = "SOMENAME";
        private const string TestStringSomeNumberName = "SOMENUMBERS987";
        #endregion
    }
}