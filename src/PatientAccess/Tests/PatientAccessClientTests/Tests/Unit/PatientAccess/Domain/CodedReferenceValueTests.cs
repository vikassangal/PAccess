using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CodedReferenceValueTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CodedReferenceValueTests
        [TestFixtureSetUp()]
        public static void SetUpCodedReferenceValueTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCodedReferenceValueTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestEquals()
        {
            Object aObject = new Object();
            CodedReferenceValue aCodedReferenceValue  = new CodedReferenceValue( 0L, "DESC" );
            CodedReferenceValue aCodedReferenceValue1 = new CodedReferenceValue( 1L, "DESC2" );
            
            Assert.IsFalse( aCodedReferenceValue.Equals( aObject ) );
            Assert.IsFalse( aObject.Equals( aCodedReferenceValue ) );
            Assert.IsTrue( aCodedReferenceValue.Equals( aCodedReferenceValue1 ) );
            Assert.IsTrue( aCodedReferenceValue1.Equals( aCodedReferenceValue ) );
            
            aCodedReferenceValue.Code = "CODE";
            Assert.IsFalse( aCodedReferenceValue.Equals( aObject ) );
            Assert.IsFalse( aObject.Equals( aCodedReferenceValue ) );
            Assert.IsFalse( aCodedReferenceValue.Equals( aCodedReferenceValue1 ) );
            Assert.IsFalse( aCodedReferenceValue1.Equals( aCodedReferenceValue ) );

            aCodedReferenceValue.Code = "CODE";
            aCodedReferenceValue1.Code = "CODE";
            Assert.IsFalse( aCodedReferenceValue.Equals( aObject ) );
            Assert.IsFalse( aObject.Equals( aCodedReferenceValue ) );
            Assert.IsTrue( aCodedReferenceValue.Equals( aCodedReferenceValue1 ) );
            Assert.IsTrue( aCodedReferenceValue1.Equals( aCodedReferenceValue ) );

            Gender aGender = new Gender();
            Facility aFacility = new Facility();
            Assert.IsFalse( aGender.Equals( aFacility ) );
            Assert.IsFalse( aFacility.Equals( aGender ) );
            
            aGender.Code = "A";
            aFacility.Code = "A";
            Assert.IsFalse( aGender.Equals( aFacility ) );
            Assert.IsFalse( aFacility.Equals( aGender ) );

            Console.WriteLine( aGender.GetHashCode() );
            Console.WriteLine( aFacility.GetHashCode() );

            Facility aFacility2 = new Facility();
            aFacility2.Code = "A";
            Console.WriteLine( aFacility2.GetHashCode() );

            Assert.IsFalse( aFacility.Equals( null ) );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}