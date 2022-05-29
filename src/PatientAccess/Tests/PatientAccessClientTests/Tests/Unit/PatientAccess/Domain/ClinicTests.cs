using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ClinicTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ClinicTests
        [TestFixtureSetUp()]
        public static void SetUpClinicTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownClinicTests()
        {
        }
        #endregion

        #region Test Methods
        public void testClinics()
        {
            
            MedicalGroupIPA aMGIPA = new MedicalGroupIPA();;
            aMGIPA.Code = "A1234";
            aMGIPA.Name = "Pain Medical Group";
            Clinic Clinic1 = new Clinic();
            Clinic Clinic2 = new Clinic();

            Clinic1.Code = "01";
            Clinic1.Name = "Alpha Clinic";

            Clinic2.Code = "02";
            Clinic2.Name = "Body Blows Clinic";

            aMGIPA.AddClinic(Clinic1);
            aMGIPA.AddClinic(Clinic2);

            Assert.AreEqual( 2, aMGIPA.Clinics.Count,
                             "MedicalGroupIPA has  has two Clinics" );
            
           
            foreach( Clinic c in aMGIPA.Clinics )
            {
                string formattedString = String.Format(
                    "{0}{1}{2}{3}", 
                    c.Name,"    ",c.Code,Environment.NewLine);
                    
                Console.WriteLine( formattedString  );
             
            }
            
                 
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}