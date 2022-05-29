using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TypeOfAccidentTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfAccidentTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfAccidentTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfAccidentTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTypeOfAccident()
        {
            TypeOfAccident auto = new TypeOfAccident( TypeOfAccident.AUTO, "Auto" );
            TypeOfAccident AutoNoFaultInsurance = new TypeOfAccident( TypeOfAccident.AUTO_NO_FAULT_INSURANCE, "AutoNoFaultInsurance" );
            
            TypeOfAccident EmploymentRelated = new TypeOfAccident( TypeOfAccident.EMPLOYMENT_RELATED, "Employment Related" );
            TypeOfAccident TortLiability = new TypeOfAccident( TypeOfAccident.TORT_LIABILITY, "Tort Liability" );
            TypeOfAccident Other = new TypeOfAccident( TypeOfAccident.OTHER, "Other" );

            Assert.AreEqual(
                auto,
                TypeOfAccident.NewAuto()) ;
            Assert.AreEqual(
                AutoNoFaultInsurance,
                TypeOfAccident.NewAutoNoFaultInsurance()) ;
            Assert.AreEqual(
                EmploymentRelated,
                TypeOfAccident.NewEmploymentRelated()) ;
            Assert.AreEqual(
                TortLiability,
                TypeOfAccident.NewTortLiability()) ;
            Assert.AreEqual(
                Other,
                TypeOfAccident.NewOther()) ;

            auto.OccurrenceCode = new OccurrenceCode(0L,"AUTO",DateTime.Now);
            Assert.AreEqual(
                auto.OccurrenceCode.Description,
                "AUTO") ;
            Assert.AreEqual(
                auto.OccurrenceCode.Oid ,
                0L) ;

            //Non Auto TypeOfAccident Test
            TypeOfAccident nonAuto = new TypeOfAccident( TypeOfAccident.NON_AUTO, "Non-automobile" );
            Assert.AreEqual(nonAuto, TypeOfAccident.NewNonAuto());
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}