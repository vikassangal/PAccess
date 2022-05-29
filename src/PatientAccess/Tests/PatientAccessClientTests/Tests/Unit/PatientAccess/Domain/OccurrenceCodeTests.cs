using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class OccurrenceCodeTests
    {
        #region Test Methods
        [Test()]
        public void TestOccurrenceCodeConstructor()
        {
            OccurrenceCode oc = new OccurrenceCode( 18, DateTime.Now, "DT OF RETIREMENT PT/BENFY" );

            Assert.AreEqual( 18, oc.Oid );

            Assert.AreEqual( "DT OF RETIREMENT PT/BENFY", oc.Description );
        }

        [Test()]
        public void TestIsCrimeOccurrenceCode_WhenOccurrenceCodeIsAccidentCrime_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_CRIME;

            Assert.IsTrue( oc.IsCrimeOccurrenceCode(), "Given code is not Accident Crime Occurrence Code." );
        }

        [Test()]
        public void TestIsOtherAccidentOccurrenceCode_WhenOccurrenceCodeIsAccidentOther_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER;

            Assert.IsTrue( oc.IsOtherAccidentOccurrenceCode(), "Given code is not Other Accident Occurrence Code." );
        }

        [Test()]
        public void TestIsAccidentCrimeOccurrenceCode_WhenOccurrenceCodeIsNotAccidentCrime_ShouldReturnFalse()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ILLNESS;

            Assert.IsFalse( oc.IsAccidentCrimeOccurrenceCode(), "Given code is Accident Crime Occurrence Code." );
        }

        [Test()]
        public void TestIsAutoAccidentOccurrenceCode_WhenOccurrenceCodeIsAutoAccident_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;

            Assert.IsTrue( oc.IsAutoAccidentOccurrenceCode(), "Given code is not Auto Accident Occurrence Code." );
        }

        [Test()]
        public void TestIsAutoAccidentOccurrenceCode_WhenOccurrenceCodeIsAutoAccidentNoFault_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT;

            Assert.IsTrue( oc.IsAutoAccidentOccurrenceCode(), "Given code is not Auto Accident No Fault Occurrence Code." );
        }

        [Test()]
        public void TestIsAutoAccidentOccurrenceCode_WhenOccurrenceCodeIsNotAutoAccident_ShouldReturnFalse()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ILLNESS;

            Assert.IsFalse( oc.IsAutoAccidentOccurrenceCode(), "Given code is Auto Accident Occurrence Code." );
        }

        [Test()]
        public void TestIsEmploymentRelatedOrTortLiabilityOccurrenceCode_WhenOccurrenceCodeIsEmploymentRelated_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL;

            Assert.IsTrue( oc.IsEmploymentRelatedOrTortLiabilityOccurrenceCode(), "Given code is not Employment Related Occurrence Code." );
        }

        [Test()]
        public void TestIsEmploymentRelatedOrTortLiabilityOccurrenceCode_WhenOccurrenceCodeIsTortLiability_ShouldReturnTrue()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY;

            Assert.IsTrue( oc.IsEmploymentRelatedOrTortLiabilityOccurrenceCode(), "Given code is not Tort Liability Occurrence Code." );
        }

        [Test()]
        public void TestIsEmploymentRelatedOrTortLiabilityOccurrenceCode_WhenOccurrenceCodeIsNotEither_ShouldReturnFalse()
        {
            OccurrenceCode oc = new OccurrenceCode();
            oc.Code = OccurrenceCode.OCCURRENCECODE_ILLNESS;

            Assert.IsFalse( oc.IsEmploymentRelatedOrTortLiabilityOccurrenceCode(), "Given code is Employment Related or TortLiability Occurrence Code." );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}