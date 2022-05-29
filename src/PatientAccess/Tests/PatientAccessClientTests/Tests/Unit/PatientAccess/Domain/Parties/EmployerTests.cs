using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmployerTests
    {
        #region Constants
        const string ADDRESS1 = "335 Nicholson Dr.",
                     ADDRESS2 = "32321",
                     CITY = "Austin",    
                     POSTALCODE = "60505";   
        #endregion

        #region SetUp and TearDown EmployerTests
        [TestFixtureSetUp()]
        public static void SetUpEmployerTests()
        {
           
        }

        [TestFixtureTearDown()]
        public static void TearDownEmployerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestEmployerCopyAsGuarantor()
        {
            Employer emp = new Employer( 0L, DateTime.Now, "Perot Systems", "123456", 12L );
            Guarantor g = emp.CopyAsGuarantor();

            Assert.IsNotNull( g );
        }
        [Test()]
        public void TestEmployerCopyAsInsured()
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

            Employer emp = new Employer( 0L, DateTime.Now, "Perot Systems", "123456", 12L );
            emp.AddAddress(this.address);
            Insured insured = emp.CopyAsInsured();
      
            Assert.IsNotNull( insured );
        }
        [Test()]
        public void TestEmployerContactPoint()
        {
            
            Employer emp = new Employer( 0L, DateTime.Now, "Perot Systems", "123456", 12L );
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

            EmployerContactPoint empCp = new EmployerContactPoint();
            empCp.Address = address;
            empCp.TypeOfContactPoint =  TypeOfContactPoint.NewEmployerContactPointType() ;
            empCp.EmployerAddressNumber = 1;
            emp.AddContactPoint(empCp);
           
            Assert.AreEqual(
                emp.ContactPoints.Count,
                1
                );
            foreach( ContactPoint newcp in emp.ContactPoints )
            {
                if(newcp.GetType() == typeof(EmployerContactPoint ))
                {
                    EmployerContactPoint newEmpCp = (EmployerContactPoint)newcp;
                    Assert.AreEqual(
                        newEmpCp.EmployerAddressNumber,
                        1
                        );
               
                }
            }
           
            
            emp.RemoveContactPoint(empCp);
            Assert.AreEqual(
                emp.ContactPoints.Count,
                0
                );
            
            
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private readonly Address  address = new Address();
        #endregion
    }
}