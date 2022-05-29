using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmploymentTests
    {
        #region Constants
        const string ADDRESS1 = "335 Nicholson Dr.",
                     ADDRESS2 = "32321",
                     CITY = "Austin",    
                     POSTALCODE = "60505"; 
        #endregion

        #region SetUp and TearDown EmploymentTests
        [TestFixtureSetUp()]
        public static void SetUpEmploymentTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownEmploymentTests()
        {
        }
        #endregion

        #region Test Methods
        
        public void TestEmployment()
        {
            Address address = new Address( ADDRESS1,
                                           ADDRESS2,
                                           CITY,
                                           new ZipCode( POSTALCODE ),
                                           new State( 0L,
                                                      ReferenceValue.NEW_VERSION,
                                                      "TEXAS",
                                                      "TX"),
                                           new Country( 0L,
                                                        ReferenceValue.NEW_VERSION,
                                                        "United States",
                                                        "USA"),
                                           new County( 0L,
                                                       ReferenceValue.NEW_VERSION,
                                                       "ORANGE",
                                                       "122")
                ); 

            Employment empl = new Employment();
            empl.Employer = new Employer(1L,DateTime.Now, "PerotSystems","001",100);
            empl.EmployeeID = "234";
            empl.PhoneNumber = new PhoneNumber("9725770000");
            empl.Occupation = "jsdhsdjhdjh";
            empl.Status = new EmploymentStatus(3L,"Active");
            empl.Employer.PartyContactPoint = new ContactPoint(TypeOfContactPoint.NewBusinessContactPointType());
            empl.Employer.PartyContactPoint.Address = address;
            

            Assert.AreEqual(
                "234",
                empl.EmployeeID
                );

            Assert.AreEqual(
                "PerotSystems",
                empl.Employer.Name
                );
            Assert.AreEqual(
                "jsdhsdjhdjh",
                empl.Occupation
                );
            Assert.AreEqual(
                "9725770000",
                empl.PhoneNumber.AsUnformattedString()
                );
            Assert.AreEqual(
                "Active",
                empl.Status.Description
                );
            Assert.AreEqual(
                "335 Nicholson Dr.",
                empl.Employer.PartyContactPoint.Address.Address1
                );
                     
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}