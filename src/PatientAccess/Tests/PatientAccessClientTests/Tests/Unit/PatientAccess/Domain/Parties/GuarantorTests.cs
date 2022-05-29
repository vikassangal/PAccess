using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class GuarantorTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GuarantorTests
        [TestFixtureSetUp()]
        public static void SetUpGuarantorTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGuarantorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestDeepCopy()
        {
            Name guarantorsName = new Name( "Joe", "Guarantor", "D." );
            Guarantor guarantor = new Guarantor( Guarantor.NEW_OID, Guarantor.NEW_VERSION, guarantorsName );
            guarantor.AddRelationship( new Relationship( new RelationshipType( 0, DateTime.MinValue, "Parent" ), guarantor.GetType(), guarantor.GetType() ) );
            
            Guarantor copy = guarantor.DeepCopy() as Guarantor;
            
            Assert.IsNotNull(
                copy,
                "If copy is null then DeepCopy failed or casting the copy to the correct type failed"
                );

            Assert.AreEqual(
                guarantor.Name.FirstName,
                copy.Name.FirstName
                );
        }
        [Test()]
        public void TestCreditReport()
        {
            CreditReport cr = new CreditReport();

            cr.CreditScore = 459;
            cr.Report = " Credit report not available";

         
            Name guarantorsName = new Name( "Joe", "Guarantor", "D." );
            Guarantor guarantor = new Guarantor( Guarantor.NEW_OID, Guarantor.NEW_VERSION, guarantorsName );
            guarantor.CreditReport = cr;
            Assert.AreEqual(
                459,
                guarantor.CreditReport.CreditScore
                );
            
            Assert.AreEqual(
                " Credit report not available",
                guarantor.CreditReport.Report
                );
        }

        [Test()]
        public void TestAsGuarantor()
        {
            Patient aPatient = new Patient( 0, DateTime.MinValue, "Joe", "Blow", "T","title", 12345, DateTime.MaxValue, new SocialSecurityNumber( "123456789" ), new Gender( 0, DateTime.MinValue, "Male", "M" ), new Facility( 0, DateTime.MinValue, "Brookwood", "BMC" ) );
            ContactPoint cp = new ContactPoint();
            cp.Address =  new Address( 
                String.Empty, String.Empty, String.Empty, new ZipCode( String.Empty ),
                new State( 0, DateTime.Now, "TX" ), 
                new Country( 0, DateTime.Now, "US" ), 
                new County( 0, DateTime.Now, "Dallas" )
                );
            aPatient.AddContactPoint(cp);
           
            Guarantor aGuarantor = aPatient.CopyAsGuarantor();

            Assert.AreEqual(
                aPatient.Name.AsFormattedName(),
                aGuarantor.Name.AsFormattedName()
                );

            Assert.AreEqual(
                aPatient.ContactPoints.Count,
                aGuarantor.ContactPoints.Count
                );

            Console.WriteLine( aGuarantor.Sex.ToString() );

            Console.WriteLine( aPatient.ContactPoints.Count );
            Console.WriteLine( aGuarantor.ContactPoints.Count );
        }
        #endregion

        #region Support Methods

        #endregion

        #region Data Elements
        #endregion
    }
}