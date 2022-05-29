using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestClass()]
    public class ResistantOrganismPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ResistantOrganismBrokerTests
        [ClassInitialize()]
        public static void SetUpResistantOrganismBrokerTests( TestContext context )
        {
            i_ResistantOrganismBroker =
                BrokerFactory.BrokerOfType<IResistantOrganismBroker>();
        }

        [ClassCleanup()]
        public static void TearDownResistantOrganismBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [TestMethod()]
        public void TestAllResistantOrganisms()
        {
            ArrayList ResistantOrganisms = (ArrayList)i_ResistantOrganismBroker.AllResistantOrganisms( ACO_FACILITYID );
            Assert.IsTrue( ResistantOrganisms.Count > 0, "No ResistantOrganisms found" );
        }


        [TestMethod()]
        public void TestResistantOrganismsWithCode()
        {
            string code = "VRE";
            ResistantOrganism resistantOrganism = i_ResistantOrganismBroker.ResistantOrganismWith( ACO_FACILITYID, code );

            Assert.AreEqual( "VANCO R ENTEROCOCCUS", resistantOrganism.Description,
                             "description  should be VANCO R ENTEROCOCCUS" );
            Assert.IsTrue( resistantOrganism.IsValid );
        }

        [TestMethod()]
        public void TestResistantOrganismForBlank()
        {

            string blank = String.Empty;
            ResistantOrganism ro = i_ResistantOrganismBroker.ResistantOrganismWith( ACO_FACILITYID, blank );

            Assert.IsTrue( ro.IsValid, "Code Blank should be valid" );
        }

        [TestMethod()]
        public void TestResistantOrganismForInvalid()
        {

            string code = "M";
            ResistantOrganism ro = i_ResistantOrganismBroker.ResistantOrganismWith( ACO_FACILITYID, code );

            Assert.IsFalse( ro.IsValid, "Code M should be invalid" );
        }

        [TestMethod(), ExpectedException( typeof( ArgumentNullException ) )]
        public void TestResistantOrganismForNULL()
        {

            ResistantOrganism ro = i_ResistantOrganismBroker.ResistantOrganismWith( ACO_FACILITYID, null );

            Assert.IsFalse( ro.IsValid, "Code null should be invalid" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IResistantOrganismBroker i_ResistantOrganismBroker = null;
        #endregion

        #region Constants
        #endregion
    }
}