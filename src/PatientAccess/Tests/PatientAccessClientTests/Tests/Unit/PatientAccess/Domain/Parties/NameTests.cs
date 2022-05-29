using System;
using NUnit.Framework;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for NameTests.
    /// </summary>

    //TODO: Create XML summary comment for NameTests
    [TestFixture]
    [Category( "Fast" )]
    public class NameTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown NameTests
        [SetUp]
        public void SetUpNameTests()
        {
            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI);
        }

        [TearDown]
        public void TearDownNameTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructors()
        {
            this.UnitUnderTest = new Name(TEST_OID, this.TEST_VERSION, TEST_F_NAME, TEST_L_NAME, TEST_MI);
            
            Assert.AreEqual(
                TEST_OID,
                this.UnitUnderTest.Oid,
                "Name's Oid should be " + TEST_OID
                );

            Assert.AreEqual(
                this.TEST_VERSION,
                this.UnitUnderTest.Timestamp,
                "Name's Version should be " + this.TEST_VERSION
                );

            Assert.AreEqual(
                TEST_F_NAME,
                this.UnitUnderTest.FirstName,
                "Name's First Name should be " + TEST_F_NAME
                );

            Assert.AreEqual(
                TEST_L_NAME,
                this.UnitUnderTest.LastName,
                "Name's First Name should be " + TEST_L_NAME
                );

            Assert.AreEqual(
                TEST_MI,
                this.UnitUnderTest.MiddleInitial,
                "Name's First Name should be " + TEST_MI
                );

            this.UnitUnderTest = new Name(TEST_OID, this.TEST_VERSION, TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX);
            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX);
            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, "Y");

            Assert.AreEqual(
                true,
                this.UnitUnderTest.IsConfidential,
                "Show AKA flag shoud be  "
                );

            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, "N");

            Assert.AreEqual(
                false,
                this.UnitUnderTest.IsConfidential,
                "Show AKA flag shoud be  "
                );

            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, TypeOfName.Normal);

            Assert.AreEqual(
                TypeOfName.Normal, 
                this.UnitUnderTest.TypeOfName, 
                "Type of name should be " + TypeOfName.Normal
                );

            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, "Y", TypeOfName.Normal);
            this.UnitUnderTest = new Name(TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, "N", TypeOfName.Normal);

            this.UnitUnderTest.EntryDate = DateTime.Today;
            Assert.AreEqual(DateTime.Today, this.UnitUnderTest.EntryDate);
        }

        [Test()]
        public void TestAsFormattedName()
        {
           
            string formattedName = this.UnitUnderTest.AsFormattedName();
            string expectedFormat = String.Format( 
                TEST_FORMATTED_NAME, TEST_L_NAME, TEST_F_NAME, TEST_MI
                );
            
            Assert.AreEqual(
                expectedFormat,
                formattedName,
                String.Format( "Name should be: {0}", expectedFormat )
                );
        }
        [Test()]
        public void TestStripMiFromFirstName()
        {
            this.UnitUnderTest.FirstName = "Mary Joe K";
            Assert.AreEqual(
                this.UnitUnderTest.FirstName,
                "Mary Joe K",
                String.Format( "Name should be: {0}", "Mary Joe K" )
                );

            this.UnitUnderTest.FirstName = "Mary Joe";
            Assert.AreEqual(this.UnitUnderTest.FirstName, "Mary Joe", "Name should be: Mary Joe");

            this.UnitUnderTest.FirstName = "MJ";
            Assert.AreEqual( this.UnitUnderTest.FirstName, "MJ", "Name should be: MJ");
          
        }
        [Test()]
        public void TestAsFormattedNameWithSuffix()
        {
            this.UnitUnderTest = new Name(TEST_OID, this.TEST_VERSION, String.Empty, String.Empty, String.Empty);

            Assert.AreEqual(
                String.Empty,
                this.UnitUnderTest.AsFormattedNameWithSuffix(),
                "Name should be blank"
                );

            this.UnitUnderTest = new Name(TEST_OID, this.TEST_VERSION, TEST_F_NAME, TEST_L_NAME, TEST_MI);

            Assert.AreEqual(
                string.Format(TEST_FORMATTED_NAME, TEST_L_NAME, TEST_F_NAME, TEST_MI),
                this.UnitUnderTest.AsFormattedNameWithSuffix(),
                string.Format("Name should be " + TEST_FORMATTED_NAME, TEST_L_NAME, TEST_F_NAME, TEST_MI)
                );

            this.UnitUnderTest = new Name(
                TEST_OID, this.TEST_VERSION, TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX
                );
            string formattedName = this.UnitUnderTest.AsFormattedNameWithSuffix();
            string expectedFormat = String.Format( 
                TEST_FORMATTED_NAME_SUFFIX, TEST_L_NAME, TEST_F_NAME, TEST_MI, TEST_SUFFIX
                );
            
            Assert.AreEqual(
                expectedFormat,
                formattedName,
                String.Format( "Name should be: {0}", expectedFormat )
                );
        }

        [Test()]
        public void TestAsFormatedName()
        {
            this.UnitUnderTest = new Name( TEST_OID, this.TEST_VERSION, String.Empty, String.Empty, String.Empty );
            
            Assert.AreEqual( 
                String.Empty,
                this.UnitUnderTest.AsFormattedName(),
                "Name should be blank" 
                );

            this.UnitUnderTest = new Name(TEST_OID, this.TEST_VERSION, TEST_F_NAME, TEST_L_NAME, TEST_MI);

            Assert.AreEqual(
                string.Format(TEST_FORMATTED_NAME, TEST_L_NAME,TEST_F_NAME, TEST_MI),
                this.UnitUnderTest.AsFormattedName(),
                string.Format("Name should be " + TEST_FORMATTED_NAME, TEST_L_NAME, TEST_F_NAME, TEST_MI)
                );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private Name UnitUnderTest { get; set; }
        private const long TEST_OID = 2L;
        private readonly DateTime TEST_VERSION = DateTime.MaxValue;
        private const string
            TEST_F_NAME = "Joe",
            TEST_L_NAME = "Blow",
            TEST_MI = "T",
            TEST_SUFFIX = "SR",
            TEST_FORMATTED_NAME = "{0}, {1} {2}",
            TEST_FORMATTED_NAME_SUFFIX = "{0}, {1} {2}, {3}";
        #endregion
    }
}